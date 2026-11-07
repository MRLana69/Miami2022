namespace Kiloo.Common.HTTP.Internal
{
	internal class Response : IResponse
	{
		public int status { get; internal set; }

		public string message { get; internal set; }

		public byte[] bytes { get; internal set; }

		public Headers headers { get; internal set; }

		public ResponseCategory category
		{
			get
			{
				return Util.GetResponseCategory(status);
			}
		}

		public Response(int status, string message, Headers headers, byte[] bytes)
		{
			this.status = status;
			this.message = message;
			this.headers = headers;
			this.bytes = bytes;
		}

		public Response(Headers headers, byte[] bytes)
			: this(200, "OK", headers, bytes)
		{
		}
	}
}
