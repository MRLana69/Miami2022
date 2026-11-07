using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Serialization;
using Kiloo.Common.Serialization.Parsing;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Plugins.Facebook
{
	public class FacebookSession : MonoBehaviour
	{
		public enum State
		{
			CREATED = 0,
			CREATED_TOKEN_LOADED = 1,
			OPENING = 2,
			OPENED = 3,
			OPENED_TOKEN_UPDATED = 4,
			CLOSED_LOGIN_FAILED = 5,
			CLOSED = 6
		}

		public delegate void StatusChangedHandler(State status, FacebookError error);

		public delegate void DialogCompleteHandler(IDictionary<string, object> result, FacebookError error);

		public delegate void RequestCompleteHandler(IDictionary<string, object> result, FacebookError error, bool isFromCache);

		private static readonly IJsonSerializer _jsonConverter = new KilooSerializer();

		private static FacebookSession _instance = null;

		private int _id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static StatusChangedHandler statusChangedEvent__BackingField;

		private AndroidJavaObject _facebookSessionJavaObject;

		private static FacebookLogLevel _loggingLevel = FacebookLogLevel.WARN;

		public static string appId { get; set; }

		public static bool isSupported
		{
			get
			{
				return _IsSupported();
			}
		}

		public static bool dialogsSupported
		{
			get
			{
				return true;
			}
		}

		public static FacebookSession instance
		{
			get
			{
				return _instance;
			}
		}

		public static event StatusChangedHandler statusChangedEvent
		{
			add
			{
				StatusChangedHandler statusChangedHandler = statusChangedEvent__BackingField;
				StatusChangedHandler statusChangedHandler2;
				do
				{
					statusChangedHandler2 = statusChangedHandler;
					statusChangedHandler = Interlocked.CompareExchange(ref statusChangedEvent__BackingField, (StatusChangedHandler)Delegate.Combine(statusChangedHandler2, value), statusChangedHandler);
				}
				while ((object)statusChangedHandler != statusChangedHandler2);
			}
			remove
			{
				StatusChangedHandler statusChangedHandler = statusChangedEvent__BackingField;
				StatusChangedHandler statusChangedHandler2;
				do
				{
					statusChangedHandler2 = statusChangedHandler;
					statusChangedHandler = Interlocked.CompareExchange(ref statusChangedEvent__BackingField, (StatusChangedHandler)Delegate.Remove(statusChangedHandler2, value), statusChangedHandler);
				}
				while ((object)statusChangedHandler != statusChangedHandler2);
			}
		}

		public static FacebookSession Create(string permissions, FacebookDefaultAudience defaultAudience)
		{
			if (_instance != null && IsClosed(_instance.GetState()))
			{
				PluginUtil.RemoveHandlers(_instance);
				UnityEngine.Object.Destroy(_instance.gameObject);
				_instance = null;
			}
			if (_instance == null)
			{
				FacebookSession facebookSession = PluginUtil.CreateBridgeObject<FacebookSession>();
				facebookSession._id = PluginUtil.GetNextTicket();
				if (!facebookSession._Init(permissions, defaultAudience.ToString()))
				{
					if (!Application.isEditor)
					{
						LogError("FacebookSession.Create: Failed!");
					}
					UnityEngine.Object.Destroy(facebookSession.gameObject);
				}
				_instance = facebookSession;
				return _instance;
			}
			if (!Application.isEditor)
			{
				LogError("FacebookSession.Create: We already have a FacebookSession which is not closed. Call ignored.");
			}
			return null;
		}

		public static void SetLoggingLevel(FacebookLogLevel level)
		{
			_SetLoggingLevel(level);
		}

		public void OpenForRead()
		{
			if (!_OpenForRead())
			{
				LogError("FacebookSession.OpenForRead: Error");
			}
		}

		public void Cancel()
		{
			_Cancel();
		}

		public void Close(bool clearToken)
		{
			_Close(clearToken);
		}

		public State GetState()
		{
			return (State)Enum.Parse(typeof(State), _GetState());
		}

		public static bool IsOpened(State state)
		{
			return state == State.OPENED || state == State.OPENED_TOKEN_UPDATED;
		}

		public static bool IsClosed(State state)
		{
			return state == State.CLOSED || state == State.CLOSED_LOGIN_FAILED;
		}

		public static bool HasOpenInstance()
		{
			return instance != null && IsOpened(instance.GetState());
		}

		public string GetAccessToken()
		{
			return _GetAccessToken();
		}

		public DateTime? GetExpirationDateUTC()
		{
			long num = _GetExpirationDate();
			if (num != -1)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num);
			}
			return null;
		}

		public string GetPermissions()
		{
			return _GetPermissions();
		}

		public FacebookLoginType GetLoginType()
		{
			return (FacebookLoginType)Enum.Parse(typeof(FacebookLoginType), _GetLoginType());
		}

		public void RequestNewReadPermissions(string permissions)
		{
			if (!_RequestNewReadPermissions(permissions))
			{
				LogError("FacebookSession.RequestNewReadPermissions: Error");
			}
		}

		public void RequestNewPublishPermissions(string permissions, FacebookDefaultAudience defaultAudience)
		{
			if (!_RequestNewPublishPermissions(permissions, defaultAudience.ToString()))
			{
				LogError("FacebookSession.RequestNewPublishPermissions: Error");
			}
		}

		public bool CanShowNativeShareDialog()
		{
			return _CanShowNativeShareDialog();
		}

		public void ShowNativeShareDialog(IDictionary<string, object> parameters, DialogCompleteHandler dialogCompleteHandler)
		{
			int num = PluginUtil.CheckInHandler(dialogCompleteHandler, this);
			string paramsJSON = _jsonConverter.ToJson(parameters);
			if (!_ShowNativeShareDialog(paramsJSON, num))
			{
				LogError("FacebookSession.ShowNativeShareDialog: Error");
				PluginUtil.CheckOutHandler(num);
			}
		}

		public void ShowDialog(string action, IDictionary<string, object> parameters, DialogCompleteHandler dialogCompleteHandler)
		{
			int num = PluginUtil.CheckInHandler(dialogCompleteHandler, this);
			string paramsJSON = _jsonConverter.ToJson(parameters);
			if (!_ShowDialog(action, paramsJSON, num))
			{
				LogError("FacebookSession.ShowDialog: Error");
				PluginUtil.CheckOutHandler(num);
			}
		}

		public void GraphRequest(string graphPath, IDictionary<string, object> parameters, HttpMethod httpMethod, RequestCompleteHandler requestCompleteHandler)
		{
			int num = PluginUtil.CheckInHandler(requestCompleteHandler, this);
			if (!_GraphRequest(graphPath, parameters, httpMethod.ToString(), num))
			{
				LogError("FacebookSession.GraphRequest: Error");
				PluginUtil.CheckOutHandler(num);
			}
		}

		public void UploadPhotoRequest(Texture2D image, string graphPath, string caption, RequestCompleteHandler requestCompleteHandler)
		{
			int num = PluginUtil.CheckInHandler(requestCompleteHandler, this);
			if (graphPath == null)
			{
				graphPath = "me/photos";
			}
			if (!_UploadPhotoRequest(image, graphPath, caption, num))
			{
				LogError("FacebookSession.UploadPhotoRequest: Error");
				PluginUtil.CheckOutHandler(num);
			}
		}

		public void UploadPhotoRequest(string file, string graphPath, string caption, RequestCompleteHandler requestCompleteHandler)
		{
			int num = PluginUtil.CheckInHandler(requestCompleteHandler, this);
			if (!_UploadPhotoRequest(file, graphPath, caption, num))
			{
				LogError("FacebookSession.UploadPhotoRequest: Error");
				PluginUtil.CheckOutHandler(num);
			}
		}

		public string GetAndroidKeyHash()
		{
			return _GetAndroidKeyHash();
		}

		public void Test()
		{
			_Test();
		}

		[UsedImplicitly]
		private void OnFacebookSessionStateChangedWithObject(string state, IDictionary<string, object> error)
		{
			StatusChangedHandler statusChangedHandler = statusChangedEvent__BackingField;
			if (statusChangedHandler != null)
			{
				State status = State.CREATED;
				FacebookError error2 = null;
				try
				{
					status = (State)Enum.Parse(typeof(State), state);
				}
				catch (Exception)
				{
				}
				if (error != null)
				{
					error2 = new FacebookError(error);
				}
				statusChangedHandler(status, error2);
			}
		}

		[UsedImplicitly]
		private void OnFacebookSessionStateChanged(string json)
		{
			StatusChangedHandler statusChangedHandler = statusChangedEvent__BackingField;
			if (statusChangedHandler == null)
			{
				return;
			}
			State status = State.CREATED;
			FacebookError error = null;
			try
			{
				IDictionary<string, object> dictionary = _jsonConverter.FromJson<IDictionary<string, object>>(json);
				if (dictionary != null)
				{
					int num = (int)(long)dictionary["cookie"];
					if (num == _id)
					{
						object value = dictionary["state"];
						status = (State)Enum.Parse(typeof(State), (string)value);
						if (dictionary.TryGetValue("error", out value))
						{
							IDictionary<string, object> dictionary2 = value as IDictionary<string, object>;
							if (dictionary2 != null)
							{
								error = new FacebookError(dictionary2);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogError("FacebookSession.OnFacebookSessionStateChanged: Invalid json: " + ex.Message);
				return;
			}
			statusChangedHandler(status, error);
		}

		[UsedImplicitly]
		private void OnFacebookSessionDialogComplete(string json)
		{
			IDictionary<string, object> dictionary;
			int ticket;
			try
			{
				dictionary = _jsonConverter.FromJson<IDictionary<string, object>>(json);
				ticket = (int)(long)dictionary["cookie"];
			}
			catch (Exception ex)
			{
				LogError("FacebookSession.OnFacebookSessionDialogComplete: Invalid json: " + ex);
				return;
			}
			DialogCompleteHandler dialogCompleteHandler = PluginUtil.CheckOutHandler(ticket) as DialogCompleteHandler;
			if (dialogCompleteHandler == null)
			{
				return;
			}
			FacebookError error = null;
			object value;
			if (dictionary.TryGetValue("error", out value))
			{
				IDictionary<string, object> dictionary2 = value as IDictionary<string, object>;
				if (dictionary2 != null)
				{
					error = new FacebookError(dictionary2);
				}
			}
			IDictionary<string, object> result = null;
			if (dictionary.TryGetValue("result", out value))
			{
				result = value as IDictionary<string, object>;
			}
			dialogCompleteHandler(result, error);
		}

		private void OnFacebookSessionRequestCompleteWithObject(IDictionary<string, object> result, IDictionary<string, object> error, int cookie)
		{
			RequestCompleteHandler requestCompleteHandler = PluginUtil.CheckOutHandler(cookie) as RequestCompleteHandler;
			if (requestCompleteHandler != null)
			{
				FacebookError error2 = ((error != null) ? new FacebookError(error) : null);
				requestCompleteHandler(result, error2, false);
			}
		}

		[UsedImplicitly]
		private void OnFacebookSessionRequestComplete(string json)
		{
			IDictionary<string, object> dictionary;
			int ticket;
			try
			{
				dictionary = _jsonConverter.FromJson<IDictionary<string, object>>(json);
				ticket = (int)(long)dictionary["cookie"];
			}
			catch (Exception ex)
			{
				LogError("FacebookSession.OnFacebookSessionGraphRequestComplete: Invalid json: " + ex);
				return;
			}
			RequestCompleteHandler requestCompleteHandler = PluginUtil.CheckOutHandler(ticket) as RequestCompleteHandler;
			if (requestCompleteHandler == null)
			{
				return;
			}
			bool isFromCache = false;
			object value;
			if (dictionary.TryGetValue("isfromcache", out value))
			{
				isFromCache = (bool)value;
			}
			FacebookError error = null;
			if (dictionary.TryGetValue("error", out value))
			{
				IDictionary<string, object> dictionary2 = value as IDictionary<string, object>;
				if (dictionary2 != null)
				{
					error = new FacebookError(dictionary2);
				}
			}
			IDictionary<string, object> result = null;
			if (dictionary.TryGetValue("result", out value))
			{
				result = value as IDictionary<string, object>;
			}
			requestCompleteHandler(result, error, isFromCache);
		}

		[UsedImplicitly]
		private void Start()
		{
			if (this != _instance)
			{
				LogError("FacebookSession object in scene that is not the same as the _instance. Probably leaked from editor. Fix the scene please!");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private static bool _IsSupported()
		{
			return true;
		}

		private bool _Init(string permissions, string defaultAudience)
		{
			if (_facebookSessionJavaObject != null)
			{
				_facebookSessionJavaObject.Dispose();
			}
			_facebookSessionJavaObject = new AndroidJavaObject("com.kiloo.unityplugins.FacebookSession");
			_facebookSessionJavaObject.Call("setLoggingLevel", (int)_loggingLevel);
			if (permissions == null)
			{
				permissions = string.Empty;
			}
			return _facebookSessionJavaObject.Call<bool>("init", new object[4] { permissions, defaultAudience, base.name, _id });
		}

		private static void _SetLoggingLevel(FacebookLogLevel level)
		{
			_loggingLevel = level;
			if (_instance != null && _instance._facebookSessionJavaObject != null)
			{
				_instance._facebookSessionJavaObject.Call("setLoggingLevel", (int)level);
			}
		}

		private bool _OpenForRead()
		{
			return _facebookSessionJavaObject.Call<bool>("openForRead", new object[0]);
		}

		private void _Cancel()
		{
		}

		private void _Close(bool clearToken)
		{
			_facebookSessionJavaObject.Call("close", clearToken);
		}

		private string _GetState()
		{
			return _facebookSessionJavaObject.Call<string>("getState", new object[0]);
		}

		private string _GetAccessToken()
		{
			return _facebookSessionJavaObject.Call<string>("getAccessToken", new object[0]);
		}

		private long _GetExpirationDate()
		{
			return _facebookSessionJavaObject.Call<long>("getExpirationDate", new object[0]);
		}

		private string _GetLoginType()
		{
			return "NONE";
		}

		private string _GetPermissions()
		{
			return _facebookSessionJavaObject.Call<string>("getPermissions", new object[0]);
		}

		public bool _RequestNewReadPermissions(string permissions)
		{
			return _facebookSessionJavaObject.Call<bool>("requestNewReadPermissions", new object[1] { permissions });
		}

		private bool _RequestNewPublishPermissions(string permissions, string defaultAudience)
		{
			return _facebookSessionJavaObject.Call<bool>("requestNewPublishPermissions", new object[2] { permissions, defaultAudience });
		}

		private bool _CanShowNativeShareDialog()
		{
			return false;
		}

		private bool _ShowNativeShareDialog(string paramsJSON, int cookie)
		{
			return false;
		}

		private bool _ShowNativeShareDialogWithOpenGraphAction(string paramsJSON, int cookie)
		{
			return false;
		}

		private bool _ShowDialog(string action, string paramsJSON, int cookie)
		{
			return _facebookSessionJavaObject.Call<bool>("showDialog", new object[3] { action, paramsJSON, cookie });
		}

		private bool _GraphRequest(string graphPath, IDictionary<string, object> parameters, string httpMethod, int cookie)
		{
			string text = KilooJson.Serialize(parameters, null);
			return _facebookSessionJavaObject.Call<bool>("graphRequest", new object[4] { graphPath, text, httpMethod, cookie });
		}

		private bool _UploadPhotoRequest(Texture2D image, string graphPath, string caption, int cookie)
		{
			byte[] array = image.EncodeToPNG();
			return _facebookSessionJavaObject.Call<bool>("uploadPhotoRequestWithImageData", new object[4] { array, graphPath, caption, cookie });
		}

		private bool _UploadPhotoRequest(string file, string graphPath, string caption, int cookie)
		{
			return _facebookSessionJavaObject.Call<bool>("uploadPhotoRequestWithFile", new object[4] { file, graphPath, caption, cookie });
		}

		private string _GetAndroidKeyHash()
		{
			return _facebookSessionJavaObject.Call<string>("getKeyHash", new object[0]);
		}

		private void _Test()
		{
			_facebookSessionJavaObject.Call("test");
		}

		private void EnableInput(bool enable)
		{
			if (enable)
			{
				UIScreenController.Instance.HideAdsOverlay();
			}
			else
			{
				UIScreenController.Instance.ShowAdsOverlay();
			}
		}

		private static void LogError(string msg)
		{
		}

		private static void LogWarning(string msg)
		{
			if (!Globals.DEBUG_FACEBOOK_SESSION_WARNINGS)
			{
			}
		}

		[Conditional("FACEBOOK_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
