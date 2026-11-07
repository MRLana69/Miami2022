using UnityEngine;

namespace SYBO.Components.Utilities.Components
{
	public class SetActiveComponent : MonoBehaviour
	{
		[SerializeField]
		private bool _isActiveOnAwake = true;

		protected void Awake()
		{
			base.gameObject.SetActive(_isActiveOnAwake);
			base.enabled = _isActiveOnAwake;
		}
	}
}
