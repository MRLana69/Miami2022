using UnityEngine;

public class Curve
{
	public AnimationCurve curveX = new AnimationCurve();

	public AnimationCurve curveY = new AnimationCurve();

	public AnimationCurve curveZ = new AnimationCurve();

	public Vector3 MinCoords { get; private set; }

	public Vector3 MaxCoords { get; private set; }

	public float MinTime { get; private set; }

	public float MaxTime { get; private set; }

	public Curve()
	{
		curveX.postWrapMode = WrapMode.ClampForever;
		curveX.preWrapMode = WrapMode.ClampForever;
		curveY.postWrapMode = WrapMode.ClampForever;
		curveY.preWrapMode = WrapMode.ClampForever;
		curveZ.postWrapMode = WrapMode.ClampForever;
		curveZ.preWrapMode = WrapMode.ClampForever;
		MinTime = float.PositiveInfinity;
		MaxTime = float.NegativeInfinity;
		MinCoords = Vector3.one * float.PositiveInfinity;
		MaxCoords = Vector3.one * float.NegativeInfinity;
	}

	public void AddKey(float t, Vector3 value)
	{
		curveX.AddKey(t, value.x);
		curveY.AddKey(t, value.y);
		curveZ.AddKey(t, value.z);
		UpdateMinMaxTime(t);
		UpdateMinMaxCoords(value);
	}

	public void AddKey(float t, Vector3 value, Vector3 inTangent, Vector3 outTangent)
	{
		curveX.AddKey(new Keyframe(t, value.x, inTangent.x, outTangent.x));
		curveY.AddKey(new Keyframe(t, value.y, inTangent.y, outTangent.y));
		curveZ.AddKey(new Keyframe(t, value.z, inTangent.z, outTangent.z));
		UpdateMinMaxTime(t);
		UpdateMinMaxCoords(value);
	}

	public void MoveKey(int index, float t, Vector3 value, Vector3 inTangent, Vector3 outTangent)
	{
		curveX.MoveKey(index, new Keyframe(t, value.x, inTangent.x, outTangent.x));
		curveY.MoveKey(index, new Keyframe(t, value.y, inTangent.y, outTangent.y));
		curveZ.MoveKey(index, new Keyframe(t, value.z, inTangent.z, outTangent.z));
		UpdateMinMaxTime(t);
		UpdateMinMaxCoords(value);
	}

	public void MoveKey(int index, float t, Vector3 value)
	{
		curveX.MoveKey(index, new Keyframe(t, value.x));
		curveY.MoveKey(index, new Keyframe(t, value.y));
		curveZ.MoveKey(index, new Keyframe(t, value.z));
		UpdateMinMaxTime(t);
		UpdateMinMaxCoords(value);
	}

	public void SmoothTangents(int index, float weight)
	{
		curveX.SmoothTangents(index, weight);
		curveY.SmoothTangents(index, weight);
		curveZ.SmoothTangents(index, weight);
	}

	public Vector3 Evaluate(float t)
	{
		return new Vector3(curveX.Evaluate(t), curveY.Evaluate(t), curveZ.Evaluate(t));
	}

	public Vector3 EvaluateDerivative(float t)
	{
		float num = t * t;
		Vector3 vector2;
		Vector3 vector3;
		Vector3 zero;
		Vector3 vector = (vector2 = (vector3 = (zero = Vector3.zero)));
		for (int i = 1; i < curveZ.keys.Length; i++)
		{
			Keyframe keyframe = curveZ.keys[i - 1];
			Keyframe keyframe2 = curveZ.keys[i];
			if (keyframe.time <= t && t < keyframe2.time)
			{
				vector.x = curveX.keys[i - 1].value;
				vector.y = curveY.keys[i - 1].value;
				vector.z = curveZ.keys[i - 1].value;
				vector2.x = curveX.keys[i].value;
				vector2.y = curveY.keys[i].value;
				vector2.z = curveZ.keys[i].value;
				vector3.x = curveX.keys[i - 1].outTangent;
				vector3.y = curveY.keys[i - 1].outTangent;
				vector3.z = curveZ.keys[i - 1].outTangent;
				zero.x = curveX.keys[i].inTangent;
				zero.y = curveY.keys[i].inTangent;
				zero.z = curveZ.keys[i].inTangent;
				t = (t - keyframe.time) / (keyframe2.time - keyframe.time);
				num = t * t;
				break;
			}
		}
		if (vector.sqrMagnitude + vector2.sqrMagnitude + vector3.sqrMagnitude + zero.sqrMagnitude == 0f)
		{
			return Vector3.zero;
		}
		Vector3 vector4 = (6f * num - 6f * t) * vector;
		vector4 += (3f * num - 4f * t + 1f) * vector3;
		vector4 += (-6f * num + 6f * t) * vector2;
		return vector4 + (3f * num - 2f * t) * zero;
	}

	public void DrawGizmos(Color color)
	{
		Gizmos.color = color;
		int num = 1000;
		Vector3 vector = Evaluate(0f);
		for (int i = 0; i < num; i++)
		{
			float t = (MaxTime - MinTime) * (float)i / (float)(num - 1);
			Vector3 vector2 = Evaluate(t);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
	}

	private void UpdateMinMaxTime(float t)
	{
		if (t < MinTime)
		{
			MinTime = t;
		}
		if (t > MaxTime)
		{
			MaxTime = t;
		}
	}

	private void UpdateMinMaxCoords(Vector3 v)
	{
		Vector3 minCoords = MinCoords;
		minCoords.x = Mathf.Min(minCoords.x, v.x);
		minCoords.y = Mathf.Min(minCoords.y, v.y);
		minCoords.z = Mathf.Min(minCoords.z, v.z);
		MinCoords = minCoords;
		Vector3 maxCoords = MaxCoords;
		maxCoords.x = Mathf.Max(maxCoords.x, v.x);
		maxCoords.y = Mathf.Max(maxCoords.y, v.y);
		maxCoords.z = Mathf.Max(maxCoords.z, v.z);
		MaxCoords = maxCoords;
	}
}
