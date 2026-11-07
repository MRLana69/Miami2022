using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public abstract class BaseFileStorage : IStorage
	{
		private readonly IDirectory _directory;

		protected BaseFileStorage([NotNull] IDirectory directory)
		{
			if (directory == null)
			{
				throw new ArgumentNullException("directory");
			}
			_directory = directory;
		}

		protected abstract void WriteObject([NotNull] object obj, Stream outputStream);

		protected abstract object ReadObject([NotNull] Type type, [NotNull] Stream inputStream);

		public void Save(string name, object obj)
		{
			_directory.File(name).Save((Stream stream) =>
			{
				WriteObject(obj, stream);
			});
		}

		public object Load(Type type, string name)
		{
			return _directory.File(name).Load((Stream stream) => ReadObject(type, stream));
		}

		public bool ItemExists(string name)
		{
			return _directory.File(name).exists;
		}

		public void Delete(string name)
		{
			_directory.File(name).Delete();
		}

		public bool Revert(string name)
		{
			return _directory.File(name).Revert();
		}
	}
}
