using System.Collections.Generic;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Generic;
using UI.KScrollView;
using UI.Popups.AgeRestrictionPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	// Menu de rolagem (mesmo visual do de data de nascimento) para escolher o Modo de Treino ou o Tipo de Impossível.
	public class TrainingPickerPopup : UIPopup
	{
		public enum PickerTarget
		{
			Mode = 0,
			ImpossibleType = 1
		}

		public static PickerTarget target = PickerTarget.Mode;

		// Os nomes dos campos são os mesmos do AgeRestrictionPopup, porque o prefab foi copiado dele.
		[SerializeField]
		private GameObject _ageCellPrefab;

		[SerializeField]
		private ScrollView _listViewYear;

		[SerializeField]
		private UIButtonOverlayOff _acceptButton;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _okLabel;

		private List<ListContentElement> _content;

		private CenterOnChild _centerOnChild;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_centerOnChild = _listViewYear.gameObject.AddComponent<CenterOnChild>();
		}

		protected override void Show()
		{
			_content = CreateContent();
			_listViewYear.SetAdapter(new ListContentElementAdapter(_content, _ageCellPrefab, _listViewYear, _centerOnChild, string.Empty));
			int selected = (target != PickerTarget.Mode) ? ((int)TrainingOptions.CurrentImpossibleType) : ((int)TrainingOptions.CurrentMode);
			int index = _content.FindIndex((ListContentElement entry) => entry.value == selected);
			_listViewYear.SetSelection((index >= 0) ? index : Padding, ContainerPivot.Center);
			_centerOnChild.Recenter();
			_centerOnChild.selectedObjectChanged -= OnSelectionChanged;
			_centerOnChild.selectedObjectChanged += OnSelectionChanged;
			_title.text = TrainingOptions.Tr((target != PickerTarget.Mode) ? "IMPOSSIBLE_TYPE" : "TRAINING_MODE");
			_title.color = ColorData.instance.midnight;
			_okLabel.text = "OK";
			_acceptButton.GoToNormalState();
			_okLabel.GetComponent<ButtonText>().Refresh();
		}

		[UsedImplicitly]
		public void AcceptClicked()
		{
			GameObject selectedObject = _centerOnChild.selectedObject;
			if (selectedObject != null)
			{
				int value = selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
				if (value <= PaddingValue)
				{
					ClosePopup();
					return;
				}
				if (target == PickerTarget.Mode)
				{
					TrainingOptions.CurrentMode = (TrainingOptions.Mode)value;
				}
				else
				{
					TrainingOptions.CurrentImpossibleType = (TrainingOptions.ImpossibleType)value;
				}
			}
			ClosePopup();
		}

		[UsedImplicitly]
		public void YearArrowUpClicked()
		{
			ArrowClicked(true);
		}

		[UsedImplicitly]
		public void YearArrowDownClicked()
		{
			ArrowClicked(false);
		}

		// O prefab ainda tem as setas do mês (escondidas); ficam aqui só para o clique não dar erro.
		[UsedImplicitly]
		public void MonthArrowUpClicked()
		{
			ArrowClicked(true);
		}

		[UsedImplicitly]
		public void MonthArrowDownClicked()
		{
			ArrowClicked(false);
		}

		private void ArrowClicked(bool up)
		{
			if (_centerOnChild.selectedObject == null)
			{
				return;
			}
			int currentValue = _centerOnChild.selectedObject.GetComponent<ListElementHelper>().listContentElement.value;
			int index = _content.FindIndex((ListContentElement entry) => entry.value == currentValue);
			// A lista tem limite: para no primeiro e no último (as linhas vazias das pontas não contam).
			index = (up ? (index - 1) : (index + 1));
			GoTo(Mathf.Clamp(index, Padding, _content.Count - 1 - Padding));
		}

		// Linhas vazias no começo e no fim: sem elas o primeiro e o último não conseguem ficar no meio.
		private const int Padding = 2;

		private static bool IsPadding(ListContentElement element)
		{
			return element.value <= PaddingValue;
		}

		private const int PaddingValue = -1000;

		// Se arrastar até uma linha vazia, volta para o item mais perto.
		private void OnSelectionChanged()
		{
			GameObject selected = _centerOnChild.selectedObject;
			if (selected == null)
			{
				return;
			}
			ListContentElement element = selected.GetComponent<ListElementHelper>().listContentElement;
			if (!IsPadding(element))
			{
				return;
			}
			int index = _content.IndexOf(element);
			GoTo((index < Padding) ? Padding : (_content.Count - 1 - Padding));
		}

		private void GoTo(int index)
		{
			ListElementHelper helper = _content[index].helper;
			if (helper != null && helper.gameObject.activeInHierarchy && helper.listContentElement == _content[index])
			{
				_centerOnChild.CenterOnTransform(helper.transform);
			}
			else
			{
				_listViewYear.SetSelection(index, ContainerPivot.Center);
				_centerOnChild.Recenter();
			}
		}

		private static List<ListContentElement> CreateContent()
		{
			List<ListContentElement> list = new List<ListContentElement>();
			for (int i = 0; i < Padding; i++)
			{
				list.Add(new ListContentElement(string.Empty, PaddingValue - i));
			}
			if (target == PickerTarget.Mode)
			{
				foreach (TrainingOptions.Mode mode in TrainingOptions.ModeOrder)
				{
					list.Add(new ListContentElement(TrainingOptions.GetName(mode), (int)mode));
				}
			}
			else
			{
				foreach (TrainingOptions.ImpossibleType type in TrainingOptions.ImpossibleOrder)
				{
					list.Add(new ListContentElement(TrainingOptions.GetName(type), (int)type));
				}
			}
			for (int i = 0; i < Padding; i++)
			{
				list.Add(new ListContentElement(string.Empty, PaddingValue - Padding - i));
			}
			return list;
		}
	}
}
