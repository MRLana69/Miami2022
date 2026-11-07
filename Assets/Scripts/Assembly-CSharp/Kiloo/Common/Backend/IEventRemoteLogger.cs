using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Backend
{
	public interface IEventRemoteLogger : IEventLogger
	{
		bool isSending { get; }

		void Send(float maximumSendTime, float requestTimeWindow);

		Coroutine SendAsync([CanBeNull] Action onComplete = null);
	}
}
