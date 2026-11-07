using UnityEngine;

public class CurvePoint : MonoBehaviour
{
	public float t;

	public bool smoothTangents = true;

	public CurvePointTangent customIn;

	public CurvePointTangent customOut;

	public float weight = 50f;

	public Vector3 TangentInPoint
	{
		get
		{
			return (!(customIn != null)) ? Vector3.back : customIn.transform.localPosition;
		}
	}

	public Vector3 TangentOutPoint
	{
		get
		{
			return (!(customOut != null)) ? Vector3.forward : customOut.transform.localPosition;
		}
	}

	private string GetName()
	{
		return "CurvePoint (" + t + ")";
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
		if (base.gameObject.name != GetName())
		{
			base.gameObject.name = GetName();
		}
		if (!smoothTangents)
		{
			Vector3 vector = base.transform.TransformPoint(TangentInPoint);
			Vector3 vector2 = base.transform.TransformPoint(TangentOutPoint);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, vector);
			Gizmos.DrawLine(base.transform.position, vector2);
			Gizmos.color = Color.yellow * 0.8f;
			Gizmos.DrawSphere(vector, 0.3f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(vector2, 0.3f);
		}
	}
}
