using System;
using System.IO;
using UnityEngine;

namespace SYBO.Components.Utilities.IO
{
	public class PersistentDataPathUtility
	{
		public string ParentPath { get; private set; }

		public PersistentDataPathUtility()
		{
			ParentPath = Application.persistentDataPath;
		}

		public PersistentDataPathUtility(string fullPath)
		{
			ParentPath = fullPath;
		}

		public string GetStorageLocationDirectory(string subdirectory)
		{
			if (Path.IsPathRooted(subdirectory))
			{
				throw new ArgumentException(string.Format("The subdirectory must be a relative path. value={0}", subdirectory));
			}
			return FileUtility.ConvertToPathWithForwardSlashes(Path.Combine(ParentPath, subdirectory));
		}

		public string CreateStorageLocation(string subdirectory, string filename)
		{
			string storageLocationDirectory = GetStorageLocationDirectory(subdirectory);
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentException(string.Format("The filename must exist. value={0}", filename));
			}
			if (!Directory.Exists(storageLocationDirectory))
			{
				Directory.CreateDirectory(storageLocationDirectory);
			}
			return FileUtility.ConvertToPathWithForwardSlashes(Path.Combine(storageLocationDirectory, filename));
		}

		public bool DeleteStorageLocation(string subdirectory)
		{
			string storageLocationDirectory = GetStorageLocationDirectory(subdirectory);
			if (Directory.Exists(storageLocationDirectory))
			{
				Directory.Delete(storageLocationDirectory, true);
				return true;
			}
			return false;
		}
	}
}
