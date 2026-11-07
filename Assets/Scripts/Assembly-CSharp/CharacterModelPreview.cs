using UnityEngine;

public class CharacterModelPreview : MonoBehaviour
{
	[SerializeField]
	private GameObject _rigPrefab;

	private SkinnedMeshRenderer[] _renderes;

	public SkinnedMeshRenderer[] Renderes
	{
		get
		{
			return _renderes;
		}
	}

	private void Awake()
	{
		BuildRig();
	}

	private void BuildRig()
	{
		GameObject gameObject = Object.Instantiate(_rigPrefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		CharacterRig component = gameObject.GetComponent<CharacterRig>();
		_renderes = new SkinnedMeshRenderer[3];
		_renderes[0] = component.Model;
		_renderes[1] = component.OutfitModel;
		_renderes[0] = component.HeadModel;
	}
}
