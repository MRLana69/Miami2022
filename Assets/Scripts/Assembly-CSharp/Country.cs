public class Country
{
	public readonly string displayNameID;

	public readonly string flag;

	public readonly string countryCode;

	public Country(string displayNameID, string flag, string countryCode)
	{
		this.displayNameID = displayNameID;
		this.flag = flag;
		this.countryCode = countryCode;
	}
}
