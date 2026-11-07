using SYBO.Components.Base.Events;

namespace SYBO.Components.Base.Interfaces
{
	public interface IVisualEffect
	{
		VisualEffectEvent OnPlay { get; }

		VisualEffectEvent OnLoop { get; }

		VisualEffectEvent OnStop { get; }

		bool IsAutoPlay { get; }

		bool IsPlaying { get; }

		void Play();

		void Stop();
	}
}
