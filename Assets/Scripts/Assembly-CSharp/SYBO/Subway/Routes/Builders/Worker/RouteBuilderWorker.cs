using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Routes.Builders.Worker
{
	public class RouteBuilderWorker
	{
		private class WorkWrapper
		{
			private readonly Stack<IEnumerator> _workStack;

			public readonly RouteChunk Chunk;

			public readonly WorkType WorkType;

			private int _workIteration;

			public bool IsDone
			{
				get
				{
					return _workStack.Count == 0;
				}
			}

			public WorkWrapper(RouteChunk chunk, Stack<IEnumerator> workStack, WorkType workType)
			{
				_workStack = workStack;
				Chunk = chunk;
				WorkType = workType;
			}

			public bool Work()
			{
				if (IsDone)
				{
					return true;
				}
				IEnumerator enumerator = _workStack.Peek();
				if (!enumerator.MoveNext())
				{
					_workStack.Pop();
				}
				else if (enumerator.Current is IEnumerator)
				{
					_workStack.Push(enumerator.Current as IEnumerator);
				}
				_workIteration++;
				return _workStack.Count == 0;
			}

			public void WorkUntilDone()
			{
				do
				{
					Work();
				}
				while (!IsDone);
			}
		}

		private enum WorkType
		{
			Spawning = 0,
			Despawning = 1
		}

		private const float WorkSpeedupDistanceFactorDefault = 0f;

		private const float WorkSpeedupDistanceFactorMax = 1f;

		private const float WorkTimeAllowanceMin = 0.002f;

		private const float WorkTimeAllowanceMax = 0.005f;

		private const int SpawnSpeedupDistance = 100;

		private const int CleanupSpeedupDistance = 1830;

		private Dictionary<RouteChunk, WorkWrapper> _chunkToWorkWrapperMap = new Dictionary<RouteChunk, WorkWrapper>();

		private Queue<WorkWrapper> _spawnWorkQueue = new Queue<WorkWrapper>();

		private Queue<WorkWrapper> _despawnWorkQueue = new Queue<WorkWrapper>();

		private Queue<WorkWrapper> _activeWorkQueue;

		public RouteBuilderWorker()
		{
			_activeWorkQueue = _despawnWorkQueue;
		}

		public void EnqueueSpawnWork(RouteChunk chunk, IEnumerator workRoutine, bool instantComplete = false)
		{
			EnqueueWork(chunk, workRoutine, WorkType.Spawning, instantComplete);
		}

		public void EnqueueDespawnWork(RouteChunk chunk, IEnumerator workRoutine, bool instantComplete = false)
		{
			EnqueueWork(chunk, workRoutine, WorkType.Despawning, instantComplete);
		}

		private void EnqueueWork(RouteChunk chunk, IEnumerator workRoutine, WorkType workType, bool instantComplete = false)
		{
			Stack<IEnumerator> stack = new Stack<IEnumerator>();
			stack.Push(workRoutine);
			WorkWrapper workWrapper = new WorkWrapper(chunk, stack, workType);
			WorkWrapper value = null;
			_chunkToWorkWrapperMap.TryGetValue(chunk, out value);
			if (value != null)
			{
				value.WorkUntilDone();
				_chunkToWorkWrapperMap.Remove(chunk);
			}
			if (instantComplete)
			{
				workWrapper.WorkUntilDone();
				return;
			}
			_chunkToWorkWrapperMap[chunk] = workWrapper;
			Queue<WorkWrapper> queue = ((workType != WorkType.Spawning) ? _despawnWorkQueue : _spawnWorkQueue);
			queue.Enqueue(workWrapper);
		}

		private void SwitchActiveWorkQueue()
		{
			if (_activeWorkQueue == _spawnWorkQueue && 0 < _despawnWorkQueue.Count)
			{
				_activeWorkQueue = _despawnWorkQueue;
			}
			else if (_activeWorkQueue == _despawnWorkQueue)
			{
				_activeWorkQueue = _spawnWorkQueue;
			}
		}

		private float ComputeDistanceFactor(float chunkEdgeZ, float requiredDonePositionZ, float workRampupDistance)
		{
			float result = 0f;
			float num = chunkEdgeZ - requiredDonePositionZ;
			if (num < 0f)
			{
				result = 1f;
			}
			else if (num < workRampupDistance)
			{
				result = (workRampupDistance - num) / workRampupDistance;
			}
			return result;
		}

		public void UpdateWorker(float reqPositionReadyGameZ, float reqPositionCleanedUpGameZ)
		{
			SwitchActiveWorkQueue();
			if (_activeWorkQueue.Count == 0)
			{
				return;
			}
			WorkWrapper workWrapper = _activeWorkQueue.Peek();
			float a = ((workWrapper.WorkType != WorkType.Spawning) ? ComputeDistanceFactor(workWrapper.Chunk.GamePositionEnd.z, reqPositionCleanedUpGameZ, 1830f) : ComputeDistanceFactor(workWrapper.Chunk.GamePositionStartZ, reqPositionReadyGameZ, 100f));
			a = Mathf.Max(a, 0f);
			if (a == 1f)
			{
				workWrapper.WorkUntilDone();
			}
			else
			{
				float num = Mathf.Lerp(0.002f, 0.005f, a);
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				float num2 = 0f;
				do
				{
					workWrapper.Work();
					num2 = Time.realtimeSinceStartup - realtimeSinceStartup;
				}
				while (!workWrapper.IsDone && num2 < num);
			}
			if (workWrapper.IsDone)
			{
				_activeWorkQueue.Dequeue();
				_chunkToWorkWrapperMap.Remove(workWrapper.Chunk);
			}
		}
	}
}
