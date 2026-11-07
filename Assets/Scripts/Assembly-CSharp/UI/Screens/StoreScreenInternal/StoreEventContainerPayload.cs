using System;
using Data.Managers;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class StoreEventContainerPayload : ScriptableObject
	{
		public bool enabled = true;

		public ShopEventContainerCellAppearancePayload cellAppearancePayload;

		public Action<StoreEventContainerSmall> onContainerUpdate;

		public bool useCharacter = true;

		public bool isBoardBundle;

		public CharacterRenderTextureSetup.EventModelCharacter character;

		public BoardRenderTextureSetup.EventModelBoard board;

		public TimeSpan timeLeft
		{
			get
			{
				if (useCharacter)
				{
					return CharacterManager.CharacterExpirationTimespan(character.charID);
				}
				return BoardManager.BoardExpirationTimespan(board.boardID);
			}
		}

		public DateTime startTime
		{
			get
			{
				if (useCharacter)
				{
					return CharacterManager.CharacterStartAvailabilityDate(character.charID);
				}
				return BoardManager.BoardStartAvailabilityDate(board.boardID);
			}
		}
	}
}
