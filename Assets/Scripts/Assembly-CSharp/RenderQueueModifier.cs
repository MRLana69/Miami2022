using UnityEngine;

public class RenderQueueModifier : MonoBehaviour
{
	public enum RenderType
	{
		FRONT = 0,
		BACK = 1
	}

	public UIWidget m_target;

	public RenderType m_type;

	private Renderer[] _renderers;

	private int _lastQueue;

	private void Start()
	{
		_renderers = GetComponentsInChildren<Renderer>();
	}

	private void LateUpdate()
	{
		if (m_target == null || m_target.drawCall == null)
		{
			return;
		}
		int renderQueue = m_target.drawCall.renderQueue;
		renderQueue += ((m_type != RenderType.FRONT) ? (-2) : 2);
		if (_lastQueue != renderQueue)
		{
			_lastQueue = renderQueue;
			Renderer[] renderers = _renderers;
			foreach (Renderer renderer in renderers)
			{
				renderer.material.renderQueue = _lastQueue;
			}
		}
	}
}
