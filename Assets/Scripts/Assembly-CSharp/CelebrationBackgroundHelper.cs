using UnityEngine;

public class CelebrationBackgroundHelper : MonoBehaviour
{
	public Transform backgroundRoot;

	public Transform celebrationCloner;

	private bool shouldRotate;

	[SerializeField]
	private AnimationCurve _bragCelebrationAnimationCurve;

	public bool visible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			base.gameObject.SetActive(value);
		}
	}

	public void Hide()
	{
		visible = false;
	}

	public void Show(Quaternion rotation, bool shouldAnimateStripes, bool shouldRotateBackground)
	{
		if (!visible)
		{
			visible = true;
		}
		if (celebrationCloner.gameObject.activeSelf != shouldAnimateStripes)
		{
			celebrationCloner.gameObject.SetActive(shouldAnimateStripes);
		}
		shouldRotate = shouldRotateBackground;
		backgroundRoot.rotation = rotation;
		InvertParticlesDirection(false);
	}

	public void SetParticleAnimationSpeed(float speed)
	{
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>(false);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.simulationSpeed = speed;
		}
	}

	public void InvertParticlesDirection(bool invert)
	{
		if (invert)
		{
			celebrationCloner.localPosition = new Vector3(0f, 100f, 0f);
			celebrationCloner.localRotation = new Quaternion(0f, 0f, 180f, 0f);
		}
		else
		{
			celebrationCloner.localPosition = new Vector3(0f, -20f, 0f);
			celebrationCloner.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		}
	}

	public float GetBragCelebrationAnimationCurveValue(float pos)
	{
		return _bragCelebrationAnimationCurve.Evaluate(pos);
	}

	private void Update()
	{
		if (shouldRotate)
		{
			backgroundRoot.Rotate(0f, 0f, 45f * Time.deltaTime);
		}
	}
}
