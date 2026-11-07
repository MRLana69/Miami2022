using System;
using UnityEngine;

public class CurveParent : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private bool _disableGizmos;

	public Color color = Color.red;

	private Vector3 _localPositionAtCaching;

	[SerializeField]
	private Curve _cachedCurve;

	[SerializeField]
	[HideInInspector]
	private bool _forceLinearZ;

	public float MaxTime
	{
		get
		{
			return CachedCurve.MaxTime;
		}
	}

	public float MinTime
	{
		get
		{
			return CachedCurve.MinTime;
		}
	}

	public Curve CachedCurve
	{
		get
		{
			if (_cachedCurve == null || base.transform.localPosition != _localPositionAtCaching)
			{
				RecacheCurve();
			}
			return _cachedCurve;
		}
	}

	public void OnDrawGizmos()
	{
		if (!_disableGizmos)
		{
			DrawCurve(color);
		}
	}

	public Quaternion GetRotationForTime(float time, float timeOffset = 0.25f)
	{
		Curve cachedCurve = CachedCurve;
		Vector3 vector = cachedCurve.Evaluate(time - timeOffset);
		Vector3 vector2 = cachedCurve.Evaluate(time);
		Quaternion result = default(Quaternion);
		result.SetFromToRotation(vector, vector2);
		Vector3 vector3 = vector2 - vector;
		if (vector3 != new Vector3(0f, 0f, 0f))
		{
			result.SetLookRotation(vector3);
		}
		return result;
	}

	public Vector3 GetDerivativeForTime(float time)
	{
		Curve cachedCurve = CachedCurve;
		return cachedCurve.EvaluateDerivative(time);
	}

	public Vector3 GetLocalPositionForTime(float time)
	{
		Curve cachedCurve = CachedCurve;
		return cachedCurve.Evaluate(time);
	}

	public bool HasTimeForLocalZ(float z)
	{
		Curve cachedCurve = CachedCurve;
		return cachedCurve.MinCoords.z <= z && z <= cachedCurve.MaxCoords.z;
	}

	public float GetTimeForLocalZ(float z, bool clampForInvalidTime = false)
	{
		Curve cachedCurve = CachedCurve;
		if (HasTimeForLocalZ(z))
		{
			if (_forceLinearZ)
			{
				return (z - cachedCurve.MinCoords.z) / (cachedCurve.MaxCoords.z - cachedCurve.MinCoords.z);
			}
			float num = 0f;
			float result = 0f;
			for (float num2 = 0f; num2 <= 1f; num2 += 1E-05f)
			{
				num = cachedCurve.curveZ.Evaluate(num2);
				if (num >= z)
				{
					result = num2;
					break;
				}
			}
			return result;
		}
		if (clampForInvalidTime)
		{
			float value = cachedCurve.curveZ.keys[0].value;
			return (!(z < value)) ? 1f : 0f;
		}
		throw new Exception("CurveParent.GetTimeForZ(" + z + ") failed. Use CurveParent.HasTimeForZ() first.");
	}

	public float GetTimeForWorldZ(float z, bool clampForInvalidTime = false)
	{
		float z2 = z - base.transform.position.z;
		return GetTimeForLocalZ(z2, clampForInvalidTime);
	}

	public bool HasTimeForWorldZ(float z)
	{
		return this != null && base.transform != null && HasTimeForLocalZ(z - base.transform.position.z);
	}

	public Vector3 GetWorldPositionForTime(float time, bool clampForInvalidTime = false)
	{
		return base.transform.TransformPoint(GetLocalPositionForTime(time));
	}

	public void ForceLinearZ(bool v)
	{
		_forceLinearZ = v;
		RecacheCurve();
	}

	public string GetDebugText()
	{
		string text = string.Concat("GetDebugText() For Curve at world position: ", base.transform.position, " , ", base.transform.localPosition, "...");
		text += "\n";
		text += "\n";
		Curve cachedCurve = CachedCurve;
		Vector3 vector = default(Vector3);
		for (int i = 0; i < cachedCurve.curveX.length; i++)
		{
			vector.x = cachedCurve.curveX[i].value;
			vector.y = cachedCurve.curveY[i].value;
			vector.z = cachedCurve.curveZ[i].value;
			string text2 = text;
			text = string.Concat(text2, "i: ", i, " l: ", vector, " w: ", vector + base.transform.position);
			text += "\n";
		}
		return text + "\n";
	}

	public void RecacheCurve()
	{
		_cachedCurve = CreateCurve(base.transform);
		_localPositionAtCaching = base.transform.localPosition;
		if (_forceLinearZ)
		{
			LinearizeZ();
		}
	}

	private void LinearizeZ()
	{
		float z = _cachedCurve.Evaluate(MinTime).z;
		float z2 = _cachedCurve.Evaluate(MaxTime).z;
		float num = z2 - z;
		Keyframe[] keys = new Keyframe[2]
		{
			new Keyframe(0f, z, 0f, num),
			new Keyframe(1f, z2, num, 0f)
		};
		_cachedCurve.curveZ.keys = keys;
	}

	private static Curve CreateCurve(Transform curvePointsParent, Vector3 offset)
	{
		Curve curve = new Curve();
		CurvePoint[] componentsInChildren = curvePointsParent.GetComponentsInChildren<CurvePoint>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			CurvePoint curvePoint = componentsInChildren[i];
			curve.AddKey(curvePoint.t, curvePoint.transform.localPosition + offset, (curvePoint.transform.TransformPoint(-curvePoint.TangentInPoint) - curvePoint.transform.position) * curvePoint.weight, (curvePoint.transform.TransformPoint(curvePoint.TangentOutPoint) - curvePoint.transform.position) * curvePoint.weight);
			if (curvePoint.smoothTangents)
			{
				curve.SmoothTangents(i, curvePoint.weight);
			}
		}
		return curve;
	}

	private static Curve CreateCurve(Transform curvePointsParent)
	{
		return CreateCurve(curvePointsParent, new Vector3(0f, 0f, 0f));
	}

	private void DrawCurve(Color color)
	{
		Gizmos.color = color;
		float num = CachedCurve.MaxTime - CachedCurve.MinTime;
		int num2 = 13;
		Vector3 vector = GetWorldPositionForTime(0f);
		for (int i = 0; i < num2; i++)
		{
			float time = num * (float)i / (float)(num2 - 1);
			Vector3 worldPositionForTime = GetWorldPositionForTime(time);
			Gizmos.DrawLine(vector, worldPositionForTime);
			vector = worldPositionForTime;
		}
	}
}
