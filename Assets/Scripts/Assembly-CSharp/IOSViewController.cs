using UnityEngine;

public static class IOSViewController
{
	private static void _ShowStatusBar(bool show)
	{
	}

	private static void _SetStatusBarStyle(int style)
	{
	}

	private static void _SetStatusBarAnimation(int anim)
	{
	}

	private static void _SetEdgeProtection(int sides)
	{
	}

	private static void _SetHomeIndicator(bool show)
	{
	}

	private static bool _GetIsSimulator()
	{
		return false;
	}

	private static int _GetTopInsetInPoints()
	{
		int result = 0;
		if ((Screen.height == 2436 && Screen.width == 1125) || (Screen.height == 2688 && Screen.width == 1242))
		{
			result = 44;
		}
		return result;
	}

	private static int _GetBottomInsetInPoints()
	{
		int result = 0;
		if ((Screen.height == 2436 && Screen.width == 1125) || (Screen.height == 2688 && Screen.width == 1242))
		{
			result = 34;
		}
		return result;
	}

	private static int _GetDeviceHeightInPoints()
	{
		int result = 0;
		if (Screen.height == 2436 && Screen.width == 1125)
		{
			result = 812;
		}
		else if (Screen.height == 2688 && Screen.width == 1242)
		{
			result = 896;
		}
		return result;
	}

	public static void ShowStatusBar(bool show)
	{
		_ShowStatusBar(show);
	}

	public static void SetStatusBarStyle(IOSStatusBarStyle style)
	{
		_SetStatusBarStyle((int)style);
	}

	public static void SetStatusBarAnimation(IOSStatusBarAnimation anim)
	{
		_SetStatusBarAnimation((int)anim);
	}

	public static void SetProtectedEdges(IOSUIRectEdge sides)
	{
		_SetEdgeProtection((int)sides);
	}

	public static void ShowHomeIndicator(bool show)
	{
		_SetHomeIndicator(show);
	}

	public static bool IsSimulator()
	{
		return _GetIsSimulator();
	}

	public static int GetTopInsetInPoints()
	{
		return _GetTopInsetInPoints();
	}

	public static int GetBottomInsetInPoints()
	{
		return _GetBottomInsetInPoints();
	}

	public static int GetDeviceHeightInPoints()
	{
		return _GetDeviceHeightInPoints();
	}
}
