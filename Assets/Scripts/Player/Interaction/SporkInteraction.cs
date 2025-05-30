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
        
        string[] dialogue = { 
            "Hey kid! you okay?",
            "Those no good Flying Tea-Kettles and their leader Sir Earl of Grey �repossessed� your entire house?",
            "Well that just ain�t right! They've been nothin� but trouble ever since they built their VAULT here in the swamp.",
            "Say you're a Swamp Wompus right?",
            "I reckon you could get powerful enough to take the fight right to their doorstep if you powered yourself up a tad.",
            "I�ve heard talk of power gems around the swamp that can give you secret skills!",
            "I bet findin� some of those might prove useful to yuh",
            "If you wanna get a lay of the land try pausing the game to check the map!.. Whatever that means...",
        };
        int dialogueTracker;
        [SerializeField] private int dialogueLoopPoint;
        
        public void Interact()
        {
            
            if (dialogueTracker <= 7)
            {
                tutorialText.text = dialogue[dialogueTracker];
                TextWindow.SetActive(true);
                dialogueTracker += 1;
            }

            else if (dialogueTracker > 2) {
            
                TextWindow.SetActive(false);
                dialogueTracker = dialogueLoopPoint;
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
