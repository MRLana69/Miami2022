using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class HuntTokenCollectVFXManager : MonoBehaviour
	{
		[SerializeField]
		private HuntTokenCollectVFX _effect;

		public void PlayEffect()
		{
			if (!(_effect == null))
			{
				StartCoroutine(_effect.PlayEffect());
			}
		}
	}
}
