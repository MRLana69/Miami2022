using UnityEngine;

public class CameraCulling : MonoBehaviour
{
	private Camera _camera;

	private float[] _distances = new float[32];

	public float TransparentFXCullingDistance
	{
		set
		{
			if (_camera == null)
			{
				_camera = GetComponent<Camera>();
			}
			_distances[LayerMask.NameToLayer("TransparentFX")] = value;
			_camera.layerCullDistances = _distances;
		}
	}

	private void Awake()
	{
		_camera = GetComponent<Camera>();
	}
}
