using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject Fass;

    private List<GameObject> Fässer = new List<GameObject>();
    public List<GameObject> Schiffe = new List<GameObject>();

    public bool gamerunning = false;

    private MessageScript messageScript;

    void Start()
    {
        messageScript = FindObjectOfType<MessageScript>();
        StartCoroutine(spawnFässer());
    }

    private IEnumerator spawnFässer()
    {
        while (true)
        {
            foreach (GameObject fass in Fässer)
            {
                if (fass == null)
                {
                    Fässer.Remove(fass);
                    break;
                }
            }

            if (Fässer.Count < 10)
            {
                Vector3 spawnPosition;
                spawnPosition = findEmptySpawnPosition(0);
                Fässer.Add(Instantiate(Fass, spawnPosition, Quaternion.identity));
            }
            yield return new WaitForSeconds(2);
        }
    }

    private Vector3 findEmptySpawnPosition(int counter)
    {
        counter++;
        Vector3 spawnPosition = new Vector3(Random.Range(-8.5f, 8.5f), Random.Range(-4.5f, 4.5f), 0);
        RaycastHit2D hit = Physics2D.CircleCast(spawnPosition, .3f, Vector2.zero);
        if (hit.collider != null)
        {
            if (counter > 10)
            {
                return new Vector3(0, 0, 0);
            }
            return findEmptySpawnPosition(counter);
        }
        return spawnPosition;
    }

    public void AddShip(GameObject ship)
    {
        Schiffe.Add(ship);
    }

    void Update()
    {


        int activeShips = 0;
        GameObject gewinnerSchiff = null;
        foreach (GameObject ship in Schiffe)
        {
            if (ship.activeSelf)
            {
                activeShips++;
                gewinnerSchiff = ship;
            }
        }

        if(messageScript.spielModus == SpielModus.StreamerBattle){
            int streamerschiffcount = 0;
            int ChatSchiffCount = 0;
            foreach (GameObject ship in Schiffe)
            {
                if (ship.activeSelf)
                {
                    
                    if (ship.GetComponent<CircleSkript>().teamId == 1)
                    {
                        ChatSchiffCount++;
                    }
                    if(ship.GetComponent<CircleSkript>().teamId == 2){
                        streamerschiffcount++;
                    }
                  
                }
        }
            if(ChatSchiffCount == 0){
                Debug.Log("Streamer haben gewonnen");
                gamerunning = false;
                FindObjectOfType<MessageScript>().DebugStats();
                FindObjectOfType<MessageScript>().gewinnerSchiff = gewinnerSchiff;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if(streamerschiffcount == 0){
                Debug.Log("Chat hat gewonnen");
                gamerunning = false;
                FindObjectOfType<MessageScript>().DebugStats();
                FindObjectOfType<MessageScript>().gewinnerSchiff = gewinnerSchiff;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }


        }

        if (messageScript.spielModus == SpielModus.Teambattle)
        {
            int gewinnerTeamId = -1;
            bool gameOver = true;
            foreach (GameObject ship in Schiffe)
            {
                if (ship.activeSelf)
                {
                    if (gewinnerTeamId == -1)
                    {
                        gewinnerTeamId = ship.GetComponent<CircleSkript>().teamId;
                    }
                    if (ship.GetComponent<CircleSkript>().teamId != gewinnerTeamId)
                    {
                        gameOver = false;
                    }
                }
            }
            if (gameOver)
            {
                Debug.Log("Game Over");
                gamerunning = false;

                FindObjectOfType<MessageScript>().DebugStats();
                Debug.Log("Gewinner Team: " + gewinnerTeamId);
                FindObjectOfType<MessageScript>().teamSieger = gewinnerTeamId;
                FindObjectOfType<MessageScript>().gewinnerSchiff = gewinnerSchiff;
                FindObjectOfType<MessageScript>().gameRunning = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
            }

        }

        if (messageScript.spielModus == SpielModus.Standard)
        {

            if (activeShips <= 1 && gamerunning)
            {
                Debug.Log("Game Over");
                gamerunning = false;
                FindObjectOfType<MessageScript>().DebugStats();
                FindObjectOfType<MessageScript>().gewinnerSchiff = gewinnerSchiff;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }


}
