using Kiloo.Common.HTTP;

namespace Kiloo.Common.Serialization
{
	public static class KilooSerializerHTTPExtensions
	{
		public static IRequest WithJsonContent(this IRequest request, object jsonData)
		{
			return request.WithJsonContent(jsonData, new KilooSerializer());
		}

		public static bool TryParseJson<T>(this IResponse response, out T result)
		{
			return response.TryParseJson<T>(out result, new KilooSerializer());
		}

		public static T ParseJson<T>(this IResponse response)
		{
			return response.ParseJson<T>(new KilooSerializer());
		}
	}
}
