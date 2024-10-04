using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class StreamerSchiff : MonoBehaviour
{
    CircleSkript player;

    private float playermovingDirection;

    private bool shooting;

    private float delay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CircleSkript>();
        delay = PlayerPrefs.GetFloat("StreamerDelay", 2f);
        if (FindAnyObjectByType<MessageScript>().spielModus == SpielModus.StreamerBattle)
        {
            delay = 0f;
        }
    }

    // Update is called once per frame


    private struct DelayedInput
    {
        public float steerDirection;

        public bool isShooting;
        public float timestamp;

        public DelayedInput(float _steerDirection, bool _isShooting, float timestamp)
        {
            this.isShooting = _isShooting;
            this.steerDirection = _steerDirection;
            this.timestamp = timestamp;
        }
    }
    private Queue<DelayedInput> inputQueue = new Queue<DelayedInput>();



    void Update()
    {
        inputQueue.Enqueue(new DelayedInput(playermovingDirection, shooting, Time.time));
        ProcessDelayedInput();

    }

    private void ProcessDelayedInput()
    {

        while (inputQueue.Count > 0)
        {
            // Erhalte die erste Eingabe in der Warteschlange
            DelayedInput delayedInput = inputQueue.Peek();

            // Überprüfen, ob genug Zeit vergangen ist
            if (Time.time >= delayedInput.timestamp + delay)
            {
                // Entfernen Sie die Eingabe aus der Warteschlange
                inputQueue.Dequeue();
                float value = player._goalAngle;
                value += delayedInput.steerDirection * Time.deltaTime * 100;

                if (value > 360)
                {
                    value -= 360;
                }
                else if (value < -360)
                {
                    value += 360;
                }
                player.Steer(value);

                if (delayedInput.isShooting)
                {
                    player.Shoot();
                }

            }
            else
            {
                // Wenn die Eingabe noch nicht verarbeitet werden soll, beenden Sie die Schleife
                break;
            }
        }



    }



    public void drehen(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            playermovingDirection = context.ReadValue<float>();
        }
        if (context.canceled)
        {
            playermovingDirection = 0;
        }

    }



    public void schießen(InputAction.CallbackContext context)
    {
        Debug.Log("Schießen");
        if (context.performed)
        {

            shooting = true;
        }
        if (context.canceled)
        {
            shooting = false;
        }
    }

    public void OnDrehen()
    {
        Debug.Log("Drehen");
    }
}
