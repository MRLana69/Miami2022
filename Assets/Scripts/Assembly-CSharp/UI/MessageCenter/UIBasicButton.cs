using JetBrains.Annotations;
using UnityEngine;

namespace UI.MessageCenter
{
	public class UIBasicButton : MonoBehaviour
	{
		public enum Trigger
		{
			OnClick = 0,
			OnMouseOver = 1,
			OnMouseOut = 2,
			OnPress = 3,
			OnRelease = 4
		}

		public Trigger trigger;

		[UsedImplicitly]
		protected void OnHover(bool isOver)
		{
			if ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut))
			{
				Send();
			}
		}

		[UsedImplicitly]
		protected void OnPress(bool isPressed)
		{
			if ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease))
			{
				Send();
			}
		}

		[UsedImplicitly]
		protected void OnClick()
		{
			if (trigger == Trigger.OnClick)
			{
				Send();
			}
		}

		protected virtual void Send()
		{
		}
	}
}
