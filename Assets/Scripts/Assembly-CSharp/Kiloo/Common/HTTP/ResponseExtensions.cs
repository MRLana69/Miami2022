using System;
using JetBrains.Annotations;
using Kiloo.Common.HTTP.Internal;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.HTTP
{
	public static class ResponseExtensions
	{
		public static bool IsJson(this IResponse response)
		{
			string text = response.headers["Content-Type"];
			return text.Contains("json") || text.Contains("javascript");
		}

		public static bool IsBson(this IResponse response)
		{
			string text = response.headers["Content-Type"];
			return text.Contains("application/bson");
		}

		public static bool IsTexture(this IResponse response)
		{
			string text = response.headers["Content-Type"];
			return text.Equals("image/jpeg") || text.Equals("image/png");
		}

		[CanBeNull]
		public static Texture2D AsTexture(this IResponse response)
		{
			byte[] bytes = response.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				Texture2D texture2D = new Texture2D(4, 4);
				if (texture2D.LoadImage(bytes))
				{
					return texture2D;
				}
			}
			return null;
		}

		[CanBeNull]
		public static string AsString(this IResponse response)
		{
			return Util.ContentAsString(response.bytes, response.headers);
		}

		public static bool TryParseJson<T>(this IResponse response, out T result, IJsonSerializer jsonSerializer)
		{
			try
			{
				result = response.ParseJson<T>(jsonSerializer);
				return true;
			}
			catch (Exception)
			{
			}
			result = default(T);
			return false;
		}

		public static T ParseJson<T>(this IResponse response, IJsonSerializer jsonSerializer)
		{
			string text = response.AsString();
			if (string.IsNullOrEmpty(text))
			{
				throw new FormatException("Response data is null or empty.");
			}
			return jsonSerializer.FromJson<T>(text);
		}

		public static bool TryParseBson<T>(this IResponse response, out T result, IBsonSerializer bsonSerializer)
		{
			try
			{
				result = response.ParseBson<T>(bsonSerializer);
				return true;
			}
			catch (Exception)
			{
			}
			result = default(T);
			return false;
		}

		public static T ParseBson<T>(this IResponse response, IBsonSerializer bsonSerializer)
		{
			if (response.bytes == null)
			{
				throw new FormatException("Response bytes are null.");
			}
			return bsonSerializer.FromBson<T>(response.bytes);
		}
	}
}
