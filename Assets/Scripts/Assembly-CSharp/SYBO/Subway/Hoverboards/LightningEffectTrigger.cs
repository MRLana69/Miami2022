using System.Collections;
using Kiloo.Common.Utility;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Hoverboards
{
	public class LightningEffectTrigger : MonoBehaviour
	{
		private const float InAirThreshold = 0.1f;

		[SerializeField]
		private float _duration = 0.3f;

		[SerializeField]
		private Vector3 _direction = Vector3.forward;

		private CharacterController _characterController;

		private float _inAirTime;

		private GameObject[] _children;

		private void Awake()
		{
			_characterController = Character.Instance.CharacterController;
			_children = new GameObject[base.transform.childCount];
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i] = base.transform.GetChild(i).gameObject;
				_children[i].SetActive(false);
			}
		}

		private void LateUpdate()
		{
			bool isGrounded = _characterController.isGrounded;
			if (_inAirTime >= 0.1f && isGrounded)
			{
				for (int i = 0; i < _children.Length; i++)
				{
					_children[i].SetActive(true);
				}
				CoroutineUtil.StopCoroutine(DelayedDisable());
				CoroutineUtil.StartCoroutine(DelayedDisable());
			}
			if (_children[0].activeSelf)
			{
				base.transform.rotation = Quaternion.LookRotation(_direction);
			}
			if (!isGrounded)
			{
				_inAirTime += Time.deltaTime;
			}
			else
			{
				_inAirTime = 0f;
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
