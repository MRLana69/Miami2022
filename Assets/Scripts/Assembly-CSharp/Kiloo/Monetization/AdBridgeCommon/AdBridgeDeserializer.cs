using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public static class AdBridgeDeserializer
	{
		private static KilooSerializer _kilooSerializer;

		static AdBridgeDeserializer()
		{
			_kilooSerializer = new KilooSerializer();
		}

		public static T Deserialize<T>(string serializedMessage)
		{
			return _kilooSerializer.FromJson<T>(serializedMessage);
		}
	}
}
