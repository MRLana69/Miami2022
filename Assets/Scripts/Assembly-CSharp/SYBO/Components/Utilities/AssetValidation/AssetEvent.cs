using System;
using UnityEngine.Events;

namespace SYBO.Components.Utilities.AssetValidation
{
	[Serializable]
	public class AssetEvent : UnityEvent<IAsset>
	{
	}
}
