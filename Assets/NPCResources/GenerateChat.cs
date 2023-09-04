using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class GenerateChat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RecieveMessageNpc();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void RecieveMessageNpc()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://ezmjrq4itd.execute-api.ap-east-1.amazonaws.com/dev/comsec");
        request.Headers.Add("x-api-key", "vWu2BMUvFf83eWntvGKT197LWoHUEsC559m35nWv");
        var content = new StringContent("{\r\n  \"id\": \"randomjifwo320udsfl\",\r\n  \"question\": \"hi how can I start a company?\"\r\n}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Debug.Log(await response.Content.ReadAsStringAsync());
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}
