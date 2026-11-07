using UnityEngine;

public class SpinningGlowStar : MonoBehaviour
{
	[SerializeField]
	private UISprite _sprite1;

	[SerializeField]
	private UISprite _sprite2;

	[SerializeField]
	private UISprite _glow;

	[SerializeField]
	private Color _defaultColor;

	private void OnDisable()
	{
		_sprite1.color = _defaultColor;
		_sprite2.color = _defaultColor;
		_glow.color = _defaultColor;
	}
}
