using SYBO.Subway.Info;
using UI.Screens.CharacterScreenInternal;
using UnityEngine;

namespace UI
{
	public class CharacterScrollListSlot : MonoBehaviour
	{
		public int index = -1;

		public Characters.CharacterId characterId;

		public int themeIndex;

		public GameObject model;

		public ScrollListCharacterUI ui;

		public CharacterModelRequest request;

		public bool modelReady
		{
			get
			{
				return model != null;
			}
		}

		public void SetEnabledState(bool modelEnabled, bool uiEnabled)
		{
			if (modelReady)
			{
				model.SetActive(modelEnabled);
			}
			if (ui != null)
			{
				ui.gameObject.SetActive(uiEnabled);
				ui.isEnabled = uiEnabled;
			}
		}

		public void DestroyModel()
		{
			if (ui != null)
			{
				ui.OnModelDestroyed();
			}
			else
			{
				SDebug.LogError("CharacterScrollListSlot: Missing reference to UI on character " + characterId);
			}
			if (modelReady)
			{
				Object.Destroy(model);
				model = null;
			}
		}
	}
}
