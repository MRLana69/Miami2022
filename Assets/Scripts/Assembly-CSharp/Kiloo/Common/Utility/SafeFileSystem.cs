using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public class SafeFileSystem : IFileSystem, IDirectory, IFileSystemEntry
	{
		private abstract class FileSystemEntry : IFileSystemEntry
		{
			public readonly SafeFileSystem fileSystem;

			public readonly string absolutePath;

			public string name { get; private set; }

			public abstract bool exists { get; }

			protected FileSystemEntry([NotNull] SafeFileSystem fileSystem, [NotNull] string absolutePath)
			{
				this.fileSystem = fileSystem;
				this.absolutePath = absolutePath;
				name = Path.GetFileName(absolutePath);
			}

			public string GetPath(IDirectory relativeTo)
			{
				if (relativeTo == null)
				{
					throw new ArgumentNullException("relativeTo");
				}
				return relativeTo.ToRelativePath(absolutePath);
			}

			public IDirectory GetParent()
			{
				string directoryName = Path.GetDirectoryName(absolutePath);
				if (directoryName == fileSystem.absolutePath)
				{
					return fileSystem;
				}
				return new FileSystemDirectory(fileSystem, AbsolutePath(fileSystem.absolutePath, directoryName));
			}
		}

		private class FileSystemFile : FileSystemEntry, IFile, IFileSystemEntry
		{
			public override bool exists
			{
				get
				{
					return System.IO.File.Exists(absolutePath);
				}
			}

			public FileSystemFile(SafeFileSystem fileSystem, string absolutePath)
				: base(fileSystem, absolutePath)
			{
			}

			public long GetSize()
			{
				FileInfo fileInfo = new FileInfo(absolutePath);
				return fileInfo.Length;
			}

			public void Delete()
			{
				System.IO.File.Delete(absolutePath);
			}

			public void CopyTo(IFile destination, bool overwrite)
			{
				if (!System.IO.File.Exists(absolutePath))
				{
					throw new FileNotFoundException();
				}
				string text = fileSystem.GetAbsolutePath(destination);
				EnsureParentDirectoryExists(text);
				System.IO.File.Copy(absolutePath, text, overwrite);
			}

			public void MoveTo(IFile destination)
			{
				if (!System.IO.File.Exists(absolutePath))
				{
					throw new FileNotFoundException();
				}
				string text = fileSystem.GetAbsolutePath(destination);
				EnsureParentDirectoryExists(text);
				System.IO.File.Move(absolutePath, text);
			}

			public Stream OpenRead()
			{
				return System.IO.File.Open(absolutePath, FileMode.Open);
			}

			public Stream OpenWrite(bool replaceExisting)
			{
				EnsureParentDirectoryExists(absolutePath);
				return System.IO.File.Open(absolutePath, (!replaceExisting) ? FileMode.CreateNew : FileMode.Create);
			}

			public void Save(WriteDataFunc writeDataFunc)
			{
				string text = absolutePath + ".temp";
				string text2 = absolutePath + ".bak.temp";
				bool flag = false;
				try
				{
					EnsureParentDirectoryExists(text);
					using (FileStream outputStream = System.IO.File.Open(text, FileMode.Create))
					{
						outputStream.WriteTamperProofData(writeDataFunc, fileSystem._salt);
					}
					if (System.IO.File.Exists(absolutePath))
					{
						if (fileSystem.backups > 0)
						{
							Backup(absolutePath, 0);
						}
						else
						{
							if (System.IO.File.Exists(text2))
							{
								System.IO.File.Delete(text2);
							}
							System.IO.File.Move(absolutePath, text2);
							flag = true;
						}
					}
					System.IO.File.Move(text, absolutePath);
				}
				catch (Exception)
				{
					if (flag)
					{
						System.IO.File.Move(text2, absolutePath);
					}
					throw;
				}
				finally
				{
					System.IO.File.Delete(text2);
					System.IO.File.Delete(text);
				}
			}

			[Pure]
			public object Load(ReadDataFunc readDataFunc)
			{
				return InternalLoad(readDataFunc, absolutePath);
			}

			public bool Revert()
			{
				return Revert(absolutePath);
			}

			private bool Revert(string fullPath)
			{
				string text = fullPath + ".bak";
				if (System.IO.File.Exists(text))
				{
					string text2 = fullPath + ".temp";
					string text3 = text + ".baktmp";
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					try
					{
						System.IO.File.Delete(text3);
						System.IO.File.Move(text, text3);
						flag3 = true;
						flag2 = Revert(text);
						if (System.IO.File.Exists(fullPath))
						{
							System.IO.File.Delete(text2);
							System.IO.File.Move(fullPath, text2);
							flag = true;
						}
						System.IO.File.Move(text3, fullPath);
						return true;
					}
					catch (Exception)
					{
						if (flag)
						{
							System.IO.File.Move(text2, fullPath);
						}
						if (flag2)
						{
							Backup(text, 0);
						}
						if (flag3)
						{
							System.IO.File.Move(text3, text);
						}
						throw;
					}
					finally
					{
						System.IO.File.Delete(text2);
					}
				}
				return false;
			}

			private void Backup(string fullPath, int level)
			{
				if (level < fileSystem.backups)
				{
					string text = fullPath + ".bak";
					if (System.IO.File.Exists(text))
					{
						Backup(text, level + 1);
					}
					System.IO.File.Move(fullPath, text);
				}
				else
				{
					System.IO.File.Delete(fullPath);
				}
			}

			[Pure]
			private object InternalLoad(ReadDataFunc readDataFunc, string fullPath)
			{
				using (FileStream inputStream = new FileStream(fullPath, FileMode.Open))
				{
					return inputStream.ReadTamperProofData(readDataFunc, fileSystem._salt);
				}
			}

			private void EnsureParentDirectoryExists(string fullPath)
			{
				string directoryName = Path.GetDirectoryName(fullPath);
				if (!string.IsNullOrEmpty(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
			}
		}

		private class FileSystemDirectory : FileSystemEntry, IDirectory, IFileSystemEntry
		{
			public override bool exists
			{
				get
				{
					return Directory.Exists(absolutePath);
				}
			}

			public FileSystemDirectory(SafeFileSystem fileSystem, string absolutePath)
				: base(fileSystem, absolutePath)
			{
			}

			public IEnumerable<IDirectory> GetSubdirectories()
			{
				return fileSystem.GetDirectories(absolutePath);
			}

			public IEnumerable<IFile> GetFiles()
			{
				return fileSystem.GetFiles(absolutePath);
			}

			public IFile File(string path)
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentException("Value cannot be null or empty.", "path");
				}
				return new FileSystemFile(fileSystem, AbsolutePath(absolutePath, path));
			}

			public IDirectory Subdirectory(string path)
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentException("Value cannot be null or empty.", "path");
				}
				return new FileSystemDirectory(fileSystem, AbsolutePath(absolutePath, path));
			}

			public string ToRelativePath(string path)
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentException("Value cannot be null or empty.", "path");
				}
				return VirtualPath(absolutePath, AbsolutePath(absolutePath, path));
			}

			public void Delete(bool recursive)
			{
				Directory.Delete(absolutePath, recursive);
			}
		}

		public readonly string absolutePath;

		public readonly int backups;

		private const string TEMP_SUFFIX = ".temp";

		private const string BAK_SUFFIX = ".bak";

		private readonly byte[] _salt;

		public string name { get; private set; }

		public bool exists
		{
			get
			{
				return Directory.Exists(absolutePath);
			}
		}

		public SafeFileSystem(string path, byte[] salt = null, int backups = 0)
		{
			if (backups < 0)
			{
				throw new ArgumentOutOfRangeException("backups");
			}
			this.backups = backups;
			if (path != null)
			{
				absolutePath = Path.GetFullPath(path);
				name = Path.GetFileName(absolutePath);
			}
			if (salt != null)
			{
				_salt = new byte[salt.Length];
				Array.Copy(salt, _salt, salt.Length);
			}
		}

		[Pure]
		public SafeFileSystem WithPath([NotNull] string newPath)
		{
			return new SafeFileSystem(AbsolutePath(absolutePath, newPath), _salt, backups);
		}

		[Pure]
		public SafeFileSystem WithSalt([CanBeNull] byte[] newSalt)
		{
			return new SafeFileSystem(absolutePath, newSalt, backups);
		}

		[Pure]
		public SafeFileSystem WithBackups(int newBackups)
		{
			return new SafeFileSystem(absolutePath, _salt, newBackups);
		}

		[Pure]
		public string GetAbsolutePath(IFileSystemEntry entry)
		{
			FileSystemEntry fileSystemEntry = entry as FileSystemEntry;
			if (fileSystemEntry != null)
			{
				return fileSystemEntry.absolutePath;
			}
			SafeFileSystem safeFileSystem = entry as SafeFileSystem;
			if (safeFileSystem != null)
			{
				return safeFileSystem.absolutePath;
			}
			throw new InvalidOperationException();
		}

		public string GetPath(IDirectory relativeTo)
		{
			if (relativeTo == null)
			{
				throw new ArgumentNullException("relativeTo");
			}
			return relativeTo.ToRelativePath(absolutePath);
		}

		public IDirectory GetParent()
		{
			return null;
		}

		public IEnumerable<IDirectory> GetSubdirectories()
		{
			return GetDirectories(absolutePath);
		}

		public IEnumerable<IFile> GetFiles()
		{
			return GetFiles(absolutePath);
		}

		public IFile File(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Value cannot be null or empty.", "path");
			}
			if (absolutePath == null)
			{
				return null;
			}
			return new FileSystemFile(this, AbsolutePath(absolutePath, path));
		}

		public IDirectory Subdirectory(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Value cannot be null or empty.", "path");
			}
			if (absolutePath == null)
			{
				return null;
			}
			return new FileSystemDirectory(this, AbsolutePath(absolutePath, path));
		}

		public string ToRelativePath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Value cannot be null or empty.", "path");
			}
			return VirtualPath(absolutePath, AbsolutePath(absolutePath, path));
		}

		public void Delete(bool recursive = false)
		{
			if (absolutePath != null)
			{
				Directory.Delete(absolutePath, recursive);
			}
		}

		private static string AbsolutePath([NotNull] string root, [NotNull] string path)
		{
			if (path == null || path.Trim() == string.Empty)
			{
				throw new ArgumentException("Value cannot be null or whitespace", "path");
			}
			path = ((Path.DirectorySeparatorChar == '/') ? path.Replace('\\', Path.DirectorySeparatorChar) : path.Replace('/', Path.DirectorySeparatorChar));
			string fullPath = Path.GetFullPath(Path.Combine(root, path));
			if (!fullPath.StartsWith(root))
			{
				throw new ArgumentException("Invalid path: " + path + ", when root is " + root, "path");
			}
			return fullPath;
		}

		private static string VirtualPath([NotNull] string root, [NotNull] string absolutePath)
		{
			if (absolutePath.Length <= root.Length + 2 || absolutePath[root.Length] != Path.DirectorySeparatorChar || !absolutePath.StartsWith(root))
			{
				throw new ArgumentException("Invalid path: " + absolutePath + ", when root is: " + root, "absolutePath");
			}
			string text = absolutePath.Substring(root.Length + 1);
			if (Path.DirectorySeparatorChar != '/')
			{
				text = text.Replace(Path.DirectorySeparatorChar, '/');
			}
			return text;
		}

		private IEnumerable<IDirectory> GetDirectories([NotNull] string fullPath)
		{
			if (!Directory.Exists(fullPath))
			{
				return new IDirectory[0];
			}
			return ((IEnumerable<string>)Directory.GetDirectories(fullPath)).Select((Func<string, IDirectory>)((string s) => new FileSystemDirectory(this, s)));
		}

		private IEnumerable<IFile> GetFiles([NotNull] string fullPath)
		{
			if (!Directory.Exists(fullPath))
			{
				return new IFile[0];
			}
			return ((IEnumerable<string>)Directory.GetFiles(fullPath)).Select((Func<string, IFile>)((string s) => new FileSystemFile(this, s)));
		}
	}
}
