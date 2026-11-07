using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatTeleboardMenuEffect : ATeleboardMenuEffect
{
	[SerializeField]
	private EffectPlayer _poof1;

	[SerializeField]
	private EffectPlayer _poof2;

	private bool _lastWas1;

	private Renderer[] _cachedRenderers;

	private float _reappearDelay = 0.2f;

	[SerializeField]
	private float _offsetDistance = -1f;

	[SerializeField]
	private float _scale = 0.9f;

	private Camera _camera;

	private void OnEnable()
	{
		_poof1.gameObject.SetActive(false);
		_poof2.gameObject.SetActive(false);
		_camera = NGUITools.FindCameraForLayer(Layers.Instance._3DGUI);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		if (_cachedRenderers != null && _cachedRenderers[0] != null)
		{
			SetRenderersActive(null, true);
		}
	}

	private void OnDestroy()
	{
		OnDisable();
	}

	public override void Initialize(Transform root, bool boardOnly = false)
	{
		base.Initialize(root, boardOnly);
		if (boardOnly)
		{
			_offsetDistance = 0f;
			return;
		}
		base.transform.parent = root;
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one * _scale;
		base.transform.rotation = Quaternion.identity;
		base.transform.parent = null;
	}

	private void SetRenderersActive(CharacterModel characterModel, bool state)
	{
		if (_cachedRenderers == null && characterModel != null)
		{
			List<Renderer> list = new List<Renderer>(characterModel.GetComponentsInChildren<Renderer>());
			int num = 0;
			while (num < list.Count)
			{
				if (!list[num].enabled)
				{
					list.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			_cachedRenderers = list.ToArray();
		}
		if (_cachedRenderers == null || _cachedRenderers.Length <= 0 || !(_cachedRenderers[0] != null))
		{
			return;
		}
		for (int i = 0; i < _cachedRenderers.Length; i++)
		{
			if (_cachedRenderers[i] != null)
			{
				_cachedRenderers[i].enabled = state;
			}
		}
	}

	public override void StartTeleboardEffect(float duration, CharacterModel characterModel)
	{
		SetRenderersActive(characterModel, false);
		_poof1.gameObject.SetActive(false);
		_poof2.gameObject.SetActive(false);
		UpdatePoofs(characterModel);
		_lastWas1 = !_lastWas1;
	}

	public override void EndTeleboardEffect(float duration, CharacterModel characterModel)
	{
		StartCoroutine(DelayEndPoof(duration, characterModel));
	}

	private IEnumerator DelayEndPoof(float duration, CharacterModel characterModel)
	{
		UpdatePoofs(characterModel);
		yield return new WaitForSeconds(_reappearDelay);
		SetRenderersActive(characterModel, true);
	}

	private void UpdatePoofs(CharacterModel characterModel)
	{
		Vector3 vector = ((!(characterModel == null)) ? characterModel.CharacterRig.Hip.position : Vector3.zero);
		Vector3 vector2 = (vector - _camera.transform.position).normalized * _offsetDistance;
		if (_lastWas1)
		{
			if (!_boardOnly)
			{
				_poof2.transform.position = vector + vector2;
			}
			_poof2.gameObject.SetActive(true);
		}
		else
		{
			if (!_boardOnly)
			{
				_poof1.transform.position = vector + vector2;
			}
			_poof1.gameObject.SetActive(true);
		}
	}
}
