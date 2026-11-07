using System;

namespace UI.Popups.SecretPopupInternal
{
	public class SecretCell
	{
		public SecretCellType cellType;

		public string cellInfo;

		public string cellValue;

		public string cellTitle;

		public string cellInfo2;

		public string cellValue2;

		public Action OnUpdateCell;

		public void UpdateCell()
		{
			if (OnUpdateCell != null)
			{
				OnUpdateCell();
			}
		}
	}
}
