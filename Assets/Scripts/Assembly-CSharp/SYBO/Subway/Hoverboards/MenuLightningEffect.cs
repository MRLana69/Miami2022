using System.Collections;
using Kiloo.Common.Utility;
using UnityEngine;

namespace SYBO.Subway.Hoverboards
{
	public class MenuLightningEffect : MonoBehaviour
	{
		[SerializeField]
		private float _duration;

		[SerializeField]
		private float _minDelay;

		[SerializeField]
		private float _maxDelay;

		private float _currentDelay;

		private GameObject[] _children;

		private void Awake()
		{
			_children = new GameObject[base.transform.childCount];
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i] = base.transform.GetChild(i).gameObject;
				_children[i].SetActive(false);
			}
		}

		private void Update()
		{
			if (_currentDelay <= 0f)
			{
				_currentDelay = Random.Range(_minDelay, _maxDelay);
				return;
			}
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				for (int i = 0; i < _children.Length; i++)
				{
					_children[i].SetActive(true);
				}
				CoroutineUtil.StopCoroutine(DelayedDisable());
				CoroutineUtil.StartCoroutine(DelayedDisable());
			}
		}

		private IEnumerator DelayedDisable()
		{
			yield return new WaitForSeconds(_duration);
			for (int i = 0; i < _children.Length; i++)
			{
				if (_children[i] != null)
				{
					_children[i].SetActive(false);
				}
			}
		}
	}
}
