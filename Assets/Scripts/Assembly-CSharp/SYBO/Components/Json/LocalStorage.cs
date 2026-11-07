using System;
using System.IO;
using SYBO.Components.Json.Internal;
using SYBO.Components.Utilities;

namespace SYBO.Components.Json
{
	public class LocalStorage<T>
	{
		protected JsonContext _jsonContext;

		protected string _localUrl = string.Empty;

		private IStringStorage _stringStorage;

		private T _object;

		public T Object
		{
			get
			{
				return _object;
			}
		}

		public string LocalUrl
		{
			get
			{
				return _localUrl;
			}
		}

		public bool CanLoad
		{
			get
			{
				return _stringStorage.CanRead;
			}
		}

		public LocalStorage(T obj, string localUrl, JsonContext jsonContext = null, IStringStorage stringStorage = null)
		{
			_object = obj;
			_localUrl = FileUtility.ConvertToPathWithForwardSlashes(localUrl);
			if (jsonContext != null)
			{
				_jsonContext = jsonContext;
			}
			else
			{
				_jsonContext = new JsonContext();
			}
			if (stringStorage != null)
			{
				_stringStorage = stringStorage;
			}
			else
			{
				_stringStorage = new FileStringStorage(_localUrl);
			}
		}

		public void DeleteLocalFile()
		{
			File.Delete(_localUrl);
		}

		public bool Read()
		{
			string text = _stringStorage.Read();
			if (!string.IsNullOrEmpty(text))
			{
				T val = JsonApi.Deserialize<T>(text, _jsonContext.Duplicate());
				if (val != null)
				{
					_object = val;
					return true;
				}
				throw new Exception(string.Format("obj.IsValid() = false. Stored file may be broken for obj={0} at url={1}.", val, _localUrl));
			}
			return false;
		}

		public void ReadAndSerializeOnto(T targetObject)
		{
			string text = _stringStorage.Read();
			if (!string.IsNullOrEmpty(text))
			{
				JsonReflectionParser.DeserializeOnto(targetObject, JsonDict.Deserialize(text), _jsonContext.Duplicate());
			}
		}

		public bool Write()
		{
			if (_object != null)
			{
				return _stringStorage.Overwrite(JsonApi.Serialize(_object, _jsonContext.Duplicate()));
			}
			throw new Exception(string.Format("_object is null. Do nullcheck before calling {0}.Write().", typeof(T)));
		}

		public bool Clear()
		{
			if (CanLoad)
			{
				DeleteLocalFile();
				return true;
			}
			return false;
		}
	}
}
