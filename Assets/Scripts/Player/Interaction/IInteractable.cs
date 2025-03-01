using UnityEngine;

public interface IInteractable
{
    void Interact();
    string GetInteractText();
    Transform GetTransform();
}
