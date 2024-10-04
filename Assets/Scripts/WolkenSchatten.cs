using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolkenSchatten : MonoBehaviour
{
    public Transform wolke;
    public Transform wolkeSchatten;
    void LateUpdate()
    {
        wolkeSchatten.localScale = wolke.localScale;
    }
}
