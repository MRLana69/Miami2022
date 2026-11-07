using System;
using JetBrains.Annotations;
using Kiloo.Common.HTTP.Internal;

namespace Kiloo.Common.HTTP
{
	public static class HTTPFactory
	{
		public static IRequest CreateGet([NotNull] Uri uri)
		{
			return CreateRequest("GET", uri);
		}

		public static IRequest CreatePost([NotNull] Uri uri)
		{
			return CreateRequest("POST", uri);
		}

		public static IRequest CreatePut([NotNull] Uri uri)
		{
			return CreateRequest("PUT", uri);
		}

		public static IRequest CreateDelete([NotNull] Uri uri)
		{
			return CreateRequest("DELETE", uri);
		}

		public static IRequest CreateRequest([NotNull] string method, [NotNull] Uri uri, [CanBeNull] byte[] bytes = null)
		{
			return new UnityRequest(method, uri, bytes);
		}

		public static IRequest CreateRequest([NotNull] string method, [NotNull] string uri, [CanBeNull] byte[] bytes = null)
		{
			return CreateRequest(method, new Uri(uri), bytes);
		}

		public static IResponse CreateResponse([NotNull] Headers headers, [CanBeNull] byte[] bytes)
		{
			return new Response(headers, bytes);
		}

		public static IResponse CreateResponse(int status, string message, [NotNull] Headers headers, [CanBeNull] byte[] bytes)
		{
			return new Response(status, message, headers, bytes);
		}
	}
}
