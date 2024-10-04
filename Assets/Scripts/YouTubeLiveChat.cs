using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class VideoDetailsResponse
{
    public VideoDetailItem[] items;
}

[System.Serializable]
public class VideoDetailItem
{
    public LiveStreamingDetails liveStreamingDetails;
}

[System.Serializable]
public class LiveStreamingDetails
{
    public string activeLiveChatId;
}

[System.Serializable]
public class LiveChatMessagesResponse
{
    public LiveChatMessageItem[] items;
}

[System.Serializable]
public class LiveChatMessageItem
{
    public LiveChatSnippet snippet;
    public LiveChatAuthorDetails authorDetails;
}

[System.Serializable]
public class LiveChatSnippet
{
    public string displayMessage;
    public string publishedAt; // Zeitstempel der Nachricht
}


[System.Serializable]
public class LiveChatAuthorDetails
{
    public string displayName;
}

public class YouTubeLiveChat : MonoBehaviour
{
    private List<string> apiKey = new List<string>();

    private string apiListpath = "C:/Spiele/TwitchSchiffBattle/YouTubeApi.txt";
    private string apiKey1 = "";

    public string videoId = "YOUR_VIDEO_ID";
    private string liveChatId;
    private float refreshInterval = 0.5f; // Zeit in Sekunden zwischen den Abfragen
    private string lastMessageTimestamp = ""; // Zeitstempel der letzten Nachricht

    private List<string> users = new List<string>();
    private List<GameObject> circles = new List<GameObject>();

    private Vector3 movingDirection;
    private MessageScript messageScript;

    private int KeyNr = 0;

    void Start()
    {
        messageScript = GetComponent<MessageScript>();

        StartCoroutine(GetLiveChatId(KeyNr));
        KeyNr++;
        DontDestroyOnLoad(gameObject);
        
    }
    // function that reads the api keys from a file

    public void ReadApiKeys()
    {
        string[] lines = System.IO.File.ReadAllLines(apiListpath);
        foreach (string line in lines)
        {
            apiKey.Add(line);
        }
        apiKey1 = apiKey[1];
    }

    IEnumerator GetLiveChatId(int nr)
    {
        ReadApiKeys();
        string url = $"https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id={videoId}&key={apiKey1}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        //Debug.Log("Fet LvieChat with ID: " + nr);
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            //StartCoroutine(GetLiveChatId(KeyNr));
            //KeyNr++;
        }
        else
        {
            VideoDetailsResponse response = JsonUtility.FromJson<VideoDetailsResponse>(request.downloadHandler.text);
            if (response.items != null && response.items.Length > 0)
            {
                liveChatId = response.items[0].liveStreamingDetails.activeLiveChatId;
                StartCoroutine(GetLiveChatMessages(nr));
            }
        }
    }

    IEnumerator GetLiveChatMessages(int nr)
    {
        string url = $"https://www.googleapis.com/youtube/v3/liveChat/messages?liveChatId={liveChatId}&part=snippet,authorDetails&key={apiKey1}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            StartCoroutine(GetLiveChatId(KeyNr));
            KeyNr++;
        }
        else
        {
            LiveChatMessagesResponse response = JsonUtility.FromJson<LiveChatMessagesResponse>(request.downloadHandler.text);
            if (response.items != null)
            {
                foreach (var item in response.items)
                {
                    string messageTimestamp = item.snippet.publishedAt;

                    if (string.Compare(messageTimestamp, lastMessageTimestamp) > 0)
                    {
                        string authorName = item.authorDetails.displayName;
                        string messageText = item.snippet.displayMessage;
                        lastMessageTimestamp = messageTimestamp; // Update den Zeitstempel
                        //messageScript.ReciveMessage(authorName, messageText);
                    }
                }
            }
        }

        while (true)
        {
            url = $"https://www.googleapis.com/youtube/v3/liveChat/messages?liveChatId={liveChatId}&part=snippet,authorDetails&key={apiKey1}";
            request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                StartCoroutine(GetLiveChatId(KeyNr));
                KeyNr++;
                break;
            }
            else
            {
                LiveChatMessagesResponse response = JsonUtility.FromJson<LiveChatMessagesResponse>(request.downloadHandler.text);
                if (response.items != null)
                {
                    foreach (var item in response.items)
                    {
                        string messageTimestamp = item.snippet.publishedAt;
                        if (string.Compare(messageTimestamp, lastMessageTimestamp) > 0)
                        {
                            string authorName = item.authorDetails.displayName;
                            string messageText = item.snippet.displayMessage;
                            lastMessageTimestamp = messageTimestamp; // Update den Zeitstempel
                            messageScript.ReciveMessage(authorName, messageText, 0);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(refreshInterval);
        }

    }
}