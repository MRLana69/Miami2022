using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UnityEngine;

namespace WeeklyHunt
{
	public class WeeklyHuntIconPositioning : MonoBehaviour
	{
		public enum IconPlacement
		{
			NotSet = 0,
			FrontUIButton = 1,
			CelebrationPopup = 2,
			WeeklyHuntPopupActive = 3,
			WeeklyHuntPopupInactive = 4,
			SlideIn = 5,
			IngameCounter = 6,
			IngameCounterBackground = 7,
			WeeklyHuntPageProgress = 8
		}

		[SerializeField]
		private IconPlacement _placement;

		private bool RUN_ONCE = true;

		private int HEIGHT_PAGE_PROGRESS = 28;

		private Vector2 PAGE_ICON_POSITION = new Vector2(10.9f, 0.31f);

		public static Vector3 MODEL_ICON_POSITION_OFFSET = new Vector2(0.5f, 7f);

		public static Vector3 MODEL_ICON_ROTATION = new Vector3(5f, 8f, -9f);

		public static Vector3 MODEL_ICON_SCALE_OFFSET = Vector3.one * 0.9f;

		public static Vector3 INGAME_COUNTER_GAMEOBJECT_POSITION = new Vector3(100f, -7f, 0f);

		private int HEIGHT_INGAME = 31;

		private Vector2 INGAME_COUNTER_ICON_POSITION = new Vector2(-18f, -15.4f);

		private Vector2 INGAME_COUNTER_BACKGROUND_POSITION = new Vector2(-31.6f, 0f);

		public static float HUNT_BG_SCALE_OFFSET = -2f;

		private int HEIGHT_SLIDE_IN = 33;

		private Vector2 SLIDEIN_ICON_POSITION = new Vector2(37f, 1f);

		private int HEIGHT_CELEBRATION_POPUP = 30;

		private int CELEBRATION_POPUP_ICON_PADDING = 4;

		private float CELEBRATION_POPUP_ICON_Y_OFFSET = 1f;

		public void ForceUpdate()
		{
			SetSize();
			Update();
		}

		private void Awake()
		{
			ForceUpdate();
		}

		private void Update()
		{
			SetPosition();
			if (RUN_ONCE)
			{
				base.enabled = false;
			}
		}

		private void SetSize()
		{
			int num = 0;
			switch (_placement)
			{
			case IconPlacement.WeeklyHuntPageProgress:
				num = HEIGHT_PAGE_PROGRESS;
				break;
			case IconPlacement.CelebrationPopup:
				num = HEIGHT_CELEBRATION_POPUP;
				break;
			case IconPlacement.SlideIn:
				num = HEIGHT_SLIDE_IN;
				break;
			case IconPlacement.IngameCounter:
				num = HEIGHT_INGAME;
				break;
			}
			if (num != 0)
			{
				UISprite component = GetComponent<UISprite>();
				if ((bool)component)
				{
					component.keepAspectRatio = UIWidget.AspectRatioSource.Free;
					component.MakePixelPerfect();
					component.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
					component.height = num;
				}
			}
		}

		private void SetPosition()
		{
			Vector3? vector = null;
			switch (_placement)
			{
			case IconPlacement.CelebrationPopup:
			{
				base.transform.SetLocalPosY(CELEBRATION_POPUP_ICON_Y_OFFSET);
				Layout component = base.transform.parent.GetComponent<Layout>();
				if (component != null)
				{
					component.padding = CELEBRATION_POPUP_ICON_PADDING;
				}
				break;
			}
			case IconPlacement.WeeklyHuntPageProgress:
				vector = PAGE_ICON_POSITION;
				break;
			case IconPlacement.SlideIn:
				vector = SLIDEIN_ICON_POSITION;
				break;
			case IconPlacement.IngameCounter:
				vector = INGAME_COUNTER_ICON_POSITION;
				break;
			case IconPlacement.IngameCounterBackground:
				vector = INGAME_COUNTER_BACKGROUND_POSITION;
				break;
			}
			if (vector.HasValue)
			{
				base.transform.localPosition = vector.Value;
			}
		}
	}
}
