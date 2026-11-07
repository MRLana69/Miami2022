using System.Collections;

namespace SYBO.Components.Utilities
{
	public interface ICoroutineHandler
	{
		void StartCoroutine(IEnumerator coroutine);
	}
}
