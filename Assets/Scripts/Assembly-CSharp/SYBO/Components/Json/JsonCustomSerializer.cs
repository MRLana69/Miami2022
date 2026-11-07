using System;

namespace SYBO.Components.Json
{
	public abstract class JsonCustomSerializer
	{
		public virtual object Deserialize(JsonDict json, JsonContext context)
		{
			throw new NotImplementedException();
		}

		public virtual JsonDict Serialize(object obj, JsonContext context)
		{
			throw new NotImplementedException();
		}

		public virtual object DeserializeList(JsonList jsonList, JsonContext context)
		{
			throw new NotImplementedException();
		}

		public virtual JsonList SerializeList(object obj, JsonContext context)
		{
			throw new NotImplementedException();
		}
	}
}
