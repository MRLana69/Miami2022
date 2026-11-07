using System;

namespace SYBO.Subway.Pickups.Pickers
{
	public interface IPicker
	{
		int ExecutionOrder { get; }

		string Name { get; }

		float SpawnDistanceMinToPrevious { get; }

		float ProbabilityToSpawn { get; }

		event Action<PickerRemoveRequestEventArgs> OnRemoveRequest;

		void Add(PickupManager pickupManager);

		bool UpdatePicker();
	}
}
