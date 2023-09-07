using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        chatContent.text += "<color=blue>NPC: </color> ";
        StartCoroutine(WriteText(firstMessageNpc));
        sendButton.onClick.AddListener(GetMessageNPC);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //async void RecieveMessageNpc()
    //{
    //    var client = new HttpClient();
    //    var request = new HttpRequestMessage(HttpMethod.Post, "https://ezmjrq4itd.execute-api.ap-east-1.amazonaws.com/dev/comsec");
    //    request.Headers.Add("x-api-key", "vWu2BMUvFf83eWntvGKT197LWoHUEsC559m35nWv");
    //    var content = new StringContent("{\r\n  \"id\": \"randomjifwo320udsfl\",\r\n  \"question\": \"hi how can I start a company?\"\r\n}", null, "application/json");
    //    request.Content = content;
    //    var response = await client.SendAsync(request);
    //    response.EnsureSuccessStatusCode();
    //    Debug.Log(await response.Content.ReadAsStringAsync());
    //    Console.WriteLine(await response.Content.ReadAsStringAsync());
    //}

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

    private IEnumerator WriteText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            chatContent.text += text[i];

            // Wait for the specified delay before writing the next character
            yield return new WaitForSeconds(delay);
        }
    }

    public async void GetMessageNPC()
    {
        string id = GenerateRandomID(16);
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

}
