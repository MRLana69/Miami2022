using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface IBsonSerializer
	{
		void WriteBsonObject([NotNull] object value, [NotNull] BinaryWriter writer);

		object ReadBsonObject([NotNull] Type type, [NotNull] BinaryReader reader);

		[Pure]
		object ConvertValue([NotNull] Type type, [CanBeNull] object value);
	}
}
