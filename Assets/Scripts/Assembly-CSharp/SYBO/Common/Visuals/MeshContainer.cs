using System;
using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class MeshContainer : SingletonMonoBehaviour<MeshContainer>
	{
		private Dictionary<Mesh, ModelInfo> _combinedMeshes = new Dictionary<Mesh, ModelInfo>();

		private Dictionary<Mesh, Action<ModelInfo>> _combinedMeshCallbacks = new Dictionary<Mesh, Action<ModelInfo>>();

		private Dictionary<CombineableMeshGroup, ModelInfo> _combinedMeshesGroups = new Dictionary<CombineableMeshGroup, ModelInfo>();

		private Dictionary<CombineableMeshGroup, Action<ModelInfo>> _combinedMeshGroupCallbacks = new Dictionary<CombineableMeshGroup, Action<ModelInfo>>();

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		public void AddMesh(Mesh originalMesh, ModelInfo combinedMeshInfo)
		{
			_combinedMeshes.Add(originalMesh, combinedMeshInfo);
			Action<ModelInfo> value;
			if (_combinedMeshCallbacks.TryGetValue(originalMesh, out value))
			{
				if (value != null)
				{
					value(combinedMeshInfo);
				}
				_combinedMeshCallbacks.Remove(originalMesh);
			}
		}

		public void RemoveMesh(Mesh originalMesh)
		{
			_combinedMeshes.Remove(originalMesh);
		}

		public bool TryGetMesh(Mesh originalMesh, out ModelInfo combinedMeshInfo)
		{
			return _combinedMeshes.TryGetValue(originalMesh, out combinedMeshInfo);
		}

		public bool HasCombinedMesh(Mesh originalMesh)
		{
			return _combinedMeshes.ContainsKey(originalMesh);
		}

		public void NotifyMeshCombineInProgress(Mesh mesh)
		{
			if (!_combinedMeshCallbacks.ContainsKey(mesh))
			{
				_combinedMeshCallbacks[mesh] = null;
			}
		}

		public void NotifyMeshCombineFailed(Mesh mesh)
		{
			Action<ModelInfo> value;
			if (_combinedMeshCallbacks.TryGetValue(mesh, out value) && value != null)
			{
				value(null);
			}
		}

		public bool IsMeshCombineInProgress(Mesh mesh)
		{
			return !_combinedMeshes.ContainsKey(mesh) && _combinedMeshCallbacks.ContainsKey(mesh);
		}

		public void AddMeshGroupCombinedCallback(Mesh mesh, Action<ModelInfo> meshReceiveCallback)
		{
			Action<ModelInfo> value;
			if (_combinedMeshCallbacks.TryGetValue(mesh, out value) && value != null)
			{
				value = (Action<ModelInfo>)Delegate.Combine(value, meshReceiveCallback);
				_combinedMeshCallbacks[mesh] = value;
			}
			else
			{
				_combinedMeshCallbacks[mesh] = meshReceiveCallback;
			}
		}

		public void RemoveMeshCombinedCallback(Mesh mesh, Action<ModelInfo> meshReceiveCallback)
		{
			Action<ModelInfo> value;
			if (_combinedMeshCallbacks.TryGetValue(mesh, out value) && value != null)
			{
				value = (Action<ModelInfo>)Delegate.Remove(value, meshReceiveCallback);
			}
		}

		public void AddGroup(CombineableMeshGroup combineableMeshGroup, ModelInfo combinedMeshInfo)
		{
			if (!_combinedMeshesGroups.ContainsKey(combineableMeshGroup))
			{
				_combinedMeshesGroups.Add(combineableMeshGroup, combinedMeshInfo);
			}
			Action<ModelInfo> value;
			if (_combinedMeshGroupCallbacks.TryGetValue(combineableMeshGroup, out value))
			{
				if (value != null)
				{
					value(combinedMeshInfo);
				}
				_combinedMeshGroupCallbacks.Remove(combineableMeshGroup);
			}
		}

		public void RemoveGroup(CombineableMeshGroup combineableMeshGroup)
		{
			_combinedMeshesGroups.Remove(combineableMeshGroup);
		}

		public bool TryGetGroupMesh(CombineableMeshGroup combineableMeshGroup, out ModelInfo combinedMeshInfo)
		{
			return _combinedMeshesGroups.TryGetValue(combineableMeshGroup, out combinedMeshInfo);
		}

		public bool HasGroupMesh(CombineableMeshGroup combineableMeshGroup)
		{
			return _combinedMeshesGroups.ContainsKey(combineableMeshGroup);
		}

		public void NotifyMeshGroupCombineInProgress(CombineableMeshGroup meshGroup)
		{
			if (!_combinedMeshGroupCallbacks.ContainsKey(meshGroup))
			{
				_combinedMeshGroupCallbacks[meshGroup] = null;
			}
		}

		public void NotifyMeshGroupCombineCanceled(CombineableMeshGroup meshGroup)
		{
			Action<ModelInfo> value;
			if (_combinedMeshGroupCallbacks.TryGetValue(meshGroup, out value) && value != null)
			{
				value(null);
				_combinedMeshGroupCallbacks.Remove(meshGroup);
			}
		}

		public bool IsMeshGroupCombineInProgress(CombineableMeshGroup meshGroup)
		{
			return !_combinedMeshesGroups.ContainsKey(meshGroup) && _combinedMeshGroupCallbacks.ContainsKey(meshGroup);
		}

		public void AddMeshGroupCombinedCallback(CombineableMeshGroup meshGroup, Action<ModelInfo> meshReceiveCallback)
		{
			Action<ModelInfo> value;
			if (_combinedMeshGroupCallbacks.TryGetValue(meshGroup, out value) && value != null)
			{
				value = (Action<ModelInfo>)Delegate.Combine(value, meshReceiveCallback);
				_combinedMeshGroupCallbacks[meshGroup] = value;
			}
			else
			{
				_combinedMeshGroupCallbacks[meshGroup] = meshReceiveCallback;
			}
		}

		public void RemoveMeshGroupCombinedCallback(CombineableMeshGroup meshGroup, Action<ModelInfo> meshReceiveCallback)
		{
			Action<ModelInfo> value;
			if (_combinedMeshGroupCallbacks.TryGetValue(meshGroup, out value) && value != null)
			{
				value = (Action<ModelInfo>)Delegate.Remove(value, meshReceiveCallback);
			}
		}
	}
}
