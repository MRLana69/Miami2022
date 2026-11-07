using System;
using UnityEngine;

public class EnumToStringAttribute : PropertyAttribute
{
	public Type EnumType { get; private set; }

	public EnumToStringAttribute(Type t)
	{
		EnumType = t;
	}
}
