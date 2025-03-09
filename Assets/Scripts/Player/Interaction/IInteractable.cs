using UnityEngine;

namespace Player.Interaction
{
    public interface IInteractable
    {
        void Interact();
        string GetInteractText();
        Transform GetTransform();
    }
}
