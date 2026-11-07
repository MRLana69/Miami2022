using SYBO.Common.Utilities;
using UnityEngine;

public class UISpinner : MonoBehaviour
{
	[SerializeField]
	private UISprite _sprite;

	[SerializeField]
	private float _speed;

	[SerializeField]
	private bool _rotateClockWise;

	[SerializeField]
	private bool _startManually;

	[SerializeField]
	private bool _enableDisableSprite = true;

	[SerializeField]
	private bool _callStartSpinning;

	[SerializeField]
	private bool _callStopSpinning;

	private bool _shouldRotate;

	private float _deltaTime;

	public bool IsAnimating
	{
		get
		{
			return _shouldRotate;
		}
	}

	private void OnEnable()
	{
		if (!_startManually)
		{
			StartSpinning();
		}
	}

	private void Awake()
	{
		if (_sprite == null)
		{
			UISprite component = GetComponent<UISprite>();
			if (component != null)
			{
				_sprite = component;
			}
			else
			{
				SDebug.LogError("Sprite not set for this Spinner");
			}
		}
		if (_startManually)
		{
			StopSpinning();
		}
		else
		{
			StartSpinning();
		}
	}

	private void Update()
	{
		if (_shouldRotate)
		{
			_deltaTime = RealTimeTracker.deltaTime;
			float angle = ((!_rotateClockWise) ? ((0f - _deltaTime) * _speed) : (_deltaTime * _speed));
			_sprite.transform.Rotate(Vector3.back, angle);
		}
	}

	public void StartSpinning()
	{
		if (_enableDisableSprite)
		{
			_sprite.enabled = true;
		}
		_shouldRotate = true;
	}

	public void StopSpinning()
	{
		if (_enableDisableSprite)
		{
			_sprite.enabled = false;
		}
		_shouldRotate = false;
	}

	public void ResetSpriteRotation()
	{
		_sprite.transform.ResetLocalRotation();
	}
}
