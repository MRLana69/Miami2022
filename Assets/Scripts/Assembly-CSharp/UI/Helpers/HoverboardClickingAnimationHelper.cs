using Kiloo.Common.Timer;
using SYBO.Subway;
using UnityEngine;

namespace UI.Helpers
{
	public class HoverboardClickingAnimationHelper : MonoBehaviour
	{
		[SerializeField]
		private Animation _hoverboardClickAnimation;

		[SerializeField]
		private AnimationClip _hoverboardClickAnimationClip;

		private void Awake()
		{
			Game.OnCharacterModifierCollectionChanged += OnCharacterModifierCollectionChanged;
			Game.OnPlayerDied += OnPlayerDied;
			Game.OnGameEnded += Deactivate;
		}

		public void PlayAnimationAndDeactivate()
		{
			Activate();
			_hoverboardClickAnimation.Play(_hoverboardClickAnimationClip.name);
			Timer.ExecuteWithDelay(_hoverboardClickAnimationClip.length, Deactivate);
		}

		private void OnCharacterModifierCollectionChanged(object sender, CharacterModifierCollectionChangedEventArgs e)
		{
			if (e.CharacterModifier is Hoverboard)
			{
				Deactivate();
			}
		}

		private void OnPlayerDied(bool wasAutoRevived)
		{
			if (!wasAutoRevived)
			{
				Deactivate();
			}
		}

		private void Activate()
		{
			if (base.gameObject != null)
			{
				base.gameObject.SetActive(true);
			}
		}

		private void Deactivate()
		{
			if (this != null && base.gameObject != null)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			Game.OnCharacterModifierCollectionChanged -= OnCharacterModifierCollectionChanged;
			Game.OnPlayerDied -= OnPlayerDied;
			Game.OnGameEnded -= Deactivate;
		}
	}
}
