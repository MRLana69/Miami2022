using UnityEngine;

namespace UI.MessageCenter
{
	public class MessageCenter : MonoBehaviour
	{
		private static MessageCenter _instance;

		public static MessageCenter instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType(typeof(MessageCenter)) as MessageCenter;
					if (_instance == null)
					{
						SDebug.LogError("MessageCenter not found in the scene.");
					}
				}
				return _instance;
			}
		}

		public static bool IsInstanced
		{
			get
			{
				return _instance != null;
			}
		}

		private void Awake()
		{
			_instance = this;
		}
	}
}
