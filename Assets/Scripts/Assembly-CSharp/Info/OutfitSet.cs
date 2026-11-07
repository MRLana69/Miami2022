using System;
using System.Linq;
using SYBO.Subway.Info;
using UnityEngine;

namespace Info
{
	[Serializable]
	public class OutfitSet
	{
		public Outfit[] outfitList;

		private Characters.CharacterId? _parsedId;

		[EnumToString(typeof(Characters.CharacterId))]
		[SerializeField]
		private string _serializedCharacter = Characters.CharacterId.amsterdamCharacter01.ToString();

		public Characters.CharacterId characterId
		{
			get
			{
				if (!_parsedId.HasValue)
				{
					_parsedId = (Characters.CharacterId)Enum.Parse(typeof(Characters.CharacterId), _serializedCharacter);
				}
				return _parsedId.Value;
			}
			set
			{
				_parsedId = value;
				_serializedCharacter = value.ToString();
			}
		}

		public bool HasOutfitWithName(string outfitName)
		{
			if (outfitList == null || outfitList.Length == 0)
			{
				return false;
			}
			return outfitList.Any((Outfit outfit) => outfit.title == outfitName);
		}
	}
}
