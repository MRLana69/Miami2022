using System;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	[Serializable]
	public struct Vector3i
	{
		public int x;

		public int y;

		public int z;

		public static Vector3i zero
		{
			get
			{
				return new Vector3i(0, 0, 0);
			}
		}

		public static Vector3i one
		{
			get
			{
				return new Vector3i(1, 1, 1);
			}
		}

		public static Vector3i forward
		{
			get
			{
				return new Vector3i(0, 0, 1);
			}
		}

		public static Vector3i back
		{
			get
			{
				return new Vector3i(0, 0, -1);
			}
		}

		public static Vector3i up
		{
			get
			{
				return new Vector3i(0, 1, 0);
			}
		}

		public static Vector3i down
		{
			get
			{
				return new Vector3i(0, -1, 0);
			}
		}

		public static Vector3i right
		{
			get
			{
				return new Vector3i(1, 0, 0);
			}
		}

		public static Vector3i left
		{
			get
			{
				return new Vector3i(-1, 0, 0);
			}
		}

		public Vector3i(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3 AsVector3()
		{
			return new Vector3(x, y, z);
		}

		public static implicit operator Vector3(Vector3i v3i)
		{
			return new Vector3(v3i.x, v3i.y, v3i.z);
		}

		public static Vector3i operator +(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3i operator -(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public override string ToString()
		{
			return string.Format("[Vector3i] ({0},{1},{2})", x, y, z);
		}

		public override bool Equals(object obj)
		{
			return obj is Vector3i && Equals((Vector3i)obj);
		}

		public bool Equals(Vector3i other)
		{
			return x == other.x && y == other.y && z == other.z;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + x.GetHashCode();
			num = num * 23 + y.GetHashCode();
			return num * 23 + z.GetHashCode();
		}

		public static Vector3i operator *(Vector3i a, int scale)
		{
			return new Vector3i(a.x * scale, a.y * scale, a.z * scale);
		}

		public static bool operator ==(Vector3i a, Vector3i b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Vector3i a, Vector3i b)
		{
			return !(a == b);
		}

		public static int Manhatten(Vector3i a, Vector3i b)
		{
			Vector3i vector3i = a - b;
			return Mathf.Abs(vector3i.x) + Mathf.Abs(vector3i.y) + Mathf.Abs(vector3i.z);
		}
	}
}
