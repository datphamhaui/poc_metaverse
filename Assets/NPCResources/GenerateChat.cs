using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GenerateChat : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI chatContent;
    [SerializeField]
    private TMP_InputField input;
    [SerializeField]
    private Button sendButton;
    private string firstMessageNpc = "How can I help you?\n";
    private float delay = 0.01f;
    private static System.Random random = new System.Random();
    private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private string id;

    // Start is called before the first frame update
    void Start()
    {
        chatContent.text += "<color=blue>NPC: </color> ";
        StartCoroutine(WriteText(firstMessageNpc));
        //sendButton.onClick.AddListener(GetMessageNPC);
        sendButton.onClick.AddListener(OnSendMessage);
        input.Select();
        input.ActivateInputField();
    }

    private void OnEnable()
    {
        id = GenerateRandomID(16);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && input.text != "")
        {
            Debug.Log("Enter");
            OnSendMessage();
        }
    }

    void OnSendMessage()
    {
        // Handle when input have text
        if (input.text != "")
        {
            string playerMessage = input.text;
            chatContent.text += $"<color=green>You:</color> {playerMessage}\n";
            chatContent.text += "<color=blue>NPC: </color> ";
            StartCoroutine(WriteText("..."));
            input.readOnly = true;          //disable input field
            sendButton.interactable = false; //disable button send 
            StartCoroutine(RecieveMessageNpcWebRequest(response => CallBackGetMessageNPC(response)));
        }

    }

    private IEnumerator RecieveMessageNpcWebRequest(System.Action<string> callback)
    {
        string apiUrl = "https://ezmjrq4itd.execute-api.ap-east-1.amazonaws.com/dev/comsec";
        string apiKey = "vWu2BMUvFf83eWntvGKT197LWoHUEsC559m35nWv";
        string question = input.text;
        input.text = "";
        // Create a JSON request string
        string jsonRequest = $"{{\"id\":\"{id}\",\"question\":\"{question}\"}}";
        Debug.Log(jsonRequest);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonRequest);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log(jsonResponse);
            callback.Invoke(jsonResponse.Replace("\"", ""));
        }
        else
        {
            Debug.LogError("Request failed with status code: " + request.responseCode);
            Debug.LogError("Sorry, I can't reply at the moment");
            callback.Invoke("Sorry, I can't reply at the moment");
        }
    }

    public void CallBackGetMessageNPC(string response)
    {

        string messageNpc = response;
        chatContent.text = chatContent.text.Replace("...", "");

        StartCoroutine(WriteText(messageNpc + "\n"));

        input.readOnly = false;         //enable input field
        sendButton.interactable = true; //enable button send 


    }

    private static async Task<string> GetMessageNpcAPI(string id, string question)
    {
        string apiUrl = "https://ezmjrq4itd.execute-api.ap-east-1.amazonaws.com/dev/comsec";
        string apiKey = "vWu2BMUvFf83eWntvGKT197LWoHUEsC559m35nWv";

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        var requestData = new
        {
            id,
            question
        };
        Debug.Log("Param: " + requestData);
        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Debug.Log(jsonResponse);
            return jsonResponse;
        }
        else
        {
            Debug.Log("Request failed with status code: " + response.StatusCode);
            return "Sorry I can't reply at the moment";
        }

    }
    public async void GetMessageNPC()
    {
        string playerMessage = input.text;
        if (input.text != "")
        {
            input.text = "";

            chatContent.text += $"<color=green>You:</color> {playerMessage}\n";
            chatContent.text += "<color=blue>NPC: </color> ";

            StartCoroutine(WriteText("..."));

            input.readOnly = true;          //disable input field
            sendButton.interactable = false; //disable button send 

            string messageNpc = await GetMessageNpcAPI(id, playerMessage);
            chatContent.text = chatContent.text.Replace("...", "");

            StartCoroutine(WriteText(messageNpc + "\n"));

            input.readOnly = false;         //enable input field
            sendButton.interactable = true; //enable button send 
        }

    }

    public static string GenerateRandomID(int length)
    {
        char[] stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    private IEnumerator WriteText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            chatContent.text += text[i];

            // Wait for the specified delay before writing the next character
            yield return new WaitForSeconds(delay);
        }
    }

}
