using UnityEngine;

namespace SYBO.Subway.Placeholders
{
	public interface IPlaceholderInitializer
	{
		void Initialize(Placeholder parent, Transform spawnedObject);
	}
}
