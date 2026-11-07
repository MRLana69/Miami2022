using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP
{
	[Obsolete("Not used in Unity versions 5.4 or newer.")]
	public static class TrustedRoots
	{
		private static X509Certificate2Collection _trusted;

		public static void Init()
		{
			if (_trusted == null)
			{
				_trusted = LoadTrusted();
			}
		}

		public static bool VerifyTrust(X509Certificate2 certificate)
		{
			if (_trusted == null)
			{
				LogError("Trusted roots not loaded. Make sure you have called TrustedRoots.Init before making secure requests (from the Unity main thread).");
				return false;
			}
			X509Chain x509Chain = new X509Chain();
			x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
			x509Chain.ChainPolicy.ExtraStore.AddRange(_trusted);
			x509Chain.Build(certificate);
			bool result = true;
			bool flag = false;
			X509ChainStatus[] chainStatus = x509Chain.ChainStatus;
			for (int i = 0; i < chainStatus.Length; i++)
			{
				X509ChainStatus x509ChainStatus = chainStatus[i];
				if (x509ChainStatus.Status == X509ChainStatusFlags.UntrustedRoot)
				{
					flag = true;
				}
				else if (x509ChainStatus.Status != X509ChainStatusFlags.NoError)
				{
					result = false;
				}
			}
			if (flag)
			{
				foreach (X509ChainElement chainElement in x509Chain.ChainElements)
				{
					X509ChainStatus[] chainElementStatus = chainElement.ChainElementStatus;
					foreach (X509ChainStatus x509ChainStatus2 in chainElementStatus)
					{
						if (x509ChainStatus2.Status == X509ChainStatusFlags.UntrustedRoot && !_trusted.Contains(chainElement.Certificate))
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		[NotNull]
		private static X509Certificate2Collection LoadTrusted()
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			TextAsset[] array = Resources.LoadAll<TextAsset>("trusted_roots");
			if (array.Length > 0)
			{
				TextAsset[] array2 = array;
				foreach (TextAsset textAsset in array2)
				{
					try
					{
						x509Certificate2Collection.Import(textAsset.bytes);
					}
					catch (Exception ex)
					{
						LogError("LoadTrusted: Failed to import trusted root certificate \"" + textAsset.name + "\": " + ex);
					}
				}
			}
			return x509Certificate2Collection;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[TrustedRoots] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[TrustedRoots] " + msg);
		}
	}
}
