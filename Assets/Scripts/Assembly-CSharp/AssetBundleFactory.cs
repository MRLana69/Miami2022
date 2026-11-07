using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Utility;
using SYBO.Components.Utilities;
using UnityEngine;

public class AssetBundleFactory<T> where T : UnityEngine.Object
{
	private Dictionary<string, int> _requests = new Dictionary<string, int>();

	private Dictionary<string, List<Disposable<T>>> _disposableReferences = new Dictionary<string, List<Disposable<T>>>();

	private bool _isComponent;

	private List<string> _toRemove = new List<string>();

	public AssetBundleFactory()
	{
		_isComponent = typeof(Component).IsAssignableFrom(typeof(T));
		Factories instance = SingletonMonoBehaviour<Factories>.Instance;
		instance.OnLateUpdate = (Action)Delegate.Combine(instance.OnLateUpdate, new Action(HandleOnLateUpdate));
	}

	public bool IsAssetBundleInUse(string assetBundleName)
	{
		return _requests.ContainsKey(assetBundleName) || _disposableReferences.ContainsKey(assetBundleName);
	}

	public void RequestItemInstance(string name, Action<T> onSuccess, bool asynchronous = false)
	{
		name = name.ToLower();
		if (!_requests.ContainsKey(name))
		{
			_requests.Add(name, 0);
		}
		_requests[name]++;
		AssetBundleManager.Instance.GetAssetBundle(name, (AssetBundle bundle) =>
		{
			if (bundle == null)
			{
				onSuccess((T)null);
			}
			else if (asynchronous)
			{
				CoroutineUtil.StartCoroutine(LoadAssetFromBundlesAsync(bundle, name, onSuccess));
			}
			else
			{
				LoadAssetFromBundles(bundle, name, onSuccess);
			}
		}, asynchronous);
	}

	private void LoadAssetFromBundles(AssetBundle bundle, string name, Action<T> onSucces)
	{
		T obj = ((!_isComponent) ? bundle.LoadAsset<T>(name) : bundle.LoadAsset<GameObject>(name).GetComponent<T>());
		Disposable<T> disposable = CreateDisposableObject(name, obj);
		_requests[name]--;
		if (onSucces != null)
		{
			onSucces(disposable.ReferencedObject);
		}
	}

	private Disposable<T> CreateDisposableObject(string name, T obj)
	{
		List<Disposable<T>> value;
		if (!_disposableReferences.TryGetValue(name, out value))
		{
			value = (_disposableReferences[name] = new List<Disposable<T>>());
		}
		Disposable<T> disposable = new Disposable<T>(UnityEngine.Object.Instantiate(obj));
		value.Add(disposable);
		return disposable;
	}

	private IEnumerator LoadAssetFromBundlesAsync(AssetBundle bundle, string name, Action<T> onSucces)
	{
		if (Globals.DEBUG_ENABLE_LOADING_DELAY)
		{
			yield return new WaitForSeconds(0.5f);
		}
		AssetBundleRequest request = ((!_isComponent) ? bundle.LoadAssetAsync<T>(name) : bundle.LoadAssetAsync<GameObject>(name));
		yield return request;
		T obj = ((!_isComponent) ? (request.asset as T) : (request.asset as GameObject).GetComponent<T>());
		Disposable<T> disposable = CreateDisposableObject(name, obj);
		_requests[name]--;
		if (onSucces != null)
		{
			onSucces(disposable.ReferencedObject);
		}
	}

	private void HandleOnLateUpdate()
	{
		_toRemove.Clear();
		foreach (KeyValuePair<string, List<Disposable<T>>> disposableReference in _disposableReferences)
		{
			string key = disposableReference.Key;
			List<Disposable<T>> value = disposableReference.Value;
			int num = 0;
			while (num < value.Count)
			{
				if (value[num].ReferencedObject == null)
				{
					value.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			if (value.Count == 0 && _requests[key] == 0)
			{
				_toRemove.Add(key);
			}
		}
		foreach (string item in _toRemove)
		{
			_disposableReferences.Remove(item);
			AssetBundleManager.Instance.UnloadAssetBundle(item, true);
		}
	}
}
