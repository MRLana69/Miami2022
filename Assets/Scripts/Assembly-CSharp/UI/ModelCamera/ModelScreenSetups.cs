using System.Collections.Generic;
using UnityEngine;

namespace UI.ModelCamera
{
	public class ModelScreenSetups
	{
		private static Dictionary<UIModelController.ModelScreen, ModelScreenSetup> _setups = new Dictionary<UIModelController.ModelScreen, ModelScreenSetup>
		{
			{
				UIModelController.ModelScreen.Character,
				new ModelScreenSetup
				{
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.GameOver,
				new ModelScreenSetup
				{
					localScaleOnAllAxes = 18,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.Boards,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-30f, 0f, 100f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(20f, 154.5f, 358f)
				}
			},
			{
				UIModelController.ModelScreen.CelebrationCharacterUnlock,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-85f, -110f, -200f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.CelebrationTopRun,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-85f, -110f, -200f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.CelebrationBoardUnlock,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-80f, -100f, 0f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(30f, 210f, 10f)
				}
			},
			{
				UIModelController.ModelScreen.CelebrationHighScore,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-80f, -100f, 0f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(30f, 210f, 10f)
				}
			},
			{
				UIModelController.ModelScreen.LimitedCharacterPopup,
				new ModelScreenSetup
				{
					localPosition = new Vector3(0f, -56f, 0f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(-8f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.LimitedBoardPopup,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-30f, -56f, 100f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(12f, 154.5f, 358f)
				}
			},
			{
				UIModelController.ModelScreen.BoardPromotionPopup,
				new ModelScreenSetup
				{
					localPosition = new Vector3(0f, -40f, 100f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(10f, 165f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.FirstPurchaseBonusBigCharacter,
				new ModelScreenSetup
				{
					localPosition = new Vector3(26f, -64f, 0f),
					localScaleOnAllAxes = 18,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.CloudSaveReminderPopup,
				new ModelScreenSetup
				{
					localPosition = new Vector3(26f, -29f, 0f),
					localScaleOnAllAxes = 18,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.CharacterPromotionPopup,
				new ModelScreenSetup
				{
					localPosition = new Vector3(64f, -87f, 0f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.ShopScreenPromotion,
				new ModelScreenSetup
				{
					localPosition = new Vector3(0f, 0f, 0f),
					localScaleOnAllAxes = 15,
					localRotation = new Vector3(0f, 180f, 0f)
				}
			},
			{
				UIModelController.ModelScreen.BirthdayBoardPopup,
				new ModelScreenSetup
				{
					localPosition = new Vector3(-30f, -56f, 100f),
					localScaleOnAllAxes = 21,
					localRotation = new Vector3(0f, 160f, 358f)
				}
			}
		};

		public static ModelScreenSetup GetScreenSetup(UIModelController.ModelScreen screen)
		{
			return _setups[screen];
		}
	}
}
