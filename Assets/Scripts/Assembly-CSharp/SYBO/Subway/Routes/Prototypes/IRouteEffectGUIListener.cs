namespace SYBO.Subway.Routes.Prototypes
{
	public interface IRouteEffectGUIListener
	{
		void Initialize(IRouteEffectGUISubject subject);

		void DeInitialize();
	}
}
