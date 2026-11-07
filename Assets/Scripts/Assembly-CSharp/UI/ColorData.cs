using System.IO;
using NaughtyAttributes;
using UnityEngine;

namespace UI
{
	public class ColorData : ScriptableObject
	{
		private static ColorData _instance;

		[BoxGroup("Blue Vibrant")]
		public Color cloud;

		[BoxGroup("Blue Vibrant")]
		public Color sky;

		[BoxGroup("Blue Vibrant")]
		public Color smurf;

		[BoxGroup("Blue Vibrant")]
		public Color push;

		[BoxGroup("Blue Vibrant")]
		public Color boston;

		[BoxGroup("Blue Vibrant")]
		public Color peacock;

		[BoxGroup("Blue Vibrant")]
		public Color ocean;

		[BoxGroup("Blue Vibrant")]
		public Color royale;

		[BoxGroup("Blue Vibrant")]
		public Color midnight;

		[BoxGroup("Teal Vibrant")]
		public Color scooter;

		[BoxGroup("Teal Vibrant")]
		public Color bulbasaur;

		[BoxGroup("Teal Vibrant")]
		public Color turquoise;

		[BoxGroup("Teal Vibrant")]
		public Color teal;

		[BoxGroup("Teal Vibrant")]
		public Color pit;

		[BoxGroup("Green Vibrant")]
		public Color grape;

		[BoxGroup("Green Vibrant")]
		public Color lime;

		[BoxGroup("Green Vibrant")]
		public Color grass;

		[BoxGroup("Green Vibrant")]
		public Color avocado;

		[BoxGroup("Yellow Vibrant")]
		public Color glow;

		[BoxGroup("Yellow Vibrant")]
		public Color sun;

		[BoxGroup("Yellow Vibrant")]
		public Color yolk;

		[BoxGroup("Yellow Vibrant")]
		public Color orange;

		[BoxGroup("Yellow Vibrant")]
		public Color dutch;

		[BoxGroup("Yellow Vibrant")]
		public Color fox;

		[BoxGroup("Yellow Vibrant")]
		public Color ruby;

		[BoxGroup("Blue Dusty")]
		public Color pigeon;

		[BoxGroup("Blue Dusty")]
		public Color tuna;

		[BoxGroup("Blue Dusty")]
		public Color storm;

		[BoxGroup("Blue Dusty")]
		public Color oxford;

		[BoxGroup("Green Dusty")]
		public Color mint;

		[BoxGroup("Green Dusty")]
		public Color spring;

		[BoxGroup("Green Dusty")]
		public Color christi;

		[BoxGroup("Green Dusty")]
		public Color verdun;

		[BoxGroup("Yellow Dusty")]
		public Color sand;

		[BoxGroup("Yellow Dusty")]
		public Color desert;

		[BoxGroup("Red Dusty")]
		public Color pink;

		[BoxGroup("Red Dusty")]
		public Color peach;

		[BoxGroup("Red Dusty")]
		public Color safran;

		[BoxGroup("Red Dusty")]
		public Color crimson;

		[BoxGroup("Red Dusty")]
		public Color bordeaux;

		[BoxGroup("Red Dusty")]
		public Color mahogany;

		[BoxGroup("Purple Dusty")]
		public Color sliggo;

		[BoxGroup("Purple Dusty")]
		public Color lavender;

		[BoxGroup("Purple Dusty")]
		public Color amethyst;

		[BoxGroup("Grayscale")]
		public Color white;

		[BoxGroup("Grayscale")]
		public Color lightShadow;

		[BoxGroup("Grayscale")]
		public Color stroking;

		[BoxGroup("Grayscale")]
		public Color chrome;

		[BoxGroup("Grayscale")]
		public Color slate;

		[BoxGroup("Grayscale")]
		public Color iron;

		[BoxGroup("Grayscale")]
		public Color rock;

		[BoxGroup("Grayscale")]
		public Color smoke;

		[BoxGroup("Grayscale")]
		public Color blueBlack;

		[BoxGroup("Grayscale")]
		public Color shadow;

		[Header("BG Effects")]
		public Color stripeColor;

		public Color glowColor;

		public Color starParticlesColor;

		[Header("Word hunt Letters specific")]
		[ColorUsage(false)]
		[SerializeField]
		public Color dailyLetterColorActiveShadow;

		[ColorUsage(false)]
		[SerializeField]
		public Color dailyLetterColorInactive;

		[ColorUsage(false)]
		[SerializeField]
		public Color dailyLetterColorInactiveShadow;

		[Header("Word Hunt")]
		public Color wordHuntCurrentDayGradientTop;

		public Color wordHuntCurrentDayGradientBottom;

		public Color wordHuntBubbleColor;

		public Color wordHuntBubbleShadowColor;

		[Header("My Tour footer")]
		public Color myTourFooterColorMain;

		public Color myTourFooterColorLight;

		public Color myTourFooterColorSelected;

		private string _swatchPath = "/Editor/ColorDataSwatch.colors";

		public static ColorData instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<ColorData>("Info/ColorData");
				}
				return _instance;
			}
		}

		[Button(null)]
		private void Save()
		{
		}

		private void SaveAsSwatch()
		{
			string fileContent = "%YAML 1.1\r\n%TAG !u! tag:unity3d.com,2011:\r\n--- !u!114 &1\r\nMonoBehaviour:\r\n  m_ObjectHideFlags: 52\r\n  m_PrefabParentObject: { fileID: 0}\r\n  m_PrefabInternal: { fileID: 0}\r\n  m_GameObject: { fileID: 0}\r\n  m_Enabled: 1\r\n  m_EditorHideFlags: 1\r\n  m_Script: { fileID: 12323, guid: 0000000000000000e000000000000000, type: 0}\r\n  m_Name: ColorDataSwatch\r\n  m_EditorClassIdentifier:\r\n  m_Presets:\r\n";
			AddColorToSwatch(ref fileContent, "cloud", cloud);
			AddColorToSwatch(ref fileContent, "sky", sky);
			AddColorToSwatch(ref fileContent, "smurf", smurf);
			AddColorToSwatch(ref fileContent, "push", push);
			AddColorToSwatch(ref fileContent, "boston", boston);
			AddColorToSwatch(ref fileContent, "peacock", peacock);
			AddColorToSwatch(ref fileContent, "ocean", ocean);
			AddColorToSwatch(ref fileContent, "royale", royale);
			AddColorToSwatch(ref fileContent, "midnight", midnight);
			AddColorToSwatch(ref fileContent, "scooter", scooter);
			AddColorToSwatch(ref fileContent, "bulbasaur", bulbasaur);
			AddColorToSwatch(ref fileContent, "turquoise", turquoise);
			AddColorToSwatch(ref fileContent, "teal", teal);
			AddColorToSwatch(ref fileContent, "pit", pit);
			AddColorToSwatch(ref fileContent, "grape", grape);
			AddColorToSwatch(ref fileContent, "lime", lime);
			AddColorToSwatch(ref fileContent, "grass", grass);
			AddColorToSwatch(ref fileContent, "avocado", avocado);
			AddColorToSwatch(ref fileContent, "glow", glow);
			AddColorToSwatch(ref fileContent, "sun", sun);
			AddColorToSwatch(ref fileContent, "yolk", yolk);
			AddColorToSwatch(ref fileContent, "orange", orange);
			AddColorToSwatch(ref fileContent, "dutch", dutch);
			AddColorToSwatch(ref fileContent, "fox", fox);
			AddColorToSwatch(ref fileContent, "ruby", ruby);
			AddColorToSwatch(ref fileContent, "pigeon", pigeon);
			AddColorToSwatch(ref fileContent, "tuna", tuna);
			AddColorToSwatch(ref fileContent, "storm", storm);
			AddColorToSwatch(ref fileContent, "oxford", oxford);
			AddColorToSwatch(ref fileContent, "mint", mint);
			AddColorToSwatch(ref fileContent, "spring", spring);
			AddColorToSwatch(ref fileContent, "christi", christi);
			AddColorToSwatch(ref fileContent, "verdun", verdun);
			AddColorToSwatch(ref fileContent, "sand", sand);
			AddColorToSwatch(ref fileContent, "desert", desert);
			AddColorToSwatch(ref fileContent, "pink", pink);
			AddColorToSwatch(ref fileContent, "peach", peach);
			AddColorToSwatch(ref fileContent, "safran", safran);
			AddColorToSwatch(ref fileContent, "crimson", crimson);
			AddColorToSwatch(ref fileContent, "bordeaux", bordeaux);
			AddColorToSwatch(ref fileContent, "mahogany", mahogany);
			AddColorToSwatch(ref fileContent, "sliggo", mahogany);
			AddColorToSwatch(ref fileContent, "lavender", lavender);
			AddColorToSwatch(ref fileContent, "amethyst", amethyst);
			AddColorToSwatch(ref fileContent, "white", white);
			AddColorToSwatch(ref fileContent, "lightShadow", lightShadow);
			AddColorToSwatch(ref fileContent, "stroking", stroking);
			AddColorToSwatch(ref fileContent, "chrome", chrome);
			AddColorToSwatch(ref fileContent, "slate", slate);
			AddColorToSwatch(ref fileContent, "iron", iron);
			AddColorToSwatch(ref fileContent, "rock", rock);
			AddColorToSwatch(ref fileContent, "smoke", smoke);
			AddColorToSwatch(ref fileContent, "blueBlack", blueBlack);
			AddColorToSwatch(ref fileContent, "shadow", shadow);
			Debug.Log("Created at path: " + Application.dataPath + _swatchPath);
			using (StreamWriter streamWriter = new StreamWriter(File.Create(Application.dataPath + _swatchPath)))
			{
				streamWriter.Write(fileContent);
			}
		}

		private void AddColorToSwatch(ref string fileContent, string colorName, Color color)
		{
			string format = "  - m_Name: {0}\r\n    m_Color: {{r: {1}, g: {2}, b: {3}, a: {4}}}\r\n";
			fileContent += string.Format(format, colorName, color.r, color.g, color.b, color.a);
		}
	}
}
