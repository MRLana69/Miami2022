using System.Collections;
using UnityEngine;

public class Sound
{
	private So _manager;

	private GameObject gameObject;

	private bool _isAvailable = true;

	public AudioSource audioSource;

	public bool destroyAfterPlay;

	private string _name;

	public bool IsAvailable
	{
		get
		{
			return _isAvailable;
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
	}

	public bool loop
	{
		set
		{
			audioSource.loop = value;
		}
	}

	public Sound(So manager)
	{
		_manager = manager;
		gameObject = new GameObject();
		_name = "empty";
		gameObject.name = _name;
		gameObject.transform.parent = manager.transform;
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	public void destroySelf()
	{
		_manager.removeSound(this);
		Object.Destroy(gameObject);
	}

	public void Stop()
	{
		Pause();
		Rewind();
		destroySelf();
	}

	public void Pause()
	{
		audioSource.Stop();
	}

	public void Rewind()
	{
		audioSource.time = 0f;
	}

	public IEnumerator fadeOutAndStop(float duration)
	{
		return audioSource.fadeOut(duration, () =>
		{
			Stop();
		});
	}

	public IEnumerator playAudioClip(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position, bool willLoop = false)
	{
		_name = audioClip.name;
		if (gameObject != null)
		{
			gameObject.name = _name;
		}
		if (audioSource != null)
		{
			audioSource.clip = audioClip;
			audioSource.volume = Random.Range(minVolume, maxVolume);
			audioSource.pitch = Random.Range(minPitch, maxPitch);
		}
		return play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position, willLoop);
	}

	public IEnumerator play(AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position, bool willLoop)
	{
		bool hasGameObject = false;
		bool hasAudioSource = false;
		if (gameObject != null)
		{
			hasGameObject = true;
			gameObject.transform.position = position;
		}
		if ((bool)audioSource)
		{
			hasAudioSource = true;
			audioSource.rolloffMode = rolloff;
			audioSource.volume = Random.Range(minVolume, maxVolume);
			audioSource.pitch = Random.Range(minPitch, maxPitch);
			audioSource.loop = willLoop;
			_isAvailable = false;
			audioSource.GetComponent<AudioSource>().Play();
			if (!audioSource.loop)
			{
				// tempo real: com o jogo pausado (tela de Salve-me) o canal também é liberado
				// e com pitch abaixo de 1 o som dura mais que o arquivo: espera ele terminar inteiro
				yield return new WaitForSecondsRealtime(audioSource.clip.length / Mathf.Max(0.1f, Mathf.Abs(audioSource.pitch)) + 0.1f);
				if (audioSource != null)
				{
					audioSource.Stop();
				}
				if (destroyAfterPlay)
				{
					destroySelf();
				}
				_isAvailable = true;
			}
		}
		if (hasGameObject && hasAudioSource)
		{
		}
	}
}
