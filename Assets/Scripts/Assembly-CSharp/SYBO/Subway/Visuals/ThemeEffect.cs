using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public abstract class ThemeEffect : ScriptableObject
	{
		public abstract void Initialize(Transform root);

		public abstract void UpdateEffect();
	}
}
