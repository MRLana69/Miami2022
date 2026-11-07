using System;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public abstract class Route : MonoBehaviour
	{
		public class CharacterEntryStartedEventArgs : EventArgs
		{
			public readonly Route FromRoute;

			public readonly Route ToRoute;

			public CharacterEntryStartedEventArgs(Route fromRoute, Route toRoute)
			{
				FromRoute = fromRoute;
				ToRoute = toRoute;
			}
		}

		public class CharacterEntrySkippedEventArgs : EventArgs
		{
			public readonly bool HasCharacterStartedEntryTransition;

			public CharacterEntrySkippedEventArgs(bool hasCharacterStartedEntryTransition)
			{
				HasCharacterStartedEntryTransition = hasCharacterStartedEntryTransition;
			}
		}

		public class CharacterEnteredEventArgs : EventArgs
		{
			public readonly Route FromRoute;

			public CharacterEnteredEventArgs(Route fromRoute)
			{
				FromRoute = fromRoute;
			}
		}

		public class CharacterExitedEventArgs : EventArgs
		{
			public readonly Route ToRoute;

			public CharacterExitedEventArgs(Route toRoute)
			{
				ToRoute = toRoute;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<CharacterEntryStartedEventArgs> OnCharacterEntryStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<CharacterEntrySkippedEventArgs> OnCharacterEntrySkipped__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<CharacterEnteredEventArgs> OnCharacterEntered__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler OnCharacterEnteredLanded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<CharacterExitedEventArgs> OnCharacterExited__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler OnDestructing__BackingField;

		protected RouteManager _routeManager;

		protected Transform _transform;

		private Character _character;

		private bool _characterEnteredAndLanded;

		private float _characterEnterTimeStamp;

		private float _characterExitedTimeStamp;

		public abstract RouteBuilder Builder { get; }

		public abstract RouteType Type { get; }

		public Vector3 OriginGamePosition { get; private set; }

		public bool CharacterSkippedEntry { get; private set; }

		public bool CharacterEntered { get; private set; }

		public bool HasCharacterStartedEntryTransition { get; private set; }

		public bool IsCharacterInThisRoute
		{
			get
			{
				return _routeManager != null && _routeManager.CharacterRoute == this;
			}
		}

		public float TimeCharacterInRoute
		{
			get
			{
				return _characterExitedTimeStamp - _characterEnterTimeStamp;
			}
		}

		public float TimeCharacterEnterTimeStamp
		{
			get
			{
				return _characterEnterTimeStamp;
			}
		}

		public event EventHandler<CharacterEntryStartedEventArgs> OnCharacterEntryStarted
		{
			add
			{
				EventHandler<CharacterEntryStartedEventArgs> eventHandler = OnCharacterEntryStarted__BackingField;
				EventHandler<CharacterEntryStartedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEntryStarted__BackingField, (EventHandler<CharacterEntryStartedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CharacterEntryStartedEventArgs> eventHandler = OnCharacterEntryStarted__BackingField;
				EventHandler<CharacterEntryStartedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEntryStarted__BackingField, (EventHandler<CharacterEntryStartedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<CharacterEntrySkippedEventArgs> OnCharacterEntrySkipped
		{
			add
			{
				EventHandler<CharacterEntrySkippedEventArgs> eventHandler = OnCharacterEntrySkipped__BackingField;
				EventHandler<CharacterEntrySkippedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEntrySkipped__BackingField, (EventHandler<CharacterEntrySkippedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CharacterEntrySkippedEventArgs> eventHandler = OnCharacterEntrySkipped__BackingField;
				EventHandler<CharacterEntrySkippedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEntrySkipped__BackingField, (EventHandler<CharacterEntrySkippedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<CharacterEnteredEventArgs> OnCharacterEntered
		{
			add
			{
				EventHandler<CharacterEnteredEventArgs> eventHandler = OnCharacterEntered__BackingField;
				EventHandler<CharacterEnteredEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEntered__BackingField, (EventHandler<CharacterEnteredEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CharacterEnteredEventArgs> eventHandler = OnCharacterEntered__BackingField;
				EventHandler<CharacterEnteredEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEntered__BackingField, (EventHandler<CharacterEnteredEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler OnCharacterEnteredLanded
		{
			add
			{
				EventHandler eventHandler = OnCharacterEnteredLanded__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEnteredLanded__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnCharacterEnteredLanded__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEnteredLanded__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<CharacterExitedEventArgs> OnCharacterExited
		{
			add
			{
				EventHandler<CharacterExitedEventArgs> eventHandler = OnCharacterExited__BackingField;
				EventHandler<CharacterExitedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterExited__BackingField, (EventHandler<CharacterExitedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CharacterExitedEventArgs> eventHandler = OnCharacterExited__BackingField;
				EventHandler<CharacterExitedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterExited__BackingField, (EventHandler<CharacterExitedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler OnDestructing
		{
			add
			{
				EventHandler eventHandler = OnDestructing__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnDestructing__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnDestructing__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnDestructing__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public virtual void Initialize(RouteManager routeManager, Vector3 originGamePosition, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, RouteType previousRouteType, SyboRandom randomGenerator)
		{
			_routeManager = routeManager;
			_transform = GetComponent<Transform>();
			_transform.parent = _routeManager.transform;
			OriginGamePosition = originGamePosition;
			_transform.SetAsLastSibling();
			Builder.Initialize(routeManager, this, sectionSpawnMode, chunkBuildMode, previousRouteType, randomGenerator);
			CharacterEntered = false;
			HasCharacterStartedEntryTransition = false;
		}

		protected virtual void OnDestroy()
		{
			if (OnDestructing__BackingField != null)
			{
				OnDestructing__BackingField(this, EventArgs.Empty);
			}
			if (_character != null)
			{
				_character.OnLanding -= Character_OnLanding;
			}
		}

		protected void OnEnable()
		{
		}

		protected void OnValidate()
		{
			Builder.Validate(this);
		}

		public static Vector3 GetPosition(float x, float z)
		{
			return Vector3.forward * z + Vector3.right * (x - 20f);
		}

		public static int GetTrackX(int trackIndex)
		{
			return trackIndex * 20;
		}

		public void UpdateRoute(float characterGamePositionZ, SyboRandom random)
		{
			Builder.UpdateBuilder(characterGamePositionZ);
		}

		public virtual void CharacterStartEntryTransition(Route fromRoute, Route toRoute)
		{
			HasCharacterStartedEntryTransition = true;
			if (OnCharacterEntryStarted__BackingField != null)
			{
				OnCharacterEntryStarted__BackingField(this, new CharacterEntryStartedEventArgs(fromRoute, toRoute));
			}
		}

		public virtual void NotifyCharacterEntrySkipped()
		{
			CharacterSkippedEntry = true;
			if (OnCharacterEntrySkipped__BackingField != null)
			{
				OnCharacterEntrySkipped__BackingField(this, new CharacterEntrySkippedEventArgs(HasCharacterStartedEntryTransition));
			}
		}

		public void NotifyCharacterEntered(Character character, Route previousRoute)
		{
			_character = character;
			_characterEnterTimeStamp = Game.Instance.ElapsedGameDuration;
			_character.OnLanding += Character_OnLanding;
			CharacterEntered = true;
			RaiseOnCharacterEntered(new CharacterEnteredEventArgs(previousRoute));
		}

		public virtual void CharacterEnteredLanded()
		{
		}

		public virtual void NotifyCharacterExited(Route nextRoute)
		{
			if (_character != null)
			{
				_character.OnLanding -= Character_OnLanding;
			}
			_characterExitedTimeStamp = Game.Instance.ElapsedGameDuration;
			Builder.SetBuildMode(SectionSpawnMode.All, ChunkBuildMode.VisualsOnly);
			RaiseOnCharacterExited(new CharacterExitedEventArgs(nextRoute));
		}

		public virtual void CharacterStartExitTransition(Route nextRoute)
		{
		}

		public Vector3 GetRelativePosition(Vector3 gamePosition)
		{
			return gamePosition - OriginGamePosition;
		}

		public override string ToString()
		{
			return base.name;
		}

		protected virtual void RaiseOnCharacterEntered(CharacterEnteredEventArgs args)
		{
			if (OnCharacterEntered__BackingField != null)
			{
				OnCharacterEntered__BackingField(this, args);
			}
		}

		protected virtual void RaiseOnCharacterExited(CharacterExitedEventArgs args)
		{
			if (OnCharacterExited__BackingField != null)
			{
				OnCharacterExited__BackingField(this, args);
			}
		}

		private void Character_OnLanding(Character character)
		{
			if (!_characterEnteredAndLanded)
			{
				CharacterEnteredLanded();
			}
			_characterEnteredAndLanded = true;
			_character.OnLanding -= Character_OnLanding;
			if (OnCharacterEnteredLanded__BackingField != null)
			{
				OnCharacterEnteredLanded__BackingField(this, EventArgs.Empty);
			}
		}
	}
}
