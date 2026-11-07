using System;

namespace SYBO.Components.Utilities
{
	public class ClassWrapper<T> : IEquatable<ClassWrapper<T>> where T : IEquatable<T>
	{
		public T Value;

		public bool Equals(ClassWrapper<T> other)
		{
			return Value.Equals(other.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
