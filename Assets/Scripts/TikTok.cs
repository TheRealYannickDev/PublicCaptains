using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TikTokLiveUnity;

public class TikTok : MonoBehaviour
{
    string userId = "yannick_dev";

    private string lastMessageTimestamp = ""; // Zeitstempel der letzten Nachricht
    private MessageScript messageScript;

    private bool loadingChat = true;



    void Awake()
    {
        userId = PlayerPrefs.GetString("TikTokUsername", "yannick_dev");
        messageScript = GetComponent<MessageScript>();
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    async void Start()
    {
        StartCoroutine(LoadingChatMessages());
        TikTokLiveManager.Instance.OnChatMessage += (liveclient, giftEvent) =>
        {
            //Debug.Log("Chat message received: " + giftEvent.Message + " Chatter Name: " + giftEvent.Sender.NickName + "Timestamp: " + giftEvent.ClientSendTime);
            if (loadingChat)
            {
                return;
            }
            messageScript.ReciveMessage(giftEvent.Sender.NickName, giftEvent.Message, 2);
        };

        await TikTokLiveManager.Instance.ConnectToStream(userId);
    }


    private IEnumerator LoadingChatMessages(){
        yield return new WaitForSeconds(3);
        loadingChat = false;
    }

}
