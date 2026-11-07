using System.Collections;
using System.Collections.Generic;
using Info;
using SYBO.Subway.Managers;
using UnityEngine;

public class So : MonoBehaviour
{
	public static So Instance;

	public int initialCapacity = 5;

	public int maxCapacity = 50;

	private List<Sound> _soundList;

	private Sound _bgSound;

	private bool _audioWasPlaying;

	private float _audioTime;

	private void Awake()
	{
		if (DemoVideoModeManager.IsEnabled)
		{
			Settings.optionSfx = true;
			Settings.optionMusic = false;
		}
		if (Instance != null)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		_soundList = new List<Sound>(initialCapacity);
		for (int i = 0; i < initialCapacity; i++)
		{
			_soundList.Add(new Sound(this));
		}
		Settings.onSfxSettingChanged += OnSfxSettingsChanged;
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			if (_bgSound != null)
			{
				_audioWasPlaying = true;
				_audioTime = _bgSound.audioSource.time;
			}
		}
		else if (_audioWasPlaying)
		{
			_audioWasPlaying = false;
			_bgSound.audioSource.time = _audioTime;
			_bgSound.audioSource.Play();
		}
	}

	private void OnSfxSettingsChanged()
	{
		if (!Settings.optionSfx)
		{
			MuteAllSfx();
		}
	}

	public void playBGMusic(AudioClip audioClip, float volume, bool loop)
	{
		if (Settings.optionMusic)
		{
			if (_bgSound == null)
			{
				_bgSound = new Sound(this);
			}
			_bgSound.loop = loop;
		}
	}

	private void MuteAllSfx()
	{
		foreach (Sound sound in _soundList)
		{
			sound.Pause();
		}
	}

	public void beginPlaySound(AudioClipInfo audioClip)
	{
		StartCoroutine(playSoundAsync(audioClip));
	}

	private IEnumerator playSoundAsync(AudioClipInfo audioClip)
	{
		yield return null;
		playSound(audioClip);
	}

	public Sound playSound(AudioClipInfo audioClip, bool willLoop = false)
	{
		if (audioClip.Clip == null)
		{
			return null;
		}
		return playSound(audioClip.Clip, audioClip.Rollof, audioClip.minVolume, audioClip.maxVolume, audioClip.minPitch, audioClip.maxPitch, base.transform.position, willLoop);
	}

	public Sound playSound(AudioClipInfo audioClipInfo, float hardVolume, float hardPitch)
	{
		return playSound(audioClipInfo.Clip, audioClipInfo.Rollof, hardVolume, hardVolume, hardPitch, hardPitch, base.transform.position);
	}

	public Sound playSound(AudioClipInfo audioClipInfo, float hardVolume, float hardPitch, bool willLoop = false)
	{
		return playSound(audioClipInfo.Clip, audioClipInfo.Rollof, hardVolume, hardVolume, hardPitch, hardPitch, base.transform.position, willLoop);
	}

	public Sound playSound(AudioClip audioClip, AudioRolloffMode rolloff, float volume, Vector3 position)
	{
		return playSound(audioClip, rolloff, volume, volume, 1f, 1f, position);
	}

	public Sound playSound(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position, bool willLoop = false)
	{
		if (!Settings.optionSfx)
		{
			return null;
		}
		Sound sound = null;
		bool flag = false;
		bool flag2 = false;
		string text = audioClip.name;
		foreach (Sound sound2 in _soundList)
		{
			if (!sound2.IsAvailable)
			{
				continue;
			}
			if (sound2.name == text)
			{
				sound = sound2;
				flag = true;
				break;
			}
			if (!flag2)
			{
				if (sound2.name == "empty")
				{
					flag2 = true;
				}
				sound = sound2;
			}
		}
		if (sound == null)
		{
			// Todos os canais ocupados: cria um novo (até maxCapacity). Antes reusava o canal 0, e os sons
			// da morte (batida, queda, câmera) caíam todos no mesmo canal e um cortava o outro.
			if (_soundList.Count < maxCapacity)
			{
				sound = new Sound(this);
				_soundList.Add(sound);
			}
			else
			{
				sound = _soundList[0];
			}
		}
		if (flag)
		{
			StartCoroutine(sound.play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position, willLoop));
		}
		else
		{
			StartCoroutine(sound.playAudioClip(audioClip, rolloff, minVolume, maxVolume, minPitch, maxPitch, position, willLoop));
		}
		return sound;
	}

	public void removeSound(Sound s)
	{
		_soundList.Remove(s);
	}
}
