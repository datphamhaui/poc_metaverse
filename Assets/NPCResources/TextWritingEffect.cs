using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Use this for UI Text
//using TMPro; // Uncomment this line if using TextMeshPro

public class TextWritingEffect : MonoBehaviour
{
    //public TMP_Text textMeshPro; // Uncomment this line if using TextMeshPro
    public TextMeshProUGUI uiText; // Comment this line if using TextMeshPro

    public string fullText = "This is the text you want to write out." ;
    public float delay = 0.1f;

    private string currentText = "";

    private void Start()
    {
        if (uiText == null)
        {
            Debug.LogError("UI Text component is not assigned.");
            enabled = false;
            return;
        }

        // Initialize the text
        uiText.text = currentText;

        // Start the coroutine to write the text
        StartCoroutine(WriteText());

        // Gửi tin nhắn của bạn và của NPC vào đối tượng Text
        SendMessage("Player", "Hello, how can I start a company?");
        SendMessage("NPC", "Starting a company requires careful planning and research.");
        SendMessage("Player", "Thank you for the advice!");
    }

    private IEnumerator WriteText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText += fullText[i];
            UpdateText();

            // Wait for the specified delay before writing the next character
            yield return new WaitForSeconds(delay);
        }
    }

    private void UpdateText()
    {
        // Update the UI Text component with the currentText
        if (uiText != null)
        {
            uiText.text = currentText;
        }
        /* Uncomment this section if using TextMeshPro
        else if (textMeshPro != null)
        {
            textMeshPro.text = currentText;
        }
        */
    }

    public TextMeshProUGUI chatText;

    // Phương thức để hiển thị tin nhắn
    private void SendMessage(string sender, string message)
    {
        // Định dạng văn bản để tin nhắn của bạn hiển thị bên phải và của NPC hiển thị bên trái
        string formattedMessage = "";

        if (sender == "Player")
        {
            formattedMessage = $"<align=right><color=blue>{sender}:</color> {message}</align>\n";
        }
        else if (sender == "NPC")
        {
            formattedMessage = $"<align=left><color=green>{sender}:</color> {message}</align>\n";
        }

        // Thêm tin nhắn vào đối tượng Text
        chatText.text += formattedMessage;
    }
}
