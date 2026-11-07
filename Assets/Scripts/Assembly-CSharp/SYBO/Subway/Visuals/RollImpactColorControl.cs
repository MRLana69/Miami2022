using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	public class RollImpactColorControl : MonoBehaviour
	{
		[SerializeField]
		private Renderer _rollImpactRockRenderer;

		[SerializeField]
		private ParticleSystem _rollImpactDustParticles;

		private void Start()
		{
			Theme theme = ThemeManager.Instance.Theme;
			_rollImpactRockRenderer.material.SetColor("_Color", theme.RollImpactRockColor);
			ParticleSystem.MinMaxGradient startColor = new ParticleSystem.MinMaxGradient(theme.RollImpactDustColorMin, theme.RollImpactDustColorMax);
			ParticleSystem.MainModule main = _rollImpactDustParticles.main;
			main.startColor = startColor;
		}
	}
}
