using System.Collections;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using Missions;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Themes;
using UnityEngine;

public class AudioStateLoop : SingletonMonoBehaviour<AudioStateLoop>
{
	public AudioSource musicPlayer;

	private float musicVolume;

	private bool isOtherAudioPlaying;

	public float menuMusicVolume = 0.3f;

	public float ingameMusicVolume = 0.3f;

	public AudioClip jetpackLoop;

	public float jetpackVolume = 0.4f;

	public float jetpackMinPitch = 0.8f;

	public float jetpackMaxPitch = 1.1f;

	public AudioClip magnetLoop;

	public float magnetVolume = 0.4f;

	public float magnetMinPitch = 0.8f;

	public float magnetMaxPitch = 1.1f;

	public AudioClip mysteryBoxOpenSound;

	public float mysteryVolume = 1f;

	public AudioClip mysteryRewardShowSound;

	public float mysteryRewardVolume = 1f;

	public AudioClip celebrationTapSound;

	public float celebrationTapVolume = 1f;

	public AudioClip unlockSound;

	public float unlockSoundVolume = 1f;

	public AudioClip reviveSound;

	public float reviveVolume = 1f;

	public float revivePauseTime = 1f;

	public float reviveFadeUpTime = 2f;

	private AudioSource jetpackSource;

	private AudioSource magnetSource;

	private AudioSource mysterySource;

	private AudioSource mysteryRewardSource;

	private AudioSource celebrationTapSource;

	private AudioSource reviveSource;

	private AudioSource unlockSource;

	private bool hasPlayedIntro;

	private bool _resumeJetpackOnUnpause;

	private bool _resumeCoinMagnetOnUnpause;

	public float fadeDownTime = 0.5f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	protected override void SubclassInitialize()
	{
		base.SubclassInitialize();
		jetpackSource = base.gameObject.AddComponent<AudioSource>();
		jetpackSource.clip = jetpackLoop;
		jetpackSource.volume = jetpackVolume;
		jetpackSource.loop = true;
		jetpackSource.playOnAwake = false;
		magnetSource = base.gameObject.AddComponent<AudioSource>();
		magnetSource.clip = magnetLoop;
		magnetSource.volume = magnetVolume;
		magnetSource.loop = true;
		magnetSource.playOnAwake = false;
		mysterySource = base.gameObject.AddComponent<AudioSource>();
		mysterySource.clip = mysteryBoxOpenSound;
		mysterySource.volume = mysteryVolume;
		mysterySource.playOnAwake = false;
		celebrationTapSource = base.gameObject.AddComponent<AudioSource>();
		celebrationTapSource.clip = celebrationTapSound;
		celebrationTapSource.volume = celebrationTapVolume;
		celebrationTapSource.playOnAwake = false;
		mysteryRewardSource = base.gameObject.AddComponent<AudioSource>();
		mysteryRewardSource.clip = mysteryRewardShowSound;
		mysteryRewardSource.volume = mysteryRewardVolume;
		mysteryRewardSource.playOnAwake = false;
		unlockSource = base.gameObject.AddComponent<AudioSource>();
		unlockSource.clip = unlockSound;
		unlockSource.volume = unlockSoundVolume;
		unlockSource.playOnAwake = false;
		reviveSource = base.gameObject.AddComponent<AudioSource>();
		reviveSource.clip = reviveSound;
		reviveSource.volume = reviveVolume;
		reviveSource.playOnAwake = false;
		musicVolume = ingameMusicVolume;
		UpdateMusicPlayer();
		SetGameSoundVolume();
		musicPlayer.bypassEffects = true;
		Game.Instance.OnPauseChange += OnPauseChange;
		Settings.onMusicSettingsChanged += OnMusicSettingChanged;
		Settings.onSfxSettingChanged += OnSfxSettingChanged;
		ThemeManager.Instance.ThemeChanged += OnThemeChanged;
	}

	protected void Start()
	{
		OnMusicSettingChanged();
		OnSfxSettingChanged();
	}

	private void OnPauseChange(bool isPaused)
	{
		if (isPaused)
		{
			if (jetpackSource.isPlaying)
			{
				_resumeJetpackOnUnpause = true;
				jetpackSource.Pause();
			}
			if (magnetSource.isPlaying)
			{
				_resumeCoinMagnetOnUnpause = true;
				magnetSource.Pause();
			}
			return;
		}
		if (MissionsManager.Instance.inRun)
		{
			if (_resumeJetpackOnUnpause)
			{
				jetpackSource.UnPause();
			}
			if (_resumeCoinMagnetOnUnpause)
			{
				magnetSource.UnPause();
			}
		}
		_resumeJetpackOnUnpause = false;
		_resumeCoinMagnetOnUnpause = false;
	}

	private void OnThemeChanged(Theme theme)
	{
		musicPlayer.clip = theme.GameMusic;
	}

	private void OnMusicSettingChanged()
	{
		OnThemeChanged(ThemeManager.Instance.Theme);
		if (Settings.optionMusic)
		{
			musicPlayer.Play();
		}
		else
		{
			musicPlayer.Pause();
		}
	}

	public void ForceStopMusic()
	{
		musicPlayer.Pause();
	}

	private void OnSfxSettingChanged()
	{
		if (Settings.optionSfx)
		{
			jetpackSource.volume = jetpackVolume;
			magnetSource.volume = magnetVolume;
		}
		else
		{
			jetpackSource.volume = 0f;
			magnetSource.volume = 0f;
		}
	}

	private void UpdateMusicPlayer()
	{
		musicPlayer.volume = musicVolume;
	}

	private void SetGameSoundVolume()
	{
		if (Settings.anySoundEnabled)
		{
			isOtherAudioPlaying = DeviceUtility.GetIOSOtherAudioPlaying();
			if (isOtherAudioPlaying)
			{
				Settings.DisableAllSoundOptions();
			}
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StartCoroutine(ResumeSoundAfterFrames(1));
		}
	}

	private IEnumerator ResumeSoundAfterFrames(int frames)
	{
		int index = 0;
		while (index < frames)
		{
			index++;
			yield return null;
		}
		SetGameSoundVolume();
	}

	public void PlayCelebrationOpenSound()
	{
		if (Settings.optionSfx)
		{
			mysterySource.Play();
			StartCoroutine(MusicFader(fadeUpTime, pauseTime));
		}
	}

	public void PlayRewardMysterySound()
	{
		if (Settings.optionSfx)
		{
			mysteryRewardSource.Play();
			StartCoroutine(MusicFader(fadeUpTime, pauseTime));
		}
	}

	public void PlayCelebrationTapSound()
	{
		if (Settings.optionSfx)
		{
			celebrationTapSource.Play();
			StartCoroutine(MusicFader(fadeUpTime, pauseTime));
		}
	}

	public void PlayUnlockSound()
	{
		if (Settings.optionSfx)
		{
			unlockSource.Play();
			StartCoroutine(MusicFader(fadeUpTime, pauseTime));
		}
	}

	public void PlayReviveSound()
	{
		if (Settings.optionSfx)
		{
			reviveSource.Play();
			StartCoroutine(MusicFader(reviveFadeUpTime, revivePauseTime));
		}
	}

	private IEnumerator MusicFader(float timeFadeUp, float timePauseTime)
	{
		float counter = 0f;
		float startFade = musicVolume;
		float fadeSpeed = 1f / fadeDownTime;
		while (counter < 1f)
		{
			musicVolume = Mathf.Lerp(startFade, 0f, counter);
			UpdateMusicPlayer();
			counter += Time.deltaTime * fadeSpeed;
			yield return null;
		}
		musicVolume = 0f;
		UpdateMusicPlayer();
		yield return new WaitForSeconds(timePauseTime);
		counter = 0f;
		fadeSpeed = 1f / timeFadeUp;
		while (counter < 1f)
		{
			musicVolume = Mathf.Lerp(0f, menuMusicVolume, counter);
			UpdateMusicPlayer();
			counter += Time.deltaTime * fadeSpeed;
			yield return null;
		}
		musicVolume = menuMusicVolume;
		UpdateMusicPlayer();
	}

	public void ChangeLoop(AudioState audioState)
	{
		switch (audioState)
		{
		case AudioState.Menu:
			if (hasPlayedIntro)
			{
				musicPlayer.bypassEffects = false;
				musicVolume = menuMusicVolume;
				UpdateMusicPlayer();
			}
			break;
		case AudioState.Ingame:
			if (hasPlayedIntro)
			{
				musicPlayer.timeSamples = 0;
			}
			else
			{
				hasPlayedIntro = true;
			}
			musicPlayer.bypassEffects = true;
			musicVolume = ingameMusicVolume;
			UpdateMusicPlayer();
			break;
		case AudioState.Jetpack:
			PlayLoop(jetpackSource, jetpackMaxPitch, jetpackMaxPitch);
			break;
		case AudioState.JetpackStop:
			StopLoop(jetpackSource);
			break;
		case AudioState.Magnet:
			PlayLoop(magnetSource, magnetMinPitch, magnetMaxPitch);
			break;
		case AudioState.MagnetStop:
			StopLoop(magnetSource);
			break;
		}
	}

	public void PlayLoop(AudioSource audioSource)
	{
		audioSource.pitch = Random.Range(0.8f, 1.1f);
		audioSource.Play();
	}

	public void PlayLoop(AudioSource audioSource, float minPitch, float maxPitch)
	{
		audioSource.pitch = Random.Range(minPitch, maxPitch);
		audioSource.Play();
	}

	public void StopLoop(AudioSource audioSource)
	{
		audioSource.Stop();
	}
}
