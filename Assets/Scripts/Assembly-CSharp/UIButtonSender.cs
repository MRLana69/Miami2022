using UnityEngine;

public class UIButtonSender : MonoBehaviour
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

	protected virtual void OnHover(bool isOver)
	{
		if ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut))
		{
			Send();
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		if ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnPress))
		{
			Send(isPressed);
		}
	}

	protected virtual void OnClick()
	{
		if (trigger == Trigger.OnClick)
		{
			Send();
		}
	}

	protected virtual void Send()
	{
	}

	protected virtual void Send(bool isPressed)
	{
	}
}
