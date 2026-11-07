using System;
using Info;
using SYBO.Subway.Info;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	[Serializable]
	public class DuoPromotionEntry
	{
		[Header("Character Info")]
		[SerializeField]
		private SYBO.Subway.Info.Characters.CharacterId _characterId;

		[SerializeField]
		private AnimationClip _characterAnimation;

		[SerializeField]
		private bool _useSpecialAssets;

		[SerializeField]
		private int _characterThemeIndex;

		[Header("Popup Character Info")]
		[SerializeField]
		private Vector3 _popupCharacterPosition = new Vector3(0f, 0f, -35f);

		[SerializeField]
		private Vector3 _popupCharacterRotation = new Vector3(10f, 115f, -80f);

		[SerializeField]
		private float _popupCharacterScale = 13f;

		[Header("Store Character Info")]
		[SerializeField]
		private Vector3 _storeCharacterPosition = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 _storeCharacterRotation = new Vector3(10f, 115f, -80f);

		[SerializeField]
		private float _storeCharacterScale = 13f;

		[Header("Board Info")]
		[SerializeField]
		private Boards.BoardId _boardId = Boards.BoardId.hongKongBoard01;

		[SerializeField]
		private AnimationClip _boardAnimation;

		[Header("Popup Board Info")]
		[SerializeField]
		private Vector3 _popupBoardPosition = new Vector3(0f, 0f, -35f);

		[SerializeField]
		private Vector3 _popupBoardRotation = new Vector3(10f, 115f, -80f);

		[SerializeField]
		private float _popupBoardScale = 13f;

		[Header("Store Board Info")]
		[SerializeField]
		private Vector3 _storeBoardPosition = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 _storeBoardRotation = new Vector3(10f, 115f, -80f);

		[SerializeField]
		private float _storeBoardScale = 13f;

		public SYBO.Subway.Info.Characters.CharacterId CharacterId
		{
			get
			{
				return _characterId;
			}
		}

		public AnimationClip CharacterAnimation
		{
			get
			{
				return _characterAnimation;
			}
		}

		public bool UseSpecialAssets
		{
			get
			{
				return _useSpecialAssets;
			}
		}

		public int CharacterThemeIndex
		{
			get
			{
				return _characterThemeIndex;
			}
		}

		public Boards.BoardId BoardId
		{
			get
			{
				return _boardId;
			}
		}

		public Vector3 PopupBoardPosition
		{
			get
			{
				return _popupBoardPosition;
			}
		}

		public Vector3 PopupBoardRotation
		{
			get
			{
				return _popupBoardRotation;
			}
		}

		public Vector3 PopupBoardScale
		{
			get
			{
				return Vector3.one * _popupBoardScale;
			}
		}

		public AnimationClip BoardAnimation
		{
			get
			{
				return _boardAnimation;
			}
		}

		public Vector3 StoreBoardPosition
		{
			get
			{
				return _storeBoardPosition;
			}
		}

		public Vector3 StoreBoardRotation
		{
			get
			{
				return _storeBoardRotation;
			}
		}

		public Vector3 StoreBoardScale
		{
			get
			{
				return Vector3.one * _storeBoardScale;
			}
		}

		public Vector3 PopupCharacterPosition
		{
			get
			{
				return _popupCharacterPosition;
			}
		}

		public Vector3 PopupCharacterRotation
		{
			get
			{
				return _popupCharacterRotation;
			}
		}

		public Vector3 PopupCharacterScale
		{
			get
			{
				return Vector3.one * _popupCharacterScale;
			}
		}

		public Vector3 StoreCharacterPosition
		{
			get
			{
				return _storeCharacterPosition;
			}
		}

		public Vector3 StoreCharacterRotation
		{
			get
			{
				return _storeCharacterRotation;
			}
		}

		public Vector3 StoreCharacterScale
		{
			get
			{
				return Vector3.one * _storeCharacterScale;
			}
		}
	}
}
