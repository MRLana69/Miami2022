namespace SYBO.Common.Visuals
{
	public interface IQualityPlatformSupport
	{
		void Initialize();

		bool IsSupported(QualityFeature feature);
	}
}
