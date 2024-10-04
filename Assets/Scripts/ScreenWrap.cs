using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenWrap : MonoBehaviour
{
    public bool isEnabled = true;
      private Camera mainCamera;
    [SerializeField]private Vector2 screenBounds;

    void Start()
    {
        FindObjectOfType<WolkenSpawner>().OnWolkenSpawned.AddListener(() => isEnabled = false);
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        //screenBounds = new Vector2(9, 5);
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;

        if (transform.position.x > screenBounds.x)
        {
            if(!isEnabled)
            {
                gameObject.SetActive(false);
                return;
            }
            newPosition.x = -screenBounds.x;
        }
        else if (transform.position.x < -screenBounds.x)
        {
            if(!isEnabled)
            {
                gameObject.SetActive(false);
                return;
            }
            newPosition.x = screenBounds.x;
        }

        if (transform.position.y > screenBounds.y)
        {
            if(!isEnabled)
            {
                gameObject.SetActive(false);
                return;
            }
            newPosition.y = -screenBounds.y;
        }
        else if (transform.position.y < -screenBounds.y)
        {
            if(!isEnabled)
            {
                gameObject.SetActive(false);
                return;
            }
            newPosition.y = screenBounds.y;
        }

        transform.position = newPosition;
    }
}
