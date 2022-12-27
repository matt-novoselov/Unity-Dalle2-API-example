using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;

public class MyClass
{
    public string prompt;
    public int n;
    public string size;
}

public class UnityDalle2API : MonoBehaviour
{
    private string url;
    public string YourApiKey = "";
    public string YourPrompt = "Red balloon";
    public string ImageSize = "256x256";

    public IEnumerator FillAndSend(string json)
    {
        using (var request = new UnityWebRequest("https://api.openai.com/v1/images/generations", "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {YourApiKey}");
            request.SetRequestHeader("Accept", " text/plain");

            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            url = request.downloadHandler.text.Split('"')[7];
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        gameObject.GetComponent<Renderer>().material.mainTexture = DownloadHandlerTexture.GetContent(www);
    }
    public void NewPrompt()
    {
        MyClass myObject = new MyClass();
        myObject.prompt = YourPrompt;
        myObject.n = 1;
        myObject.size = ImageSize;
        string json = JsonUtility.ToJson(myObject);
        StartCoroutine(FillAndSend(json));
    }
    public void Start()
    {
        NewPrompt();
    }
}