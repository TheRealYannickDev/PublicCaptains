using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class WolkenSpawner : MonoBehaviour
{
    public List<BoxCollider2D> FinalBoxen = new List<BoxCollider2D>();
    public SpriteRenderer Schatten;
    public UnityEvent OnWolkenSpawned;
    public GameObject Blitz;
    public List<GameObject> WolkenParents = new List<GameObject>();
    public List<Sprite> WolkeSprites = new List<Sprite>();
    Vector2 spawn1 = new Vector2(10, 16);
    Vector2 spawn2 = new Vector2(28, 5.5f);

    List<GameObject> Wolken = new List<GameObject>();
    List<Vector2> WolkenStartPos = new List<Vector2>();
    List<Vector2> WolkenEndPos = new List<Vector2>();
    List<Vector2> WolkenFinalPos = new List<Vector2>();
    List<Vector2> WolkenStartScale = new List<Vector2>();
    List<Vector2> WolkenEndScale = new List<Vector2>();
    public GameObject Wolke;
    private float timer = 0;

    private float endTimer = 0;
    private float animTimer = 0;

    private bool wolklenActive = false;

    private bool wolkenEndPhase = false;
    private bool blitzeAktiv = false;
    List<GameObject> cantBeHit = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //SpawnWolken();
        StartCoroutine(StartWolken());
        instWolken();
    }

    private IEnumerator StartWolken()
    {
        yield return new WaitForSeconds(160);
        wolklenActive = true;
        yield return new WaitForSeconds(100);
        wolkenEndPhase = true;
    }

    private void instWolken()
    {
        for (int i = 0; i < WolkenParents.Count; i++)
        {
            WolkenStartPos.Add(WolkenParents[i].transform.position);
            foreach (Transform child in WolkenParents[i].transform)
            {
                // Child-Objekt zur Liste hinzufügen
                Wolken.Add(child.gameObject);
                child.GetComponentInChildren<SpriteRenderer>().sprite = WolkeSprites[Random.Range(0, WolkeSprites.Count)];
                WolkenStartScale.Add(child.transform.localScale);
                WolkenEndScale.Add(child.transform.localScale);
            }
        }
        WolkenEndPos.Add(WolkenStartPos[0] + (Vector2.up * -2));
        WolkenEndPos.Add(WolkenStartPos[1] + (Vector2.up * 2));
        WolkenEndPos.Add(WolkenStartPos[2] + (Vector2.right * -2));
        WolkenEndPos.Add(WolkenStartPos[3] + (Vector2.right * 2));
        WolkenFinalPos.Add(WolkenStartPos[0] + (Vector2.up * -3.5f));
        WolkenFinalPos.Add(WolkenStartPos[1] + (Vector2.up * 3.5f));
        WolkenFinalPos.Add(WolkenStartPos[2] + (Vector2.right * -3.5f));
        WolkenFinalPos.Add(WolkenStartPos[3] + (Vector2.right * 3.5f));
    }

    public void SpawnWolken()
    {
        int vorzeichen = Random.Range(0, 2) == 0 ? -1 : 1;
        for (int i = 0; i < 200; i++)
        {
            vorzeichen = Random.Range(0, 2) == 0 ? -1 : 1;
            if (Random.Range(0, 2) == 0)
            {
                Wolken.Add(Instantiate(Wolke, new Vector2(Random.Range(spawn1.x * -1, spawn1.x), spawn1.y * vorzeichen), Quaternion.identity));
                WolkenStartPos.Add(Wolken[Wolken.Count - 1].transform.position);
                WolkenEndPos.Add(Wolken[Wolken.Count - 1].transform.position + (Vector3.up * vorzeichen * -1 * Random.Range(0.5f, 1f)));
            }
            else
            {
                Wolken.Add(Instantiate(Wolke, new Vector2(spawn2.x * vorzeichen, Random.Range(spawn2.y * -1, spawn2.y)), Quaternion.identity));
                WolkenStartPos.Add(Wolken[Wolken.Count - 1].transform.position);
                WolkenEndPos.Add(Wolken[Wolken.Count - 1].transform.position + (Vector3.right * vorzeichen * -1 * Random.Range(0.5f, 1f)));
            }

            WolkenStartScale.Add(Wolken[Wolken.Count - 1].transform.localScale);
            WolkenEndScale.Add(Wolken[Wolken.Count - 1].transform.localScale);
            Wolken[Wolken.Count - 1].GetComponentInChildren<SpriteRenderer>().sprite = WolkeSprites[Random.Range(0, WolkeSprites.Count)];
        }
    }
    void Update()
    {
        if (wolklenActive == false)
        {
            return;
        }
        if (timer < 11)
            moveWOlken();
        AnimateWolken();
        if (wolkenEndPhase && endTimer < 11)
        {
            MoveFinalWolken();
        }
    }


    private void moveWOlken()
    {
        for (int i = 0; i < WolkenParents.Count; i++)
        {
            WolkenParents[i].transform.position = Vector2.Lerp(WolkenStartPos[i], WolkenEndPos[i], timer / 10);
            Schatten.color = new Color(0, 0, 0, Mathf.Lerp(0, 0.4f, timer / 10));
        }

        timer += Time.deltaTime;

        if (timer > 10)
        {
            blitzeAktiv = true;
            //OnWolkenSpawned.Invoke();
        }
    }
    private void MoveFinalWolken()
    {
        for (int i = 0; i < WolkenParents.Count; i++)
        {
            WolkenParents[i].transform.position = Vector2.Lerp(WolkenEndPos[i], WolkenFinalPos[i], endTimer / 10);
        }

        endTimer += Time.deltaTime;
        if (endTimer > 10)
        {
            foreach (BoxCollider2D box in FinalBoxen)
            {
                box.enabled = true;
            }
        }
    }

    private void AnimateWolken()
    {
        if (animTimer == 0)
        {
            for (int i = 0; i < WolkenEndScale.Count; i++)
            {
                WolkenEndScale[i] = new Vector2(Random.Range(.8f, 1.2f), Random.Range(.8f, 1.2f));
            }
        }
        for (int i = 0; i < Wolken.Count; i++)
        {
            Wolken[i].transform.GetChild(0).localScale = Vector2.Lerp(WolkenStartScale[i], WolkenEndScale[i], animTimer / 5);
        }
        if (animTimer < 5)
        {
            animTimer += Time.deltaTime;
        }
        else

        {
            for (int i = 0; i < WolkenEndScale.Count; i++)
            {
                WolkenStartScale[i] = WolkenEndScale[i];
            }

            animTimer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ship")
        {
            if (cantBeHit.Contains(other.gameObject))
            {
                return;
            }
            cantBeHit.Add(other.gameObject);
            StartCoroutine(reSetCantBeHit(other.gameObject));
            if (blitzeAktiv)
            {
                Quaternion rot = Quaternion.Euler(0, 0, 0);
                Vector3 posAdd = Vector3.zero;
                if (other.transform.root.position.y < -2.5f)
                {
                    posAdd = Vector3.up * -10;
                    rot = Quaternion.Euler(0, 0, 180);
                    other.transform.root.GetComponent<CircleSkript>().Steer(0);
                }
                else if (other.transform.root.position.y > 2.5f)
                {
                    posAdd = Vector3.up * +10;
                    rot = Quaternion.Euler(0, 0, 0);
                    other.transform.root.GetComponent<CircleSkript>().Steer(180);
                }
                else if (other.transform.root.position.x < -5.5)
                {
                    posAdd = Vector3.right * -17.9f;
                    rot = Quaternion.Euler(0, 0, 90);
                    other.transform.root.GetComponent<CircleSkript>().Steer(90);
                }
                else if (other.transform.root.position.x > 5.5)
                {
                    posAdd = Vector3.right * 17.9f;
                    rot = Quaternion.Euler(0, 0, 270);
                    other.transform.root.GetComponent<CircleSkript>().Steer(270);
                }

                other.transform.root.GetComponent<CircleSkript>().TakeDamage();


                Instantiate(Blitz, other.transform.position + posAdd, rot);
                AudioManager.Instance.Play("Blitz" + Random.Range(0, 4));
            }
        }
    }

    private IEnumerator reSetCantBeHit(GameObject ship)
    {
        yield return new WaitForSeconds(2);
        cantBeHit.Remove(ship);
    }
}

