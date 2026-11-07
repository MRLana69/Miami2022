using System.Collections;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	[RequireComponent(typeof(UISprite))]
	public class UIScaleAnimation : MonoBehaviour
	{
		private const float COMPLETION_THRESHOLD = 0.01f;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float _duration = 1f;

		[SerializeField]
		[MinMaxSlider(0f, 10f)]
		private Vector2 _scaleRange = new Vector2(1f, 5f);

		[SerializeField]
		[Range(0f, 1f)]
		private float _fadeFrom = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _fadeTo = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _restPeriod = 0.5f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _spawnRate = 0.2f;

		[SerializeField]
		private int _spawnAmount = 3;

		private UISprite _originalSprite;

		private UISprite[] _sprites;

		private float[] _spawnTimes;

		private float _timer = float.MinValue;

		private Vector3 _startScale = Vector3.zero;

		private Vector3 _endScale = Vector3.zero;

		private void Awake()
		{
			_startScale = Vector3.one * _scaleRange.x;
			_endScale = Vector3.one * _scaleRange.y;
			_spawnTimes = new float[_spawnAmount];
			for (int i = 0; i < _spawnAmount; i++)
			{
				_spawnTimes[i] = (float)i * _spawnRate;
			}
			_originalSprite = GetComponent<UISprite>();
			_sprites = new UISprite[_spawnAmount];
			_originalSprite.enabled = false;
			for (int j = 0; j < _spawnAmount; j++)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.parent = base.transform.parent;
				gameObject.layer = 26;
				_sprites[j] = gameObject.AddComponent<UISprite>();
				_sprites[j].atlas = _originalSprite.atlas;
				_sprites[j].spriteName = _originalSprite.spriteName;
				_sprites[j].color = _originalSprite.color;
				_sprites[j].gameObject.SetActive(false);
				_sprites[j].transform.localPosition = _originalSprite.transform.localPosition;
			}
			ResetAllSprites();
		}

		private void OnEnable()
		{
			RestartAnimation();
		}

		private void RestartAnimation()
		{
			ResetAllSprites();
			_timer = 0f;
		}

		private void ResetAllSprites()
		{
			UISprite[] sprites = _sprites;
			foreach (UISprite uISprite in sprites)
			{
				if (!(uISprite == null))
				{
					uISprite.gameObject.SetActive(false);
					uISprite.transform.localScale = _startScale;
					uISprite.alpha = _fadeFrom;
					uISprite.gameObject.SetActive(true);
				}
			}
		}

		private void Update()
		{
			if (!base.transform.parent.gameObject.activeInHierarchy)
			{
				return;
			}
			bool flag = false;
			UISprite[] sprites = _sprites;
			foreach (UISprite uISprite in sprites)
			{
				if (uISprite.isVisible)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			for (int j = 0; j < _spawnAmount; j++)
			{
				float num = _timer - (float)j * _spawnRate;
				if (num < 0f)
				{
					break;
				}
				float num2 = num / _duration;
				Vector3 localScale = Vector3.Lerp(_startScale, _endScale, num2);
				float alpha = Mathf.Lerp(_fadeFrom, _fadeTo, num2);
				if (1f - num2 <= 0.01f)
				{
					_sprites[j].gameObject.SetActive(false);
					if (j == _spawnAmount - 1)
					{
						CoroutineUtil.StartCoroutine(WaitForSecondsAndRestart(_restPeriod));
					}
				}
				else
				{
					_sprites[j].gameObject.SetActive(true);
					_sprites[j].transform.localScale = localScale;
					_sprites[j].alpha = alpha;
				}
			}
			_timer += Time.deltaTime;
		}

		private IEnumerator WaitForSecondsAndRestart(float restPeriod)
		{
			yield return new WaitForSeconds(restPeriod);
			RestartAnimation();
		}
	}
}
