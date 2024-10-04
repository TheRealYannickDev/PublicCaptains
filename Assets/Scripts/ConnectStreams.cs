using System.Collections;
using System.Collections.Generic;
using TikTokLiveSharp.Models.Protobuf.LinkmicCommon.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ConnectStreams : MonoBehaviour
{
    public TMP_InputField tiktokUsername;
    public TMP_InputField twitchUsername;
    public TextMeshProUGUI streamerDelay;
    public TextMeshProUGUI maxPlayersNr;

    public Toggle streamerSchiff;

    public Toggle geisterschiff;
    public Slider streamerDelaySlider;
    public Slider maxPlayersSlider;


    void Start()
    {
        PlayerPrefs.SetInt("StreamOnlyMode", 0);
        if (PlayerPrefs.HasKey("TikTokUsername"))
        {
            tiktokUsername.text = PlayerPrefs.GetString("TikTokUsername");
        }
        if (PlayerPrefs.HasKey("TwitchUsername"))
        {
            twitchUsername.text = PlayerPrefs.GetString("TwitchUsername");
        }
        if (PlayerPrefs.HasKey("StreamerSchiff"))
        {
            streamerSchiff.isOn = PlayerPrefs.GetInt("StreamerSchiff") == 1;
        }
        else
        {
            streamerSchiff.isOn = true;
            PlayerPrefs.SetInt("StreamerSchiff", 1);
        }
        if (PlayerPrefs.HasKey("Geisterschiff"))
        {
            geisterschiff.isOn = PlayerPrefs.GetInt("Geisterschiff") == 1;
        }
        else
        {
            geisterschiff.isOn = false;
            PlayerPrefs.SetInt("Geisterschiff", 0);
        }
        SetStreamerDelay(PlayerPrefs.GetFloat("StreamerDelay", 2));
        SetMaxPlayers(PlayerPrefs.GetInt("MaxPlayers", 40));

    }
    public void StartLobby(int gameType)
    {
        PlayerPrefs.SetInt("GameType", gameType);
        SceneManager.LoadScene("Lobby");
    }

    public void TikTokUsername(string username)
    {
        Debug.Log("TikTok Username: " + username);
        PlayerPrefs.SetString("TikTokUsername", username);
    }

    public void TwitchUsername(string username)
    {
        Debug.Log("Twitch Username: " + username);
        PlayerPrefs.SetString("TwitchUsername", username);
    }

    public void StreamerSchiff(bool isStreamer)
    {
        PlayerPrefs.SetInt("StreamerSchiff", isStreamer ? 1 : 0);
    }

    public void Geisterschiff(bool isGeisterschiff)
    {
        PlayerPrefs.SetInt("Geisterschiff", isGeisterschiff ? 1 : 0);
    }

    public void SetStreamerDelay(float delay)
    {
        PlayerPrefs.SetFloat("StreamerDelay", delay);
        float roundedDelay = Mathf.Round(delay * 10f) / 10f;
        streamerDelay.text = roundedDelay.ToString();
        streamerDelaySlider.value = delay;
    }

    public void SetMaxPlayers(float maxPlayers)
    {
        PlayerPrefs.SetInt("MaxPlayers", (int)maxPlayers);
        maxPlayersNr.text = maxPlayers.ToString();
        maxPlayersSlider.value = maxPlayers;
    }

    public void startStreamOnlyMode()
    {
        SceneManager.LoadScene("StreamOnlyMenü");
    }
}
