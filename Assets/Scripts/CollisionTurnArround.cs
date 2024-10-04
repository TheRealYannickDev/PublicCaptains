using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTurnArround : MonoBehaviour
{

    private CircleSkript circleSkript;

void Start()
{
    circleSkript = GetComponentInParent<CircleSkript>();
}

    void OnCollisionEnter2D(Collision2D other)
    {

            circleSkript.TurnAround();
    }
}
