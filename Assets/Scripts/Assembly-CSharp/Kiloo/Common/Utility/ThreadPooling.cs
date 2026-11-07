using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Common.Utility
{
	[ExecuteInEditMode]
	public class ThreadPooling : MonoBehaviour
	{
		public delegate void ProgressDelegate(Action progress);

		private class Task
		{
			public Action process;

			public StackTrace stackTrace;
		}

		private static ThreadPooling s_instance;

		private readonly List<Action> LockedTasks = new List<Action>();

		private readonly List<Action> Tasks = new List<Action>();

		private readonly List<Exception> LockedExceptions = new List<Exception>();

		private readonly List<Exception> Exceptions = new List<Exception>();

		private static int _numThreads;

		public static void QueueUserWorkItem(Action task, Action callback)
		{
			QueueUserWorkItemWithProgress((ProgressDelegate progress) =>
			{
				task.SafeInvoke();
			}, callback);
		}

		public static void QueueUserWorkItemWithProgress(Action<ProgressDelegate> task, Action callback)
		{
			if (s_instance == null)
			{
				GameObject gameObject = new GameObject(typeof(ThreadPooling).Name);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				GameObject gameObject2 = gameObject;
				if (Application.isPlaying)
				{
					UnityEngine.Object.DontDestroyOnLoad(gameObject2);
				}
				s_instance = gameObject2.AddComponent<ThreadPooling>();
			}
			s_instance.RunAsync(task, callback);
		}

		private void RunAsync(Action<ProgressDelegate> task, Action callback)
		{
			Task task2 = new Task();
			task2.process = () =>
			{
				try
				{
					task.SafeInvoke((Action progress) =>
					{
						lock (LockedTasks)
						{
							LockedTasks.Add(progress);
						}
					});
					lock (LockedTasks)
					{
						LockedTasks.Add(callback);
					}
				}
				catch (Exception item)
				{
					lock (LockedExceptions)
					{
						LockedExceptions.Add(item);
					}
				}
			};
			task2.stackTrace = new StackTrace();
			Task state = task2;
			Interlocked.Increment(ref _numThreads);
			ThreadPool.QueueUserWorkItem(RunAction, state);
		}

		private static void RunAction(object newTask)
		{
			Task task = (Task)newTask;
			try
			{
				task.process();
			}
			catch (Exception ex)
			{
				throw new Exception(string.Concat(ex, "\n", task.stackTrace));
			}
			finally
			{
				Interlocked.Decrement(ref _numThreads);
			}
		}

		public static void Foreach<T>(IList<T> elements, Action<T, ProgressDelegate> iterate, Action callback)
		{
			Action<int, ProgressDelegate> iterate2 = (int i, ProgressDelegate progress) =>
			{
				iterate(elements[i], progress);
			};
			For(0, elements.Count, iterate2, callback);
		}

		public static void For(int start, int end, Action<int, ProgressDelegate> iterate, Action callback)
		{
			int nextIteration = Mathf.Min(start, end);
			int endIteration = Mathf.Max(start, end);
			int num = Mathf.Min(Mathf.Clamp(Environment.ProcessorCount - _numThreads - 1, 1, int.MaxValue), endIteration);
			ActionQueue actionQueue = new ActionQueue();
			for (int i = 0; i < num; i++)
			{
				actionQueue.AddAction((Action action) =>
				{
					QueueUserWorkItemWithProgress((ProgressDelegate progress) =>
					{
						int num2;
						while ((num2 = Interlocked.Increment(ref nextIteration) - 1) < endIteration)
						{
							iterate((start >= end) ? (start - num2 - 1) : num2, progress);
						}
					}, action);
				});
			}
			actionQueue.RunParallel(callback);
		}

		private void Update()
		{
			lock (LockedTasks)
			{
				Tasks.Clear();
				Tasks.AddRange(LockedTasks);
				LockedTasks.Clear();
			}
			foreach (Action task in Tasks)
			{
				task.SafeInvoke();
			}
			lock (LockedExceptions)
			{
				Exceptions.Clear();
				Exceptions.AddRange(LockedExceptions);
				LockedExceptions.Clear();
			}
			using (List<Exception>.Enumerator enumerator2 = Exceptions.GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					Exception current2 = enumerator2.Current;
					throw current2;
				}
			}
		}

		public static void Destroy()
		{
			if (s_instance != null)
			{
				UnityEngine.Object.DestroyImmediate(s_instance.gameObject);
				s_instance = null;
			}
		}
	}
}
