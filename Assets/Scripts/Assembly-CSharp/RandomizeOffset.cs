using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeOffset : MonoBehaviour
{
	[Serializable]
	public class RandomOffsetsData
	{
		public bool left = true;

		public bool mid = true;

		public bool right = true;
	}

	[SerializeField]
	private RandomOffsetsData randomOffsets;

	public static readonly string RandomOffsetsFieldName = "randomOffsets";

	private List<float> _offsets = new List<float>(3);

	public RandomOffsetsData RandomOffsets
	{
		get
		{
			return randomOffsets;
		}
	}

	private void OnEnable()
	{
		ChooseRandomOffset();
	}

	public void ChooseRandomOffset()
	{
		_offsets.Clear();
		if (randomOffsets.left)
		{
			_offsets.Add(-20f);
		}
		if (randomOffsets.mid)
		{
			_offsets.Add(0f);
		}
		if (randomOffsets.right)
		{
			_offsets.Add(20f);
		}
		if (0 < _offsets.Count)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = _offsets[UnityEngine.Random.Range(0, _offsets.Count)];
			base.transform.localPosition = localPosition;
		}
	}

	public void SetRandomOffsetsData(RandomOffsetsData newRandomOffsetsData)
	{
		randomOffsets = newRandomOffsetsData;
		ChooseRandomOffset();
	}
}
