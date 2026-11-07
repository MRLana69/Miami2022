using UnityEngine;

public class Rotate : MonoBehaviour
{
	private Transform m_trans;

	public float Rotate_X;

	public float Rotate_Y;

	public float Rotate_Z;

	public bool PingPong;

	[Tooltip("used for ping pong only")]
	public float Rotate_X_To;

	[Tooltip("used for ping pong only")]
	public float Rotate_Y_To;

	[Tooltip("used for ping pong only")]
	public float Rotate_Z_To;

	[Tooltip("used for ping pong only")]
	public float Rotate_X_Time_Offset;

	[Tooltip("used for ping pong only")]
	public float Rotate_Y_Time_Offset;

	[Tooltip("used for ping pong only")]
	public float Rotate_Z_Time_Offset;

	[Tooltip("used for ping pong only")]
	public float Speed = 100f;

	private void Start()
	{
		m_trans = base.transform;
	}

	private void Update()
	{
		float x = 0f;
		float y = 0f;
		float z = 0f;
		if (PingPong)
		{
			if (Rotate_X_To != Rotate_X)
			{
				x = Mathf.PingPong(Time.time * Speed + Rotate_X_Time_Offset, Rotate_X_To - Rotate_X) + Rotate_X;
			}
			if (Rotate_Y_To != Rotate_Y)
			{
				y = Mathf.PingPong(Time.time * Speed + Rotate_Y_Time_Offset, Rotate_Y_To - Rotate_Y) + Rotate_Y;
			}
			if (Rotate_Z_To != Rotate_Z)
			{
				z = Mathf.PingPong(Time.time * Speed + Rotate_Z_Time_Offset, Rotate_Z_To - Rotate_Z) + Rotate_Z;
			}
			m_trans.eulerAngles = new Vector3(x, y, z);
		}
		else
		{
			x = Rotate_X * Time.deltaTime;
			y = Rotate_Y * Time.deltaTime;
			z = Rotate_Z * Time.deltaTime;
			m_trans.Rotate(new Vector3(x, y, z));
		}
	}
}
