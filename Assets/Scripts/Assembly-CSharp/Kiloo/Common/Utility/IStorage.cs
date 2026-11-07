using System;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface IStorage
	{
		void Save([NotNull] string name, [NotNull] object obj);

		object Load([NotNull] Type type, [NotNull] string name);

		bool ItemExists(string name);

		void Delete(string name);

		bool Revert(string name);
	}
}
