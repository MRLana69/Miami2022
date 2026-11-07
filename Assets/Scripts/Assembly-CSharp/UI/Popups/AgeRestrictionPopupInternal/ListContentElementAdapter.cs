using System.Collections.Generic;
using UI.KScrollView;
using UnityEngine;

namespace UI.Popups.AgeRestrictionPopupInternal
{
	public class ListContentElementAdapter : ListAdapter<ListContentElement>
	{
		private readonly GameObject _ageCellPrefab;

		private readonly ScrollView _scrollView;

		private readonly CenterOnChild _centerOnChild;

		private readonly string _initialContent;

		public ListContentElementAdapter(List<ListContentElement> list, GameObject ageCell, ScrollView scrollView, CenterOnChild centerOnChild, string initialContent)
			: base(list)
		{
			_ageCellPrefab = ageCell;
			_scrollView = scrollView;
			_centerOnChild = centerOnChild;
			_initialContent = initialContent;
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
		{
			Container container2 = container;
			if (container2 == null)
			{
				container2 = new Container(_ageCellPrefab);
				ListContentElementReferences listContentElementReferences = new ListContentElementReferences();
				listContentElementReferences.listElementHelper = container2.prefabInstance.GetComponent<ListElementHelper>();
				listContentElementReferences.scrollView = _scrollView;
				listContentElementReferences.centerOnChild = _centerOnChild;
				listContentElementReferences.initialContent = _initialContent;
				ListContentElementReferences controlComponent = listContentElementReferences;
				container2.controlComponent = controlComponent;
			}
			if (!isMeasurePass)
			{
				ListContentElementReferences listContentElementReferences2 = (ListContentElementReferences)container2.controlComponent;
				ListContentElement dataItem = GetDataItem(dataIndex);
				listContentElementReferences2.listElementHelper.InitCell(dataItem, listContentElementReferences2);
			}
			return container2;
		}
	}
}
