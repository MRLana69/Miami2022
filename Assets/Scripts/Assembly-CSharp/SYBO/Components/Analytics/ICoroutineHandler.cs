using System.Collections;

namespace SYBO.Components.Analytics
{
	public interface ICoroutineHandler
	{
		void StartCoroutine(IEnumerator coroutine);
	}
}
