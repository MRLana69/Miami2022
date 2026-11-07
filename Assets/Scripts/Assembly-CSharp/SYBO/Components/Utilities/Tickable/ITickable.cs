namespace SYBO.Components.Utilities.Tickable
{
	public interface ITickable
	{
		bool IsTickable { get; }

		void OnTick(bool isApplicationFocused, bool isApplicationPaused, float deltaTime);
	}
}
