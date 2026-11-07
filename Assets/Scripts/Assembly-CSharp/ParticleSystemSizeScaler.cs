using UnityEngine;

public class ParticleSystemSizeScaler : MonoBehaviour
{
	private void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = component.main;
		main.startSizeMultiplier = base.transform.lossyScale.x;
		component.Stop();
		component.Play(true);
	}
}
