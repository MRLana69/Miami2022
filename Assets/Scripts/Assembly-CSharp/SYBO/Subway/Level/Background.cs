using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class Background
	{
		public float SkylineLeftLimit = -350f;

		public float SkylineRightLimit = 750f;

		public Shader LayerCommonMaterialShader;

		public float LayerSpacing = 5f;

		public BackgroundLayer[] Layers = new BackgroundLayer[0];

		[Range(-500f, 2000f)]
		public float GradientTop = 1f;

		[Range(-500f, 500f)]
		public float GradientBottom = 0.5f;

		public Color GradientTopColor;

		public Color GradientBottomColor;

		public BackgroundCulling[] Culling = new BackgroundCulling[0];

		public Color BackgroundCullingColor = Color.white;
	}
}
