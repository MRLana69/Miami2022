using System;
using UnityEngine;

namespace SYBO.Common.Utilities.Flags
{
	public class EnumFlagPropertyAttribute : PropertyAttribute
	{
		public Type EnumType;

		public EnumFlagPropertyAttribute(Type enumType)
		{
			EnumType = enumType;
		}
	}
}
