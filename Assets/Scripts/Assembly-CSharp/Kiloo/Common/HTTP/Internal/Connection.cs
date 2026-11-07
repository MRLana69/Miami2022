using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP.Internal
{
	[Obsolete("Not used in Unity versions 5.4 or newer.")]
	internal class Connection : IDisposable
	{
		private static readonly List<Connection> _idleConnections = new List<Connection>();

		private Stream _stream;

		private bool _idle;

		private DateTime? _expires;

		public string host { get; private set; }

		public int port { get; private set; }

		public bool useSsl { get; private set; }

		public int sendTimeout { get; private set; }

		public int receiveTimeout { get; private set; }

		public bool ignoreCertificateErrors { get; private set; }

		public bool reused
		{
			get
			{
				DateTime? expires = _expires;
				return expires.HasValue;
			}
		}

		private Connection(string host, int port, bool useSsl, int sendTimeout, int receiveTimeout, bool ignoreCertificateErrors)
		{
			this.host = host;
			this.port = port;
			this.useSsl = useSsl;
			this.sendTimeout = sendTimeout;
			this.receiveTimeout = receiveTimeout;
			this.ignoreCertificateErrors = ignoreCertificateErrors;
			Connect();
		}

		public Stream GetStream()
		{
			CheckValidOperation();
			return _stream;
		}

		public static Connection Open(string host, int port, bool useSsl, int sendTimeout, int receiveTimeout, bool ignoreCertificateErrors = false)
		{
			Connection connection = null;
			lock (_idleConnections)
			{
				CloseExpiredIdleConnections();
				foreach (Connection idleConnection in _idleConnections)
				{
					if (idleConnection.host == host && idleConnection.port == port && idleConnection.useSsl == useSsl && idleConnection.sendTimeout == sendTimeout && idleConnection.receiveTimeout == receiveTimeout && idleConnection.ignoreCertificateErrors == ignoreCertificateErrors)
					{
						connection = idleConnection;
						break;
					}
				}
				if (connection != null)
				{
					_idleConnections.Remove(connection);
					connection._idle = false;
				}
			}
			if (connection == null)
			{
				connection = new Connection(host, port, useSsl, sendTimeout, receiveTimeout, ignoreCertificateErrors);
			}
			return connection;
		}

		public void KeepAlive(int milliseconds)
		{
			CheckValidOperation();
			lock (_idleConnections)
			{
				_idleConnections.Add(this);
				_idle = true;
				_expires = DateTime.UtcNow.AddMilliseconds(milliseconds);
				CloseExpiredIdleConnections();
			}
		}

		public void Reconnect()
		{
			Close();
			Connect();
		}

		public static void CloseExpiredIdleConnections()
		{
			lock (_idleConnections)
			{
				DateTime utcNow = DateTime.UtcNow;
				int count = _idleConnections.Count;
				for (int num = count - 1; num >= 0; num--)
				{
					Connection connection = _idleConnections[num];
					DateTime? expires = connection._expires;
					if (utcNow >= expires)
					{
						_idleConnections.RemoveAt(num);
						connection.Close();
					}
				}
			}
		}

		public static void CloseIdleConnectionsToHost(string host)
		{
			lock (_idleConnections)
			{
				int count = _idleConnections.Count;
				for (int num = count - 1; num >= 0; num--)
				{
					Connection connection = _idleConnections[num];
					if (connection.host == host)
					{
						_idleConnections.RemoveAt(num);
						connection.Close();
					}
				}
			}
		}

		public static void CloseIdleConnections()
		{
			lock (_idleConnections)
			{
				int count = _idleConnections.Count;
				for (int num = count - 1; num >= 0; num--)
				{
					Connection connection = _idleConnections[num];
					_idleConnections.RemoveAt(num);
					connection.Close();
				}
			}
		}

		public void Close()
		{
			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
		}

		public void Dispose()
		{
			Close();
		}

		private void Connect()
		{
			Stream stream;
			try
			{
				TcpClient tcpClient = new TcpClient();
				tcpClient.SendTimeout = sendTimeout;
				tcpClient.ReceiveTimeout = receiveTimeout;
				tcpClient.Connect(host, port);
				stream = tcpClient.GetStream();
				if (useSsl)
				{
					SslStream sslStream = new SslStream(stream, false, ValidateServerCertificate);
					sslStream.AuthenticateAsClient(host);
					stream = sslStream;
				}
			}
			catch (AuthenticationException ex)
			{
				throw new HTTPSecurityException(ex.Message, ex);
			}
			_stream = stream;
		}

		private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool result = true;
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != SslPolicyErrors.None)
				{
					result = ignoreCertificateErrors;
				}
				else if (TrustedRoots.VerifyTrust(new X509Certificate2(certificate)))
				{
					sslPolicyErrors = SslPolicyErrors.None;
				}
				else
				{
					result = ignoreCertificateErrors;
				}
			}
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
			}
			return result;
		}

		private void CheckValidOperation()
		{
			if (_idle || _stream == null)
			{
				throw new InvalidOperationException("Connection not valid. Make sure you call Open before using.");
			}
		}

		[Conditional("HTTP_CONNECTION_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[Connection] " + msg);
		}
	}
}
