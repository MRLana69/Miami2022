using System;
using UnityEngine;

namespace SYBO.Subway.Routes.Log
{
	[Serializable]
	public class RoutesLogData
	{
		private float _timeStamp;

		private RoutesLogType _routesLogType;

		private object[] _args;

		protected string _message = string.Empty;

		public float TimeStamp
		{
			get
			{
				return _timeStamp;
			}
		}

		public RoutesLogType RoutesLogType
		{
			get
			{
				return _routesLogType;
			}
		}

		public object[] Args
		{
			get
			{
				return _args;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public RoutesLogData(RoutesLogType pickupLogType, object[] args)
		{
			_routesLogType = pickupLogType;
			_args = args;
			_timeStamp = Time.timeSinceLevelLoad;
			_message = string.Format("[RoutesLogData (RoutesLogType={0},TimeStamp={1})]", _routesLogType, _timeStamp);
		}

		public override string ToString()
		{
			return _message;
		}
	}
}
