using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.KScrollView
{
	[RequireComponent(typeof(UIPanel))]
	public class ScrollView : MonoBehaviour
	{
		private enum Infinite
		{
			None = 0,
			BothWays = 1
		}

		private enum BoundsCalculationMode
		{
			FullSize = 0,
			Incremental = 1
		}

		private enum Arrangement
		{
			Vertical = 0,
			Horizontal = 1
		}

		private class Recycler
		{
			public readonly List<Container> activeContainers = new List<Container>();

			private readonly Dictionary<int, List<Container>> _recycledContainers = new Dictionary<int, List<Container>>();

			public void AddToBeginningOfActiveContainers(Container container)
			{
				activeContainers.Insert(0, container);
			}

			public void AddToEndOfActiveContainers(Container container)
			{
				activeContainers.Add(container);
			}

			public Container FirstContainer()
			{
				if (activeContainers.Count > 0)
				{
					return activeContainers.First();
				}
				return null;
			}

			public Container LastContainer()
			{
				return activeContainers.Last();
			}

			public bool HasActiveContainers()
			{
				return activeContainers.Count > 0;
			}

			public void RecycleContainer(Container container)
			{
				activeContainers.Remove(container);
				container.onRecycleContainer.SafeInvoke();
				if (!_recycledContainers.ContainsKey(container.modelType))
				{
					_recycledContainers.Add(container.modelType, new List<Container>());
				}
				_recycledContainers[container.modelType].Add(container);
				container.modelTransform.localPosition = new Vector3(Screen.width * 2, Screen.height * 2, 0f);
			}

			public void Clear()
			{
				List<Container> list = new List<Container>(activeContainers);
				foreach (Container item in list)
				{
					RecycleContainer(item);
				}
				activeContainers.Clear();
				foreach (List<Container> value in _recycledContainers.Values)
				{
					foreach (Container item2 in value)
					{
						UnityEngine.Object.Destroy(item2.prefabInstance);
					}
					value.Clear();
				}
				_recycledContainers.Clear();
			}

			public Container RequestContainer(int modelType)
			{
				Container result = null;
				if (_recycledContainers.Count > 0)
				{
					if (!_recycledContainers.ContainsKey(modelType))
					{
						_recycledContainers.Add(modelType, new List<Container>());
					}
					else
					{
						List<Container> list = _recycledContainers[modelType];
						if (list.Count > 0)
						{
							result = list[0];
							list.RemoveAt(0);
						}
					}
				}
				return result;
			}

			public void RecycleActiveContainers()
			{
				List<Container> list = new List<Container>(activeContainers);
				activeContainers.Clear();
				foreach (Container item in list)
				{
					RecycleContainer(item);
				}
			}

			public void SetActiveContainerPositions()
			{
				activeContainers.ForEach((Container c) =>
				{
					c.SetPosition();
				});
			}

			public string PrintActiveContainers()
			{
				return activeContainers.Aggregate(string.Empty, (string current, Container item) => current + item.ToString() + " \n");
			}
		}

		[SerializeField]
		private Arrangement _arrangement = Arrangement.Horizontal;

		[FormerlySerializedAs("cellSpaceX")]
		[SerializeField]
		private int _cellPaddingHorizontal;

		[FormerlySerializedAs("cellSpaceY")]
		[SerializeField]
		private int _cellPaddingVertical;

		[SerializeField]
		private Infinite _infinite;

		private UICenterOnChild _centerOnChild;

		[NonSerialized]
		private UIPanel _rootPanel;

		[NonSerialized]
		private UIScrollView _uiScrollView;

		private Vector2 _nextAppendPosition = Vector2.zero;

		private Vector2 _nextPrependPosition = Vector2.zero;

		private bool _dataChangedDelegatesAdded;

		private AbstractListAdapter _adapter;

		private Recycler _recycler;

		private int _firstContainerDataIndex;

		private int _lastContainerDataIndex;

		private SpringPanel _springPanel;

		public Action onPreLayout;

		private int _currentMaxListCount;

		private int _currentMinListCount;

		[FormerlySerializedAs("boundsCalculationMode")]
		[SerializeField]
		private BoundsCalculationMode _boundsCalculationMode;

		private Bounds _listViewBounds;

		public UIPanel rootPanel
		{
			get
			{
				return (!_rootPanel) ? (_rootPanel = base.gameObject.GetComponent<UIPanel>()) : _rootPanel;
			}
		}

		public UIScrollView uiScrollView
		{
			get
			{
				return (!_uiScrollView) ? (_uiScrollView = base.gameObject.GetComponent<UIScrollView>()) : _uiScrollView;
			}
		}

		public Bounds listViewBounds
		{
			get
			{
				return _listViewBounds;
			}
		}

		public bool infinite
		{
			get
			{
				return _infinite == Infinite.BothWays;
			}
		}

		private int GetDataEntryCount()
		{
			return _adapter.GetDataEntryCount();
		}

		private int GetLastDataEntryIndex()
		{
			return GetDataEntryCount() - 1;
		}

		private void InitScrollView()
		{
			if (_adapter != null)
			{
				if (!_dataChangedDelegatesAdded)
				{
					_adapter.onNotifyDataSetChanged += OnDataSetChanged;
					rootPanel.onClipMove = OnClipMove;
					_dataChangedDelegatesAdded = true;
				}
				_recycler = new Recycler();
			}
			uiScrollView.movement = ((_arrangement != Arrangement.Horizontal) ? UIScrollView.Movement.Vertical : UIScrollView.Movement.Horizontal);
		}

		[UsedImplicitly]
		private void Start()
		{
			_centerOnChild = GetComponent<UICenterOnChild>();
		}

		private void OnClipMove(UIPanel panel)
		{
			if (_adapter == null)
			{
				return;
			}
			HandleScrollUp();
			HandleScrollDown();
			if (_infinite == Infinite.BothWays)
			{
				Vector3 center = _listViewBounds.center;
				if (_arrangement == Arrangement.Horizontal)
				{
					center.x = rootPanel.clipOffset.x;
				}
				else
				{
					center.y = rootPanel.clipOffset.y;
				}
				_listViewBounds.center = center;
			}
		}

		private void ClearList()
		{
			uiScrollView.DisableSpring();
			_lastContainerDataIndex = 0;
			_firstContainerDataIndex = 0;
			_nextAppendPosition = Vector2.zero;
			_nextPrependPosition = Vector2.zero;
			rootPanel.transform.localPosition = Vector3.zero;
			rootPanel.clipOffset = Vector2.zero;
			_recycler.Clear();
		}

		private void OnDataSetChanged(int selection)
		{
			if (GetDataEntryCount() == 0)
			{
				ClearList();
			}
			else
			{
				SetSelection(selection);
			}
			uiScrollView.UpdateScrollbars(false);
			rootPanel.transform.localPosition = Vector3.zero;
			rootPanel.clipOffset = Vector2.zero;
		}

		public void CancelLoading()
		{
			_springPanel = GetComponent<SpringPanel>();
			_springPanel.target = Vector3.zero;
			_springPanel.enabled = true;
		}

		public void SetAdapter(AbstractListAdapter adapter)
		{
			if (_recycler != null)
			{
				_recycler.Clear();
			}
			_adapter = adapter;
			_dataChangedDelegatesAdded = false;
			InitScrollView();
			SetSelection(0);
			uiScrollView.UpdateScrollbars(false);
		}

		public void PrintActiveContainers()
		{
		}

		private void PrependContainer(int position, Container container, bool setPosition)
		{
			container.positionIndex = position;
			container.SetModelType(_adapter.GetModelType(position));
			container.Measure();
			container.SetParent(rootPanel.transform);
			Container container2 = _recycler.FirstContainer();
			_nextPrependPosition = container2.coordinates;
			_nextPrependPosition.x -= container.width * 0.5f + container2.width * 0.5f + (float)_cellPaddingHorizontal + container.bounds.center.x - container2.bounds.center.x;
			_nextPrependPosition.y += container.height * 0.5f + container2.height * 0.5f + (float)_cellPaddingVertical - container.bounds.center.y + container2.bounds.center.y;
			container.bottom = container2.top;
			container.top = container.bottom - container.height - (float)_cellPaddingVertical;
			container.right = container2.left;
			container.left = container.right - container.width - (float)_cellPaddingHorizontal;
			Vector2 nextPrependPosition = _nextPrependPosition;
			if (_arrangement == Arrangement.Horizontal)
			{
				nextPrependPosition.y = rootPanel.baseClipRegion.y;
			}
			else
			{
				nextPrependPosition.x = rootPanel.baseClipRegion.x;
			}
			container.coordinates = nextPrependPosition;
			_recycler.AddToBeginningOfActiveContainers(container);
			_firstContainerDataIndex--;
			if (setPosition)
			{
				container.SetPosition();
			}
		}

		private void AppendContainer(int position, Container container, bool setPosition)
		{
			container.positionIndex = position;
			container.SetModelType(_adapter.GetModelType(position));
			container.SetParent(rootPanel.transform);
			container.Measure();
			Container container2 = null;
			if (_recycler.HasActiveContainers())
			{
				container2 = _recycler.LastContainer();
				_nextAppendPosition = container2.coordinates;
			}
			if (container2 != null)
			{
				_nextAppendPosition.x += container.width * 0.5f + container2.width * 0.5f + (float)_cellPaddingHorizontal + container2.bounds.center.x - container.bounds.center.x;
				_nextAppendPosition.y -= container.height * 0.5f + container2.height * 0.5f + (float)_cellPaddingVertical - container2.bounds.center.y + container.bounds.center.y;
				container.top = container2.bottom;
				container.bottom = container.top + container.height + (float)_cellPaddingVertical;
				container.left = container2.right;
				container.right = container.left + container.width + (float)_cellPaddingHorizontal;
			}
			Vector2 coordinates = (container.coordinates = _nextAppendPosition);
			if (_arrangement == Arrangement.Horizontal)
			{
				coordinates.y = rootPanel.baseClipRegion.y;
			}
			else
			{
				coordinates.x = rootPanel.baseClipRegion.x;
			}
			container.coordinates = coordinates;
			_recycler.AddToEndOfActiveContainers(container);
			_lastContainerDataIndex++;
			if (setPosition)
			{
				container.SetPosition();
			}
		}

		public void SetSelection(int positionIndex, ContainerPivot containerPivot = ContainerPivot.Top, float offset = 0f)
		{
			if (_adapter == null)
			{
				SDebug.LogError(base.name + " : Set the adapter first.", base.gameObject);
				return;
			}
			positionIndex = Mathf.Clamp(positionIndex, 0, GetLastDataEntryIndex());
			FillFromIndex(positionIndex, offset, containerPivot);
			uiScrollView.currentMomentum = Vector3.zero;
		}

		public void SmoothScrollToIndex(int dataIndex, ContainerPivot containerPivot = ContainerPivot.Center, Action onFinished = null)
		{
			Container container = _recycler.activeContainers.Find((Container item) => item.positionIndex == dataIndex);
			if (container != null)
			{
				SmoothScrollToItem(container.modelTransform, containerPivot, onFinished);
				return;
			}
			if (dataIndex <= _firstContainerDataIndex)
			{
				Vector3 localOffset = default(Vector3);
				for (int num = _firstContainerDataIndex; num >= dataIndex; num--)
				{
					Container container2 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(num)), num);
					container2.SetModelType(_adapter.GetModelType(num));
					container2.Measure();
					container2.SetParent(rootPanel.transform);
					if (_arrangement == Arrangement.Vertical)
					{
						localOffset += new Vector3(0f, container2.height + (float)_cellPaddingVertical, 0f);
					}
					else
					{
						localOffset += new Vector3(container2.width + (float)_cellPaddingHorizontal, 0f, 0f);
					}
					_recycler.RecycleContainer(container2);
				}
				SmoothScrollToOffset(localOffset, onFinished);
				return;
			}
			Vector3 localOffset2 = default(Vector3);
			for (int num2 = _lastContainerDataIndex; num2 <= dataIndex; num2++)
			{
				Container container3 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(num2)), num2);
				container3.SetModelType(_adapter.GetModelType(num2));
				container3.Measure();
				container3.SetParent(rootPanel.transform);
				if (_arrangement == Arrangement.Vertical)
				{
					localOffset2 -= new Vector3(0f, container3.height + (float)_cellPaddingVertical, 0f);
				}
				else
				{
					localOffset2 -= new Vector3(container3.width + (float)_cellPaddingHorizontal, 0f, 0f);
				}
				_recycler.RecycleContainer(container3);
			}
			SmoothScrollToOffset(localOffset2, onFinished);
		}

		public void SmoothScrollToItem(Transform target, ContainerPivot containerPivot = ContainerPivot.Center, Action onFinished = null)
		{
			if (_centerOnChild != null)
			{
				_centerOnChild.CenterOn(target);
				return;
			}
			Vector3[] worldCorners = _uiScrollView.panel.worldCorners;
			Vector3 position;
			switch (containerPivot)
			{
			case ContainerPivot.Center:
			case ContainerPivot.None:
				position = (worldCorners[2] + worldCorners[0]) * 0.5f;
				break;
			case ContainerPivot.Top:
				position = ((_arrangement != Arrangement.Vertical) ? ((worldCorners[0] + worldCorners[1]) * 0.5f) : ((worldCorners[1] + worldCorners[2]) * 0.5f));
				break;
			case ContainerPivot.Bottom:
				position = ((_arrangement != Arrangement.Vertical) ? ((worldCorners[2] + worldCorners[3]) * 0.5f) : ((worldCorners[0] + worldCorners[3]) * 0.5f));
				break;
			default:
				position = (worldCorners[2] + worldCorners[0]) * 0.5f;
				break;
			}
			Transform cachedTransform = _uiScrollView.panel.cachedTransform;
			Vector3 vector = cachedTransform.InverseTransformPoint(target.position);
			Vector3 vector2 = cachedTransform.InverseTransformPoint(position);
			Vector3 localOffset = vector - vector2;
			UIWidget component = target.GetComponent<UIWidget>();
			if (containerPivot != ContainerPivot.Center && containerPivot != ContainerPivot.None && component != null)
			{
				switch (_arrangement)
				{
				case Arrangement.Horizontal:
					localOffset.x += ((containerPivot != ContainerPivot.Bottom) ? 0.5f : (-0.5f)) * (float)component.width;
					break;
				case Arrangement.Vertical:
					localOffset.y += ((containerPivot != ContainerPivot.Bottom) ? 0.5f : (-0.5f)) * (float)component.height;
					break;
				}
			}
			SmoothScrollToOffset(localOffset, onFinished);
		}

		private void SmoothScrollToOffset(Vector3 localOffset, Action onFinished, float strength = 8f)
		{
			if (!_uiScrollView.canMoveHorizontally)
			{
				localOffset.x = 0f;
			}
			if (!_uiScrollView.canMoveVertically)
			{
				localOffset.y = 0f;
			}
			localOffset.z = 0f;
			Transform cachedTransform = _uiScrollView.panel.cachedTransform;
			SpringPanel.Begin(_uiScrollView.panel.cachedGameObject, cachedTransform.localPosition - localOffset, strength).onFinished = () =>
			{
				if (onFinished != null)
				{
					onFinished();
				}
			};
		}

		public IEnumerator AnimateSwapPositions(int dataPosition1, int dataPosition2, float time, Action onComplete = null)
		{
			Container item1 = _recycler.activeContainers.Find((Container T) => T.positionIndex == dataPosition1);
			Container item2 = _recycler.activeContainers.Find((Container T) => T.positionIndex == dataPosition2);
			if (item1 != null && item2 != null)
			{
				Vector3 origin1 = item1.modelTransform.localPosition;
				Vector3 origin2 = item2.modelTransform.localPosition;
				float timePassed = 0f;
				while (timePassed < time)
				{
					timePassed += Time.deltaTime;
					float ratio = (float)Math.Sin(Math.PI / 2.0 * (double)timePassed / (double)time);
					item1.modelTransform.localPosition = Vector3.Lerp(origin1, origin2, ratio);
					item2.modelTransform.localPosition = Vector3.Lerp(origin2, origin1, ratio);
					yield return new WaitForEndOfFrame();
				}
				item1.modelTransform.localPosition = origin2;
				item2.modelTransform.localPosition = origin1;
				item1.positionIndex = dataPosition2;
				item2.positionIndex = dataPosition1;
			}
			onComplete.SafeInvoke();
		}

		public void Recenter()
		{
			if (_centerOnChild != null)
			{
				_centerOnChild.Recenter();
			}
		}

		private void FillFromIndex(int i, float topOffset, ContainerPivot containerPivot = ContainerPivot.None)
		{
			int dataEntryCount = GetDataEntryCount();
			if (dataEntryCount == 0 || i < 0 || dataEntryCount < i)
			{
				return;
			}
			uiScrollView.DisableSpring();
			_recycler.Clear();
			rootPanel.transform.localPosition = Vector3.zero;
			rootPanel.clipOffset = Vector2.zero;
			_firstContainerDataIndex = i;
			_lastContainerDataIndex = i;
			CalculateVirtualRelativeWidgetBounds(_boundsCalculationMode);
			Container container = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(i)), i);
			if (container != null)
			{
				float num = 0f;
				float num2 = 0f;
				container.isMeasured = false;
				container.Measure();
				num = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y - container.height * 0.5f - container.bounds.center.y - topOffset;
				num2 = container.width * 0.5f - rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x + container.bounds.center.x + topOffset;
				switch (containerPivot)
				{
				case ContainerPivot.Top:
					num = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y - container.height * 0.5f - container.bounds.center.y;
					num2 = container.width * 0.5f - rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x + container.bounds.center.x;
					topOffset = 0f;
					break;
				case ContainerPivot.Center:
					num = 0f;
					num2 = 0f;
					topOffset = ((_arrangement != Arrangement.Horizontal) ? (rootPanel.baseClipRegion.w / 2f - container.height / 2f) : (rootPanel.baseClipRegion.z / 2f - container.width / 2f));
					break;
				case ContainerPivot.Bottom:
					num = (0f - rootPanel.baseClipRegion.w) * 0.5f + rootPanel.baseClipRegion.y + container.height * 0.5f - container.bounds.center.y;
					num2 = (0f - container.width) * 0.5f + rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x + container.bounds.center.x;
					topOffset = ((_arrangement != Arrangement.Horizontal) ? (rootPanel.baseClipRegion.w - container.height) : (rootPanel.baseClipRegion.z - container.width));
					break;
				}
				container.coordinates = ((_arrangement != Arrangement.Vertical) ? new Vector2(num2, rootPanel.baseClipRegion.y) : new Vector2(rootPanel.baseClipRegion.x, num));
				if (_arrangement == Arrangement.Vertical)
				{
					container.top = topOffset;
					container.bottom = container.top + container.height + (float)_cellPaddingVertical;
					container.left = 0f;
					container.right = container.left + container.width;
				}
				else
				{
					container.top = 0f;
					container.bottom = container.top + container.height;
					container.left = topOffset;
					container.right = topOffset + container.width + (float)_cellPaddingHorizontal;
				}
				container.SetModelType(_adapter.GetModelType(i));
				container.SetParent(rootPanel.transform);
				Vector3 center = _listViewBounds.center;
				if (_arrangement == Arrangement.Vertical)
				{
					center.y -= topOffset;
					_listViewBounds.center = center;
				}
				else
				{
					center.x += topOffset + container.bounds.center.x;
					_listViewBounds.center = center;
				}
				_firstContainerDataIndex--;
				_lastContainerDataIndex++;
				container.positionIndex = i;
				if (_infinite == Infinite.BothWays)
				{
					if (_firstContainerDataIndex < 0)
					{
						_firstContainerDataIndex = GetLastDataEntryIndex();
					}
					if (_lastContainerDataIndex == GetDataEntryCount())
					{
						_lastContainerDataIndex = 0;
					}
				}
				_recycler.AddToEndOfActiveContainers(container);
				if (_arrangement == Arrangement.Vertical)
				{
					while (_lastContainerDataIndex < GetDataEntryCount() && _recycler.LastContainer().bottom < rootPanel.baseClipRegion.w)
					{
						Container container2 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_lastContainerDataIndex)), _lastContainerDataIndex);
						if (container2 != null)
						{
							AppendContainer(_lastContainerDataIndex, container2, false);
							if (_lastContainerDataIndex == GetLastDataEntryIndex() && _infinite == Infinite.BothWays)
							{
								_lastContainerDataIndex = 0;
							}
						}
					}
					while (_firstContainerDataIndex >= 0 && _recycler.FirstContainer().top > 0f)
					{
						Container container3 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_firstContainerDataIndex)), _firstContainerDataIndex);
						if (container3 != null)
						{
							PrependContainer(_firstContainerDataIndex, container3, false);
							if (_infinite == Infinite.BothWays && _firstContainerDataIndex < 0)
							{
								_firstContainerDataIndex = GetLastDataEntryIndex();
							}
						}
					}
				}
				else
				{
					while (_lastContainerDataIndex < GetDataEntryCount() && _recycler.LastContainer().right < rootPanel.baseClipRegion.z)
					{
						Container container4 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_lastContainerDataIndex)), _lastContainerDataIndex);
						if (container4 != null)
						{
							AppendContainer(_lastContainerDataIndex, container4, false);
							if (_lastContainerDataIndex == GetDataEntryCount() && _infinite == Infinite.BothWays)
							{
								_lastContainerDataIndex = 0;
							}
						}
					}
					while (_firstContainerDataIndex >= 0 && _recycler.FirstContainer().left > 0f)
					{
						Container container5 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_firstContainerDataIndex)), _firstContainerDataIndex);
						if (container5 != null)
						{
							PrependContainer(_firstContainerDataIndex, container5, false);
							if (_infinite == Infinite.BothWays && _firstContainerDataIndex < 0)
							{
								_firstContainerDataIndex = GetLastDataEntryIndex();
							}
						}
					}
				}
				if (_infinite != Infinite.BothWays && _arrangement == Arrangement.Vertical)
				{
					if (_lastContainerDataIndex == GetDataEntryCount() && _recycler.LastContainer().bottom < rootPanel.baseClipRegion.w)
					{
						container = _recycler.LastContainer();
						float num3 = (0f - rootPanel.baseClipRegion.w) * 0.5f + container.height * 0.5f - container.bounds.center.y + rootPanel.baseClipRegion.y;
						float num4 = container.coordinates.y - num3;
						_recycler.RecycleActiveContainers();
						_firstContainerDataIndex = GetLastDataEntryIndex();
						_lastContainerDataIndex = GetLastDataEntryIndex();
						container = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(GetLastDataEntryIndex())), GetLastDataEntryIndex());
						container.isMeasured = false;
						container.Measure();
						container.positionIndex = GetLastDataEntryIndex();
						container.coordinates.y = num3;
						container.coordinates.x = rootPanel.baseClipRegion.x;
						container.bottom = rootPanel.baseClipRegion.w;
						container.top = container.bottom - container.height;
						container.SetModelType(_adapter.GetModelType(GetLastDataEntryIndex()));
						container.SetParent(rootPanel.transform);
						_recycler.AddToEndOfActiveContainers(container);
						_firstContainerDataIndex--;
						_lastContainerDataIndex++;
						while (_firstContainerDataIndex >= 0 && _recycler.FirstContainer().top > 0f)
						{
							Container container6 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_firstContainerDataIndex)), _firstContainerDataIndex);
							if (container6 != null)
							{
								PrependContainer(_firstContainerDataIndex, container6, false);
							}
							if (_infinite == Infinite.BothWays && _firstContainerDataIndex < 0)
							{
								_firstContainerDataIndex = GetLastDataEntryIndex();
							}
						}
						center = _listViewBounds.center;
						center.y -= num4;
						_listViewBounds.center = center;
					}
					if (_firstContainerDataIndex < 0 && _recycler.FirstContainer().top > 0f)
					{
						container = _recycler.FirstContainer();
						float num5 = rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y - container.height * 0.5f - container.bounds.center.y;
						float num6 = container.coordinates.y - num5;
						_recycler.RecycleActiveContainers();
						_firstContainerDataIndex = 0;
						_lastContainerDataIndex = 0;
						container = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(0)), 0);
						container.isMeasured = false;
						container.Measure();
						container.positionIndex = _firstContainerDataIndex;
						container.coordinates.y = num5;
						container.coordinates.x = rootPanel.baseClipRegion.x;
						container.bottom = container.height;
						container.top = 0f;
						container.SetModelType(_adapter.GetModelType(0));
						container.SetParent(rootPanel.transform);
						_recycler.AddToEndOfActiveContainers(container);
						_firstContainerDataIndex--;
						_lastContainerDataIndex++;
						while (_lastContainerDataIndex < GetDataEntryCount() && _recycler.LastContainer().bottom < rootPanel.baseClipRegion.w)
						{
							Container container7 = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_lastContainerDataIndex)), _lastContainerDataIndex);
							if (container7 != null)
							{
								AppendContainer(_lastContainerDataIndex, container7, false);
							}
							if (_lastContainerDataIndex == GetDataEntryCount() && _infinite == Infinite.BothWays)
							{
								_lastContainerDataIndex = 0;
							}
						}
						center = _listViewBounds.center;
						center.y -= num6;
						_listViewBounds.center = center;
					}
				}
				_currentMaxListCount = _lastContainerDataIndex;
				_currentMinListCount = _firstContainerDataIndex;
				if (_boundsCalculationMode == BoundsCalculationMode.Incremental)
				{
					CalculateVirtualRelativeWidgetBounds(BoundsCalculationMode.Incremental);
					Container firstVisibleContainerOnScreen = GetFirstVisibleContainerOnScreen();
					if (firstVisibleContainerOnScreen != null)
					{
						if (_arrangement == Arrangement.Horizontal)
						{
							center = _listViewBounds.center;
							center.x += firstVisibleContainerOnScreen.left;
							_listViewBounds.center = center;
						}
						else
						{
							center = _listViewBounds.center;
							center.y -= firstVisibleContainerOnScreen.top;
							_listViewBounds.center = center;
						}
					}
				}
			}
			onPreLayout.SafeInvoke();
			_recycler.SetActiveContainerPositions();
			PrintActiveContainers();
		}

		private bool ShouldRecycleContainer(Container container)
		{
			if (_arrangement == Arrangement.Horizontal)
			{
				if (rootPanel.clipOffset.x > container.right)
				{
					return true;
				}
				if (rootPanel.clipOffset.x + rootPanel.baseClipRegion.z - container.bounds.center.x < container.left)
				{
					return true;
				}
			}
			else
			{
				if (rootPanel.transform.localPosition.y + (float)_cellPaddingVertical > container.bottom)
				{
					return true;
				}
				float num = rootPanel.transform.localPosition.y + rootPanel.baseClipRegion.w - container.bounds.center.y + (float)_cellPaddingHorizontal;
				if (num < container.top)
				{
					return true;
				}
			}
			return false;
		}

		private bool ShouldAppendContainer(Container container)
		{
			if (_arrangement == Arrangement.Vertical)
			{
				if (rootPanel.transform.localPosition.y + rootPanel.baseClipRegion.w - (float)_cellPaddingVertical > container.bottom)
				{
					return true;
				}
			}
			else if (rootPanel.clipOffset.x + rootPanel.baseClipRegion.z > container.right)
			{
				return true;
			}
			return false;
		}

		private bool ShouldPrependContainer(Container container)
		{
			if (_arrangement == Arrangement.Vertical)
			{
				if (rootPanel.transform.localPosition.y + (float)_cellPaddingVertical < container.top)
				{
					return true;
				}
			}
			else if (rootPanel.clipOffset.x < container.left)
			{
				return true;
			}
			return false;
		}

		private void RecycleContainersAtBeginning()
		{
			while (ShouldRecycleContainer(_recycler.FirstContainer()) && _recycler.FirstContainer() != _recycler.LastContainer())
			{
				Container container = _recycler.FirstContainer();
				_firstContainerDataIndex = container.positionIndex;
				_recycler.RecycleContainer(container);
				if (_recycler.HasActiveContainers())
				{
					container = _recycler.FirstContainer();
					_nextPrependPosition = container.coordinates;
				}
			}
		}

		private void HandleScrollUp()
		{
			if (!_recycler.HasActiveContainers())
			{
				return;
			}
			RecycleContainersAtBeginning();
			while (ShouldAppendContainer(_recycler.LastContainer()) && _lastContainerDataIndex < GetDataEntryCount())
			{
				Container container = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_lastContainerDataIndex)), _lastContainerDataIndex);
				if (container == null)
				{
					continue;
				}
				AppendContainer(_lastContainerDataIndex, container, true);
				if (_currentMaxListCount < _lastContainerDataIndex)
				{
					if (_boundsCalculationMode == BoundsCalculationMode.Incremental)
					{
						AppendContainerToBounds(container);
					}
					_currentMaxListCount = _lastContainerDataIndex;
				}
			}
			if (_lastContainerDataIndex == GetDataEntryCount() && _infinite == Infinite.BothWays)
			{
				_lastContainerDataIndex = 0;
			}
		}

		private void AppendContainerToBounds(Container container)
		{
			Vector3 size = _listViewBounds.size;
			Vector3 center = _listViewBounds.center;
			if (_arrangement == Arrangement.Vertical)
			{
				int cellPaddingVertical = _cellPaddingVertical;
				size.y += container.height;
				center.y -= container.height / 2f;
				if (_lastContainerDataIndex <= GetLastDataEntryIndex())
				{
					size.y += cellPaddingVertical;
					center.y -= (float)cellPaddingVertical / 2f;
				}
			}
			else
			{
				int cellPaddingHorizontal = _cellPaddingHorizontal;
				size.x += container.width;
				center.x += container.width / 2f;
				if (_lastContainerDataIndex <= GetLastDataEntryIndex())
				{
					size.x += cellPaddingHorizontal;
					center.x += (float)cellPaddingHorizontal / 2f;
				}
			}
			_listViewBounds.size = size;
			_listViewBounds.center = center;
		}

		private void RecycleContainersAtEnd()
		{
			while (ShouldRecycleContainer(_recycler.LastContainer()) && _recycler.FirstContainer() != _recycler.LastContainer())
			{
				Container container = _recycler.LastContainer();
				_lastContainerDataIndex = container.positionIndex;
				_recycler.RecycleContainer(container);
				if (_recycler.HasActiveContainers())
				{
					container = _recycler.LastContainer();
					_nextAppendPosition = container.coordinates;
				}
			}
		}

		private void HandleScrollDown()
		{
			if (!_recycler.HasActiveContainers())
			{
				return;
			}
			RecycleContainersAtEnd();
			while (ShouldPrependContainer(_recycler.FirstContainer()) && _firstContainerDataIndex >= 0)
			{
				Container container = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(_firstContainerDataIndex)), _firstContainerDataIndex);
				if (container == null)
				{
					continue;
				}
				PrependContainer(_firstContainerDataIndex, container, true);
				if (_currentMinListCount > _firstContainerDataIndex)
				{
					if (_boundsCalculationMode == BoundsCalculationMode.Incremental)
					{
						PrependContainerToBounds(container);
					}
					_currentMinListCount = _firstContainerDataIndex;
				}
			}
			if (_infinite == Infinite.BothWays && _firstContainerDataIndex < 0)
			{
				_firstContainerDataIndex = GetLastDataEntryIndex();
			}
		}

		private void PrependContainerToBounds(Container container)
		{
			Vector3 size = _listViewBounds.size;
			Vector3 center = _listViewBounds.center;
			if (_arrangement == Arrangement.Vertical)
			{
				size.y += container.height + (float)_cellPaddingVertical;
				center.y += container.height / 2f + (float)_cellPaddingVertical / 2f;
			}
			else
			{
				size.x += container.width + (float)_cellPaddingHorizontal;
				center.x -= container.width / 2f + (float)_cellPaddingHorizontal / 2f;
			}
			_listViewBounds.size = size;
			_listViewBounds.center = center;
		}

		private void CalculateVirtualRelativeWidgetBounds(BoundsCalculationMode mode)
		{
			Vector3 size;
			Vector3 center;
			switch (mode)
			{
			case BoundsCalculationMode.FullSize:
				if (_infinite == Infinite.None)
				{
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					float num6 = 0f;
					for (int i = 0; i < GetDataEntryCount(); i++)
					{
						Container container = _adapter.GetContainer(_recycler.RequestContainer(_adapter.GetModelType(i)), i, true);
						container.SetModelType(_adapter.GetModelType(i));
						container.Measure();
						container.SetParent(rootPanel.transform);
						if (_arrangement == Arrangement.Horizontal)
						{
							num3 += container.width + (float)_cellPaddingHorizontal;
							if (_firstContainerDataIndex > 0 && _firstContainerDataIndex < GetDataEntryCount() && _firstContainerDataIndex > i)
							{
								num6 += container.width + (float)_cellPaddingHorizontal;
							}
						}
						else
						{
							num4 += container.height + (float)_cellPaddingVertical;
							if (_firstContainerDataIndex >= 0 && _firstContainerDataIndex < GetDataEntryCount() && _firstContainerDataIndex > i)
							{
								num6 += container.height + (float)_cellPaddingVertical;
							}
						}
						num5 += container.bounds.size.z;
						_recycler.RecycleContainer(container);
					}
					if (_arrangement == Arrangement.Horizontal)
					{
						size = new Vector3(num3 - (float)_cellPaddingHorizontal, rootPanel.baseClipRegion.w, num5);
						center = new Vector3(num3 * 0.5f - rootPanel.baseClipRegion.z * 0.5f - num6 + rootPanel.baseClipRegion.x - (float)_cellPaddingHorizontal / 2f, rootPanel.baseClipRegion.y, 0f);
					}
					else
					{
						size = new Vector3(rootPanel.baseClipRegion.z, num4 - (float)_cellPaddingVertical, num5);
						center = new Vector3(rootPanel.baseClipRegion.x, (0f - num4) * 0.5f + rootPanel.baseClipRegion.w * 0.5f + num6 + (float)_cellPaddingVertical / 2f + rootPanel.baseClipRegion.y, 0f);
					}
					Bounds bounds = new Bounds(center, size);
					_listViewBounds = bounds;
					return;
				}
				if (_infinite == Infinite.BothWays)
				{
					size = ((_arrangement != Arrangement.Vertical) ? new Vector3(rootPanel.baseClipRegion.z * 3f, rootPanel.baseClipRegion.w, 0f) : new Vector3(rootPanel.baseClipRegion.z, rootPanel.baseClipRegion.w * 3f, 0f));
					Bounds bounds2 = new Bounds(Vector3.zero, size);
					_listViewBounds = bounds2;
					return;
				}
				break;
			case BoundsCalculationMode.Incremental:
				if (_arrangement == Arrangement.Vertical)
				{
					float num = 0f;
					foreach (Container activeContainer in _recycler.activeContainers)
					{
						num += activeContainer.height + (float)_cellPaddingVertical;
					}
					size = new Vector3(rootPanel.baseClipRegion.z, num, 0f);
					center = new Vector3(0f, (0f - num) * 0.5f + rootPanel.baseClipRegion.w * 0.5f + rootPanel.baseClipRegion.y, 0f);
				}
				else
				{
					float num2 = 0f;
					foreach (Container activeContainer2 in _recycler.activeContainers)
					{
						num2 += activeContainer2.width + (float)_cellPaddingHorizontal;
					}
					size = new Vector3(num2, rootPanel.baseClipRegion.w, 0f);
					center = new Vector3(num2 * 0.5f - rootPanel.baseClipRegion.z * 0.5f + rootPanel.baseClipRegion.x, 0f, 0f);
				}
				_listViewBounds = new Bounds(center, size);
				return;
			}
			_listViewBounds = new Bounds(Vector3.zero, Vector3.zero);
		}

		[CanBeNull]
		public Container GetFirstVisibleContainerOnScreen()
		{
			if (_recycler != null)
			{
				return _recycler.FirstContainer();
			}
			return null;
		}

		[Conditional("SCROLL_VIEW_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
