using UnityEngine;

public class CountryCellController : MonoBehaviour
{
	[SerializeField]
	public UILabel countryName;

	[SerializeField]
	public UISprite countryFlag;

	[SerializeField]
	public UISprite statusSprite;

	public string countryLocale;

	private void OnEnable()
	{
	}
}
