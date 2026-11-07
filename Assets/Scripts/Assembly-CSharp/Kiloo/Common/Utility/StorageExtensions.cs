using System;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class StorageExtensions
	{
		public static T Load<T>([NotNull] this IStorage storage, [NotNull] string name)
		{
			return (T)storage.Load(typeof(T), name);
		}

		public static bool TryLoad<T>([NotNull] this IStorage storage, [NotNull] string name, out T obj)
		{
			try
			{
				obj = (T)storage.Load(typeof(T), name);
				return true;
			}
			catch (Exception)
			{
				obj = default(T);
				return false;
			}
		}
	}
}
