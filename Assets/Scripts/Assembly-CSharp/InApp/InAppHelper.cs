using UnityEngine;

namespace InApp
{
	public class InAppHelper : MonoBehaviour
	{
		[SerializeField]
		private AudioClip purchaseSuccessSound;

		private void Start()
		{
			InAppManager.instance.purchaseSuccessSound = purchaseSuccessSound;
		}
	}
}
