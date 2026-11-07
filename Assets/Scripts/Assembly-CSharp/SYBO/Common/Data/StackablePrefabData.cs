using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Common.Data
{
	[Serializable]
	public class StackablePrefabData
	{
		[SerializeField]
		private List<GameObject> _prefabs;

		[SerializeField]
		private StackableBuildDirection _stackableBuildDirection;

		public List<GameObject> Prefabs
		{
			get
			{
				return _prefabs;
			}
		}

		public StackableBuildDirection StackableBuildDirection
		{
			get
			{
				return _stackableBuildDirection;
			}
		}
	}
}
