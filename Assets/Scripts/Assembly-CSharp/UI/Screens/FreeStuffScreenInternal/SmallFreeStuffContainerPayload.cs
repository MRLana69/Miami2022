using System;
using UI.FreeStuff;
using UnityEngine;

namespace UI.Screens.FreeStuffScreenInternal
{
	public class SmallFreeStuffContainerPayload
	{
		public Color32 bgGradientTopColor;

		public Color32 bgGradientBottomColor;

		public Color32 outlineGradientTopColor;

		public Color32 outlineGradientBottomColor;

		public Color32 glowColor;

		public Color32 titleTextColor;

		public Color32 titleTextShadow;

		public Color32 descriptionTextColor;

		public Color32 descriptionTextShadow;

		public string iconSpriteName;

		public string headerString;

		public string descriptionString;

		public string buttonString;

		public string buttonLoadingString;

		public VideoButtonPayload adPayload;

		public Action clickAction;
	}
}
