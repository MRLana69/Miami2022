namespace Kiloo.Common.JsonUtil
{
	public interface IJsonConverter<T>
	{
		string SerializeObject(T value);

		T DeserializeObject(string value);
	}
}
