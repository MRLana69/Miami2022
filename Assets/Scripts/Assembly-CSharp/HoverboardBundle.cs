using System;
using System.Collections.Generic;
using Info;
using UnityEngine;

public class HoverboardBundle : MonoBehaviour
{
	[Serializable]
	public class CustomSet
	{
		public Hoverboard.BoardAbility ability;

		public GameObject baseModelPrefab;

		public GameObject modelPrefab;

		public GameObject VanityFxPrefab;

		public GameObject VanityMenuFxPrefab;

		public bool useDefaultmodel = true;

		public bool useSuperSneakers = true;

		public GameObject ExtraFXPrefab;

		public GameObject ExtraMenuFXPrefab;
	}

	[SerializeField]
	private CustomSet[] customSets;

	private bool converted;

	private Boards.BoardId _boardId;

	[SerializeField]
	[EnumToString(typeof(Boards.BoardId))]
	private string _serializedBoardId = string.Empty;

	[SerializeField]
	private GameObject modelPrefab;

	[SerializeField]
	private GameObject previewModelPrefab;

	private CharacterModel _charactermodel;

	public CustomSet[] CustomSets
	{
		get
		{
			return customSets;
		}
	}

	public Boards.BoardId boardId
	{
		get
		{
			if (!converted)
			{
				converted = true;
				_boardId = (Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), _serializedBoardId);
			}
			return _boardId;
		}
	}

	public CharacterModel Charactermodel
	{
		get
		{
			return _charactermodel;
		}
		set
		{
			_charactermodel = value;
		}
	}

	public GameObject GetPrefab()
	{
		return modelPrefab;
	}

	public void AddModel(bool isMenu)
	{
		if (!(modelPrefab != null))
		{
			return;
		}
		GameObject gameObject = InstatiateAndParentModelPrefab(modelPrefab);
		if (isMenu)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.gameObject.layer = Layers.Instance._3DGUI;
			}
		}
		if (_charactermodel != null)
		{
			_charactermodel.CurrentHoverboadBasemodel = gameObject.GetComponent<Renderer>();
		}
	}

	public void AddCustomPreviewModel()
	{
		if (previewModelPrefab != null)
		{
			GameObject gameObject = InstatiateAndParentModelPrefab(previewModelPrefab);
			gameObject.gameObject.layer = Layers.Instance._3DGUI;
			_charactermodel.CurrentHoverboadBasemodel = gameObject.GetComponent<Renderer>();
		}
		else
		{
			AddModel(true);
		}
	}

	public void AddCustomBaseModel(int customSet, bool isMenu)
	{
		if (customSets[customSet].baseModelPrefab != null)
		{
			GameObject gameObject = InstatiateAndParentModelPrefab(customSets[customSet].baseModelPrefab);
			Renderer componentInChildren = gameObject.GetComponentInChildren<Renderer>();
			if (isMenu)
			{
				componentInChildren.gameObject.layer = Layers.Instance._3DGUI;
			}
			if (_charactermodel != null)
			{
				_charactermodel.CurrentHoverboadBasemodel = componentInChildren;
			}
		}
	}

	public void AddCustomSetModelAndEffect(int customSet, bool isMenu, bool addEffect)
	{
		List<Renderer> list = new List<Renderer>();
		if (customSets[customSet].modelPrefab != null)
		{
			GameObject gameObject = InstatiateAndParentModelPrefab(customSets[customSet].modelPrefab);
			gameObject.GetComponentsInChildren(list);
		}
		foreach (Renderer item in list)
		{
			if (_charactermodel != null)
			{
				_charactermodel.CurrentHoverboardModelsRenderes.Add(item);
			}
			if (isMenu)
			{
				item.gameObject.layer = Layers.Instance._3DGUI;
			}
		}
		if (!addEffect)
		{
			return;
		}
		if (isMenu)
		{
			if (customSets[customSet].VanityMenuFxPrefab != null)
			{
				GameObject gameObject2 = InstatiateAndParentModelPrefab(customSets[customSet].VanityMenuFxPrefab);
				gameObject2.gameObject.layer = Layers.Instance._3DGUI;
			}
		}
		else if (customSets[customSet].VanityFxPrefab != null)
		{
			InstatiateAndParentModelPrefab(customSets[customSet].VanityFxPrefab);
		}
	}

	private GameObject InstatiateAndParentModelPrefab(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		return gameObject;
	}
}
