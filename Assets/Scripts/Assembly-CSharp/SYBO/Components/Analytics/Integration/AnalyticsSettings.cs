using System;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	[Serializable]
	public class AnalyticsSettings
	{
		private const bool DebugDefaultEnabled = false;

		private const string DebugDefaultBaseURL = "https://subwaysurf.dev.bladesofbrim.com";

		private const string DebugDefaultApplicationID = "subwaysurf";

		private const string DebugDefaultUserAgentString = "subwaysurf/01.69.00";

		private const float DebugDefaultSamplingThreshold = 1f;

		private const float MaxLiveSamplingThreshold = 0.1f;

		public const bool DefaultEnabled = false;

		public const bool DefaultFlushEnabled = true;

		public const string DefaultBaseURL = null;

		public const float DefaultSamplingThreshold = 0f;

		public const int DefaultMaxGamesWithoutFlush = 10;

		public const int DefaultGamesInSessionBeforeFirstFlush = 1;

		public const int UndefinedMaxGamesWithoutFlush = -1;

		public const int UndefinedGamesInSessionBeforeFirstFlush = -1;

		public const bool DefaultOnlyValidatedPurchases = true;

		private const string DefaultApplicationID = "subwaysurf";

		private const string DefaultUserAgentString = null;

		private const string DefaultApiVersion = "4.0";

		private const float DefaultWebRequestTimeoutSeconds = 60f;

		private const int DefaultSessionTimeoutSeconds = 1800;

		private const int DefaultChunkSizeThresholdKB = 512;

		private const int DefaultChunkCountLimit = 4;

		private const int DefaultStorageLimitKB = 2048;

		private readonly byte[] DefaultKey = new byte[30]
		{
			98, 186, 120, 38, 103, 4, 244, 246, 111, 205,
			80, 35, 152, 252, 173, 182, 117, 168, 124, 142,
			111, 72, 156, 151, 96, 204, 194, 147, 10, 46
		};

		private const string DefaultFilename = "analytics.log";

		[Header("Dynamic Config - retrieved from Online Settings")]
		[Tooltip("Determines if analytics will initialize and create user account. Does not control posting and flushing.")]
		[SerializeField]
		public bool Enabled;

		[Tooltip("Determines if analytics will flush (send) the data.")]
		[SerializeField]
		public bool FlushEnabled = true;

		[Tooltip("BaseURL + /(ApplicationID)/analytics")]
		public string BaseURL;

		[Tooltip("Sampling threshold (0-1)")]
		[SerializeField]
		[Range(0f, 1f)]
		private float _samplingThreshold;

		[Tooltip("-1 means no flush based on game count")]
		[Range(-1f, 100f)]
		public int MaxGamesWithoutFlush = 10;

		[Tooltip("-1 means session game count doesn't cause first flush")]
		[Range(-1f, 100f)]
		public int GamesInSessionBeforeFirstFlush = 1;

		[Tooltip("Requires purchase to have been validated by backend (if used for given platform)")]
		public bool OnlyValidatedPurchases = true;

		[Header("Static Config")]
		[Tooltip("Used for the url")]
		public string ApplicationID = "subwaysurf";

		public string ApiVersion = "4.0";

		public float WebRequestTimeoutSeconds = 60f;

		public int SessionTimeoutSeconds = 1800;

		public int ChunkSizeThresholdKB = 512;

		public int StorageLimitKB = 2048;

		public byte[] Key;

		[Header("Runtime Generated")]
		public string UserAgentString;

		public float SamplingThreshold
		{
			get
			{
				return _samplingThreshold;
			}
			set
			{
				_samplingThreshold = Mathf.Min(value, 0.1f);
			}
		}

		public void SetDefaults()
		{
			Enabled = false;
			FlushEnabled = true;
			BaseURL = null;
			ApplicationID = "subwaysurf";
			UserAgentString = null;
			SamplingThreshold = 0f;
			SetCommonDefaults();
		}

		public void SetDebugDefaults()
		{
			Enabled = false;
			FlushEnabled = true;
			BaseURL = "https://subwaysurf.dev.bladesofbrim.com";
			ApplicationID = "subwaysurf";
			UserAgentString = "subwaysurf/01.69.00";
			SamplingThreshold = 1f;
			SetCommonDefaults();
		}

		private void SetCommonDefaults()
		{
			ApiVersion = "4.0";
			WebRequestTimeoutSeconds = 60f;
			SessionTimeoutSeconds = 1800;
			ChunkSizeThresholdKB = 512;
			StorageLimitKB = 2048;
			Key = new byte[DefaultKey.Length];
			DefaultKey.CopyTo(Key, 0);
			MaxGamesWithoutFlush = 10;
			GamesInSessionBeforeFirstFlush = 1;
			OnlyValidatedPurchases = true;
		}

		public void CopyFrom(AnalyticsSettings other)
		{
			Enabled = other.Enabled;
			FlushEnabled = other.FlushEnabled;
			BaseURL = other.BaseURL;
			ApplicationID = other.ApplicationID;
			UserAgentString = other.UserAgentString;
			SamplingThreshold = other.SamplingThreshold;
			MaxGamesWithoutFlush = other.MaxGamesWithoutFlush;
			GamesInSessionBeforeFirstFlush = other.GamesInSessionBeforeFirstFlush;
			OnlyValidatedPurchases = other.OnlyValidatedPurchases;
			WebRequestTimeoutSeconds = other.WebRequestTimeoutSeconds;
			SessionTimeoutSeconds = other.SessionTimeoutSeconds;
			ChunkSizeThresholdKB = other.ChunkSizeThresholdKB;
			StorageLimitKB = other.StorageLimitKB;
			Key = other.Key;
		}
	}
}
