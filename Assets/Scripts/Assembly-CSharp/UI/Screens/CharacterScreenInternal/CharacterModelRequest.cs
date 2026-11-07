using System.Collections.Generic;
using System.Diagnostics;
using SYBO.Components.Utilities;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Screens.CharacterScreenInternal
{
	public class CharacterModelRequest
	{
		public enum RequestState
		{
			None = 0,
			RequestingBundles = 1,
			BuildingCharacter = 2,
			Finished = 3,
			Cancelled = 4
		}

		public class CharacterBundleRequest
		{
			public readonly Characters.CharacterId characterId;

			public readonly int Theme;

			public readonly string[] AssetBundleNames;

			private int _loadedBundleCount;

			private List<CharacterModelRequest> _requests = new List<CharacterModelRequest>();

			public CharacterBundleRequest(Characters.CharacterId characterId, int theme)
			{
				this.characterId = characterId;
				AssetBundleNames = SingletonMonoBehaviour<Factories>.Instance.GetReferencedAssetBundleNames(this.characterId, theme, false);
				_loadedBundleCount = 0;
			}

			public void RegisterModelRequest(CharacterModelRequest modelRequest)
			{
				_requests.Add(modelRequest);
			}

			public bool RemoveModelRequest(CharacterModelRequest modelRequest)
			{
				_requests.Remove(modelRequest);
				return _requests.Count == 0;
			}

			public int GetPriority()
			{
				int num = int.MaxValue;
				foreach (CharacterModelRequest request in _requests)
				{
					num = Mathf.Min(request._priority, num);
				}
				return num;
			}

			public void OnBundleLoaded(AssetBundle bundle)
			{
				if (++_loadedBundleCount != AssetBundleNames.Length)
				{
					return;
				}
				AssetBundleManager.Instance.CharacterBundleRequestCompleted(this);
				bool flag = false;
				foreach (CharacterModelRequest request in _requests)
				{
					if (request._state != RequestState.Cancelled)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					string[] assetBundleNames = AssetBundleNames;
					foreach (string text in assetBundleNames)
					{
						if (!SingletonMonoBehaviour<Factories>.Instance.IsAssetBundleInUse(text))
						{
							AssetBundleManager.Instance.UnloadAssetBundle(text, true);
						}
					}
					return;
				}
				foreach (CharacterModelRequest request2 in _requests)
				{
					request2.OnCharacterBundlesLoaded();
				}
			}
		}

		private const string CHARACTER_MODEL_REQUEST_DEBUG_LOG_TAG = "CharacterModelRequest: ";

		private readonly CharacterListManager _listManager;

		private readonly Characters.CharacterId _characterId;

		private readonly int _theme;

		private readonly int _scrollIndex;

		private readonly bool _synchronous;

		private RequestState _state;

		private int _priority;

		public Characters.CharacterId characterId
		{
			get
			{
				return _characterId;
			}
		}

		public RequestState State
		{
			get
			{
				return _state;
			}
		}

		public CharacterModelRequest(Characters.CharacterId charId, int theme, int scrollIndex, CharacterListManager listManager, bool synchronous = false)
		{
			_characterId = charId;
			_theme = theme;
			_scrollIndex = scrollIndex;
			_listManager = listManager;
			_synchronous = synchronous;
			_state = RequestState.RequestingBundles;
			if (!_synchronous)
			{
				AssetBundleManager.Instance.EnqueueAsyncCharacterBundleRequest(_characterId, _theme, OnCharacterBundlesLoaded, this);
			}
			else
			{
				OnCharacterBundlesLoaded();
			}
		}

		public void SetPriority(int priority)
		{
			_priority = priority;
		}

		public void CancelRequest()
		{
			_state = RequestState.Cancelled;
			AssetBundleManager.Instance.RemoveAsyncCharacterBundleRequest(this);
		}

		private void OnCharacterBundlesLoaded()
		{
			_state = RequestState.BuildingCharacter;
			CharacterModelPreviewFactory.Instance.GetCharacterModelPreview(_characterId.ToString(), _theme, HandleCompletedRequest, !_synchronous);
		}

		private void HandleCompletedRequest(GameObject characterModel)
		{
			if (_state == RequestState.Cancelled)
			{
				Object.Destroy(characterModel);
				return;
			}
			_state = RequestState.Finished;
			_listManager.OnModelLoaded(_characterId, _theme, _scrollIndex, characterModel, _synchronous);
		}

		[Conditional("ENABLE_CHARACTER_MODEL_REQUEST_LOGS")]
		private static void Log(string msg, Object context = null)
		{
		}
	}
}
