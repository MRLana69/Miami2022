using UnityEngine;

namespace UI.ModelCamera
{
	public class ModelScreenSetup
	{
		private const int NOT_SET_INT_VALUE = -1;

		public Vector3 localPosition { get; set; }

		public Vector3 localScale { get; set; }

		public Vector3 localRotation { get; set; }

		public int localScaleOnAllAxes { get; set; }

		public int modelGoLayer { get; set; }

		public Vector3 cameraLocalPosition { get; set; }

		public Vector3 cameraLocalScale { get; set; }

		public Vector3 cameraLocalRotation { get; set; }

		public ModelScreenSetup()
		{
			modelGoLayer = -1;
			localScaleOnAllAxes = -1;
		}

		public void SetupScreen(Transform modelTransform, Transform cameraTransform, int cameraLayer)
		{
			modelTransform.localPosition = localPosition;
			modelTransform.localRotation = Quaternion.Euler(localRotation);
			SetLocalScale(modelTransform);
			Utility.SetLayerRecursively(modelTransform, cameraLayer);
			cameraTransform.localPosition = cameraLocalPosition;
			cameraTransform.localRotation = Quaternion.Euler(cameraLocalRotation);
			cameraTransform.localScale = cameraLocalScale;
		}

		private void SetLocalScale(Transform modelTransform)
		{
			if (localScaleOnAllAxes > -1)
			{
				modelTransform.localScale = Vector3.one * localScaleOnAllAxes;
			}
			else
			{
				modelTransform.localScale = localScale;
			}
		}
	}
}
