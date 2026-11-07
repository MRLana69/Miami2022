using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Routes.Builders
{
	[Serializable]
	public abstract class ConstrainingRouteBuilder : RouteBuilder, IConstrainingRouteBuilder
	{
		public const int MinConstraintLength = 180;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler OnReachedLengthVisibleFromEntry__BackingField;

		private List<RouteBuilder> _constrainedRouteBuilders = new List<RouteBuilder>();

		public bool HasReachedLengthVisibleFromEntry
		{
			get
			{
				return DistanceVisibleFromEntry <= _length;
			}
		}

		public int ConstraintStartZ
		{
			get
			{
				return (int)_originGamePosition.z;
			}
		}

		public int ConstraintEndZ
		{
			get
			{
				if (base.SectionCountUsed == 0)
				{
					return ConstraintStartZ + 180;
				}
				return (!_stopped) ? base.SpawnedLeadingEdgeGamePositionZ : base.BuiltLeadingEdgeGamePositionZ;
			}
		}

		public ICollection<RouteBuilder> ConstrainedBuilders
		{
			get
			{
				return _constrainedRouteBuilders;
			}
		}

		public abstract ICollection<RouteType> ConstrainedRouteTypes { get; }

		public event EventHandler OnReachedLengthVisibleFromEntry
		{
			add
			{
				EventHandler eventHandler = OnReachedLengthVisibleFromEntry__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnReachedLengthVisibleFromEntry__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnReachedLengthVisibleFromEntry__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnReachedLengthVisibleFromEntry__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public abstract RouteConstraint GetConstraint(RouteType routeType);

		public override void UpdateBuilder(float characterWorldZ)
		{
			bool hasReachedLengthVisibleFromEntry = HasReachedLengthVisibleFromEntry;
			base.UpdateBuilder(characterWorldZ);
			if (!hasReachedLengthVisibleFromEntry && HasReachedLengthVisibleFromEntry && OnReachedLengthVisibleFromEntry__BackingField != null)
			{
				OnReachedLengthVisibleFromEntry__BackingField(this, EventArgs.Empty);
			}
		}

		public void CollectConstrained()
		{
			_constrainedRouteBuilders.Clear();
			AddFromListIfConstrained(base.BuildersAbove);
			AddFromListIfConstrained(base.BuildersBelow);
			AddIfConstrained(base.BuilderForward);
		}

		private void AddFromListIfConstrained(ICollection<RouteBuilder> builders)
		{
			foreach (RouteBuilder builder in builders)
			{
				AddIfConstrained(builder);
			}
		}

		private void AddIfConstrained(RouteBuilder builder)
		{
			if (builder != null && ConstrainedRouteTypes.Contains(builder.Route.Type))
			{
				_constrainedRouteBuilders.Add(builder);
			}
		}

		public override void ConnectBuilder(RouteBuilder builderToConnect)
		{
			base.ConnectBuilder(builderToConnect);
			CollectConstrained();
		}

		public override void DisconnectBuilder(RouteBuilder builderToDisconnect)
		{
			base.DisconnectBuilder(builderToDisconnect);
			CollectConstrained();
		}

		public bool IsConstrainingStartAt(int rangeStartZ)
		{
			return ConstraintStartZ <= rangeStartZ && rangeStartZ < ConstraintEndZ;
		}

		public bool IsConstrainingEndAt(int rangeEndZ)
		{
			return ConstraintStartZ < rangeEndZ && rangeEndZ <= ConstraintEndZ;
		}

		public bool IsConstrainingRange(int startZ, int endZ)
		{
			return IsConstrainingStartAt(startZ) || IsConstrainingEndAt(endZ);
		}

		private void OnDrawGizmos()
		{
			float num = ConstraintEndZ - ConstraintStartZ;
			Vector3 gamePosition = _originGamePosition + Vector3.forward * num;
			Vector3 worldPosition = GamePositionRoot.Instance.GetWorldPosition(gamePosition);
			Vector3 size = new Vector3(61f, 100f, num / 2f);
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(worldPosition, size);
		}
	}
}
