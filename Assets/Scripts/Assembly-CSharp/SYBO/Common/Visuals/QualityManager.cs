using System.Collections.Generic;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class QualityManager : SingletonMonoBehaviour<QualityManager>
	{
		private List<QualityFeature> _prewarmParticleSystems = new List<QualityFeature>();

		private IQualityPlatformSupport _platformSupport;

		protected override void SubclassInitialize()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				_platformSupport = new QualityPlatformSupportAndroid();
				break;
			case RuntimePlatform.IPhonePlayer:
				_platformSupport = new QualityPlatformSupportIOS();
				break;
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
				_platformSupport = new QualityPlatformSupportWSA();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsEditor:
				_platformSupport = new QualityPlatformSupportEditor();
				break;
			default:
				SDebug.LogError("Undefined platform quality settings for " + Application.platform);
				return;
			}
			_platformSupport.Initialize();
		}

		public bool IsSupported(QualityFeature feature)
		{
			if (feature == QualityFeature.None)
			{
				return true;
			}
			return _platformSupport.IsSupported(feature);
		}

		public bool PrewarmParticleSystems(QualityFeature feature)
		{
			return _prewarmParticleSystems.Contains(feature);
		}
	}
}
