using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeightList
{
	[Serializable]
	public class WeightEntry
	{
		public Transform Transform;

		public Vector2 Weight;

		public WeightEntry(Transform transform, Vector2 weight)
		{
			Transform = transform;
			Weight = weight;
		}

		public WeightEntry(Transform transform)
			: this(transform, Vector2.zero)
		{
		}
	}

	[SerializeField]
	private List<WeightEntry> _entries = new List<WeightEntry>();

	public List<WeightEntry> Entries
	{
		get
		{
			return new List<WeightEntry>(_entries);
		}
	}

	public List<Vector2> CalculateProportionalWeights()
	{
		Vector2 zero = Vector2.zero;
		List<Vector2> list = new List<Vector2>();
		foreach (WeightEntry entry in Entries)
		{
			if (!entry.Transform.gameObject.activeSelf)
			{
				list.Add(Vector2.zero);
				continue;
			}
			zero.x += entry.Weight.x;
			zero.y += entry.Weight.y;
			list.Add(entry.Weight);
		}
		for (int i = 0; i < list.Count; i++)
		{
			Vector2 value = list[i];
			value.x /= zero.x;
			value.y /= zero.y;
			list[i] = value;
		}
		return list;
	}

	private bool WeightEntryExists(Transform transform)
	{
		foreach (WeightEntry entry in _entries)
		{
			if (entry.Transform == transform)
			{
				return true;
			}
		}
		return false;
	}

	private void AddWeightEntry(Transform transform)
	{
		if (WeightEntryExists(transform))
		{
			Debug.LogError("Trying to add weight entry that already exists.");
		}
		else
		{
			_entries.Add(new WeightEntry(transform));
		}
	}

	public Vector2 WeightForTransform(Transform transform)
	{
		foreach (WeightEntry entry in _entries)
		{
			if (entry.Transform == transform)
			{
				return entry.Weight;
			}
		}
		return Vector2.zero;
	}

	public void SyncList(Transform layout)
	{
		for (int i = 0; i < layout.transform.childCount; i++)
		{
			Transform child = layout.transform.GetChild(i);
			if (!WeightEntryExists(child))
			{
				AddWeightEntry(child);
			}
		}
		_entries.RemoveAll((WeightEntry obj) => obj.Transform == null);
	}
}
