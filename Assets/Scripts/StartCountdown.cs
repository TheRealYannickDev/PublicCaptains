using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    int countdown = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(countdownRoutine());
    }

    private IEnumerator countdownRoutine()
    {
        yield return new WaitForSeconds(1);
        countdown--;
        countdownText.text = countdown.ToString();
        if (countdown > 0)
        {
            StartCoroutine(countdownRoutine());
        }
        else
        {
            countdownText.text = "GO!";
            FindObjectOfType<MessageScript>().StartSchiffe();
            yield return new WaitForSeconds(1);
            countdownText.text = "";
            // Start the game
        }
        // Start the game
    }


}
