using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    MessageScript messageScript;

    public static StartGame instance;

    void Start()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        messageScript = FindObjectOfType<MessageScript>();
    }


    public void startGame(){

        SceneManager.LoadScene(2);
    }

    // start game when scene is loaded
    void OnLevelWasLoaded(int level)
    {
        if(level == 2){
            messageScript.startGame();
        }
    }
}
