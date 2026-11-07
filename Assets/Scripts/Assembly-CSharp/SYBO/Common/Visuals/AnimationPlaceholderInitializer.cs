using SYBO.Subway.Placeholders;
using SYBO.Subway.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	[RequireComponent(typeof(PlaceholderPrefabProviderBase))]
	[RequireComponent(typeof(Placeholder))]
	public class AnimationPlaceholderInitializer : MonoBehaviour, IPlaceholderInitializer
	{
		[Tooltip("Min and max value of randomized animation start time. Clamped based on default animation clip length on target prefab.")]
		[SerializeField]
		private AnimationInitializer.Config _config;

		public void Initialize(Placeholder parent, Transform spawnedObject)
		{
			AnimationInitializer component = spawnedObject.GetComponent<AnimationInitializer>();
			component.InitializeAnimation(_config);
		}
	}
}
