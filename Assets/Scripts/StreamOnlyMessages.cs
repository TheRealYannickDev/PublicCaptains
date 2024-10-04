using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class StreamOnlyMessages : MonoBehaviour
{
    public TextMeshProUGUI teamBattleText;
    public TextMeshProUGUI JederGegenJedenText;

    public TextMeshProUGUI timerText;

    private int ffaStimmen = 0;
    private int teamBattleStimmen = 0;

    private int timer = 60;

void Start()
{
    teamBattleText.text = "0";
    JederGegenJedenText.text = "0";
    StartCoroutine(Timer());
}
    
    public void StreamOnlyMessage(String message)
    {
        Debug.Log("StreamOnlyMessage: " + message);
        if(message.Contains("Teambattle") || message.Contains("teambattle") || message.Contains("TeamBattle"))
        {
            GameMode(1);
        }
        if(message.Contains("ffa") || message.Contains("FFA") || message.Contains("Ffa"))
        {
            GameMode(0);
        }
    }

    private void GameMode(int gameType)
    {
        if(gameType == 0)
        {
            ffaStimmen++;
            JederGegenJedenText.text = ffaStimmen.ToString();
        }
        if(gameType == 1)
        {
            teamBattleStimmen++;
            teamBattleText.text = teamBattleStimmen.ToString();
        }
        
    }

    private IEnumerator Timer(){
        while(timer > 0){
            timerText.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }
        if(teamBattleStimmen > ffaStimmen){
            PlayerPrefs.SetInt("GameType", 1);
        }else{
            PlayerPrefs.SetInt("GameType", 0);
        }
        FindAnyObjectByType<MessageScript>().spielModus = (SpielModus)PlayerPrefs.GetInt("GameType");
        Destroy(FindObjectOfType<MessageScript>().gameObject);
        SceneManager.LoadScene("StreamOnlyLobby");
    }


}
