using SYBO.Common.StateSystem;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using SYBO.Subway.Inputs;

namespace SYBO.Subway.StateSystem.States
{
	public abstract class InGameState : State
	{
		protected const float CharacterChangeTrackLength = 30f;

		protected Game _game;

		protected Character _character;

		protected CharacterCamera _characterCamera;

		public InGameState(int id)
			: base(id)
		{
			_game = Game.Instance;
			_character = _game.Character;
			_characterCamera = CharacterCamera.Instance;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_game.UpdateMeters();
		}

		public virtual void HandleSwipe(SwipeDir swipeDir)
		{
		}

		public virtual void HandleCriticalHit()
		{
		}

		public virtual void HandleDoubleTap()
		{
		}
	}
}
