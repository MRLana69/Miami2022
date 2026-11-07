using System.Collections.Generic;
using UnityEngine;

namespace Kiloo.Common.Backend.Authentication
{
	public interface IKilooAuthPlatform
	{
		string name { get; }

		IDictionary<string, string> value { get; }

		string signature { get; }

		Coroutine UpdateAsync();
	}
}
