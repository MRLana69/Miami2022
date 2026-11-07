namespace Kiloo.Common.Utility
{
	public interface IFileSaver
	{
		void Save(string file, object obj);

		T Load<T>(string file);

		bool TryLoad<T>(string file, out T obj);
	}
}
