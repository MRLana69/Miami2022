namespace Kiloo.Common.Plugins.Notifications
{
	public interface INotification
	{
		string alert { get; }

		int badge { get; }

		string sound { get; }

		object this[string key] { get; }
	}
}
