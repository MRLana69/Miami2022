using System;
using System.IO;
using SYBO.Components.Logging;

namespace SYBO.Components.Utilities
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AssetLoaderPathAttribute : Attribute
	{
		public const string ErrorDirectoryDoesNotExist = "The specified path is deemed invalid. Please ensure that the direcotry exists. Specified path: {0}";

		public const string ErrorDirectoryResources = "The specified directory path does not end with '.../Resources'. Specified path: {0}";

		public const string ErrorDirectoryAssets = "The specified directory path does not start with 'Assets/'. Specified path: {0}";

		public const string FileExtension = ".asset";

		private string _directory;

		private string _assetName;

		public string Directory
		{
			get
			{
				if (ValidateDirectoryPath(_directory))
				{
					return _directory;
				}
				return null;
			}
		}

		public string AssetName
		{
			get
			{
				return _assetName;
			}
		}

		public string AssetNameWithFileExtension
		{
			get
			{
				return string.Format("{0}{1}", AssetName, ".asset");
			}
		}

		public string FullPath
		{
			get
			{
				if (Directory == null)
				{
					return null;
				}
				return FileUtility.ConvertToPathWithForwardSlashes(Path.Combine(Directory, AssetNameWithFileExtension));
			}
		}

		public AssetLoaderPathAttribute(string directoryPath, string assetName)
		{
			_directory = directoryPath.TrimEnd('/');
			_assetName = assetName;
		}

		private bool ValidateDirectoryPath(string directoryPath)
		{
			if (!directoryPath.StartsWith("Assets/"))
			{
				Log.Utilities.LogErrorFormat("The specified directory path does not start with 'Assets/'. Specified path: {0}", directoryPath);
				return false;
			}
			if (!directoryPath.EndsWith("/Resources"))
			{
				Log.Utilities.LogErrorFormat("The specified directory path does not end with '.../Resources'. Specified path: {0}", directoryPath);
				return false;
			}
			return true;
		}

		private string GetFullSystemPathTo(string assetFolderRelativePath)
		{
			string path = assetFolderRelativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return Path.Combine(System.IO.Directory.GetCurrentDirectory(), path);
		}
	}
}
