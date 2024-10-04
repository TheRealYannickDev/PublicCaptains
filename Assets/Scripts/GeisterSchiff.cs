using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeisterSchiff : MonoBehaviour
{
    private CircleSkript player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CircleSkript>();
    }


    public void Steer(float direction)
    {
        player.Steer(direction);
    }
    

    public void Shoot()
    {
        player.Shoot();
    }

    public void Upgrade(){
        player.UpgradeKanone();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Seele")
        {
            Destroy(other.gameObject);
            Upgrade();
        }
    }
}
