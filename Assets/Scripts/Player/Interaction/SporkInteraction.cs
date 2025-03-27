using Managers;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

namespace Player.Interaction
{
    public class SporkInteraction : MonoBehaviour//, IInteractable
    {
        [SerializeField] private GameObject TextWindow;
        [SerializeField] private TextMeshProUGUI tutorialText;

        string[] dialogue = { "test 1", "test 2", "Test 3, wow this works! " };
        int dialogueTracker;
        [SerializeField] private int dialogueLoopPoint;

        public void Interact()
        {
            TextWindow.SetActive(true);
            if (dialogueTracker <= dialogue.Length)
            {
                tutorialText.text = dialogue[dialogueTracker];
                dialogueTracker += 1;
            }
            else
            {
                TextWindow.SetActive(false);
                dialogueTracker = dialogueLoopPoint;
            }
        }
    }
}
