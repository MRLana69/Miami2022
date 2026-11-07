using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace SYBO.Components.Logging.Ftp
{
	public class FtpClient
	{
		public class FtpException : Exception
		{
			public int ResultCode;

			public FtpException(string message, int resultCode = -1)
				: base(message)
			{
				ResultCode = resultCode;
			}

			public FtpException(string message, Exception innerException, int resultCode = -1)
				: base(message, innerException)
			{
				ResultCode = resultCode;
			}

			public static FtpException FromResult(string result)
			{
				FtpException ex = new FtpException(result.Substring(4));
				int.TryParse(result.Substring(0, 3), out ex.ResultCode);
				return ex;
			}
		}

		private static int BUFFER_SIZE = 512;

		private static Encoding ASCII = Encoding.ASCII;

		private bool verboseDebugging;

		private string server = "localhost";

		private string remotePath = ".";

		private string username = "anonymous";

		private string password = "anonymous@anonymous.net";

		private string message;

		private string result;

		private int port = 21;

		private int bytes;

		private int resultCode;

		private bool loggedin;

		private bool binMode;

		private byte[] buffer = new byte[BUFFER_SIZE];

		private Socket clientSocket;

		private int timeoutSeconds = 10;

		public bool VerboseDebugging
		{
			get
			{
				return verboseDebugging;
			}
			set
			{
				verboseDebugging = value;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

		public int Timeout
		{
			get
			{
				return timeoutSeconds;
			}
			set
			{
				timeoutSeconds = value;
			}
		}

		public string Server
		{
			get
			{
				return server;
			}
			set
			{
				server = value;
			}
		}

		public int RemotePort
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

		public string RemotePath
		{
			get
			{
				return remotePath;
			}
			set
			{
				remotePath = value;
			}
		}

		public string Username
		{
			get
			{
				return username;
			}
			set
			{
				username = value;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public bool BinaryMode
		{
			get
			{
				return binMode;
			}
			set
			{
				if (binMode != value)
				{
					if (value)
					{
						sendCommand("TYPE I");
					}
					else
					{
						sendCommand("TYPE A");
					}
					if (resultCode != 200)
					{
						throw FtpException.FromResult(result);
					}
				}
			}
		}

		public FtpClient()
		{
		}

		public FtpClient(string server, string username, string password)
		{
			this.server = server;
			this.username = username;
			this.password = password;
		}

		public FtpClient(string server, string username, string password, int timeoutSeconds, int port)
		{
			this.server = server;
			this.username = username;
			this.password = password;
			this.timeoutSeconds = timeoutSeconds;
			this.port = port;
		}

		public void Login()
		{
			if (loggedin)
			{
				Close();
			}
			Debug.Log("[FtpClient] Opening connection to " + server);
			IPAddress iPAddress = null;
			IPEndPoint iPEndPoint = null;
			try
			{
				clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				iPAddress = Dns.GetHostAddresses(server)[0];
				iPEndPoint = new IPEndPoint(iPAddress, port);
				clientSocket.Connect(iPEndPoint);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				if (clientSocket != null && clientSocket.Connected)
				{
					clientSocket.Close();
				}
				throw new FtpException("Couldn't connect to remote server", ex);
			}
			readResponse();
			if (resultCode != 220)
			{
				Close();
				throw new FtpException(result.Substring(4));
			}
			sendCommand("USER " + username);
			if (resultCode != 331 && resultCode != 230)
			{
				cleanup();
				throw new FtpException(result.Substring(4));
			}
			if (resultCode != 230)
			{
				sendCommand("PASS " + password);
				if (resultCode != 230 && resultCode != 202)
				{
					cleanup();
					throw new FtpException(result.Substring(4));
				}
			}
			loggedin = true;
			Debug.Log("[FtpClient] Connected to " + server);
			ChangeDir(remotePath);
		}

		public void Close()
		{
			Debug.Log("[FtpClient] Closing connection to " + server);
			if (clientSocket != null)
			{
				sendCommand("QUIT");
			}
			cleanup();
		}

		public string[] GetFileList()
		{
			return GetFileList("*.*");
		}

		public string[] GetFileList(string mask)
		{
			if (!loggedin)
			{
				Login();
			}
			Socket socket = createDataSocket();
			sendCommand("NLST " + mask);
			if (resultCode != 150 && resultCode != 125)
			{
				throw new FtpException(result.Substring(4));
			}
			message = string.Empty;
			DateTime dateTime = DateTime.Now.AddSeconds(timeoutSeconds);
			while (dateTime > DateTime.Now)
			{
				int num = socket.Receive(buffer, buffer.Length, SocketFlags.None);
				message += ASCII.GetString(buffer, 0, num);
				if (num < buffer.Length)
				{
					break;
				}
			}
			string[] array = message.Replace("\r", string.Empty).Split('\n');
			socket.Close();
			if (message.IndexOf("No such file or directory") != -1)
			{
				array = new string[0];
			}
			readResponse();
			if (resultCode != 226)
			{
				array = new string[0];
			}
			return array;
		}

		public long GetFileSize(string fileName)
		{
			if (!loggedin)
			{
				Login();
			}
			sendCommand("SIZE " + fileName);
			long num = 0L;
			if (resultCode == 213)
			{
				return long.Parse(result.Substring(4));
			}
			throw new FtpException(result.Substring(4));
		}

		public void Download(string remFileName)
		{
			Download(remFileName, string.Empty, false);
		}

		public void Download(string remFileName, bool resume)
		{
			Download(remFileName, string.Empty, resume);
		}

		public void Download(string remFileName, string locFileName)
		{
			Download(remFileName, locFileName, false);
		}

		public void Download(string remFileName, string locFileName, bool resume)
		{
			if (!loggedin)
			{
				Login();
			}
			BinaryMode = true;
			Debug.Log("[FtpClient] Downloading file " + remFileName + " from " + server + "/" + remotePath);
			if (locFileName.Equals(string.Empty))
			{
				locFileName = remFileName;
			}
			FileStream fileStream = null;
			fileStream = (File.Exists(locFileName) ? new FileStream(locFileName, FileMode.Open) : File.Create(locFileName));
			Socket socket = createDataSocket();
			long num = 0L;
			if (resume)
			{
				num = fileStream.Length;
				if (num > 0)
				{
					sendCommand("REST " + num);
					if (resultCode != 350)
					{
						num = 0L;
						Debug.Log("[FtpClient] Resuming not supported:" + result.Substring(4));
					}
					else
					{
						Debug.Log("[FtpClient] Resuming at offset " + num);
						fileStream.Seek(num, SeekOrigin.Begin);
					}
				}
			}
			sendCommand("RETR " + remFileName);
			if (resultCode != 150 && resultCode != 125)
			{
				throw new FtpException(result.Substring(4));
			}
			DateTime dateTime = DateTime.Now.AddSeconds(timeoutSeconds);
			while (dateTime > DateTime.Now)
			{
				bytes = socket.Receive(buffer, buffer.Length, SocketFlags.None);
				fileStream.Write(buffer, 0, bytes);
				if (bytes <= 0)
				{
					break;
				}
			}
			fileStream.Close();
			if (socket.Connected)
			{
				socket.Close();
			}
			readResponse();
			if (resultCode != 226 && resultCode != 250)
			{
				throw new FtpException(result.Substring(4));
			}
		}

		public void Upload(string fileName)
		{
			Upload(fileName, false, null);
		}

		public void Upload(string fileName, string destFilename)
		{
			Upload(fileName, false, destFilename);
		}

		public void Upload(string fileName, bool resume)
		{
			Upload(fileName, false, null);
		}

		public void Upload(string fileName, bool resume, string destFilename)
		{
			if (!loggedin)
			{
				Login();
			}
			if (destFilename == null)
			{
				destFilename = fileName;
			}
			Socket socket = null;
			long num = 0L;
			if (resume)
			{
				try
				{
					BinaryMode = true;
					num = GetFileSize(Path.GetFileName(fileName));
				}
				catch (Exception)
				{
					num = 0L;
				}
			}
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
			if (resume && fileStream.Length < num)
			{
				Debug.Log("[FtpClient] Overwriting " + fileName);
				num = 0L;
			}
			else if (resume && fileStream.Length == num)
			{
				fileStream.Close();
				Debug.Log("[FtpClient] Skipping completed " + fileName + " - turn resume off to not detect.");
				return;
			}
			socket = createDataSocket();
			if (num > 0)
			{
				sendCommand("REST " + num);
				if (resultCode != 350)
				{
					Debug.Log("[FtpClient] Resuming not supported");
					num = 0L;
				}
			}
			sendCommand("STOR " + Path.GetFileName(destFilename));
			if (resultCode != 125 && resultCode != 150)
			{
				throw FtpException.FromResult(result);
			}
			if (num != 0)
			{
				Debug.Log("[FtpClient] Resuming at offset " + num);
				fileStream.Seek(num, SeekOrigin.Begin);
			}
			Debug.Log("[FtpClient] Uploading file " + fileName + " to " + remotePath);
			while ((bytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				socket.Send(buffer, bytes, SocketFlags.None);
			}
			fileStream.Close();
			if (socket.Connected)
			{
				socket.Close();
			}
			readResponse();
			if (resultCode != 226 && resultCode != 250)
			{
				throw new FtpException(result.Substring(4));
			}
		}

		public void WriteFile(string remoteFileName, byte[] data)
		{
			if (!loggedin)
			{
				Login();
			}
			Socket socket = null;
			BinaryMode = true;
			socket = createDataSocket();
			sendCommand("STOR " + remoteFileName);
			Debug.Log("[FtpClient] Uploading file " + remoteFileName + " to " + remotePath);
			socket.Send(data, data.Length, SocketFlags.None);
			if (socket.Connected)
			{
				socket.Close();
			}
			readResponse();
			if (resultCode != 226 && resultCode != 250)
			{
				throw new FtpException(result, int.Parse(result.Substring(4)));
			}
		}

		public void AppendFile(string remoteFileName, byte[] data)
		{
			if (!loggedin)
			{
				Login();
			}
			Socket socket = null;
			BinaryMode = true;
			socket = createDataSocket();
			sendCommand("APPE " + remoteFileName);
			Debug.Log("[FtpClient] Uploading file " + remoteFileName + " to " + remotePath);
			socket.Send(data, data.Length, SocketFlags.None);
			if (socket.Connected)
			{
				socket.Close();
			}
			readResponse();
			if (resultCode != 226 && resultCode != 250)
			{
				throw new FtpException(result.Substring(4));
			}
		}

		public void UploadDirectory(string path, bool recurse)
		{
			UploadDirectory(path, recurse, "*.*");
		}

		public void UploadDirectory(string path, bool recurse, string mask)
		{
			string[] array = path.Replace("/", "\\").Split('\\');
			string text = array[array.Length - 1];
			if (GetFileList(text).Length < 1)
			{
				MakeDir(text);
			}
			ChangeDir(text);
			string[] files = Directory.GetFiles(path, mask);
			foreach (string fileName in files)
			{
				Upload(fileName, true);
			}
			if (recurse)
			{
				string[] directories = Directory.GetDirectories(path);
				foreach (string path2 in directories)
				{
					UploadDirectory(path2, recurse, mask);
				}
			}
			ChangeDir("..");
		}

		public void DeleteFile(string fileName)
		{
			if (!loggedin)
			{
				Login();
			}
			sendCommand("DELE " + fileName);
			if (resultCode != 250)
			{
				throw new FtpException(result.Substring(4));
			}
			Debug.Log("[FtpClient] Deleted file " + fileName);
		}

		public void RenameFile(string oldFileName, string newFileName, bool overwrite)
		{
			if (!loggedin)
			{
				Login();
			}
			sendCommand("RNFR " + oldFileName);
			if (resultCode != 350)
			{
				throw new FtpException(result.Substring(4));
			}
			if (!overwrite && GetFileList(newFileName).Length > 0)
			{
				throw new FtpException("File already exists");
			}
			sendCommand("RNTO " + newFileName);
			if (resultCode != 250)
			{
				throw new FtpException(result.Substring(4));
			}
			Debug.Log("[FtpClient] Renamed file " + oldFileName + " to " + newFileName);
		}

		public void MakeDir(string dirName)
		{
			if (!loggedin)
			{
				Login();
			}
			sendCommand("MKD " + dirName);
			if (resultCode != 250 && resultCode != 257)
			{
				throw new FtpException(result.Substring(4));
			}
			Debug.Log("[FtpClient] Created directory " + dirName);
		}

		public void RemoveDir(string dirName)
		{
			if (!loggedin)
			{
				Login();
			}
			sendCommand("RMD " + dirName);
			if (resultCode != 250)
			{
				throw new FtpException(result.Substring(4));
			}
			Debug.Log("[FtpClient] Removed directory " + dirName);
		}

		public void ChangeDir(string dirName)
		{
			if (dirName != null && !dirName.Equals(".") && dirName.Length != 0)
			{
				if (!loggedin)
				{
					Login();
				}
				sendCommand("CWD " + dirName);
				if (resultCode != 250)
				{
					throw FtpException.FromResult(result);
				}
				sendCommand("PWD");
				if (resultCode != 257)
				{
					throw FtpException.FromResult(result);
				}
				remotePath = message.Split('"')[1];
				Debug.Log("[FtpClient] Current directory is " + remotePath);
			}
		}

		private void readResponse()
		{
			message = string.Empty;
			result = readLine();
			if (result.Length > 3)
			{
				resultCode = int.Parse(result.Substring(0, 3));
			}
			else
			{
				result = null;
			}
		}

		private string readLine()
		{
			clientSocket.ReceiveTimeout = Timeout * 1000;
			do
			{
				bytes = clientSocket.Receive(buffer, buffer.Length, SocketFlags.None);
				message += ASCII.GetString(buffer, 0, bytes);
			}
			while (bytes >= buffer.Length);
			string[] array = message.Split('\n');
			if (message.Length > 2)
			{
				message = array[array.Length - 2];
			}
			else
			{
				message = array[0];
			}
			if (message.Length > 4 && !message.Substring(3, 1).Equals(" "))
			{
				return readLine();
			}
			if (verboseDebugging)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					Debug.LogFormat("[FtpClient] {0}", array[i]);
				}
			}
			return message;
		}

		private void sendCommand(string command)
		{
			if (verboseDebugging)
			{
				Debug.LogFormat("[FtpClient] {0}", command);
			}
			byte[] array = Encoding.ASCII.GetBytes((command + "\r\n").ToCharArray());
			clientSocket.Send(array, array.Length, SocketFlags.None);
			readResponse();
		}

		private Socket createDataSocket()
		{
			sendCommand("PASV");
			if (resultCode != 227)
			{
				throw new FtpException(result.Substring(4));
			}
			int num = result.IndexOf('(');
			int num2 = result.IndexOf(')');
			string text = result.Substring(num + 1, num2 - num - 1);
			int[] array = new int[6];
			int length = text.Length;
			int num3 = 0;
			string text2 = string.Empty;
			for (int i = 0; i < length; i++)
			{
				if (num3 > 6)
				{
					break;
				}
				char c = char.Parse(text.Substring(i, 1));
				if (char.IsDigit(c))
				{
					text2 += c;
				}
				else if (c != ',')
				{
					throw new FtpException("Malformed PASV result: " + result);
				}
				if (c == ',' || i + 1 == length)
				{
					try
					{
						array[num3++] = int.Parse(text2);
						text2 = string.Empty;
					}
					catch (Exception innerException)
					{
						throw new FtpException("Malformed PASV result (not supported?): " + result, innerException);
					}
				}
			}
			string hostNameOrAddress = array[0] + "." + array[1] + "." + array[2] + "." + array[3];
			int num4 = (array[4] << 8) + array[5];
			Socket socket = null;
			IPEndPoint iPEndPoint = null;
			try
			{
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				iPEndPoint = new IPEndPoint(Dns.GetHostEntry(hostNameOrAddress).AddressList[0], num4);
				socket.Connect(iPEndPoint);
				return socket;
			}
			catch (Exception innerException2)
			{
				if (socket != null && socket.Connected)
				{
					socket.Close();
				}
				throw new FtpException("Can't connect to remote server", innerException2);
			}
		}

		private void cleanup()
		{
			if (clientSocket != null)
			{
				clientSocket.Close();
				clientSocket = null;
			}
			loggedin = false;
		}

		~FtpClient()
		{
			cleanup();
		}
	}
}
