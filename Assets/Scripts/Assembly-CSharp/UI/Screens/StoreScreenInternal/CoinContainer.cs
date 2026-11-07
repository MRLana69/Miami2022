using System;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class CoinContainer : MonoBehaviour
	{
		private const float SPECIAL_SALE_ELEMENTS_OFFSET = -150f;

		private const float DEFAULT_OFFSET_FOR_ELEMENTS_ON_DISPLAY_VALUE = -59.5f;

		[NonSerialized]
		public float offsetForElementsOnDisplay = -59.5f;

		[SerializeField]
		private UITable _table;

		public int counterForElementsOnDisplay { get; set; }

		public void Reset()
		{
			counterForElementsOnDisplay = 0;
			foreach (Transform item in _table.transform)
			{
				NGUITools.SetActive(item.gameObject, false);
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}

		public void ActivateSpecialSale()
		{
			offsetForElementsOnDisplay = -150f;
		}

		public void UseDefaultOffsetForElementsOnDisplay()
		{
			offsetForElementsOnDisplay = -59.5f;
		}

		public void RepositionTable()
		{
			_table.Reposition();
		}

		public void RepositionTableNow()
		{
			_table.repositionNow = true;
		}

		public GameObject AddChildPrefab(GameObject prefab)
		{
			return _table.gameObject.AddChild(prefab);
		}

		public GameObject AddChildGo(GameObject go)
		{
			go.transform.parent = _table.transform;
			return go;
		}
	}
}
