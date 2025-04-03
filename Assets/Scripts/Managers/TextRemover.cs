using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextRemover : MonoBehaviour
{
    [SerializeField] private GameObject TextWindow;
    [SerializeField] private TextMeshProUGUI tutorialText;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {    
            tutorialText.text = "";
            TextWindow.SetActive(false);

        }
    }
}
