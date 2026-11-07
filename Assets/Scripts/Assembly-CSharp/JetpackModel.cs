using UnityEngine;

public class JetpackModel : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer mesh;

	[SerializeField]
	private Transform jetpackCloudPositionL;

	[SerializeField]
	private Transform jetpackCloudPositionR;

	[SerializeField]
	private Transform[] jetpackThrusters;

	public MeshRenderer Mesh
	{
		get
		{
			return mesh;
		}
	}

	public Transform JetpackCloudPositionL
	{
		get
		{
			return jetpackCloudPositionL;
		}
	}

	public Transform JetpackCloudPositionR
	{
		get
		{
			return jetpackCloudPositionR;
		}
	}

	public Transform[] JetpackThrusters
	{
		get
		{
			return jetpackThrusters;
		}
	}
}
