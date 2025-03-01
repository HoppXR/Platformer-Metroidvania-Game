using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText;
    [SerializeField] private int levelIndex;
    [SerializeField] private float levelTransitionTime;

    public void Interact()
    {
        GameManager.Instance.LoadLevelTimer(levelIndex, levelTransitionTime);
    }

    public string GetInteractText()
    {
        return interactionText;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
