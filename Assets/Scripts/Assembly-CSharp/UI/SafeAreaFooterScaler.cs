using Kiloo.Common.Extensions;
using UnityEngine;
using Utilities;

namespace UI
{
	public class SafeAreaFooterScaler : MonoBehaviour
	{
		[SerializeField]
		private Transform[] footerItems;

		private bool _hasOffsetButtons;

		private void Start()
		{
			if (_hasOffsetButtons)
			{
				return;
			}
			if (SafeArea.IsRestricted)
			{
				Transform[] array = footerItems;
				foreach (Transform transform in array)
				{
					transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
				}
			}
			_hasOffsetButtons = true;
		}
	}
}
