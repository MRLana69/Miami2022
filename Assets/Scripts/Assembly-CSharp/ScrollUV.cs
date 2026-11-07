using UnityEngine;

public class ScrollUV : MonoBehaviour
{
	[SerializeField]
	private Vector2 _uvSpeed;

	private MeshRenderer _renderer;

	private Material _material;

	private void Awake()
	{
		_renderer = base.gameObject.GetComponent<MeshRenderer>();
		_material = new Material(_renderer.material);
		_renderer.material = _material;
	}

	private void Update()
	{
		Vector2 textureOffset = _material.GetTextureOffset("_MainTex");
		_material.SetTextureOffset("_MainTex", textureOffset + _uvSpeed * Time.deltaTime);
	}
}
