using System;
using UnityEngine;

namespace UI.RenderTexture
{
	[Serializable]
	public class RenderTextureObject
	{
		[Tooltip("Name of this list item (just used for the sake of clear overview when setting up the RenderTexture in the inspector)")]
		public string name;

		[Tooltip("Reference to the prefab to instantiate for this object")]
		public GameObject prefab;

		[Tooltip("Desired position of the object, relative to the camera")]
		public Vector3 position;

		[Tooltip("Desired rotation of the object, relative to the camera")]
		public Vector3 rotation;

		[Tooltip("Desired scale of the object")]
		public Vector3 scale = Vector3.one;

		public AnimationClip animationClip;

		[HideInInspector]
		public float animationDelay;
	}
}
