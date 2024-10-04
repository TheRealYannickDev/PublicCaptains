using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Runtime.InteropServices;



public class Schiffe
{
    public string users;
    public int cahtId;
    public string color;
    public GameObject ship;

    public int treffer = 0;
    public int versenkt = 0;

}

public enum SpielModus { Standard, Teambattle, StreamerBattle };


public class MessageScript : MonoBehaviour
{
    public SpielModus spielModus = SpielModus.Standard;
    private int LastTeam = 4;
    public GameObject Schiff;
    public GameObject SchiffStreamer;
    public GameObject PiratenSchiff;

    public GameObject LobbySchiff;
    public GameObject LobbySchiffStreamer;
    public List<Schiffe> schiffe = new List<Schiffe>();
    public List<string> users = new List<string>();
    public GameObject nameTag;
    public GameObject NamensListe;

    public GameObject Geisterschiff;
    private GameObject activeGeisterschiff;
    public bool gameRunning = false;
    public bool lobbyRunning = false;

    public GameObject gewinnerSchiff;

    public bool activateGeisterSchiff = false;

    public GameObject teamBattleGrafik;
    private GameObject activeTeamBattleGrafik;
    [HideInInspector] public int teamSieger = 0;

    private int maxPlayers = 40;

    private string streamerName = "Yannick";


    // Start is called before the first frame update
    void Start()
    {
        maxPlayers = PlayerPrefs.GetInt("MaxPlayers", 40);
        spielModus = (SpielModus)PlayerPrefs.GetInt("GameType", 0);
        DontDestroyOnLoad(gameObject);
        activateGeisterSchiff = PlayerPrefs.GetInt("Geisterschiff", 0) == 1;
        if ((PlayerPrefs.GetInt("StreamerSchiff", 1) == 1 && spielModus == SpielModus.Standard) || spielModus == SpielModus.StreamerBattle)
        {
            AddStreamerSchiff();
        }
    }

    public void AddStreamerSchiff()
    {
        if(PlayerPrefs.HasKey("TikTokUsername")){
            streamerName = PlayerPrefs.GetString("TikTokUsername", "Yannick");
        }
        if(PlayerPrefs.HasKey("TwitchUsername")){
            streamerName = PlayerPrefs.GetString("TwitchUsername", "Yannick");
        }
        
        users.Add(streamerName);
        schiffe.Add(new Schiffe());
        schiffe[schiffe.Count - 1].users = streamerName;
        schiffe[schiffe.Count - 1].cahtId = 3;
        schiffe[schiffe.Count - 1].color = "#FFFFFF";
        //GameObject _nameTag = Instantiate(nameTag, NamensListe.transform);
        //_nameTag.GetComponent<TextMeshProUGUI>().text = chatName;
        spawnStreamerShiff();
    }

    public void ReciveMessage(string chatName, string message, int chatType)
    {
        if(FindObjectOfType<StreamOnlyMessages>()){
            FindObjectOfType<StreamOnlyMessages>().StreamOnlyMessage(message);
        }

        if (spielModus == SpielModus.Teambattle)
        {
            ReciveTeammodusMessage(chatName, message);
            return;
        }


        if (users.Contains(chatName) == false && (message.Contains("JOIN") || message.Contains("join") || message.Contains("Join") || message.Contains("joyn") || message.Contains("Joyn")) && lobbyRunning == true)
        {
            if(users.Count >= maxPlayers)
            {
                return;
            }

            users.Add(chatName);
            schiffe.Add(new Schiffe());
            schiffe[schiffe.Count - 1].users = chatName;
            schiffe[schiffe.Count - 1].cahtId = chatType;
            schiffe[schiffe.Count - 1].color = "#FFFFFF";
            //GameObject _nameTag = Instantiate(nameTag, NamensListe.transform);
            //_nameTag.GetComponent<TextMeshProUGUI>().text = chatName;
            Debug.Log("User " + chatName + " joined the game");
            spawnLobbyShiff();
        }

        if (gameRunning == true)
        {
            GameAction(chatName, message);
        }
        if (lobbyRunning == true)
        {
            LobbyAction(chatName, message);
        }

        
    }

    private void ReciveTeammodusMessage(string chatName, string message)
    {
        if (users.Contains(chatName) == false && (message.Contains("join") || message.Contains("JOIN") || message.Contains("Join") || message.Contains("joyn") || message.Contains("Joyn")) && lobbyRunning == true)
        {
            users.Add(chatName);
            schiffe.Add(new Schiffe());
            schiffe[schiffe.Count - 1].users = chatName;
            schiffe[schiffe.Count - 1].cahtId = LastTeam;
            schiffe[schiffe.Count - 1].color = "#FFFFFF";

            if (LastTeam == 4)
            {
                LastTeam = 5;
            }
            else
            {
                LastTeam = 4;
            }
            //GameObject _nameTag = Instantiate(nameTag, NamensListe.transform);
            //_nameTag.GetComponent<TextMeshProUGUI>().text = chatName;
            Debug.Log("User " + chatName + " joined the game");
            spawnLobbyShiff();
        }

        if (gameRunning == true)
        {
            GameAction(chatName, message);
        }
        if (lobbyRunning == true)
        {
            LobbyAction(chatName, message);
        }
    }

    private void LobbyAction(string chatName, string message)
    {
        int index = users.IndexOf(chatName);
        if (index < 0)
        {
            return;
        }
        if (schiffe[index].ship == null)
        {
            return;
        }
        if (message.Contains("party") || message.Contains("Party") || message.Contains("PARTY"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().party();
        }

        if (message.Contains("f") || message.Contains("F"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
        }

        if (ExtractNumber(message) != "")
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(float.Parse(ExtractNumber(message)));
        }

        if (message.Contains("w") || message.Contains("W"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(0);
        }

        if (message.Contains("s") || message.Contains("S"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(180);
        }
        if (message.Contains("a") || message.Contains("A"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(270);
        }

        if (message.Contains("d") || message.Contains("D"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(90);
        }

        if ((message.Contains("w") && message.Contains("a")) || (message.Contains("W") && message.Contains("A")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(315);
        }
        if ((message.Contains("w") && message.Contains("d")) || (message.Contains("W") && message.Contains("D")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(45);
        }
        if ((message.Contains("s") && message.Contains("a")) || (message.Contains("S") && message.Contains("A")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(225);
        }
        if ((message.Contains("s") && message.Contains("d")) || (message.Contains("S") && message.Contains("D")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(135);
        }

        if (message.Contains("!color") || message.Contains("!Color") || message.Contains("!farbe") || message.Contains("!Farbe") || message.Contains("!colour") || message.Contains("!Colour"))
        {
            string hexColor = "";
            if (message.Contains("#"))
            {
                int startPos = message.LastIndexOf("#");
                if(message.Length - startPos < 7){
                    return;
                }
                int length = 7;
                hexColor = message.Substring(startPos, length);
                schiffe[index].ship.GetComponent<CircleSkript>().SetSegelColor(hexColor);
            }
            else
            {
                hexColor = setOtherColors(message);
                schiffe[index].ship.GetComponent<CircleSkript>().SetSegelColor(hexColor);
            }

            schiffe[index].color = hexColor;


        }
    }

    private string setOtherColors(string message)
    {
        string hexColorvalue = "";
        if (message.Contains("Grün") || message.Contains("grün") || message.Contains("Green") || message.Contains("green"))
        {
            hexColorvalue = "#00FF00";
        }
        if (message.Contains("Rot") || message.Contains("rot") || message.Contains("Red") || message.Contains("red"))
        {
            hexColorvalue = "#FF0000";
        }
        if (message.Contains("Blau") || message.Contains("blau") || message.Contains("Blue") || message.Contains("blue"))
        {
            hexColorvalue = "#0000FF";
        }
        if (message.Contains("Gelb") || message.Contains("gelb") || message.Contains("Yellow") || message.Contains("yellow"))
        {
            hexColorvalue = "#FFFF00";
        }
        if (message.Contains("Lila") || message.Contains("lila") || message.Contains("Purple") || message.Contains("purple"))
        {
            hexColorvalue = "#800080";
        }
        if (message.Contains("Schwarz") || message.Contains("schwarz") || message.Contains("Black") || message.Contains("black"))
        {
            hexColorvalue = "#000000";
        }
        if (message.Contains("Weiß") || message.Contains("weiß") || message.Contains("White") || message.Contains("white"))
        {
            hexColorvalue = "#FFFFFF";
        }
        if (message.Contains("Orange") || message.Contains("orange"))
        {
            hexColorvalue = "#FFA500";
        }
        if (message.Contains("Pink") || message.Contains("pink"))
        {
            hexColorvalue = "#FFC0CB";
        }
        if (message.Contains("Braun") || message.Contains("braun") || message.Contains("Brown") || message.Contains("brown"))
        {
            hexColorvalue = "#A52A2A";
        }
        if (message.Contains("Türkis") || message.Contains("türkis") || message.Contains("Turquoise") || message.Contains("turquoise"))
        {
            hexColorvalue = "#40E0D0";
        }
        if (message.Contains("Grau") || message.Contains("grau") || message.Contains("Gray") || message.Contains("gray"))
        {
            hexColorvalue = "#808080";
        }
        if (message.Contains("Hellblau") || message.Contains("hellblau") || message.Contains("Lightblue") || message.Contains("lightblue"))
        {
            hexColorvalue = "#ADD8E6";
        }
        if (message.Contains("Hellgrün") || message.Contains("hellgrün") || message.Contains("Lightgreen") || message.Contains("lightgreen"))
        {
            hexColorvalue = "#90EE90";
        }
        if (message.Contains("Hellrot") || message.Contains("hellrot") || message.Contains("Lightred") || message.Contains("lightred"))
        {
            hexColorvalue = "#FFC0CB";
        }
        if (message.Contains("Hellgelb") || message.Contains("hellgelb") || message.Contains("Lightyellow") || message.Contains("lightyellow"))
        {
            hexColorvalue = "#FFFFE0";
        }
        if (message.Contains("Helllila") || message.Contains("helllila") || message.Contains("Lightpurple") || message.Contains("lightpurple"))
        {
            hexColorvalue = "#E6E6FA";
        }
        if (message.Contains("Hellorange") || message.Contains("hellorange") || message.Contains("Lightorange") || message.Contains("lightorange"))
        {
            hexColorvalue = "#FFD700";
        }
        if (message.Contains("Hellpink") || message.Contains("hellpink") || message.Contains("Lightpink") || message.Contains("lightpink"))
        {
            hexColorvalue = "#FFB6C1";
        }
        if (message.Contains("Hellbraun") || message.Contains("hellbraun") || message.Contains("Lightbrown") || message.Contains("lightbrown"))
        {
            hexColorvalue = "#D2B48C";
        }
        if (message.Contains("Helltürkis") || message.Contains("helltürkis") || message.Contains("Lightturquoise") || message.Contains("lightturquoise"))
        {
            hexColorvalue = "#AFEEEE";
        }
        if (message.Contains("Hellgrau") || message.Contains("hellgrau") || message.Contains("Lightgray") || message.Contains("lightgray"))
        {
            hexColorvalue = "#D3D3D3";
        }
        if (message.Contains("Dunkelblau") || message.Contains("dunkelblau") || message.Contains("Darkblue") || message.Contains("darkblue"))
        {
            hexColorvalue = "#00008B";
        }
        if (message.Contains("Dunkelgrün") || message.Contains("dunkelgrün") || message.Contains("Darkgreen") || message.Contains("darkgreen"))
        {
            hexColorvalue = "#006400";
        }
        if (message.Contains("Dunkelrot") || message.Contains("dunkelrot") || message.Contains("Darkred") || message.Contains("darkred"))
        {
            hexColorvalue = "#8B0000";
        }
        if (message.Contains("Dunkelgelb") || message.Contains("dunkelgelb") || message.Contains("Darkyellow") || message.Contains("darkyellow"))
        {
            hexColorvalue = "#B8860B";
        }
        if (message.Contains("Dunkellila") || message.Contains("dunkellila") || message.Contains("Darkpurple") || message.Contains("darkpurple"))
        {
            hexColorvalue = "#9932CC";
        }
        if (message.Contains("Dunkelorange") || message.Contains("dunkelorange") || message.Contains("Darkorange") || message.Contains("darkorange"))
        {
            hexColorvalue = "#FF8C00";
        }
        if (message.Contains("Dunkelpink") || message.Contains("dunkelpink") || message.Contains("Darkpink") || message.Contains("darkpink"))
        {
            hexColorvalue = "#C71585";
        }
        if (message.Contains("Dunkelbraun") || message.Contains("dunkelbraun") || message.Contains("Darkbrown") || message.Contains("darkbrown"))
        {
            hexColorvalue = "#8B4513";
        }
        if (message.Contains("Dunkeltürkis") || message.Contains("dunkeltürkis") || message.Contains("Darkturquoise") || message.Contains("darkturquoise"))
        {
            hexColorvalue = "#00CED1";
        }
        if (message.Contains("Dunkelgrau") || message.Contains("dunkelgrau") || message.Contains("Darkgray") || message.Contains("darkgray"))
        {
            hexColorvalue = "#A9A9A9";
        }

        return hexColorvalue;

    }

    private void GeisterschiffAction(string message)
    {
        if (ExtractNumber(message) != "")
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(float.Parse(ExtractNumber(message)));
        }

        if (message.Contains("f") || message.Contains("F"))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Shoot();
        }

        if (message.Contains("w") || message.Contains("W"))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(0);
        }

        if (message.Contains("s") || message.Contains("S"))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(180);
        }
        if (message.Contains("a") || message.Contains("A"))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(270);
        }

        if (message.Contains("d") || message.Contains("D"))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(90);
        }

        if ((message.Contains("w") && message.Contains("a")) || (message.Contains("W") && message.Contains("A")))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(315);
        }
        if ((message.Contains("w") && message.Contains("d")) || (message.Contains("W") && message.Contains("D")))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(45);
        }
        if ((message.Contains("s") && message.Contains("a")) || (message.Contains("S") && message.Contains("A")))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(225);
        }
        if ((message.Contains("s") && message.Contains("d")) || (message.Contains("S") && message.Contains("D")))
        {
            activeGeisterschiff.GetComponent<GeisterSchiff>().Steer(135);
        }

    }

    private void GameAction(string chatName, string message)
    {
        int index = users.IndexOf(chatName);
        if (index < 0)
        {
            if (activateGeisterSchiff)
            {
                GeisterschiffAction(message);
            }
            return;
        }
        if (schiffe[index].ship == null || schiffe[index].ship.activeSelf == false)
        {
            if (activateGeisterSchiff)
            {
                GeisterschiffAction(message);
            }
            return;
        }

        if (ExtractNumber(message) != "")
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(float.Parse(ExtractNumber(message)));
        }

        if (message.Contains("f") || message.Contains("F"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
        }

        if (message.Contains("w") || message.Contains("W"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(0);
        }

        if (message.Contains("s") || message.Contains("S"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(180);
        }
        if (message.Contains("a") || message.Contains("A"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(270);
        }

        if (message.Contains("d") || message.Contains("D"))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(90);
        }

        if ((message.Contains("w") && message.Contains("a")) || (message.Contains("W") && message.Contains("A")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(315);
        }
        if ((message.Contains("w") && message.Contains("d")) || (message.Contains("W") && message.Contains("D")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(45);
        }
        if ((message.Contains("s") && message.Contains("a")) || (message.Contains("S") && message.Contains("A")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(225);
        }
        if ((message.Contains("s") && message.Contains("d")) || (message.Contains("S") && message.Contains("D")))
        {
            schiffe[index].ship.GetComponent<CircleSkript>().Steer(135);
        }

        /*
                switch (message)
                {
                    case "fir":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    case "f":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    case "Fir":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    case "F":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    case "FIRE":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    case "fire":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    case "Fire":
                        schiffe[index].ship.GetComponent<CircleSkript>().Shoot();
                        break;
                    default:
                        break;
                }
            */
    }

    private void spawnLobbyShiff()
    {
        GameObject newCircle = Instantiate(LobbySchiff, new Vector3(Random.Range(-2.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);
        newCircle.GetComponent<CircleSkript>().id = users.Count - 1;
        newCircle.GetComponent<CircleSkript>().SetName(users[users.Count - 1]);
        newCircle.GetComponent<CircleSkript>().SetColor(schiffe[schiffe.Count - 1].cahtId);
        newCircle.GetComponent<CircleSkript>().startGame();
        schiffe[schiffe.Count - 1].ship = newCircle;
    }

    private void spawnStreamerShiff()
    {
        GameObject newCircle;
        if (spielModus == SpielModus.StreamerBattle)
        {
            newCircle = Instantiate(PiratenSchiff, new Vector3(Random.Range(-2.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);

        }
        else
        {
            newCircle = Instantiate(LobbySchiffStreamer, new Vector3(Random.Range(-2.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);
        }
        newCircle.GetComponent<CircleSkript>().id = users.Count - 1;
        newCircle.GetComponent<CircleSkript>().SetName(users[users.Count - 1]);
        newCircle.GetComponent<CircleSkript>().SetColor(schiffe[schiffe.Count - 1].cahtId);
        newCircle.GetComponent<CircleSkript>().startGame();
        schiffe[schiffe.Count - 1].ship = newCircle;
    }

    public void startGame()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (spielModus == SpielModus.Teambattle)
        {

            activeTeamBattleGrafik = Instantiate(teamBattleGrafik);
        }


        for (int i = 0; i < schiffe.Count; i++)
        {
            GameObject newCircle = null;
            if (schiffe[i].cahtId == 3)
            {
                if (spielModus == SpielModus.StreamerBattle)
                {
                    newCircle = Instantiate(PiratenSchiff, new Vector3(Random.Range(-2.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);
                }
                else
                {
                    newCircle = Instantiate(SchiffStreamer, new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);
                }
            }
            else
            {
                newCircle = Instantiate(Schiff, new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);
            }
            newCircle.GetComponent<CircleSkript>().id = i;
            newCircle.GetComponent<CircleSkript>().SetName(users[i]);
            newCircle.GetComponent<CircleSkript>().SetColor(schiffe[i].cahtId);
            newCircle.GetComponent<CircleSkript>().gameHasStarted = false;
            newCircle.GetComponent<CircleSkript>().SetSegelColor(schiffe[i].color);
            if (spielModus == SpielModus.Teambattle)
            {
                newCircle.GetComponent<CircleSkript>().teamId = schiffe[i].cahtId;
            }
            else if (spielModus == SpielModus.StreamerBattle)
            {
                if (schiffe[i].cahtId == 3)
                {
                    newCircle.GetComponent<CircleSkript>().teamId = 2;
                }
                else
                {
                    newCircle.GetComponent<CircleSkript>().teamId = 1;
                }
            }
            else
            {
                newCircle.GetComponent<CircleSkript>().teamId = 0;
            }

            schiffe[i].ship = newCircle;
            gameManager.AddShip(newCircle);

        }

        foreach (Schiffe schiff in schiffe)
        {
            Debug.Log("Shiff: " + schiff.users + " is now in the game" + " shiff GameObject: " + schiff.ship);
        }
        if (activateGeisterSchiff && spielModus == SpielModus.Standard)
        {
            activeGeisterschiff = Instantiate(Geisterschiff, new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-3.5f, 3.5f), 0), Quaternion.identity);
        }
        gameManager.gamerunning = true;
        lobbyRunning = false;


    }


    public void StartSchiffe()
    {
        gameRunning = true;
        foreach (Schiffe schiff in schiffe)
        {
            Debug.Log("Shiff: " + schiff.users + " is now in the game" + " shiff GameObject: " + schiff.ship);
            schiff.ship.GetComponent<CircleSkript>().startGame();
        }
        if (activateGeisterSchiff && spielModus == SpielModus.Standard)
        {
            activeGeisterschiff.GetComponent<CircleSkript>().startGame();
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

    public void SchiffTreffer(GameObject _schiff)
    {
        foreach (Schiffe schiff in schiffe)
        {
            if (schiff.ship == _schiff)
            {
                schiff.treffer++;
            }
        }

    }

    public void SchiffVersenkt(GameObject _schiff)
    {
        foreach (Schiffe schiff in schiffe)
        {
            if (schiff.ship == _schiff)
            {
                if (activateGeisterSchiff)
                {
                    //activeGeisterschiff.GetComponent<GeisterSchiff>().Upgrade();
                }
                schiff.versenkt++;
            }
        }

    }

    void Update()
    {
        if (spielModus == SpielModus.Teambattle && gameRunning == true)
        {
            TeamBattlestandings();
        }
    }

    private void TeamBattlestandings()
    {
        int team1 = 0;
        int team2 = 0;
        foreach (Schiffe schiff in schiffe)
        {
            if (schiff.cahtId == 4)
            {
                team1 += schiff.ship.GetComponent<CircleSkript>().health;
            }
            if (schiff.cahtId == 5)
            {
                team2 += schiff.ship.GetComponent<CircleSkript>().health; ;
            }
        }
        float healthPercentage = 0;
        healthPercentage = (float)team1 / (team1 + team2);
        activeTeamBattleGrafik.GetComponentInChildren<Slider>().value = healthPercentage;
        Debug.Log("Team 1: " + team1 + " Team 2: " + team2);
    }

    public void DebugStats()
    {
        foreach (Schiffe schiff in schiffe)
        {
            Debug.Log("Schiff: " + schiff.users + " Treffer: " + schiff.treffer + " Versenkt: " + schiff.versenkt);
        }
    }
}
