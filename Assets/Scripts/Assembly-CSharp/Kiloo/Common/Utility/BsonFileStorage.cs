using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public class BsonFileStorage : BaseFileStorage
	{
		private readonly IBsonSerializer _bsonSerializer;

		public BsonFileStorage([NotNull] IDirectory directory, [NotNull] IBsonSerializer bsonSerializer)
			: base(directory)
		{
			if (bsonSerializer == null)
			{
				throw new ArgumentNullException("bsonSerializer");
			}
			_bsonSerializer = bsonSerializer;
		}

		protected override void WriteObject(object obj, Stream outputStream)
		{
			_bsonSerializer.WriteBsonObject(obj, new BinaryWriter(outputStream));
		}

		protected override object ReadObject(Type type, Stream inputStream)
		{
			return _bsonSerializer.ReadBsonObject(type, new BinaryReader(inputStream));
		}
	}
}
