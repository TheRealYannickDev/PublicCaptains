using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StreamOnlyLobby : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private int timer = 60;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("StreamOnlyMode", 1);
        StartCoroutine(Timer());
    }

      private IEnumerator Timer(){
        while(timer > 0){
            timerText.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }

        FindAnyObjectByType<MessageScript>().spielModus = (SpielModus)PlayerPrefs.GetInt("GameType");
        SceneManager.LoadScene("Game");
    }
}
