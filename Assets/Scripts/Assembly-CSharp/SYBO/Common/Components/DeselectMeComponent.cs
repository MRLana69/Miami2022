using UnityEngine;

namespace SYBO.Common.Components
{
	public class DeselectMeComponent : MonoBehaviour
	{
		[SerializeField]
		private Transform _redirectTarget;

		[SerializeField]
		private DeselectMeMode _deselectMeMode;

		public Transform RedirectTarget
		{
			get
			{
				return _redirectTarget;
			}
		}

		public DeselectMeMode DeselectMeMode
		{
			get
			{
				return _deselectMeMode;
			}
			set
			{
				_deselectMeMode = value;
			}
		}
	}
}
