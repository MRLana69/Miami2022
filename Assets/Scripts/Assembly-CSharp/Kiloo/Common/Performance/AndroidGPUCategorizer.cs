using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace Kiloo.Common.Performance
{
	public static class AndroidGPUCategorizer
	{
		public delegate void UknownGPUHandler(string gpuName);

		private const string GPU_SCORES_PATH = "Kiloo/Performance/AndroidGPUScores";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static UknownGPUHandler _unknownGPUHandler__BackingField;

		private static Dictionary<string, int> _gpuScores = new Dictionary<string, int>();

		private static bool _isInitialized = false;

		public static int gpuScore { get; private set; }

		public static GPUCategory gpuCategory { get; private set; }

		public static string gpuInfo
		{
			get
			{
				return string.Concat(SystemInfo.graphicsDeviceName, " Category: ", gpuCategory, " Score: ", gpuScore);
			}
		}

		private static event UknownGPUHandler _unknownGPUHandler
		{
			add
			{
				UknownGPUHandler uknownGPUHandler = _unknownGPUHandler__BackingField;
				UknownGPUHandler uknownGPUHandler2;
				do
				{
					uknownGPUHandler2 = uknownGPUHandler;
					uknownGPUHandler = Interlocked.CompareExchange(ref _unknownGPUHandler__BackingField, (UknownGPUHandler)Delegate.Combine(uknownGPUHandler2, value), uknownGPUHandler);
				}
				while ((object)uknownGPUHandler != uknownGPUHandler2);
			}
			remove
			{
				UknownGPUHandler uknownGPUHandler = _unknownGPUHandler__BackingField;
				UknownGPUHandler uknownGPUHandler2;
				do
				{
					uknownGPUHandler2 = uknownGPUHandler;
					uknownGPUHandler = Interlocked.CompareExchange(ref _unknownGPUHandler__BackingField, (UknownGPUHandler)Delegate.Remove(uknownGPUHandler2, value), uknownGPUHandler);
				}
				while ((object)uknownGPUHandler != uknownGPUHandler2);
			}
		}

		public static bool IsSupportedOnPlatform()
		{
			return Application.platform == RuntimePlatform.Android;
		}

		public static void Initialize(CategoryThresholds thresholds, UknownGPUHandler unknownGPUHandler = null)
		{
			_gpuScores = LoadGPUScores();
			_unknownGPUHandler__BackingField = unknownGPUHandler;
			_isInitialized = true;
			CalculateGpuCaterogy(thresholds);
		}

		public static void CalculateGpuCaterogy(CategoryThresholds thresholds)
		{
			if (_isInitialized)
			{
				gpuCategory = GetCategory(_gpuScores, thresholds, _unknownGPUHandler__BackingField);
			}
		}

		private static Dictionary<string, int> LoadGPUScores()
		{
			_gpuScores = new Dictionary<string, int>();
			TextAsset textAsset = Resources.Load<TextAsset>("Kiloo/Performance/AndroidGPUScores");
			string[] array = textAsset.text.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string text2 = text.Trim('\n', ' ', '\r');
				if (text2.Length != 0 && text2[0] != '#')
				{
					string[] array2 = text.Split(' ');
					string key = array2[0];
					int value = int.Parse(array2[1]);
					_gpuScores[key] = value;
				}
			}
			return _gpuScores;
		}

		private static GPUCategory GetCategory(Dictionary<string, int> gpuToScore, CategoryThresholds thresholds, UknownGPUHandler unknownGPUHandler)
		{
			if (IsSupportedOnPlatform())
			{
				string trimmedGPUName = GetTrimmedGPUName();
				string text = string.Empty;
				foreach (KeyValuePair<string, int> item in gpuToScore)
				{
					if (trimmedGPUName.Contains(item.Key) && item.Key.Length > text.Length)
					{
						text = item.Key;
						gpuScore = item.Value;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					return GetCategoryFromScore(gpuScore, thresholds);
				}
				if (unknownGPUHandler != null)
				{
					unknownGPUHandler(SystemInfo.graphicsDeviceName);
				}
				return GPUCategory.Unknown;
			}
			return GPUCategory.Unknown;
		}

		private static string GetTrimmedGPUName()
		{
			string text = SystemInfo.graphicsDeviceName.ToLower();
			text = text.Replace("(tm)", string.Empty);
			Regex regex = new Regex("[^a-z0-9]");
			return regex.Replace(text, string.Empty);
		}

		private static GPUCategory GetCategoryFromScore(int score, CategoryThresholds thresholds)
		{
			if ((float)score < thresholds.high)
			{
				return GPUCategory.Low;
			}
			return GPUCategory.High;
		}
	}
}
