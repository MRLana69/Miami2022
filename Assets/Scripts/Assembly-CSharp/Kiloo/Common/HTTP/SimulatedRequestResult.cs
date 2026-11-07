using System;

namespace Kiloo.Common.HTTP
{
	public class SimulatedRequestResult
	{
		public float chance;

		public float latency;

		public RequestState state;

		public Exception exception;

		public IResponse response;
	}
}
