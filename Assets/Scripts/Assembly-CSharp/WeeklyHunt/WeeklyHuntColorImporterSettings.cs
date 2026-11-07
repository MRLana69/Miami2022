using UnityEngine;

namespace WeeklyHunt
{
	public class WeeklyHuntColorImporterSettings : ScriptableObject
	{
		public Texture2D salesTexture;

		public Texture2D huntTexture;

		public Vector2[] salesPixelSamplers = new Vector2[0];

		public Vector2[] huntPixelSamplers = new Vector2[0];

		public Color32[] miscColors = new Color32[0];

		public Color32[] salesColors = new Color32[0];

		public Color32[] huntColors = new Color32[0];
	}
}
