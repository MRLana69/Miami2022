using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public interface IJsonSerializer
	{
		void WriteJsonValue([CanBeNull] object value, [NotNull] TextWriter writer);

		object ReadJsonValue([NotNull] Type type, [NotNull] TextReader reader);

		[Pure]
		object ConvertValue([NotNull] Type type, [CanBeNull] object value);
	}
}
