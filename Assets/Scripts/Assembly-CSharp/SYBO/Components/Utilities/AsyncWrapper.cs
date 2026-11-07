using System;

namespace SYBO.Components.Utilities
{
	public abstract class AsyncWrapper
	{
		protected IAsyncResult _asyncResult;

		protected Action _asyncMethod;

		protected Exception Error;

		public void EndInvoke()
		{
			_asyncMethod.EndInvoke(_asyncResult);
			if (Error != null)
			{
				throw Error;
			}
		}
	}
	public class AsyncWrapper<T> : AsyncWrapper
	{
		private Action<T> _method;

		private T _argument;

		public AsyncWrapper(Action<T> method)
		{
			_method = method;
			_asyncMethod = ThreadCatchingWrapper;
		}

		public void ThreadCatchingWrapper()
		{
			try
			{
				_method(_argument);
			}
			catch (Exception error)
			{
				Error = error;
			}
		}

		public void BeginInvoke(T argument, AsyncCallback callback)
		{
			_argument = argument;
			_asyncResult = _asyncMethod.BeginInvoke(callback, null);
		}
	}
	public class AsyncWrapper<T1, T2> : AsyncWrapper
	{
		private Action<T1, T2> _method;

		private T1 _argument1;

		private T2 _argument2;

		public AsyncWrapper(Action<T1, T2> method)
		{
			_method = method;
			_asyncMethod = ThreadCatchingWrapper;
		}

		public void ThreadCatchingWrapper()
		{
			try
			{
				_method(_argument1, _argument2);
			}
			catch (Exception error)
			{
				Error = error;
			}
		}

		public void BeginInvoke(T1 argument1, T2 argument2, AsyncCallback callback)
		{
			_argument1 = argument1;
			_argument2 = argument2;
			_asyncResult = _asyncMethod.BeginInvoke(callback, null);
		}
	}
}
