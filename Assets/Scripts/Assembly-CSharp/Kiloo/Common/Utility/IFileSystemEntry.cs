using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface IFileSystemEntry
	{
		string name { get; }

		bool exists { get; }

		string GetPath([NotNull] IDirectory relativeTo);

		IDirectory GetParent();
	}
}
