using UnityEngine;

public abstract class ATeleboardMenuEffect : MonoBehaviour
{
	protected Transform _rootTransform;

	protected bool _boardOnly;

	public virtual void Initialize(Transform root, bool boardOnly = false)
	{
		_rootTransform = root;
		_boardOnly = boardOnly;
	}

	public abstract void StartTeleboardEffect(float duration, CharacterModel characterModel);

	public abstract void EndTeleboardEffect(float duration, CharacterModel characterModel);
}
