using System.Collections;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UnityEngine;

public class BoardAnimation : MonoBehaviour
{
	[Header("DEBUG VALUES")]
	private bool _animateFloating = true;

	private Transform boardObject;

	private Vector3 startPosition;

	private Vector3 startRotation;

	[SerializeField]
	private float xPositiondifference = 2f;

	[SerializeField]
	private float yPositiondifference = 2f;

	[SerializeField]
	private float zPositiondifference;

	[SerializeField]
	private float xPositionSpeed = 1.5f;

	[SerializeField]
	private float yPositionSpeed = 1.5f;

	[SerializeField]
	private float zPositionSpeed;

	[SerializeField]
	private float xRotationDifference = 4f;

	[SerializeField]
	private float yRotationDifference = 4f;

	[SerializeField]
	private float zRotationDifference = 4f;

	[SerializeField]
	private float xRotationSpeed = 0.5f;

	[SerializeField]
	private float yRotationSpeed = 0.5f;

	[SerializeField]
	private float zRotationSpeed = 0.5f;

	private float delay;

	private bool set;

	private void Start()
	{
		Init(base.transform);
	}

	private void Init(Transform transform)
	{
		if (!set)
		{
			delay = Random.Range(0f, 0.5f);
			boardObject = transform;
			startPosition = transform.localPosition;
			startRotation = transform.localRotation.eulerAngles;
			CoroutineUtil.StartCoroutine(BoardFloating());
			set = true;
		}
	}

	private IEnumerator BoardFloating()
	{
		yield return new WaitForSeconds(delay);
		float currentxPositionSpeedDuration = 0f;
		float currentyPositionSpeedDuration = 0f;
		float currentzPositionSpeedDuration = 0f;
		float currentxRotationSpeedDuration = 0f;
		float currentyRotationSpeedDuration = 0f;
		float currentzRotationSpeedDuration = 0f;
		float xPositionTimeMultiplier = 1f;
		float yPositionTimeMultiplier = 1f;
		float zPositionTimeMultiplier = 1f;
		float xRotationTimeMultiplier = 1f;
		float yRotationTimeMultiplier = 1f;
		float zRotationTimeMultiplier = 1f;
		float xPositionPositionModifer = 1f;
		float yPositionPositionModifer = 1f;
		float zPositionPositionModifer = 1f;
		float xRotationPositionModifer = 1f;
		float yRotationPositionModifer = 1f;
		float zRotationPositionModifer = 1f;
		while (_animateFloating)
		{
			if (currentxPositionSpeedDuration > 1f)
			{
				xPositionTimeMultiplier = -1f;
			}
			else if (currentxPositionSpeedDuration < 0f)
			{
				xPositionTimeMultiplier = 1f;
				xPositionPositionModifer *= -1f;
			}
			if (currentyPositionSpeedDuration > 1f)
			{
				yPositionTimeMultiplier = -1f;
			}
			else if (currentyPositionSpeedDuration < 0f)
			{
				yPositionTimeMultiplier = 1f;
				yPositionPositionModifer *= -1f;
			}
			if (currentzPositionSpeedDuration > 1f)
			{
				zPositionTimeMultiplier = -1f;
			}
			else if (currentzPositionSpeedDuration < 0f)
			{
				zPositionTimeMultiplier = 1f;
				zPositionPositionModifer *= -1f;
			}
			currentxPositionSpeedDuration = currentxPositionSpeedDuration.Clamp(0f, 1f);
			currentyPositionSpeedDuration = currentyPositionSpeedDuration.Clamp(0f, 1f);
			currentzPositionSpeedDuration = currentzPositionSpeedDuration.Clamp(0f, 1f);
			currentxPositionSpeedDuration += Time.unscaledDeltaTime * xPositionTimeMultiplier * xPositionSpeed;
			currentyPositionSpeedDuration += Time.unscaledDeltaTime * yPositionTimeMultiplier * yPositionSpeed;
			currentzPositionSpeedDuration += Time.unscaledDeltaTime * zPositionTimeMultiplier * zPositionSpeed;
			Vector3 currentPosition = Vector3.zero;
			currentPosition.x = xPositiondifference * Easings.CubicEaseOut(currentxPositionSpeedDuration) * xPositionPositionModifer;
			currentPosition.y = yPositiondifference * Easings.CubicEaseOut(currentyPositionSpeedDuration) * yPositionPositionModifer;
			currentPosition.z = zPositiondifference * Easings.CubicEaseOut(currentzPositionSpeedDuration) * zPositionPositionModifer;
			boardObject.localPosition = startPosition + currentPosition;
			if (currentxRotationSpeedDuration >= 1f)
			{
				xRotationTimeMultiplier = -1f;
			}
			else if (currentxRotationSpeedDuration <= 0f)
			{
				xRotationTimeMultiplier = 1f;
				xRotationPositionModifer *= -1f;
			}
			if (currentyRotationSpeedDuration >= 1f)
			{
				yRotationTimeMultiplier = -1f;
			}
			else if (currentyRotationSpeedDuration <= 0f)
			{
				yRotationTimeMultiplier = 1f;
				yRotationPositionModifer *= -1f;
			}
			if (currentzRotationSpeedDuration >= 1f)
			{
				zRotationTimeMultiplier = -1f;
			}
			else if (currentzRotationSpeedDuration <= 0f)
			{
				zRotationTimeMultiplier = 1f;
				zRotationPositionModifer *= -1f;
			}
			currentxRotationSpeedDuration = currentxRotationSpeedDuration.Clamp(0f, 1f);
			currentyRotationSpeedDuration = currentyRotationSpeedDuration.Clamp(0f, 1f);
			currentzRotationSpeedDuration = currentzRotationSpeedDuration.Clamp(0f, 1f);
			currentxRotationSpeedDuration += Time.unscaledDeltaTime * xRotationTimeMultiplier * xRotationSpeed;
			currentyRotationSpeedDuration += Time.unscaledDeltaTime * yRotationTimeMultiplier * yRotationSpeed;
			currentzRotationSpeedDuration += Time.unscaledDeltaTime * zRotationTimeMultiplier * zRotationSpeed;
			Vector3 newRotation = Vector3.zero;
			newRotation.x = xRotationDifference * Easings.CubicEaseOut(currentxRotationSpeedDuration) * xRotationPositionModifer;
			newRotation.y = yRotationDifference * Easings.CubicEaseOut(currentyRotationSpeedDuration) * yRotationPositionModifer;
			newRotation.z = zRotationDifference * Easings.CubicEaseOut(currentzRotationSpeedDuration) * zRotationPositionModifer;
			boardObject.rotation = Quaternion.Euler(startRotation + newRotation);
			yield return null;
		}
	}
}
