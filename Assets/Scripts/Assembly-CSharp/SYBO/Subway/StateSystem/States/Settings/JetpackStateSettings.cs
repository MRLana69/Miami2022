using UnityEngine;

namespace SYBO.Subway.StateSystem.States.Settings
{
	public class JetpackStateSettings : ScriptableSingleton<JetpackStateSettings>
	{
		[SerializeField]
		private float _speedup = 2f;

		[SerializeField]
		private float _coinOffset = 200f;

		public float Speedup
		{
			get
			{
				return _speedup;
			}
		}

		public float CoinOffset
		{
			get
			{
				return _coinOffset;
			}
		}
	}
}
