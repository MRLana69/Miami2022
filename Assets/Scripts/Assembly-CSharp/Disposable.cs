using UnityEngine;

public class Disposable<T> where T : Object
{
	public T ReferencedObject { get; private set; }

	public Disposable(T obj)
	{
		ReferencedObject = obj;
	}
}
