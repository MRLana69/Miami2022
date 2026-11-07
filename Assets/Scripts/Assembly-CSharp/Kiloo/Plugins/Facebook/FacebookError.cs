using System;
using System.Collections.Generic;

namespace Kiloo.Plugins.Facebook
{
	public class FacebookError
	{
		public enum Category
		{
			INVALID = 0,
			AUTHENTICATION_RETRY = 1,
			AUTHENTICATION_REOPEN_SESSION = 2,
			PERMISSION = 3,
			SERVER = 4,
			THROTTLING = 5,
			CANCELED = 6,
			OTHER = 7,
			BAD_REQUEST = 8,
			CLIENT = 9
		}

		public const int INVALID_ERROR_CODE = -1;

		public const int INVALID_HTTP_STATUS_CODE = -1;

		public Category category;

		public int requestStatusCode = -1;

		public int errorCode = -1;

		public int subErrorCode = -1;

		public string errorType;

		public string errorMessage;

		public bool shouldNotifyUser;

		public string userActionMessage;

		public string exceptionType;

		public string exceptionMessage;

		public FacebookError()
		{
		}

		public FacebookError(IDictionary<string, object> error)
		{
			object value;
			if (error.TryGetValue("category", out value))
			{
				try
				{
					category = (Category)Enum.Parse(typeof(Category), (string)value);
				}
				catch (Exception)
				{
				}
			}
			if (error.TryGetValue("requeststatuscode", out value))
			{
				requestStatusCode = (int)(long)value;
			}
			if (error.TryGetValue("errorcode", out value))
			{
				errorCode = (int)(long)value;
			}
			if (error.TryGetValue("suberrorcode", out value))
			{
				subErrorCode = (int)(long)value;
			}
			if (error.TryGetValue("errortype", out value))
			{
				errorType = (string)value;
			}
			if (error.TryGetValue("errormessage", out value))
			{
				errorMessage = (string)value;
			}
			if (error.TryGetValue("shouldnotifyuser", out value))
			{
				shouldNotifyUser = (bool)value;
			}
			if (error.TryGetValue("useractionmessage", out value))
			{
				userActionMessage = (string)value;
			}
			if (error.TryGetValue("exceptiontype", out value))
			{
				exceptionType = (string)value;
			}
			if (error.TryGetValue("exceptionmessage", out value))
			{
				exceptionMessage = (string)value;
			}
		}

		public override string ToString()
		{
			return string.Format("FacebookError( category={0},\n    requestStatusCode={1},\n    errorCode={2},\n    subErrorCode={3},\n    errorType=\"{4}\",\n    errorMessage=\"{5}\",\n    shouldNotifyUser={6},\n    userActionMessage=\"{7}\",\n    exceptionType=\"{8}\",\n    exceptionMessage=\"{9}\" )", category, requestStatusCode, errorCode, subErrorCode, errorType, errorMessage, shouldNotifyUser, userActionMessage, exceptionType, exceptionMessage);
		}
	}
}
