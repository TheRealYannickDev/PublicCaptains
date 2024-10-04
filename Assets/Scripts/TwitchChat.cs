using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using TMPro;
using System.Text.RegularExpressions;

public class TwitchChat : MonoBehaviour
{
    public float speed = 10;
    [SerializeField] GameObject kreis;
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    MessageScript messageScript;

    private string username = "YannickDev";
    private string oauth = "";


    private Vector3 movingDirection;

    private bool gameRunning = false;
    // Start is called before the first frame update

  
    private void Awake()
    {
        username = PlayerPrefs.GetString("TwitchUsername", "YannickDev");
        DontDestroyOnLoad(gameObject);
        ChatConnect();
        messageScript = GetComponent<MessageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadChat();
    }


    public void ChatConnect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS oauth:"+oauth);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :YannickDev");
        writer.WriteLine("JOIN #" + username);
        writer.Flush();
    }

    private void ReadChat()
    {
        if (twitchClient.Available > 0)
        {
            string message = reader.ReadLine();
            if (message.Contains("PING"))
            {
                writer.WriteLine("PONG :tmi.twitch.tv");
                writer.Flush();
                return;
            }

            //filter out the chat message
            if (message.Contains("PRIVMSG"))
            {
                //get the users name by splitting at the !
                int splitPoint = message.IndexOf("!", 1);
                string chatName = message.Substring(1, splitPoint - 1);
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                messageScript.ReciveMessage(chatName, message, 1);
            }
            
        }
    }

  


   

    public string ExtractNumber(string input)
    {
        // Definiere das Muster für eine oder mehrere zusammenhängende Ziffern
        string pattern = @"\d+";

        // Finde die erste Übereinstimmung des Musters im Eingabestring
        Match match = Regex.Match(input, pattern);

        // Wenn eine Übereinstimmung gefunden wurde, gib sie zurück, ansonsten einen leeren String
        if (match.Success)
        {
            return match.Value;
        }
        else
        {
            return string.Empty;
        }
    }



   
}
