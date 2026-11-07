using UnityEngine;

namespace SYBO.Subway.Level
{
	public class Environment : EnvironmentObject
	{
		[SerializeField]
		private EnvironmentKind _environmentKind;

		[Range(2f, 100f)]
		[SerializeField]
		private int _blockCount;

		public EnvironmentKind EnvironmentKind
		{
			get
			{
				return _environmentKind;
			}
		}

		public int BlockCount
		{
			get
			{
				return _blockCount;
			}
		}

		private void OnValidate()
		{
			int min = ((_environmentKind.Type != EnvironmentType.Epic) ? 2 : 12);
			int midPieceLengthInBlocks = EnvironmentInfo.GetByKind(_environmentKind).MidPieceLengthInBlocks;
			_blockCount = Mathf.Clamp(_blockCount, min, int.MaxValue);
			if (midPieceLengthInBlocks > 0)
			{
				_blockCount = _blockCount / midPieceLengthInBlocks * midPieceLengthInBlocks;
			}
		}

		private void OnDrawGizmos()
		{
			if (!(_environmentKind == EnvironmentKind.None))
			{
				float num = 90f * (float)_blockCount;
				float num2 = 80f;
				Gizmos.color = Color.blue;
				Vector3 center = base.transform.position + 0.5f * (num * Vector3.forward + num2 * Vector3.up);
				Vector3 size = new Vector3(60f, num2, num);
				Gizmos.DrawWireCube(center, size);
			}
		}
	}
}
