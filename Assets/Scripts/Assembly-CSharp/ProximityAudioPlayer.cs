using Info;
using SYBO.Subway.Characters;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudioPlayer : MonoBehaviour
{
	[SerializeField]
	private AudioClipInfo _clipInfo;

	[SerializeField]
	private float _maxZProximity = 500f;

	[SerializeField]
	private float _minZProximity = -100f;

	[SerializeField]
	private bool _useParentPosition;

	private bool _muted;

	private AudioSource _source;

	private Character _character;

	private void Start()
	{
		_source = GetComponent<AudioSource>();
		_source.clip = _clipInfo.Clip;
		_source.rolloffMode = _clipInfo.Rollof;
		_character = Character.Instance;
		Settings.onSfxSettingChanged += OnSfxSettingChanged;
		OnSfxSettingChanged();
	}

	private void OnSfxSettingChanged()
	{
		if (Settings.optionSfx)
		{
			_muted = false;
			_source.volume = Random.Range(_clipInfo.minVolume, _clipInfo.maxVolume);
		}
		else
		{
			_muted = true;
			_source.volume = 0f;
		}
	}

	public void ProximityBasedPlay()
	{
		Transform transform = ((!_useParentPosition) ? base.transform : base.transform.parent);
		float num = transform.position.z - _character.TransformPosition.z;
		if (num >= _minZProximity && num <= _maxZProximity)
		{
			_source.pitch = Random.Range(_clipInfo.minPitch, _clipInfo.maxPitch);
			if (!_muted)
			{
				_source.volume = Random.Range(_clipInfo.minVolume, _clipInfo.maxVolume);
			}
			else
			{
				_source.volume = 0f;
			}
			_source.Play();
		}
	}

	public void Stop()
	{
		_source.Stop();
	}
}
