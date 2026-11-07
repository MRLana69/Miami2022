using System.IO;
using System.Text;

namespace SYBO.Components.Json
{
	public class FileStringStorage : IStringStorage
	{
		private string _path;

		bool IStringStorage.CanRead
		{
			get
			{
				using (Stream stream = CreateStream())
				{
					return stream.CanRead;
				}
			}
		}

		public FileStringStorage(string path)
		{
			_path = path;
		}

		bool IStringStorage.Overwrite(string content)
		{
			Stream stream = null;
			try
			{
				stream = CreateStream();
				if (stream != null && stream.CanWrite)
				{
					stream.SetLength(0L);
					using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
					{
						streamWriter.WriteLine(content);
						streamWriter.Flush();
						return true;
					}
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
			return false;
		}

		string IStringStorage.Read()
		{
			Stream stream = CreateStream();
			try
			{
				if (stream != null && stream.CanRead)
				{
					stream.Position = 0L;
					using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
			return string.Empty;
		}

		private Stream CreateStream()
		{
			return new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
		}
	}
}
