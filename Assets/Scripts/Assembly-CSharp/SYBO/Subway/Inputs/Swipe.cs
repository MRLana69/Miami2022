using UnityEngine;

namespace SYBO.Subway.Inputs
{
	public class Swipe
	{
		public Vector3 start;

		public Vector3 end;

		public float startTime;

		public float endTime;

		public override string ToString()
		{
			return string.Format("[Swipe (start:{0}, end:{1}]", start, end);
		}
	}
}
