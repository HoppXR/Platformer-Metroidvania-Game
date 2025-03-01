using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    
    [Header("Interaction")]
    [SerializeField] private float interactRange;

    private void Start()
    {
        input.InteractEvent += Interact;
    }
    
    private void Interact()
    {
        IInteractable interactable = GetInteractable();
        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    public IInteractable GetInteractable()
    {
        if (!this) return null;
        
        List<IInteractable> interactableList = new List<IInteractable>();
        Collider[] interactables = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider interactable in interactables)
        {
            if (interactable.TryGetComponent(out IInteractable interactableObject))
            {
                interactableList.Add(interactableObject);
            }
        }
        
        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) <
                    Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
                {
                    closestInteractable = interactable;
                }
            }
        }
        return closestInteractable;
    }
}
