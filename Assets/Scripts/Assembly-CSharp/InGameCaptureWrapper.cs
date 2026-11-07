using UnityEngine;

public class InGameCaptureWrapper : MonoBehaviour
{
	[SerializeField]
	private int _width;

	[SerializeField]
	private int _height;

	public int Width
	{
		get
		{
			return _width;
		}
		set
		{
			_width = value;
		}
	}

	public int Height
	{
		get
		{
			return _height;
		}
		set
		{
			_height = value;
		}
	}
}
