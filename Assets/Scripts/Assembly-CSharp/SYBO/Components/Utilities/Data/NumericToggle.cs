using System;
using UnityEngine;

namespace SYBO.Components.Utilities.Data
{
	[Serializable]
	public abstract class NumericToggle
	{
		public const string DefaultMessage = "(Default)";

		public const int UnsetInt = -1;

		[SerializeField]
		private bool _isEnabled;

		[HideInInspector]
		private bool _isEnabledOriginal;

		[HideInInspector]
		[SerializeField]
		private string _inspectorMessage = string.Empty;

		[HideInInspector]
		[SerializeField]
		private string _inspectorMessageOriginal;

		[HideInInspector]
		[SerializeField]
		private bool _showInspectorMessage;

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
			}
		}

		public bool IsEnabledOriginal
		{
			get
			{
				return _isEnabledOriginal;
			}
			set
			{
				_isEnabledOriginal = value;
			}
		}

		public string InspectorMessage
		{
			get
			{
				return _inspectorMessage;
			}
			set
			{
				_inspectorMessage = value;
			}
		}

		public string InspectorMessageOriginal
		{
			get
			{
				return _inspectorMessageOriginal;
			}
			set
			{
				_inspectorMessageOriginal = value;
			}
		}

		public bool ShowInspectorMessage
		{
			get
			{
				return _showInspectorMessage;
			}
			set
			{
				_showInspectorMessage = value;
			}
		}

		protected abstract bool IsOriginalValue();

		public virtual void OnChanged()
		{
			ShowInspectorMessage = IsOriginalValue();
		}
	}
}
