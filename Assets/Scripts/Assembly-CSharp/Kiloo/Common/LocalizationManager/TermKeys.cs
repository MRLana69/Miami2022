using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Kiloo.Common.LocalizationManager
{
	[CreateAssetMenu(menuName = "Localization/TermKeys")]
	public class TermKeys : ScriptableObject
	{
		[InfoBox("Remember to generate LegalKeys code (Kiloo/Localization/Generate Legal Keys", InfoBoxType.Normal, null)]
		[ReorderableList]
		public List<string> keys;
	}
}
