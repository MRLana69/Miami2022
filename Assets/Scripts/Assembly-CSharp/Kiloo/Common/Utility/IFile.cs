using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface IFile : IFileSystemEntry
	{
		long GetSize();

		void Delete();

		void CopyTo([NotNull] IFile destination, bool overwrite = false);

		void MoveTo([NotNull] IFile destination);

		[Pure]
		Stream OpenRead();

		[Pure]
		Stream OpenWrite(bool replaceExisting);

		void Save(WriteDataFunc writeDataFunc);

		[Pure]
		object Load(ReadDataFunc readDataFunc);

		bool Revert();
	}
}
