using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface IDirectory : IFileSystemEntry
	{
		[Pure]
		IEnumerable<IDirectory> GetSubdirectories();

		[Pure]
		IEnumerable<IFile> GetFiles();

		[Pure]
		IFile File([NotNull] string path);

		[Pure]
		IDirectory Subdirectory([NotNull] string path);

		[Pure]
		string ToRelativePath([NotNull] string path);

		void Delete(bool recursive = false);
	}
}
