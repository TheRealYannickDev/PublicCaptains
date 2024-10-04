using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchiffsFrackSkript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyAfterTime(5f));
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        float timer = 0;{

            while(timer < time){
                timer += Time.deltaTime;
                yield return null;
            }
        
        }

        timer = 0;
        while(timer < 3){
            timer += Time.deltaTime;
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, timer / 3));
            yield return null;
        }
        Destroy(gameObject);
    }
}
