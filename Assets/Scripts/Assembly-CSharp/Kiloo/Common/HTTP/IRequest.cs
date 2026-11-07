using System;
using JetBrains.Annotations;

namespace Kiloo.Common.HTTP
{
	public interface IRequest
	{
		string method { get; }

		byte[] bytes { get; set; }

		Uri uri { get; set; }

		IResponse response { get; set; }

		uint maxRedirectCount { get; set; }

		bool acceptGzip { get; set; }

		Exception exception { get; set; }

		RequestState state { get; set; }

		bool ignoreContentMD5 { get; set; }

		bool ignoreCertificateErrors { get; set; }

		float timeout { get; set; }

		float keepAlive { get; set; }

		float minBackOffTime { get; set; }

		float maxBackOffTime { get; set; }

		Headers headers { get; }

		bool isFinished { get; }

		bool isSuccessful { get; }

		[NotNull]
		string statusMessage { get; }

		void PrepareFetchResponse();

		void FetchResponse();

		void FetchResponseAsync();
	}
}
