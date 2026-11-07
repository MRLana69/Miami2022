using System;
using System.Collections.Generic;
using System.Text;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Benchmarking
{
	public class InstantiatedObjectsMetric : BenchmarkMetric<int>
	{
		private int _instantiatedObjectsCount;

		private bool _reportObjectNames;

		private StringBuilder _instantiatedObjectsString = new StringBuilder();

		private List<string> _stringEntries = new List<string>();

		public InstantiatedObjectsMetric(bool reportObjectNames = false)
		{
			_reportObjectNames = reportObjectNames;
		}

		public override void Initialize(int initialSampleCount)
		{
			base.Initialize(initialSampleCount);
			PoolManager.OnObjectInstantiated = (Action<Transform>)Delegate.Combine(PoolManager.OnObjectInstantiated, new Action<Transform>(PoolManager_OnObjectInstantiated));
		}

		public override void Cleanup()
		{
			base.Cleanup();
			PoolManager.OnObjectInstantiated = (Action<Transform>)Delegate.Remove(PoolManager.OnObjectInstantiated, new Action<Transform>(PoolManager_OnObjectInstantiated));
			_instantiatedObjectsCount = 0;
			_instantiatedObjectsString.Length = 0;
			_stringEntries.Clear();
		}

		private void PoolManager_OnObjectInstantiated(Transform transform)
		{
			_instantiatedObjectsCount++;
			if (_reportObjectNames)
			{
				_instantiatedObjectsString.Append(transform.name);
				_instantiatedObjectsString.Append(',');
			}
		}

		public override void Update(float unscaledDeltaTime)
		{
		}

		public override void Sample()
		{
			_samples.Add(_instantiatedObjectsCount);
			_instantiatedObjectsCount = 0;
			if (_reportObjectNames && _instantiatedObjectsString.Length > 0)
			{
				string item = "{FRAME=" + Time.frameCount + ": " + _instantiatedObjectsString.ToString() + "}";
				_stringEntries.Add(item);
				_instantiatedObjectsString.Length = 0;
			}
		}

		public override List<string> Serialize()
		{
			List<string> list = base.Serialize();
			if (_reportObjectNames)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int count = _stringEntries.Count;
				for (int i = 0; i < count; i++)
				{
					stringBuilder.Append(_stringEntries[i]);
					if (i < count - 1)
					{
						stringBuilder.Append(Benchmark.SeparatorCharacter);
					}
				}
				list.Add(stringBuilder.ToString());
			}
			return list;
		}
	}
}
