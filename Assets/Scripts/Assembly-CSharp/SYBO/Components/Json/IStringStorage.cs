namespace SYBO.Components.Json
{
	public interface IStringStorage
	{
		bool CanRead { get; }

		bool Overwrite(string content);

		string Read();
	}
}
