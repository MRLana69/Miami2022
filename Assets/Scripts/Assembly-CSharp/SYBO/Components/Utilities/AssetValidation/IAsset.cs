namespace SYBO.Components.Utilities.AssetValidation
{
	public interface IAsset
	{
		AssetEvent OnValidated { get; }
	}
}
