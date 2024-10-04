using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CircleSkript : MonoBehaviour
{
    [SerializeField] private GameObject geisterSeele;
    [SerializeField] private GameObject schildUpgrade;
    [SerializeField] private bool isPirateShip = false;
    [SerializeField] private GameObject Explosion;

    [SerializeField] private List<SpriteRenderer> segel = new List<SpriteRenderer>();
    [SerializeField] private List<SpriteRenderer> ankerLeben;
    [SerializeField] private GameObject LebensObject;
    [SerializeField] private GameObject LebensParent;


    [SerializeField] private float shootStrength = 30;
    [SerializeField] private GameObject KanonenKugel;
    [SerializeField] private GameObject DestroyedShip;

    [SerializeField] private List<Sprite> SchiffSprites;
    [SerializeField] private List<Sprite> SchiffSegelSprites;
    [SerializeField] private GameObject SchiffSprite;
    [SerializeField] private GameObject SchiffSchattenSprite;
    [SerializeField] private GameObject SchiffSegelSchattenSprite;
    [SerializeField] private List<ParticleSystem> SchiffPartikel = new List<ParticleSystem>();
    public TextMeshPro nameText;
    public TextMeshPro BefehlText;
    public int id;
    public float speed = 1;
    public float MaxSpeed = 10;

    public int health = 3;
    bool isalive = true;
    private bool shield = false;

    private Rigidbody2D rb;

    private bool canShoot = true;

    [HideInInspector] public bool gameHasStarted = false;


    private Vector3 movingDirection;
    private Coroutine steeringRoutine;

    private int UpgradeStatus = 0;
    [SerializeField] private List<GameObject> SchiffeUpgrades = new List<GameObject>();
    [SerializeField] private SpriteRenderer schiffSchatten;

    [SerializeField] private List<Transform> Kanonen = new List<Transform>();
    private int kanonenanzahl = 2;

    public float _goalAngle;
    private bool isGeisterschiff = false;

    public int teamId = 0;



    // Start is called before the first frame update
    void Start()
    {
        if (isPirateShip)
        {
            pirateUpgardeLife();
            pirateUpgardeLife();
        }
        rb = GetComponent<Rigidbody2D>();
        SchiffSprite.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        foreach (GameObject schiffSpriteObject in SchiffeUpgrades)
        {
            schiffSpriteObject.transform.rotation = Quaternion.Euler(0, 0, SchiffSprite.transform.rotation.eulerAngles.z);
        }
        SchiffSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
        SchiffSegelSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
        if (GetComponent<GeisterSchiff>() != null)
        {
            Debug.Log("Geisteschiff am start!");
            isGeisterschiff = true;
        }
        if (isGeisterschiff)
        {
            return;
        }
        foreach (ParticleSystem partikel in SchiffPartikel)
        {
            partikel.Stop();
        }

    }
    public void startGame()
    {
        gameHasStarted = true;
        if (isGeisterschiff)
        {
            return;
        }
        foreach (ParticleSystem partikel in SchiffPartikel)
        {
            partikel.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameHasStarted)
        {
            forwardforce();
        }
    }

    public void Steer(float goalAngle)
    {
        if (!isalive)
        {
            return;
        }
        if (goalAngle < -360 || goalAngle > 360)
        {
            return;
        }
        if (steeringRoutine != null)
        {
            StopCoroutine(steeringRoutine);
        }
        _goalAngle = goalAngle;
        steeringRoutine = StartCoroutine(SteerBoat(goalAngle * -1));
    }

    public void TurnAround()
    {
        float currentAngle = SchiffSprite.transform.rotation.eulerAngles.z * -1;
        currentAngle += 90;

        if (currentAngle > 360)
        {
            currentAngle -= 360;
        }
        else if (currentAngle < -360)
        {
            currentAngle += 360;
        }
        Debug.Log("Current Angle: " + currentAngle + "PreviousAngle: " + SchiffSprite.transform.rotation.eulerAngles.z);
        Steer(currentAngle);
    }

    public void party()
    {
        Debug.Log("Party");
        if (steeringRoutine != null)
        {
            StopCoroutine(steeringRoutine);
        }

        StartCoroutine(Party());
    }

    private IEnumerator Party()
    {
        Color oldSegelColor = segel[0].color;
        float farbenWechsel = 0;
        float timer = 0;
        float startAngle = SchiffeUpgrades[0].transform.rotation.eulerAngles.z;
        float angle = 999999;
        float newAngle = startAngle;
        while (timer < 5)
        {

            newAngle = Mathf.Lerp(startAngle, angle, timer / 5);
            Debug.Log("StartAngle: " + startAngle + "NewAngle: " + newAngle + "Goal Angle: " + angle);
            foreach (GameObject schiffSpriteObject in SchiffeUpgrades)
            {
                schiffSpriteObject.transform.rotation = Quaternion.Euler(0, 0, newAngle);
            }
            SchiffSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
            SchiffSegelSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
            timer += Time.deltaTime;

            farbenWechsel += Time.deltaTime;
            if (farbenWechsel > 0.2f)
            {
                segel[0].color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                SchiffSprite.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                farbenWechsel = 0;
            }

            yield return null;
        }

        segel[0].color = oldSegelColor;
        SchiffSprite.GetComponent<SpriteRenderer>().color = Color.white;

    }

    private IEnumerator SteerBoat(float angle)
    {
        WrapAngle((int)angle);
        float startAngle = SchiffeUpgrades[0].transform.rotation.eulerAngles.z;
        if (startAngle >= 360)
        {
            startAngle -= 360;
        }
        if (startAngle < 0)
        {
            startAngle += 360;
        }
        float newAngle;
        float timecounter = 0;
        float direction = 1;
        if (startAngle - angle > 180)
        {
            startAngle -= 360;
        }
        else if (startAngle - angle < -180)
        {
            startAngle += 360;
        }

        if (angle > startAngle)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        newAngle = startAngle;
        while (((newAngle - angle < 0) && (direction == 1)) || ((newAngle - angle > 0) && (direction == -1)))
        {
            timecounter += Time.deltaTime;
            newAngle = startAngle + (direction * speed * timecounter * 80);  //Mathf.Lerp(startAngle, angle, Time.deltaTime * speed * 2);
            foreach (GameObject schiffSpriteObject in SchiffeUpgrades)
            {
                schiffSpriteObject.transform.rotation = Quaternion.Euler(0, 0, newAngle);
            }
            SchiffSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
            SchiffSegelSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
            yield return null;
        }
        foreach (GameObject schiffSpriteObject in SchiffeUpgrades)
        {
            schiffSpriteObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        SchiffSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
        SchiffSegelSchattenSprite.transform.localRotation = SchiffSprite.transform.localRotation;
        yield return null;
    }

    int WrapAngle(int angle)
    {
        // Der Modulo-Operator stellt sicher, dass der Winkel innerhalb von 0 bis 359 bleibt
        int wrappedAngle = angle % 360;

        // Wenn der Winkel negativ ist, passe ihn an, um innerhalb von 0 bis 359 zu bleiben
        if (wrappedAngle < 0)
        {
            wrappedAngle += 360;
        }

        return wrappedAngle;
    }

    public void SetName(string name)
    {
        if (name.Length > 8)
        {
            name = name.Substring(0, 8);
        }
        this.nameText.text = name;
    }

    public void SetBefehl(string befehl)
    {
        this.BefehlText.text = befehl;
    }

    //Funktion die die Rotation aus der Bewegung berechnet
    private float rotation(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        return angle;
    }

    //funktion di eaus der Roation eine bewgungs vektor berechnet
    private Vector2 direction(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    //funktion die eine Force in die Richtung der Schiffsrotation gibt
    private void forwardforce()
    {
        rb.velocity = direction(SchiffSprite.transform.rotation.eulerAngles.z + 90) * MaxSpeed;
    }

    public void Shoot()
    {
        if (!isalive || !canShoot)
        {
            return;
        }
        canShoot = false;
        StartCoroutine(Reload());
        foreach (Transform kanone in Kanonen)
        {
            if (!kanone.gameObject.activeSelf)
            {
                continue;
            }
            kanone.GetComponent<KanonenSkript>().teamId = teamId;
            kanone.GetComponent<KanonenSkript>().Shoot(shootStrength, isGeisterschiff);
            //GameObject kugel = Instantiate(KanonenKugel, kanone.position, kanone.rotation);
            //kugel.GetComponent<KanonenKugel>().SetShootingShip(gameObject);
            //kugel.GetComponent<Rigidbody2D>().AddForce((kanone.transform.position - kanone.parent.transform.position).normalized * shootStrength, ForceMode2D.Impulse);
            //Debug.Log("Kugel abgefeuert: " + kugel.GetComponent<Rigidbody2D>().velocity.magnitude + "Local Position: " + kanone.localPosition.normalized + "Shoot Strength: " + shootStrength + "ForceMode2D.Impulse");
        }
        AudioManager.Instance.Play("Shot" + Random.Range(0, 10));
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(2);
        canShoot = true;
    }

    public void TakeDamage()
    {
        if (shield)
        {
            shield = false;
            schildUpgrade.SetActive(false);
            return;
        }
        health--;
        LoseLive();
        StartCoroutine(HitEffect());
        if (health <= 0)
        {
            isalive = false;
            shipDead();
            this.gameObject.SetActive(false);

        }
    }

    public void TakeDamage(GameObject shooter)
    {
        if (shield)
        {
            shield = false;
            schildUpgrade.SetActive(false);
            return;
        }
        health--;
        LoseLive();
        StartCoroutine(HitEffect());
        if (health <= 0)
        {
            FindObjectOfType<MessageScript>().SchiffVersenkt(shooter);
            FindObjectOfType<MessageScript>().SchiffTreffer(shooter);
            shooter.GetComponent<CircleSkript>().UpgradeKanone();
            isalive = false;
            shipDead();
            this.gameObject.SetActive(false);
        }
        else
        {
            FindObjectOfType<MessageScript>().SchiffTreffer(shooter);
            //shooter.GetComponent<CircleSkript>().UpgradeKanone();
        }
    }

    public void UnUpgrade()
    {
        if (shield)
        {
            return;
        }
        if (UpgradeStatus == 0 || kanonenanzahl == 2)
        {
            return;
        }

        if (UpgradeStatus == 1 && kanonenanzahl == 2)
        {
            UpgradeStatus--;
            //UpgradeLeben();
            MaxSpeed = 0.45f;
            speed = 0.8f;
        }
        else if (UpgradeStatus == 1 && kanonenanzahl < 7)
        {
            kanonenanzahl -= 2;

        }
        else if (UpgradeStatus == 1 && kanonenanzahl == 7)
        {
            kanonenanzahl -= 1;
        }
        else if (UpgradeStatus == 2 && kanonenanzahl == 7)
        {
            UpgradeStatus--;
            MaxSpeed = 0.4f;
            speed = 0.6f;
            //UpgradeLeben();
        }
        else if (UpgradeStatus == 2 && kanonenanzahl > 7)
        {
            kanonenanzahl -= 2;
        }
        else if (UpgradeStatus == 3 && kanonenanzahl == 11)
        {
            UpgradeStatus--;
            //UpgradeLeben();
            MaxSpeed = 0.3f;
            speed = 0.4f;
        }
        else if (UpgradeStatus == 3 && kanonenanzahl > 11)
        {
            kanonenanzahl -= 2;
        }
        if (UpgradeStatus != 3)
        {

            SchiffeUpgrades[UpgradeStatus].SetActive(true);
            SchiffeUpgrades[UpgradeStatus + 1].SetActive(false);
        }
        SchiffSegelSchattenSprite.GetComponent<SpriteRenderer>().sprite = SchiffSegelSprites[UpgradeStatus];
        schiffSchatten.sprite = SchiffeUpgrades[UpgradeStatus].GetComponent<SpriteRenderer>().sprite;
        Kanonen.Clear();
        foreach (Transform kanone in SchiffeUpgrades[UpgradeStatus].transform.GetChild(0))
        {
            Kanonen.Add(kanone);
        }
        foreach (Transform kanone in Kanonen)
        {
            kanone.gameObject.SetActive(false);
        }

        for (int i = 0; i < kanonenanzahl; i++)
        {
            Kanonen[i].gameObject.SetActive(true);
        }
    }

    public void UpgradeKanone()
    {
        if (isPirateShip)
        {
            return;
        }
        if (UpgradeStatus == 0)
        {
            UpgradeStatus++;
            //UpgradeLeben();
            MaxSpeed = 0.45f;
            speed = 0.8f;
        }
        else if (UpgradeStatus == 1 && kanonenanzahl < 6)
        {
            kanonenanzahl += 2;

        }
        else if (UpgradeStatus == 1 && kanonenanzahl == 6)
        {
            kanonenanzahl += 1;
        }
        else if (UpgradeStatus == 1 && kanonenanzahl >= 7)
        {
            UpgradeStatus++;
            MaxSpeed = 0.4f;
            speed = 0.6f;
            //UpgradeLeben();
        }
        else if (UpgradeStatus == 2 && kanonenanzahl < 11)
        {
            kanonenanzahl += 2;
        }
        else if (UpgradeStatus == 2 && kanonenanzahl >= 11)
        {
            UpgradeStatus++;
            //UpgradeLeben();
            MaxSpeed = 0.3f;
            speed = 0.4f;
        }
        else if (UpgradeStatus == 3 && kanonenanzahl < 17)
        {
            kanonenanzahl += 2;
        }
        else if (UpgradeStatus == 3 && kanonenanzahl >= 17)
        {
            return;
        }

        SchiffeUpgrades[UpgradeStatus].SetActive(true);
        SchiffeUpgrades[UpgradeStatus - 1].SetActive(false);
        SchiffSegelSchattenSprite.GetComponent<SpriteRenderer>().sprite = SchiffSegelSprites[UpgradeStatus];
        schiffSchatten.sprite = SchiffeUpgrades[UpgradeStatus].GetComponent<SpriteRenderer>().sprite;
        Kanonen.Clear();
        foreach (Transform kanone in SchiffeUpgrades[UpgradeStatus].transform.GetChild(0))
        {
            Kanonen.Add(kanone);
        }
        foreach (Transform kanone in Kanonen)
        {
            kanone.gameObject.SetActive(false);
        }

        for (int i = 0; i < kanonenanzahl; i++)
        {
            Kanonen[i].gameObject.SetActive(true);
        }
        /*
        switch (UpgradeStatus)
        {
            case 1:
                Kanonen[2].gameObject.SetActive(true);
                Kanonen[3].gameObject.SetActive(true);
                break;
            case 2:
                Kanonen[4].gameObject.SetActive(true);
                Kanonen[5].gameObject.SetActive(true);
                break;
            case 3:
                Kanonen[6].gameObject.SetActive(true);
                break;
            default:
                shootStrength++;
                break;
        }
        */
    }

    private void pirateUpgardeLife()
    {
        Instantiate(LebensObject, LebensParent.transform);


        int i = 0;
        // Methode zum Sammeln der Child-Objekte aufrufen
        foreach (Transform child in LebensParent.transform)
        {
            child.transform.localPosition = new Vector3(i * 0.25f, 0.5f, 0);
            i++;
        }
        i--;

        foreach (Transform child in LebensParent.transform)
        {
            child.transform.localPosition = new Vector3(i * -0.125f + child.transform.localPosition.x, 0.5f, 0);

        }
        health++;
    }

    public void UpgradeLeben()
    {
        if (isGeisterschiff || isPirateShip)
        {
            return;
        }

        Instantiate(LebensObject, LebensParent.transform);


        int i = 0;
        // Methode zum Sammeln der Child-Objekte aufrufen
        foreach (Transform child in LebensParent.transform)
        {
            child.transform.localPosition = new Vector3(i * 0.25f, 0.5f, 0);
            i++;
        }
        i--;

        foreach (Transform child in LebensParent.transform)
        {
            child.transform.localPosition = new Vector3(i * -0.125f + child.transform.localPosition.x, 0.5f, 0);

        }
        health++;

    }

    public void LoseLive()
    {
        if (isGeisterschiff)
        {
            return;
        }


        int i = 0;
        // Methode zum Sammeln der Child-Objekte aufrufen
        if (LebensParent.transform.childCount > health)
            Destroy(LebensParent.transform.GetChild(health).gameObject);

        foreach (Transform child in LebensParent.transform)
        {
            child.transform.localPosition = new Vector3(i * 0.25f, 0.5f, 0);
            i++;
        }
        i--;

        foreach (Transform child in LebensParent.transform)
        {
            child.transform.localPosition = new Vector3(i * -0.125f + child.transform.localPosition.x, 0.5f, 0);
        }
    }

    public void UpgradeSegel()
    {
        if (MaxSpeed >= 0.6f)
        {
            return;
        }
        MaxSpeed += .05f;
    }

    public void UpgradeShield()
    {
        if (shield)
        {
            return;
        }
        shield = true;
        schildUpgrade.SetActive(true);
    }

    public void SetSegelColor(string colorHex)
    {
        Debug.Log("Set Segel Color: " + colorHex);
        Color newCol;

        if (ColorUtility.TryParseHtmlString(colorHex, out newCol))
        {
            foreach (SpriteRenderer _segel in segel)
            {
                _segel.color = newCol;
            }

        }
        if(colorHex != "#FFFFFF"){
            nameText.color = segel[0].GetComponent<SpriteRenderer>().color;
        }
        
    }

    public void SetColor(int i)
    {
        if (i == 0)
        {
            nameText.color = Color.red;
        }
        if (i == 1)
        {
            nameText.color = new Color(100f / 255f, 65f / 255f, 165f / 255f);
        }
        if (i == 2)
        {
            nameText.color = Color.black;
        }
        if (i == 3)
        {
            nameText.color = Color.yellow;
        }
        if (i == 4)
        {
            nameText.color = new Color(14f / 255f, 17f / 255f, 125f / 255f);
            //nameText.color = new Color(33f / 255f, 97f / 255f, 18f / 255f);
        }
        if (i == 5)
        {
            nameText.color = new Color(115f / 255f, 8f / 255f, 8f / 255f);
        }
    }

    private IEnumerator HitEffect()
    {
        AudioManager.Instance.Play("Hit" + Random.Range(0, 5));
        SchiffSprite.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        SchiffSprite.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void shipDead()
    {
        if (FindObjectOfType<GeisterSchiff>())
        {
            Instantiate(geisterSeele, transform.position, Quaternion.identity);
        }
        Instantiate(Explosion, transform.position + Vector3.up * 0.6f, Quaternion.identity);
        Instantiate(DestroyedShip, transform.position, Quaternion.identity);
    }
}
