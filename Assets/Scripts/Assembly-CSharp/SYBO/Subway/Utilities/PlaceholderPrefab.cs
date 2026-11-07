using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Utilities
{
	public class PlaceholderPrefab : MonoBehaviour, IPrefabBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private TrackObject[] _trackObjects;

		[SerializeField]
		[HideInInspector]
		private Selector[] _selectors;

		public TrackObject[] TrackObjects
		{
			get
			{
				return _trackObjects;
			}
		}

		public Selector[] Selectors
		{
			get
			{
				return _selectors;
			}
		}

		public void Initialize()
		{
			_trackObjects = GetComponentsInChildren<TrackObject>(true);
			_selectors = GetComponentsInChildren<Selector>(true);
		}
	}
}
