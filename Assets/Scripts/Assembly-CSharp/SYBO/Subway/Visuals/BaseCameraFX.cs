using SYBO.Common.Exceptions;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class BaseCameraFX : MonoBehaviour
	{
		[SerializeField]
		private GameObject _onGameObject;

		[SerializeField]
		private GameObject _offGameObject;

		[HideInInspector]
		private CameraFXState _cameraFXState;

		public CameraFXState CameraFXState
		{
			get
			{
				return _cameraFXState;
			}
			set
			{
				_cameraFXState = value;
				switch (_cameraFXState)
				{
				case CameraFXState.On:
					_onGameObject.SetActive(true);
					_offGameObject.SetActive(false);
					break;
				case CameraFXState.Off:
					_onGameObject.SetActive(false);
					_offGameObject.SetActive(true);
					break;
				default:
					SwitchDefaultException.Throw(_cameraFXState.ToString());
					break;
				}
			}
		}

		protected virtual void Start()
		{
			CameraFXState = CameraFXState.Off;
		}
	}
}
