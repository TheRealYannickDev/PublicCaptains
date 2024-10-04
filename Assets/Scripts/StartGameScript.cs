using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    public void StartGameClick(){
        FindAnyObjectByType<StartGame>().startGame();
    }

    private void Start() {
        FindObjectOfType<MessageScript>().lobbyRunning = true;
    }
}
