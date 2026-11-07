using System;
using UnityEngine;

public abstract class ATeleboardEffect : MonoBehaviour
{
	public Action OnTeleboardEffectEnd { get; set; }

	public abstract void StartTeleBoardEffect();

	public abstract void ResetTeleboardEffect();
}
