using System;

namespace SYBO.Subway.Routes.Prototypes
{
	public interface IRouteEffectGUISubject
	{
		event Action OnCharacterEntryStarted;

		event Action OnEnteredSkatePark;

		event Action OnEnteredTallyScreen;

		event Action OnExitedSkatePark;

		event Action<int> OnGearChanged;

		event Action<int> OnTokenChanged;
	}
}
