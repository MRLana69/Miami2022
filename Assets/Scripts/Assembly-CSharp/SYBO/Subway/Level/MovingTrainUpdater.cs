using System.Collections.Generic;
using System.Linq;
using Info;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class MovingTrainUpdater : MonoBehaviour
	{
		private class TrainAudioWrapper
		{
			private GameObject _passingTrainGameObject;

			private Transform _passingTrainTransform;

			private AudioSource _passingTrainAudioSource;

			private MovingTrain _currentPassingTrain;

			private float previousVolume = 1f;

			public MovingTrain CurrentPassingTrain
			{
				get
				{
					return _currentPassingTrain;
				}
			}

			public TrainAudioWrapper(Transform parent)
			{
				_passingTrainGameObject = new GameObject("PassingTrainAudio");
				_passingTrainTransform = _passingTrainGameObject.transform;
				_passingTrainTransform.SetParent(parent);
				_passingTrainAudioSource = _passingTrainGameObject.AddComponent<AudioSource>();
				_passingTrainAudioSource.minDistance = 20f;
				_passingTrainAudioSource.maxDistance = 50f;
				_passingTrainAudioSource.playOnAwake = false;
				_passingTrainAudioSource.loop = true;
				_passingTrainAudioSource.spatialBlend = 1f;
			}

			public void SetAudioClip(AudioClip clip)
			{
				_passingTrainAudioSource.clip = clip;
			}

			public void ResetPassingTrain()
			{
				_currentPassingTrain = null;
				_passingTrainAudioSource.Stop();
			}

			public void SetPassingTrain(MovingTrain train)
			{
				if (train != _currentPassingTrain)
				{
					_currentPassingTrain = train;
					_passingTrainAudioSource.pitch = Random.Range(0.8f, 1.1f);
					_passingTrainAudioSource.volume = Random.Range(0.1f, 0.6f);
					if (!Settings.optionSfx)
					{
						Mute();
					}
					_passingTrainAudioSource.timeSamples = Random.Range(0, _passingTrainAudioSource.timeSamples);
					_passingTrainAudioSource.Play();
				}
			}

			public void Mute()
			{
				previousVolume = _passingTrainAudioSource.volume;
				_passingTrainAudioSource.volume = 0f;
			}

			public void Unmute()
			{
				_passingTrainAudioSource.volume = previousVolume;
			}

			public void Pause(bool pause)
			{
				if (pause)
				{
					if (_passingTrainAudioSource.isPlaying)
					{
						_passingTrainAudioSource.Pause();
					}
				}
				else if (_currentPassingTrain != null)
				{
					_passingTrainAudioSource.Play();
				}
			}

			public void UpdatePosition()
			{
				if (_currentPassingTrain != null)
				{
					_passingTrainTransform.position = _currentPassingTrain.GetTrainModelPosition();
				}
			}
		}

		private const int MaxNumberOfTrainAudios = 3;

		private const float MaxPlayingDistance = 300f;

		private List<MovingTrain> _trains = new List<MovingTrain>(20);

		private TrainAudioWrapper[] _audioWrappers = new TrainAudioWrapper[3];

		private Dictionary<MovingTrain, TrainAudioWrapper> _trainToAudioWrapperMap = new Dictionary<MovingTrain, TrainAudioWrapper>();

		private float _lastCameraPositionZ;

		private void Awake()
		{
			for (int i = 0; i < 3; i++)
			{
				TrainAudioWrapper trainAudioWrapper = new TrainAudioWrapper(base.transform);
				trainAudioWrapper.ResetPassingTrain();
				_audioWrappers[i] = trainAudioWrapper;
			}
			UpdateSfxVolumeToSettings();
			Game.Instance.OnPauseChange += Game_OnPauseChange;
			Settings.onSfxSettingChanged += OnSfxSettingChanged;
		}

		private void Game_OnPauseChange(bool pause)
		{
			for (int i = 0; i < 3; i++)
			{
				if (_audioWrappers[i] != null)
				{
					_audioWrappers[i].Pause(pause);
				}
			}
		}

		private void OnSfxSettingChanged()
		{
			UpdateSfxVolumeToSettings();
		}

		private void UpdateSfxVolumeToSettings()
		{
			for (int i = 0; i < 3; i++)
			{
				if (Settings.optionSfx)
				{
					if (_audioWrappers[i] != null)
					{
						_audioWrappers[i].Unmute();
					}
				}
				else if (_audioWrappers[i] != null)
				{
					_audioWrappers[i].Mute();
				}
			}
		}

		public void SetAudioClip(AudioClip clip)
		{
			for (int i = 0; i < 3; i++)
			{
				if (_audioWrappers[i] != null)
				{
					_audioWrappers[i].SetAudioClip(clip);
				}
			}
		}

		public void AddMovingTrain(MovingTrain train)
		{
			_trains.Add(train);
		}

		public void RemoveMovingTrain(MovingTrain train)
		{
			_trains.Remove(train);
			TrainAudioWrapper value;
			if (_trainToAudioWrapperMap.TryGetValue(train, out value))
			{
				value.ResetPassingTrain();
			}
		}

		private void Update()
		{
			if (Camera.main == null)
			{
				return;
			}
			float currentSpeed = Game.Instance.CurrentSpeed;
			float z = Character.Instance.TransformPosition.z;
			float z2 = Camera.main.transform.position.z;
			int count = _trains.Count;
			int num = count - 1;
			while (0 <= num)
			{
				MovingTrain movingTrain = _trains[num];
				movingTrain.UpdateMovingTrain(currentSpeed, z, z2);
				num--;
			}
			_trains.Sort((MovingTrain a, MovingTrain b) => a.DistanceToCameraZ.CompareTo(b.DistanceToCameraZ));
			int num2 = count - 1;
			while (0 <= num2)
			{
				MovingTrain movingTrain2 = _trains[num2];
				bool flag = num2 < 3 && movingTrain2.DistanceToCameraZ < 300f;
				TrainAudioWrapper value;
				_trainToAudioWrapperMap.TryGetValue(movingTrain2, out value);
				if (!flag && value != null)
				{
					value.ResetPassingTrain();
					_trainToAudioWrapperMap.Remove(movingTrain2);
				}
				else if (flag && value == null)
				{
					TrainAudioWrapper trainAudioWrapper = _audioWrappers.FirstOrDefault((TrainAudioWrapper w) => w.CurrentPassingTrain == null);
					trainAudioWrapper.SetPassingTrain(movingTrain2);
					_trainToAudioWrapperMap.Add(movingTrain2, trainAudioWrapper);
				}
				num2--;
			}
			for (int num3 = 0; num3 < 3; num3++)
			{
				_audioWrappers[num3].UpdatePosition();
			}
		}
	}
}
