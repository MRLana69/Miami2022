using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Backend.Authentication;
using Kiloo.Common.Utility;
using Kiloo.Google;
using UnityEngine;

namespace Kiloo.Integration.Google
{
	public class GoogleKilooAuthPlatform : IKilooAuthPlatform
	{
		public const string NAME = "Google";

		private IGoogleProfile _profile;

		public string name
		{
			get
			{
				return "Google";
			}
		}

		public IDictionary<string, string> value
		{
			get
			{
				object result;
				if (_profile == null)
				{
					result = null;
				}
				else
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("googleId", _profile.userId);
					dictionary.Add("serverAuthCode", _profile.serverAuthCode);
					result = dictionary;
				}
				return (IDictionary<string, string>)result;
			}
		}

		public string signature
		{
			get
			{
				if (_profile != null)
				{
					return Convert.ToBase64String(Checksum.ComputeSHA256(_profile.userId));
				}
				return null;
			}
		}

		public Coroutine UpdateAsync()
		{
			if (GoogleSignInPlugin.isSupported)
			{
				return CoroutineUtil.StartCoroutine(UpdateAsyncCoroutine());
			}
			return null;
		}

		private IEnumerator UpdateAsyncCoroutine()
		{
			while (!GoogleSignInPlugin.isInitialized)
			{
				yield return null;
			}
			GoogleSignInPlugin google = GoogleSignInPlugin.instance;
			GoogleSignInPluginRequest request = new GoogleSignInPluginRequest
			{
				silent = true
			};
			yield return google.SignIn(request);
			if (request.profile == null || request.profile.userId == null || request.profile.serverAuthCode == null)
			{
				_profile = null;
			}
			else
			{
				_profile = request.profile;
			}
		}
	}
}
