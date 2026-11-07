using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Utilities.Flags;
using SYBO.Subway.Managers;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public abstract class BasePicker : IPicker
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickerRemoveRequestEventArgs> _onRemoveRequest__BackingField;

		protected PickupManager _pickupManager;

		protected PickerData _pickerData;

		private List<PickupData> _filteredPickupDatas = new List<PickupData>();

		public int ExecutionOrder
		{
			get
			{
				return _pickerData.ExecutionOrder;
			}
		}

		public float SpawnDistanceMinToPrevious
		{
			get
			{
				return _pickerData.SpawnDistanceMinToPrevious;
			}
		}

		public float ProbabilityToSpawn
		{
			get
			{
				return _pickerData.ProbabilityToSpawn;
			}
		}

		public string Name
		{
			get
			{
				return _pickerData.PickerType.ToString();
			}
		}

		event Action<PickerRemoveRequestEventArgs> IPicker.OnRemoveRequest
		{
			add
			{
				_onRemoveRequest += value;
			}
			remove
			{
				_onRemoveRequest -= value;
			}
		}

		protected event Action<PickerRemoveRequestEventArgs> _onRemoveRequest
		{
			add
			{
				Action<PickerRemoveRequestEventArgs> action = _onRemoveRequest__BackingField;
				Action<PickerRemoveRequestEventArgs> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref _onRemoveRequest__BackingField, (Action<PickerRemoveRequestEventArgs>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickerRemoveRequestEventArgs> action = _onRemoveRequest__BackingField;
				Action<PickerRemoveRequestEventArgs> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref _onRemoveRequest__BackingField, (Action<PickerRemoveRequestEventArgs>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void Add(PickupManager pickupManager)
		{
		}

		protected void AddProtected(PickupManager pickupManager, PickerType pickerType)
		{
			_pickupManager = pickupManager;
			_pickerData = _pickupManager.GetPickerDataForPickerType(pickerType);
			if (_pickerData == null)
			{
				throw new UnityException(GetType().Name + ".pickerData must be not-null. Fix immediately");
			}
		}

		public bool UpdatePicker()
		{
			return true;
		}

		protected void UpdatePickerProtected(List<PickupSpawnPoint> populatedSpawnedPoints = null)
		{
			MaybeChooseSpawnPoint(populatedSpawnedPoints);
		}

		protected void MaybeChooseSpawnPoint(List<PickupSpawnPoint> populatedSpawnedPoints = null)
		{
			PickupData pickupData = null;
			if (DemoVideoModeManager.IsEnabled)
			{
				return;
			}
			for (int i = 0; i < _pickupManager.PickupSpawnPointList.Count; i++)
			{
				PickupSpawnPoint pickupSpawnPoint = _pickupManager.PickupSpawnPointList[i];
				if (!pickupSpawnPoint.IsAvailableForSpawning())
				{
					continue;
				}
				pickupData = ((pickupSpawnPoint.SpawnPointMode != SpawnPointMode.WillForcePickupType) ? MaybeChoosePickupDataForSpawnPoint(pickupSpawnPoint) : _pickupManager.GetPickupDataForPickupType(pickupSpawnPoint.ForceSpawnPickupType));
				if (pickupData != null)
				{
					pickupSpawnPoint.SpawnPickup(pickupData, _pickerData);
					if (populatedSpawnedPoints != null)
					{
						populatedSpawnedPoints.Add(pickupSpawnPoint);
					}
				}
			}
		}

		private PickupData MaybeChoosePickupDataForSpawnPoint(PickupSpawnPoint pickupSpawnPoint)
		{
			float value = UnityEngine.Random.value;
			if (_pickerData.ProbabilityToSpawn <= value)
			{
				return null;
			}
			if (!_pickerData.IsCompatibleWithSpawnPoint(pickupSpawnPoint))
			{
				return null;
			}
			PickupData result = null;
			_filteredPickupDatas.Clear();
			PickupData[] pickupData = _pickupManager._pickupData;
			foreach (PickupData pickupData2 in pickupData)
			{
				if (FlagsHelper.IsSet(_pickerData.PickupType, pickupData2.PickupType) && 0f < pickupData2.SpawnWeight && pickupData2.PickupEnabledState == PickupEnabledState.Enabled && pickupData2.IsCompatibleWithSpawnPoint(pickupSpawnPoint) && (!Globals.DEBUG_DISABLE_JETPACK || pickupData2.PickupType != PickupType.JetPack))
				{
					_filteredPickupDatas.Add(pickupData2);
				}
			}
			if (0 < _filteredPickupDatas.Count)
			{
				result = ChoosePickupDataBasedUponWeight(_filteredPickupDatas);
			}
			return result;
		}

		protected PickupData ChoosePickupDataBasedUponWeight(List<PickupData> pickupDataList)
		{
			PickupData result = null;
			float num = 0f;
			for (int i = 0; i < pickupDataList.Count; i++)
			{
				num += pickupDataList[i].SpawnWeight;
			}
			float num2 = UnityEngine.Random.value * num;
			float num3 = 0f;
			for (int j = 0; j < pickupDataList.Count; j++)
			{
				PickupData pickupData = pickupDataList[j];
				num3 += pickupData.SpawnWeight;
				if (num2 <= num3)
				{
					result = pickupData;
					break;
				}
			}
			return result;
		}

		protected void TriggerOnRequestRemoval(PickerRemoveRequestEventArgs requestRemovalEvent)
		{
			if (_onRemoveRequest__BackingField != null)
			{
				_onRemoveRequest__BackingField(requestRemovalEvent);
			}
		}
	}
}
