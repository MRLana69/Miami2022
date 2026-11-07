using System;
using UnityEngine;

namespace Kiloo
{
	[Serializable]
	public abstract class BaseTimeDelay : MonoBehaviour
	{
		public abstract float delay { get; }
	}
}
