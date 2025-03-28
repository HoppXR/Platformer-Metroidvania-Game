using Managers;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

namespace Player.Interaction
{
    public class SporkInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionText;
        [SerializeField] private GameObject TextWindow;
        [SerializeField] private TextMeshProUGUI tutorialText;
        
        string[] dialogue = { "Hey kid, you okay?", "test 2", "Test 3, wow this works! " };
        int dialogueTracker;
        [SerializeField] private int dialogueLoopPoint;
        
        public void Interact()
        {
            
            if (dialogueTracker <= 2)
            {
                tutorialText.text = dialogue[dialogueTracker];
                TextWindow.SetActive(true);
                dialogueTracker += 1;
            }

            else if (dialogueTracker > 2) {
            
                TextWindow.SetActive(false);
                dialogueTracker = 0;
            }
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
}
