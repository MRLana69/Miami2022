using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	public class FitSpriteToWidget : MonoBehaviour
	{
		public enum FittingMode
		{
			Constrain = 0,
			Fill = 1,
			FitWidth = 2,
			FitHeight = 3,
			Stretch = 4,
			PixelPerfect = 5,
			MaxSize = 6
		}

		private enum Execute
		{
			Off = 0,
			OnEnable = 1,
			OnStart = 2,
			OnUpdate = 3,
			OnFixedUpdate = 4,
			OnLateUpdate = 5
		}

		[Tooltip("The sprite to fit into a widget container")]
		[SerializeField]
		private UISprite _spriteToFit;

		[Tooltip("The UI widget container to fit the sprite to")]
		[SerializeField]
		private UIWidget _widgetContainer;

		[Tooltip("The FittingMode used to fit the sprite to the widget")]
		[SerializeField]
		public FittingMode fittingMode;

		[Tooltip("When should the fitting happen?")]
		[SerializeField]
		private Execute _execute;

		public UIWidget widgetContainer
		{
			set
			{
				_widgetContainer = value;
			}
		}

		private void OnEnable()
		{
			if (_execute == Execute.OnEnable)
			{
				FitNow();
			}
		}

		private void Start()
		{
			if (_execute == Execute.OnStart)
			{
				FitNow();
			}
		}

		private void Update()
		{
			if (_execute == Execute.OnUpdate)
			{
				FitNow();
			}
		}

		private void FixedUpdate()
		{
			if (_execute == Execute.OnFixedUpdate)
			{
				FitNow();
			}
		}

		private void LateUpdate()
		{
			if (_execute == Execute.OnLateUpdate)
			{
				FitNow();
			}
		}

		[Button(null)]
		public void FitNow()
		{
			if (_spriteToFit != null && _widgetContainer != null)
			{
				_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.Free;
				_spriteToFit.MakePixelPerfect();
				int num = _spriteToFit.GetAtlasSprite().paddingLeft + _spriteToFit.GetAtlasSprite().paddingRight;
				int num2 = _spriteToFit.GetAtlasSprite().paddingBottom + _spriteToFit.GetAtlasSprite().paddingTop;
				int num3 = (int)((float)(_spriteToFit.GetAtlasSprite().width + num) * _spriteToFit.atlas.pixelSize);
				int num4 = (int)((float)(_spriteToFit.GetAtlasSprite().height + num2) * _spriteToFit.atlas.pixelSize);
				_spriteToFit.transform.position = _widgetContainer.transform.position;
				_spriteToFit.aspectRatio = (float)num3 / (float)num4;
				switch (fittingMode)
				{
				case FittingMode.Constrain:
					FitConstrain();
					break;
				case FittingMode.Fill:
					FitFill();
					break;
				case FittingMode.FitWidth:
					FitWidth();
					break;
				case FittingMode.FitHeight:
					FitHeight();
					break;
				case FittingMode.Stretch:
					FitStretch();
					break;
				case FittingMode.PixelPerfect:
					FitPixelPerfect(num3, num4);
					break;
				case FittingMode.MaxSize:
					FitMaxSize(num3, num4);
					break;
				}
			}
		}

		private void FitMaxSize(int spriteWidth, int spriteHeight)
		{
			if (_spriteToFit.height <= _widgetContainer.height && _spriteToFit.width <= _widgetContainer.width)
			{
				FitPixelPerfect(spriteWidth, spriteHeight);
			}
			else
			{
				FitConstrain();
			}
		}

		private void FitPixelPerfect(int spriteWidth, int spriteHeight)
		{
			_spriteToFit.width = spriteWidth;
			_spriteToFit.height = spriteHeight;
		}

		private void FitStretch()
		{
			_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			_spriteToFit.width = _widgetContainer.width;
			_spriteToFit.height = _widgetContainer.height;
		}

		private void FitHeight()
		{
			_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
			_spriteToFit.height = _widgetContainer.height;
		}

		private void FitWidth()
		{
			_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
			_spriteToFit.width = _widgetContainer.width;
		}

		private void FitFill()
		{
			if (_spriteToFit.aspectRatio < _widgetContainer.aspectRatio)
			{
				_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
				_spriteToFit.width = _widgetContainer.width;
			}
			else
			{
				_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
				_spriteToFit.height = _widgetContainer.height;
			}
		}

		private void FitConstrain()
		{
			if (_spriteToFit.aspectRatio >= _widgetContainer.aspectRatio)
			{
				_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
				_spriteToFit.width = _widgetContainer.width;
			}
			else
			{
				_spriteToFit.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
				_spriteToFit.height = _widgetContainer.height;
			}
		}
	}
}
