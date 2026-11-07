namespace Kiloo.Common.HTTP
{
	public static class HTTPStatus
	{
		public const int CONTINUE = 100;

		public const int SWITCHING_PROTOCOLS = 101;

		public const int OK = 200;

		public const int CREATED = 201;

		public const int ACCEPTED = 202;

		public const int NON_AUTHORITATIVE_INFORMATION = 203;

		public const int NO_CONTENT = 204;

		public const int RESET_CONTENT = 205;

		public const int PARTIAL_CONTENT = 206;

		public const int MULTIPLE_CHOICES = 300;

		public const int MOVED_PERMANENTLY = 301;

		public const int FOUND = 302;

		public const int SEE_OTHER = 303;

		public const int NOT_MODIFIED = 304;

		public const int USE_PROXY = 305;

		public const int TEMPORARY_REDIRECT = 307;

		public const int BAD_REQUEST = 400;

		public const int UNAUTHORIZED = 401;

		public const int PAYMENT_REQUIRED = 402;

		public const int FORBIDDEN = 403;

		public const int NOT_FOUND = 404;

		public const int METHOD_NOT_ALLOWED = 405;

		public const int NOT_ACCEPTABLE = 406;

		public const int PROXY_AUTHENTICATION_REQUIRED = 407;

		public const int REQUEST_TIMEOUT = 408;

		public const int CONFLICT = 409;

		public const int GONE = 410;

		public const int LENGTH_REQUIRED = 411;

		public const int PRECONDITION_FAILED = 412;

		public const int REQUEST_ENTITY_TOO_LARGE = 413;

		public const int REQUEST_URI_TOO_LONG = 414;

		public const int UNSUPPORTED_MEDIA_TYPE = 415;

		public const int REQUESTED_RANGE_NOT_SATISFIABLE = 416;

		public const int EXPECTATION_FAILED = 417;

		public const int INTERNAL_SERVER_ERROR = 500;

		public const int NOT_IMPLEMENTED = 501;

		public const int BAD_GATEWAY = 502;

		public const int SERVICE_UNAVAILABLE = 503;

		public const int GATEWAY_TIMEOUT = 504;

		public const int HTTP_VERSION_NOT_SUPPORTED = 505;

		public static string ToString(int status)
		{
			switch (status)
			{
			case 100:
				return "Continue";
			case 101:
				return "Switching Protocols";
			case 200:
				return "OK";
			case 201:
				return "Created";
			case 202:
				return "Accepted";
			case 203:
				return "Non-Authoritative Information";
			case 204:
				return "No Content";
			case 205:
				return "Reset Content";
			case 206:
				return "Partial Content";
			case 300:
				return "Multiple Choices";
			case 301:
				return "Moved Permanently";
			case 302:
				return "Found";
			case 303:
				return "See Other";
			case 304:
				return "Not Modified";
			case 305:
				return "Use Proxy";
			case 307:
				return "Temporary Redirect";
			case 400:
				return "Bad Request";
			case 401:
				return "Unauthorized";
			case 402:
				return "Payment Required";
			case 403:
				return "Forbidden";
			case 404:
				return "Not Found";
			case 405:
				return "Method Not Allowed";
			case 406:
				return "Not Acceptable";
			case 407:
				return "Proxy Authentication Required";
			case 408:
				return "Request Time-out";
			case 409:
				return "Conflict";
			case 410:
				return "Gone";
			case 411:
				return "Length Required";
			case 412:
				return "Precondition Failed";
			case 413:
				return "Request Entity Too Large";
			case 414:
				return "Request-URI Too Long";
			case 415:
				return "Unsupported Media Type";
			case 416:
				return "Requested range not satisfiable";
			case 417:
				return "Expectation Failed";
			case 500:
				return "Internal Server Error";
			case 501:
				return "Not Implemented";
			case 502:
				return "Bad Gateway";
			case 503:
				return "Service Unavailable";
			case 504:
				return "Gateway Time-out";
			case 505:
				return "HTTP Version not supported";
			default:
				return "Unknown " + status;
			}
		}
	}
}
