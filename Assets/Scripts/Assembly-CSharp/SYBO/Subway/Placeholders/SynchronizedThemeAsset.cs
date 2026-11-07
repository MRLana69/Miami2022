using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Placeholders
{
	public class SynchronizedThemeAsset : MonoBehaviour
	{
		[Tooltip("If a component is in the ThemedComponents list it will remain unique after replicating from the generic to themed versions. They will be untouched by replication.")]
		[FormerlySerializedAs("ThemedComponents")]
		[SerializeField]
		private List<Component> _themedComponents;

		[HideInInspector]
		[SerializeField]
		public bool _canSynchronize = true;

		public List<Component> ThemedComponents
		{
			get
			{
				return _themedComponents;
			}
		}

		public bool CanSynchronize
		{
			get
			{
				return _canSynchronize;
			}
			set
			{
				_canSynchronize = value;
			}
		}
	}
}
