using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend
{
	public sealed class OnlineSettings<T>
	{
		private class Data
		{
			public T data;

			public DateTime expires;

			public byte[] hash;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action downloaded__BackingField;

		private const string API_GET_SETTINGS = "api/v2/settings";

		private const string ONLINESETTINGS_EXPIRES = "onlinesettings_expires";

		private const string ONLINESETTINGS_HASH = "onlinesettings_hash";

		[NotNull]
		private Data _data;

		private readonly Uri _baseUri;

		private readonly string _file;

		private readonly IJsonSerializer _jsonSerializer;

		private readonly IDictionary<string, string> _parameters;

		public bool isDownloading { get; private set; }

		[NotNull]
		public T data
		{
			get
			{
				return _data.data;
			}
		}

		public IDictionary<string, string> parameters
		{
			get
			{
				return _parameters;
			}
		}

		public event Action downloaded
		{
			add
			{
				Action action = this.downloaded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref this.downloaded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = this.downloaded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref this.downloaded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public OnlineSettings([NotNull] Uri baseUri, [CanBeNull] string file, [NotNull] IJsonSerializer jsonSerializer)
		{
			if (baseUri == null)
			{
				throw new ArgumentNullException("baseUri");
			}
			if (jsonSerializer == null)
			{
				throw new ArgumentNullException("jsonSerializer");
			}
			_baseUri = baseUri;
			_file = file;
			_jsonSerializer = jsonSerializer;
			_parameters = new Dictionary<string, string>();
			_data = Load();
		}

		public Coroutine DownloadIfExpired()
		{
			if (_data.expires <= DateTime.UtcNow)
			{
				return Download();
			}
			return null;
		}

		public Coroutine Download()
		{
			isDownloading = true;
			Uri uri = new Uri(_baseUri, "api/v2/settings");
			if (parameters.Count > 0)
			{
				uri = WebClient.AddQueryParametersToUri(uri, parameters);
			}
			return KilooWebClient.CreateGet(uri).WithKilooDeviceId().WithKilooTags()
				.ThenMakeTheRequestAsync((IRequest request, bool fromCache) =>
				{
					isDownloading = false;
					IResponse response = request.response;
					ServiceResponse<T> serviceResponse = null;
					if (!request.isSuccessful)
					{
						Error serviceError = request.GetServiceError(_jsonSerializer);
						if (serviceError != null)
						{
							LogError("Download: Request failed: " + serviceError.category + " error " + serviceError.code + " - " + serviceError.message);
						}
						else
						{
							LogError("Download: Request failed: " + request.statusMessage + "\nException: " + request.exception);
						}
					}
					else if (!response.IsJson())
					{
						LogError("Download: Response is not JSON.");
					}
					else
					{
						byte[] bytes = response.bytes;
						if (bytes == null || bytes.Length == 0)
						{
							LogError("Download: Empty response.");
						}
						else
						{
							byte[] array = Checksum.ComputeSHA1(bytes);
							if (!FileUtil.ArraysAreEqual(array, _data.hash))
							{
								try
								{
									serviceResponse = response.ParseJson<ServiceResponse<T>>(_jsonSerializer);
									if (serviceResponse == null)
									{
										LogError("Download: Parsed JSon is null");
										return;
									}
								}
								catch (Exception ex)
								{
									Flurry.LogEventWithAParameter("Error deserializing JSon OnlineSettings", "Exception thrown deserializing JSon OnlineSettings", ex.ToString());
									LogError("Download: Downloaded data could not be parsed. Error: " + ex);
									return;
								}
								DateTime expires = response.headers.Get("Expires", DateTime.UtcNow);
								try
								{
									if (_file != null)
									{
										FileUtil.Save(_file, bytes);
										PlayerPrefs.SetString("onlinesettings_expires", expires.ToString("O"));
										PlayerPrefs.SetString("onlinesettings_hash", StringUtility.ToHexString(array));
										PlayerPrefs.Save();
									}
								}
								catch (Exception ex2)
								{
									LogError("Download: Error saving data: " + ex2);
								}
								_data = new Data
								{
									data = serviceResponse.result,
									hash = array,
									expires = expires
								};
								Action action = this.downloaded__BackingField;
								if (action != null)
								{
									action();
								}
							}
						}
					}
				});
		}

		private Data Load()
		{
			Data data;
			try
			{
				if (_file != null)
				{
					byte[] array = FileUtil.Load(_file);
					byte[] array2 = Checksum.ComputeSHA1(array);
					string json = Encoding.UTF8.GetString(array, 0, array.Length);
					ServiceResponse<T> serviceResponse = _jsonSerializer.FromJson<ServiceResponse<T>>(json);
					DateTime expires = DateTime.UtcNow;
					try
					{
						byte[] b = StringUtility.FromHexString(PlayerPrefs.GetString("onlinesettings_hash", null));
						if (FileUtil.ArraysAreEqual(array2, b))
						{
							expires = DateTime.Parse(PlayerPrefs.GetString("onlinesettings_expires", string.Empty));
						}
					}
					catch (Exception)
					{
					}
					data = new Data();
					data.data = serviceResponse.result;
					data.expires = expires;
					data.hash = array2;
					return data;
				}
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception ex3)
			{
				LogError("Load: Error loading cached onlinesettings: " + ex3);
			}
			T val = _jsonSerializer.FromJson<T>("{}");
			if (val == null)
			{
				throw new KilooBackendException("Could not create instance of " + typeof(T).FullName);
			}
			data = new Data();
			data.data = val;
			data.expires = DateTime.UtcNow;
			data.hash = null;
			return data;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[OnlineSettings] " + msg);
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("ONLINE_SETTINGS_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[OnlineSettings] " + msg);
		}
	}
}
