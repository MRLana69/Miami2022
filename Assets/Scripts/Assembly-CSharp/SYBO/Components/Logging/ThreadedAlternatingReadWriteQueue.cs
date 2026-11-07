using System;
using System.Collections.Generic;
using System.Threading;

namespace SYBO.Components.Logging
{
	public class ThreadedAlternatingReadWriteQueue<T>
	{
		public class ThreadedAlternateReadWriteQueueException : Exception
		{
			public ThreadedAlternateReadWriteQueueException(string message)
				: base(message)
			{
			}
		}

		private object _lockObject = new object();

		private volatile Queue<T> _readQueue;

		private volatile Queue<T> _writeQueue;

		private volatile Thread _readThread;

		public int ReadQueueCount
		{
			get
			{
				if (_readThread != Thread.CurrentThread)
				{
					throw new ThreadedAlternateReadWriteQueueException("ThreadedAlternatingReadWriteQueue: SwapQueues must be invoked from the Read Thread.");
				}
				return _readQueue.Count;
			}
		}

		public ThreadedAlternatingReadWriteQueue()
		{
			_readQueue = new Queue<T>();
			_writeQueue = new Queue<T>();
		}

		public void SetReadThread(Thread thread)
		{
			_readThread = thread;
		}

		public void SwapQueues()
		{
			if (_readThread == null)
			{
				SetReadThread(Thread.CurrentThread);
			}
			if (_readThread != Thread.CurrentThread)
			{
				throw new ThreadedAlternateReadWriteQueueException("ThreadedAlternatingReadWriteQueue: SwapQueues must be invoked from the Read Thread.");
			}
			lock (_lockObject)
			{
				if (_readQueue.Count != 0)
				{
					throw new ThreadedAlternateReadWriteQueueException("ThreadedAlternatingReadWriteQueue: You can't SwapQueues until the read queue is empty.");
				}
				Queue<T> readQueue = _readQueue;
				_readQueue = _writeQueue;
				_writeQueue = readQueue;
			}
		}

		public void Enqueue(T item)
		{
			lock (_lockObject)
			{
				_writeQueue.Enqueue(item);
			}
		}

		public T Dequeue()
		{
			if (_readThread != Thread.CurrentThread)
			{
				throw new ThreadedAlternateReadWriteQueueException("ThreadedAlternatingReadWriteQueue: Dequeue must be invoked from the Read Thread.");
			}
			return _readQueue.Dequeue();
		}

		public bool TryDequeue(out T item)
		{
			if (_readThread != Thread.CurrentThread)
			{
				throw new ThreadedAlternateReadWriteQueueException("ThreadedAlternatingReadWriteQueue: TryDequeue must be invoked from the Read Thread.");
			}
			if (_readQueue.Count == 0)
			{
				item = default(T);
				return false;
			}
			item = _readQueue.Dequeue();
			return true;
		}
	}
}
