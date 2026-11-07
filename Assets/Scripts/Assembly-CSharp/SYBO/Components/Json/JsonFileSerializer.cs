using System;
using SYBO.Components.Json.Internal;

namespace SYBO.Components.Json
{
	public class JsonFileSerializer
	{
		public delegate string GetFileTextDelegate(string filename);

		public delegate void WriteFileTextDelegate(string filename, string text);

		private GetFileTextDelegate _getFileText;

		private WriteFileTextDelegate _writeFileText;

		public JsonFileSerializer(GetFileTextDelegate getFileText, WriteFileTextDelegate writeFileText)
		{
			_getFileText = getFileText;
			_writeFileText = writeFileText;
		}

		public void SerializeToMultipleFiles(Type type, object obj, string filename, JsonContext context = null)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			SerializeToFile(type, obj, filename, context);
			while (context.JsonFileReferences.Count > 0)
			{
				JsonContext.SubtreeFileReference subtreeFileReference = context.JsonFileReferences[0];
				SerializeToFile(subtreeFileReference.Field.Field.FieldType, subtreeFileReference.Field.Field.GetValue(subtreeFileReference.SubObject), subtreeFileReference.Filename, context);
				context.JsonFileReferences.Remove(subtreeFileReference);
			}
		}

		public object DeserializeFromMultipleFiles(Type type, string filename, JsonContext context = null)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			object result = DeserializeFromFile(type, filename, context);
			while (context.JsonFileReferences.Count > 0)
			{
				JsonContext.SubtreeFileReference subtreeFileReference = context.JsonFileReferences[0];
				subtreeFileReference.Field.Field.SetValue(subtreeFileReference.SubObject, DeserializeFromFile(subtreeFileReference.Field.Field.FieldType, subtreeFileReference.Filename, context));
				context.JsonFileReferences.Remove(subtreeFileReference);
			}
			return result;
		}

		private void SerializeToFile(Type type, object obj, string filename, JsonContext context)
		{
			JsonDict jsonDict = JsonReflectionParser.ReflectionSerialize(type, obj, context);
			if (jsonDict == null)
			{
				jsonDict = new JsonDict();
			}
			string text = jsonDict.Serialize();
			_writeFileText(filename, text);
		}

		private object DeserializeFromFile(Type type, string filename, JsonContext context)
		{
			JsonDict jsonDict = null;
			string text = _getFileText(filename);
			if (text != null)
			{
				jsonDict = JsonDict.Deserialize(text);
			}
			if (jsonDict == null)
			{
				jsonDict = new JsonDict();
			}
			return JsonReflectionParser.ReflectionDeserialize(type, jsonDict, context);
		}
	}
}
