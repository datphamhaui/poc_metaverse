using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCShowHello : MonoBehaviour
{
    public TMP_Text uiTextHello;
    public string textHello = "Hello there!";
    private string currentText = "";
    private float delay = 0.1f;
    private GameObject player;

    // Start is called before the first frame update
    void OnEnable()
    {
        currentText = "";
        if (uiTextHello == null)
        {
            enabled = false;
            return;
        }

        // Initialize the text
        uiTextHello.text = currentText;

        // Start the coroutine to write the text
        StartCoroutine(WriteTextHello());
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private IEnumerator WriteTextHello()
    {
        for (int i = 0; i < textHello.Length; i++)
        {
            currentText += textHello[i];
            UpdateText();

            // Wait for the specified delay before writing the next character
            yield return new WaitForSeconds(delay);
        }
    }

    private void UpdateText()
    {
        // Update the UI Text component with the currentText
        if (uiTextHello != null)
        {
            uiTextHello.text = currentText;
        }
        
    }
}
