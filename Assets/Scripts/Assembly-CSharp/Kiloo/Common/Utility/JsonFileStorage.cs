using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public class JsonFileStorage : BaseFileStorage
	{
		private readonly IJsonSerializer _jsonSerializer;

		public JsonFileStorage([NotNull] IDirectory directory, [NotNull] IJsonSerializer jsonSerializer)
			: base(directory)
		{
			if (jsonSerializer == null)
			{
				throw new ArgumentNullException("jsonSerializer");
			}
			_jsonSerializer = jsonSerializer;
		}

		protected override void WriteObject(object obj, Stream outputStream)
		{
			StreamWriter streamWriter = new StreamWriter(outputStream);
			_jsonSerializer.WriteJsonValue(obj, streamWriter);
			streamWriter.Flush();
		}

		protected override object ReadObject(Type type, Stream inputStream)
		{
			return _jsonSerializer.ReadJsonValue(type, new StreamReader(inputStream));
		}
	}
}
