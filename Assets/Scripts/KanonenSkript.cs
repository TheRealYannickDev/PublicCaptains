using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanonenSkript : MonoBehaviour
{
    public GameObject KanonenKugel;
    public int teamId = 0;

    public void Shoot(float shootStrength, bool isGeisterschiff)
    {
        GameObject kugel = Instantiate(KanonenKugel, transform.position, transform.rotation);
        kugel.GetComponent<KanonenKugel>().setTeamId(teamId);
        kugel.GetComponent<KanonenKugel>().SetShootingShip(gameObject.transform.root.gameObject);
        if (isGeisterschiff)
        {
            kugel.GetComponent<KanonenKugel>().SetPirateKugel();
        }
        kugel.GetComponent<Rigidbody2D>().AddForce(transform.up.normalized * shootStrength, ForceMode2D.Impulse);
    }
}
