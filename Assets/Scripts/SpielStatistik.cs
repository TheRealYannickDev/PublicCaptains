using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpielStatistik : MonoBehaviour
{
    private MessageScript messageScript;
    public GameObject StatistikPanel;
    public GameObject StatistiPanelParrent;

    public TextMeshProUGUI siegerName;
    public TextMeshProUGUI siegerTrefferName;
    public TextMeshProUGUI siegerTreffer;
    public TextMeshProUGUI siegerVersenktName;
    public TextMeshProUGUI siegerVersenkt;

    public TextMeshProUGUI TeamSiegerName;



    private List<Schiffe> schiffe = new List<Schiffe>();
    // Start is called before the first frame update
    void Start()
    {
        messageScript = FindObjectOfType<MessageScript>();
        schiffe = messageScript.schiffe;
        SetLeaderBoardTable();
        if(PlayerPrefs.GetInt("StreamOnlyMode") == 1){
           Invoke("LoadStreamOnlyScene", 30);
        }
    }

    private void SetLeaderBoardTable()
    {
        if (messageScript.spielModus == SpielModus.Teambattle)
        {
            if (messageScript.teamSieger == 5)
            {
                TeamSiegerName.color = new Color(115f / 255f, 8f / 255f, 8f / 255f);
                TeamSiegerName.text = "Rot";
            }
            if (messageScript.teamSieger == 4)
            {
                TeamSiegerName.color = new Color(14f / 255f, 17f / 255f, 125f / 255f);
                TeamSiegerName.text = "Blau";
            }
        }
        schiffe.Sort((x, y) => x.treffer.CompareTo(y.treffer));
        schiffe.Sort((x, y) => x.versenkt.CompareTo(y.versenkt));
        for (int i = schiffe.Count - 1; i >= 0; i--)
        {
            GameObject stat = Instantiate(StatistikPanel, StatistiPanelParrent.transform);
            stat.GetComponentsInChildren<TextMeshProUGUI>()[0].text = schiffe[i].users;
            stat.GetComponentsInChildren<TextMeshProUGUI>()[1].text = schiffe[i].treffer.ToString();
            stat.GetComponentsInChildren<TextMeshProUGUI>()[2].text = schiffe[i].versenkt.ToString();
        }
        GameObject siegerSchiff = messageScript.gewinnerSchiff;
        foreach (Schiffe shiff in schiffe)
        {
            if (shiff.ship == siegerSchiff)
            {
                siegerName.text = shiff.users;
            }
        }
        siegerVersenkt.text = schiffe[schiffe.Count - 1].versenkt.ToString() + " Versenkt";
        siegerVersenktName.text = schiffe[schiffe.Count - 1].users;

        schiffe.Sort((x, y) => x.treffer.CompareTo(y.treffer));

        siegerTreffer.text = schiffe[schiffe.Count - 1].treffer.ToString() + " Treffer";
        siegerTrefferName.text = schiffe[schiffe.Count - 1].users;
    }

    public void LoadMenu()
    {
        Destroy(messageScript.gameObject);
        PlayerPrefs.SetInt("StreamOnlyMode", 0);
        SceneManager.LoadScene("Menü");
    }

    public void LoadLobby(){
        Destroy(messageScript.gameObject);
        PlayerPrefs.SetInt("StreamOnlyMode", 0);
        SceneManager.LoadScene("Lobby");
    }

    private void LoadStreamOnlyScene(){
        Destroy(messageScript.gameObject);
        SceneManager.LoadScene("StreamOnlyMenü");
    }

}
