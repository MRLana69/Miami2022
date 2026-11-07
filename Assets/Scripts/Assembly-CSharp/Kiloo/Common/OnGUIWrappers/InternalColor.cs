using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class InternalColor : IDisposable
	{
		protected Color previousColor { get; set; }

		protected InternalColor()
		{
		}

		public virtual void Dispose()
		{
		}
	}
}
