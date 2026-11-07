using System.Collections;
using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;

namespace SYBO.Common
{
	public class WorkLoadQueue : SingletonMonoBehaviour<WorkLoadQueue>
	{
		private List<IEnumerator> _workQueue = new List<IEnumerator>(64);

		private Stack<IEnumerator> _jobStack = new Stack<IEnumerator>();

		private bool _relaxOneFrame;

		public bool IsIdle
		{
			get
			{
				return _jobStack.Count == 0 && _workQueue.Count == 0;
			}
		}

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.Dynamic);
		}

		public void EnqueueWork(IEnumerator workItem)
		{
			_workQueue.Add(workItem);
		}

		public void CancelWork(IEnumerator workItem)
		{
			if (_jobStack.Contains(workItem))
			{
				_jobStack.Clear();
			}
			if (_workQueue.Contains(workItem))
			{
				_workQueue.Remove(workItem);
			}
		}

		private void Update()
		{
			if (_relaxOneFrame)
			{
				_relaxOneFrame = false;
				return;
			}
			Work();
			while (_workQueue.Count > 25)
			{
				Work();
			}
			if (_workQueue.Count > 20)
			{
				Work();
			}
			if (_workQueue.Count > 10)
			{
				Work();
			}
		}

		public void WorkUntilDone()
		{
			while (!IsIdle)
			{
				Work();
			}
		}

		private void Work()
		{
			if (_jobStack.Count != 0 || _workQueue.Count != 0)
			{
				if (_jobStack.Count == 0)
				{
					_jobStack.Push(_workQueue[0]);
					_workQueue.RemoveAt(0);
				}
				IEnumerator enumerator = _jobStack.Peek();
				if (!enumerator.MoveNext())
				{
					_jobStack.Pop();
				}
				else if (enumerator.Current is IEnumerator)
				{
					_jobStack.Push(enumerator.Current as IEnumerator);
					Work();
				}
			}
		}
	}
}
