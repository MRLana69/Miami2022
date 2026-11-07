using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Backend.Authentication;
using Kiloo.Common.Utility;
using Kiloo.GameCenter;
using UnityEngine;

namespace Kiloo.Integration.GameCenter
{
	public class GameCenterKilooAuthPlatform : IKilooAuthPlatform
	{
		public const string NAME = "GameCenter";

		private bool _updated;

		private Dictionary<string, string> _value;

		private string _signature;

		public string name
		{
			get
			{
				return "GameCenter";
			}
		}

		public IDictionary<string, string> value
		{
			get
			{
				IsUpdatedOrThrow();
				return _value;
			}
		}

		public string signature
		{
			get
			{
				IsUpdatedOrThrow();
				return _signature;
			}
		}

		public Coroutine UpdateAsync()
		{
			return CoroutineUtil.StartCoroutine(UpdateAsyncCoroutine());
		}

		private IEnumerator UpdateAsyncCoroutine()
		{
			if (GameCenterPlugin.isSupported)
			{
				while (!GameCenterPlugin.initialAuthenticationComplete)
				{
					yield return null;
				}
				GameCenterPlugin.LocalPlayer gameCenterPlayer = GameCenterPlugin.GetLocalPlayer();
				if (gameCenterPlayer.authenticated)
				{
					GameCenterGenerateIdentityVerificationSignatureRequest signatureRequest = new GameCenterGenerateIdentityVerificationSignatureRequest();
					yield return GameCenterPlugin.GenerateIdentityVerificationSignature(signatureRequest);
					if (signatureRequest.error == null && gameCenterPlayer.playerID != null && signatureRequest.publicKeyUrl != null && signatureRequest.salt != null && signatureRequest.signature != null)
					{
						_value = new Dictionary<string, string>
						{
							{ "playerId", gameCenterPlayer.playerID },
							{ "publicKeyUrl", signatureRequest.publicKeyUrl },
							{
								"salt",
								Convert.ToBase64String(signatureRequest.salt)
							},
							{
								"signature",
								Convert.ToBase64String(signatureRequest.signature)
							},
							{
								"timestamp",
								signatureRequest.timestamp.ToString()
							}
						};
						_signature = Convert.ToBase64String(Checksum.ComputeSHA256(gameCenterPlayer.playerID));
					}
					else
					{
						_value = null;
						_signature = null;
						LogError("Failed to get GameCenter identity verification signature: " + ((signatureRequest.error == null) ? "Invalid data" : signatureRequest.error));
					}
				}
			}
			_updated = true;
		}

		private void IsUpdatedOrThrow()
		{
			if (!_updated)
			{
				throw new InvalidOperationException("Not ready.");
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[GameCenterKilooAuthPlatform] " + msg);
		}
	}
}
