using UnityEngine;

namespace UI
{
	public class GenericResolutionSwitchHelper : MonoBehaviour
	{
		public string highResName;

		private bool _inited;

		private void Start()
		{
			if (!_inited)
			{
				Init();
			}
		}

		private void Init()
		{
			_inited = true;
			if (DeviceInfo.Instance.isHighres)
			{
				UISprite component = GetComponent<UISprite>();
				component.spriteName = highResName;
				base.transform.parent.localScale = new Vector3(0.5f, 0.5f, 1f);
				component.width *= 2;
				component.height *= 2;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x * 2f, base.transform.localPosition.y * 2f, base.transform.localPosition.z);
				TweenScale component2 = base.transform.gameObject.GetComponent<TweenScale>();
				if (component2 != null)
				{
					component2.from = new Vector3(component2.from.x * 2f, component2.from.y * 2f, component2.from.z);
					component2.to = new Vector3(component2.to.x * 2f, component2.to.y * 2f, component2.to.z);
				}
				TweenPosition component3 = base.transform.gameObject.GetComponent<TweenPosition>();
				if (component3 != null)
				{
					component3.from = new Vector3(component3.from.x * 2f, component3.from.y * 2f, component3.from.z);
					component3.to = new Vector3(component3.to.x * 2f, component3.to.y * 2f, component3.to.z);
				}
				TweenTransform component4 = base.transform.gameObject.GetComponent<TweenTransform>();
				if (component4 != null)
				{
					SDebug.LogError("TweenTransform used and not handled in resolution switcher.");
				}
			}
		}
	}
}
