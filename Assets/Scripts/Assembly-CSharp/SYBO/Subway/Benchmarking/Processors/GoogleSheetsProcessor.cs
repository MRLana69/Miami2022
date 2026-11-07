using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Common.Benchmarking;
using SYBO.Subway.Themes;
using UnityEngine;
using UnityEngine.Networking;

namespace SYBO.Subway.Benchmarking.Processors
{
	public class GoogleSheetsProcessor : IBenchmarkProcessor
	{
		private class ParameterContainer
		{
			public string Key { get; private set; }

			public string Value { get; private set; }

			public ParameterContainer(string key, string value)
			{
				Key = key;
				Value = value;
			}
		}

		private class GoogleSheetsProcessorUpdater : MonoBehaviour
		{
		}

		public static Action OnUploadCompleted;

		private const string ValueParameterKey = "val";

		private const string SpreadsheetIdParameterKey = "ssid";

		private const string WorksheetNameParameterKey = "sheet";

		private const string TemplateParameterKey = "template";

		private const string ActionParameterKey = "action";

		private const string GroupByParameterKey = "groupby";

		private string _spreadsheetId = string.Empty;

		private string _googleScriptUrl = string.Empty;

		private const string UploadSuccessResponse = "Upload Successful";

		private const string ActionConnectTestParameter = "ConnectionTest";

		private const string ConnectionTestSuccessResponse = "Connection Established";

		private string _worksheetName = string.Empty;

		private string _template = string.Empty;

		private GoogleSheetsProcessorUpdater _processorUpdater;

		private bool _connectionSuccessful;

		public char SeparatorCharacter
		{
			get
			{
				return ';';
			}
		}

		public GoogleSheetsProcessor(string worksheetNamePostfix = "", string overrideSheetId = "1HIlWcwxXV0dVjsf0gLpgFE0B0bei1vdn2xJNTAIxFv4", string overrideScriptUrl = "https://script.google.com/macros/s/AKfycbzysbyZuNXvOTJh5llqjN4N2dP4PAXusuwfaHjvTD3ESkx5Sas/exec")
		{
			_worksheetName = DeviceUtility.GetBundleVersion() + "_" + worksheetNamePostfix;
			_spreadsheetId = overrideSheetId;
			_googleScriptUrl = overrideScriptUrl;
			_template = ((!worksheetNamePostfix.Contains("memory")) ? "template" : "template_memory");
			_processorUpdater = new GameObject
			{
				hideFlags = HideFlags.HideAndDontSave
			}.AddComponent<GoogleSheetsProcessorUpdater>();
			_connectionSuccessful = false;
			_processorUpdater.StartCoroutine(TestGoogleConnection());
		}

		public void Process(IBenchmark benchmark)
		{
			if (_connectionSuccessful)
			{
				_processorUpdater.StartCoroutine(UploadReportToGoogleSheets(benchmark));
				return;
			}
			UnityEngine.Object.Destroy(_processorUpdater.gameObject);
			_processorUpdater = null;
		}

		private IEnumerator UploadReportToGoogleSheets(IBenchmark benchmark)
		{
			List<ParameterContainer> parameters = new List<ParameterContainer>
			{
				new ParameterContainer("ssid", _spreadsheetId),
				new ParameterContainer("sheet", _worksheetName),
				new ParameterContainer("groupby", SystemInfo.deviceModel),
				new ParameterContainer("val", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")),
				new ParameterContainer("val", SystemInfo.deviceModel),
				new ParameterContainer("val", SystemInfo.deviceName),
				new ParameterContainer("val", DeviceInfo.Instance.performanceLevel.ToString()),
				new ParameterContainer("val", DeviceUtility.GetBundleVersion()),
				new ParameterContainer("val", ThemeManager.Instance.Theme.name),
				new ParameterContainer("val", SystemInfo.operatingSystem),
				new ParameterContainer("val", (benchmark.ElapsedDuration / 60f).ToString("0.00"))
			};
			IBenchmarkMetric[] metrics = benchmark.Metrics;
			List<string> serializedData = Serialize(metrics);
			for (int i = 0; i < serializedData.Count; i++)
			{
				parameters.Add(new ParameterContainer("val", serializedData[i]));
			}
			UnityWebRequest request = UnityWebRequest.Post(_googleScriptUrl, ConstructPostParametersForm(parameters));
			yield return request.Send();
			if (request.isNetworkError)
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("UPLOAD_FAILED_MESSAGE"));
			}
			else
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("UPLOAD_SUCCESS_MESSAGE"));
			}
			if (OnUploadCompleted != null)
			{
				OnUploadCompleted();
			}
			UnityEngine.Object.Destroy(_processorUpdater.gameObject);
			_processorUpdater = null;
		}

		private List<string> Serialize(IBenchmarkMetric[] metrics)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < metrics.Length; i++)
			{
				list.AddRange(SerializeMetric(metrics[i]));
			}
			return list;
		}

		private List<string> SerializeMetric(IBenchmarkMetric metric)
		{
			List<string> list = new List<string>();
			List<string> list2 = metric.Serialize();
			if (metric is FrameTimeAboveMetric)
			{
				list2.Add("=COUNTIF(SPLIT(\"" + list2[0] + "\", \";\"), \"> 0\")");
			}
			bool flag = !(metric is FPSMetric) && !(metric is DeltaFrameTimeMetric);
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i]);
			}
			list.Add("=SPARKLINE(SPLIT(\"" + list2[0] + "\", \"" + SeparatorCharacter + "\")" + ((!flag) ? ")" : ", {\"charttype\",\"column\"})"));
			if (!(metric is InstantiatedObjectsMetric))
			{
				list.Add(list[0]);
				list.RemoveAt(0);
			}
			return list;
		}

		private IEnumerator TestGoogleConnection()
		{
			string url = ConstructGetUriWithParameters(parameters: new List<ParameterContainer>
			{
				new ParameterContainer("ssid", _spreadsheetId),
				new ParameterContainer("sheet", _worksheetName),
				new ParameterContainer("template", _template),
				new ParameterContainer("action", "ConnectionTest")
			}, url: _googleScriptUrl);
			UnityWebRequest request = UnityWebRequest.Get(url);
			yield return request.Send();
			if (request.isNetworkError)
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("CONNECTION_TEST_FAILED_ERROR_MESSAGE"));
				_connectionSuccessful = false;
			}
			else if (request.downloadHandler.text.Equals("Connection Established"))
			{
				_connectionSuccessful = true;
			}
			else
			{
				SDebug.LogError("Error connecting to Google Sheets: " + request.downloadHandler.text);
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("CONNECTION_TEST_FAILED_ERROR_MESSAGE"));
				_connectionSuccessful = false;
			}
		}

		private string ConstructGetUriWithParameters(string url, List<ParameterContainer> parameters)
		{
			if (parameters.Count == 0)
			{
				return url;
			}
			StringBuilder stringBuilder = new StringBuilder(url);
			stringBuilder.Append("?");
			int count = parameters.Count;
			for (int i = 0; i < count; i++)
			{
				ParameterContainer parameterContainer = parameters[i];
				stringBuilder.Append(parameterContainer.Key);
				stringBuilder.Append("=");
				stringBuilder.Append(parameterContainer.Value);
				if (i < count - 1)
				{
					stringBuilder.Append("&");
				}
			}
			return stringBuilder.ToString();
		}

		private WWWForm ConstructPostParametersForm(List<ParameterContainer> parameters)
		{
			WWWForm wWWForm = new WWWForm();
			foreach (ParameterContainer parameter in parameters)
			{
				wWWForm.AddField(parameter.Key, parameter.Value);
			}
			return wWWForm;
		}
	}
}
