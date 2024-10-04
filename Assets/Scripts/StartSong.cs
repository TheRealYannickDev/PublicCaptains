using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSong : MonoBehaviour
{
    public String songName;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play(songName);
    }

}
