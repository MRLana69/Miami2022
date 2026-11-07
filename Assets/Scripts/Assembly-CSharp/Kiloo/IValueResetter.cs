namespace Kiloo
{
	public interface IValueResetter<T>
	{
		void Init(T value);

		void ResetValues(T value);
	}
}
