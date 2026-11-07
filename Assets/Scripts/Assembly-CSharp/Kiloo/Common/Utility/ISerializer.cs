using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface ISerializer
	{
		void WriteObject([NotNull] object value, [NotNull] Stream outputStream);

		object ReadObject([NotNull] Type type, [NotNull] Stream inputStream, bool allowBuffering);

		[Pure]
		object ConvertValue([NotNull] Type type, [CanBeNull] object value);
	}
}
