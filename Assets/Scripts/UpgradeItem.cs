using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public int type = 0;

    void Start()
    {
        type = Random.Range(0,3);
        if(type == 0)
            GetComponentInChildren<Animator>().SetBool("Kanone", true);
            if(type == 1){
                GetComponentInChildren<Animator>().SetBool("Anker", true);
            
            }
            if(type == 2){
                GetComponentInChildren<Animator>().SetBool("Schild", true);
            }
    }
      
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Kollision: " + other.gameObject.name);
        if(other.tag == "Ship")
        {
            CircleSkript player = other.GetComponentInParent<CircleSkript>();
            if(player != null)
            {
                if(type == 0)
                {
                    player.UpgradeKanone();
                }
                if(type == 1)
                {
                    player.UpgradeLeben();
                }
                if(type == 2)
                {
                    player.UpgradeShield();
                }
                Destroy(gameObject);
            }
        }
    }
}
