using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class KanonenKugel : MonoBehaviour
{
    public GameObject shipHitEffect;
    private GameObject shootingShip;
    private Transform shadow;
    public GameObject explosion;
    private int teamId = 0;

    private bool isGeisterKugel = false;

    Vector2 shadowWinkel = new Vector2(-0.075f, -0.075f);
    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.GetChild(0);
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(DestroyAfterTime(1f));
    }

    public void SetShootingShip(GameObject ship)
    {
        shootingShip = ship;
    }

    public void setTeamId(int _teamId)
    {
        teamId = _teamId;
    }

    public void SetPirateKugel()
    {
        isGeisterKugel = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {


        if (other.gameObject.tag == "Ship")
        {
            if (other.transform.root.gameObject == shootingShip)
            {
                return;
            }
            if (teamId != 0 && other.transform.root.gameObject.GetComponent<CircleSkript>().teamId == teamId)
            {
                return;
            }


            Debug.Log("Hit Object: " + other.gameObject.tag + " " + other.gameObject.name);
            if (isGeisterKugel)
            {
                other.transform.root.gameObject.GetComponent<CircleSkript>().UnUpgrade();
            }
            else
            {

                other.transform.root.gameObject.GetComponent<CircleSkript>().TakeDamage(shootingShip);
            }

            HitShipEffect();
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Kanone")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

    }

    private IEnumerator DestroyAfterTime(float time)
    {
        float timer = 0;
        {

            while (timer < time)
            {
                timer += Time.deltaTime;
                shadow.transform.localPosition = shadowWinkel * Mathf.Lerp(12, 0, timer / time);
                yield return null;
            }

        }
        Instantiate(explosion, transform.position, Quaternion.identity);
        AudioManager.Instance.Play("Wasser" + Random.Range(0, 5), 0.2f);
        Destroy(gameObject);
    }

    private void HitShipEffect()
    {
        float angle = rotation(GetComponent<Rigidbody2D>().velocity);
        Instantiate(shipHitEffect, transform.position, Quaternion.Euler(0, 0, angle));
    }

    private float rotation(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        return angle;
    }
}
