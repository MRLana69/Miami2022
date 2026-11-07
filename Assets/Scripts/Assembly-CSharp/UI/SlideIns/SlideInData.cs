using System.Collections.Generic;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.SlideIns
{
	public class SlideInData : ScriptableObject
	{
		public enum SlideInType
		{
			TopRunTimer = 0,
			Mission = 1,
			MissionSet = 2,
			Letters = 3,
			LettersCompleteMysteryBox = 4,
			LettersCompleteCoins = 5,
			Unlock = 6,
			ErrorMessage = 7,
			WeeklyHunt = 8,
			OfferWall = 9,
			VideoPayout = 10,
			Brag = 11,
			Awards = 12,
			Facebook = 13,
			CharacterEventWord = 14,
			CharacterEventWordReward = 15,
			RemoveAds = 16,
			Gift = 17,
			FirstPurchaseBonus = 18,
			CloudSave = 19,
			CloudSaveMessage = 20,
			Text = 21
		}

		[SerializeField]
		private List<UISlideIn> _slideInPrefabs;

		[SerializeField]
		private AudioClipInfo _slideInSound;

		[SerializeField]
		private AudioClipInfo _slideInFanfare;

		private static SlideInData _instance;

		public static SlideInData instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<SlideInData>("Info/SlideInData");
				}
				return _instance;
			}
		}

		public T InstantiateSlideIn<T>(Transform parent) where T : UISlideIn
		{
			return Object.Instantiate(GetPrefab<T>(), parent);
		}

		public AudioClipInfo GetSlideInSound(SlideIn slideIn)
		{
			switch (slideIn.type)
			{
			case SlideInType.TopRunTimer:
			case SlideInType.Letters:
			case SlideInType.ErrorMessage:
			case SlideInType.CharacterEventWord:
			case SlideInType.CharacterEventWordReward:
			case SlideInType.FirstPurchaseBonus:
			case SlideInType.CloudSave:
			case SlideInType.CloudSaveMessage:
			case SlideInType.Text:
				return _slideInSound;
			case SlideInType.Facebook:
			{
				UISlideInFacebook.FacebookSlideInState facebookState = slideIn.facebookState;
				if (facebookState != UISlideInFacebook.FacebookSlideInState.ConnectSucceeded && facebookState != UISlideInFacebook.FacebookSlideInState.LoggedOut)
				{
					return _slideInSound;
				}
				return _slideInFanfare;
			}
			default:
				return _slideInFanfare;
			}
		}

		private void OnValidate()
		{
			foreach (UISlideIn slideInPrefab in _slideInPrefabs)
			{
				if (slideInPrefab == null)
				{
					Debug.LogError("[SlideInData]: A Slidein prefab was null!");
				}
			}
		}

		private T GetPrefab<T>() where T : UISlideIn
		{
			foreach (UISlideIn slideInPrefab in _slideInPrefabs)
			{
				T val = slideInPrefab as T;
				if (val != null)
				{
					return val;
				}
			}
			Debug.LogError("[SlideInData]: A Slidein ({0}) wasn't found in the SlideInData".Populate(typeof(T)));
			return (T)null;
		}
	}
}
