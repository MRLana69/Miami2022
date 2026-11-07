using System;
using System.Collections.Generic;

namespace UI.KScrollView
{
	public class ListAdapter<T> : AbstractListAdapter
	{
		public List<T> dataList { get; protected set; }

		protected ListAdapter(List<T> list)
		{
			dataList = list;
		}

		protected T GetDataItem(int index)
		{
			return dataList[index];
		}

		public override int GetDataEntryCount()
		{
			return dataList.Count;
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
		{
			throw new NotImplementedException();
		}
	}
}
