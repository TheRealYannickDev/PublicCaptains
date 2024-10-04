using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChangingEffekt : MonoBehaviour
{
    public Material waterMaterial;
    private float faderMultiplier = 0.1f;
    private float faderScaler = 0.1f;

    private float specualarThreshhold = -0.7f;
    private float specualarScale = 1f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(changeValues());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator changeValues(){
        faderMultiplier = Random.Range(0.1f, 0.35f);
        faderScaler = Random.Range(0.2f, 0.5f);
        specualarScale = Random.Range(0.5f, 1.8f);
        specualarThreshhold = Random.Range(-.78f, -.63f);
        float oldFaderMultiplier = waterMaterial.GetFloat("_CausticFaderMultiplier");
        float oldFaderScaler = waterMaterial.GetFloat("_CausticFaderScale");
        float oldSpecualarScale = waterMaterial.GetFloat("_SpecularScale");
        float oldSpecualarThreshhold = waterMaterial.GetFloat("_SpecularThreshold");
        float timer = 0;

        while (timer < 10){
            timer += Time.deltaTime;
            waterMaterial.SetFloat("_CausticFaderMultiplier", Mathf.Lerp(oldFaderMultiplier, faderMultiplier, timer/10));
            waterMaterial.SetFloat("_CausticFaderScale", Mathf.Lerp(oldFaderScaler, faderScaler, timer/10));
            waterMaterial.SetFloat("_SpecularScale", Mathf.Lerp(oldSpecualarScale, specualarScale, timer/10));
            waterMaterial.SetFloat("_SpecularThreshold", Mathf.Lerp(oldSpecualarThreshhold, specualarThreshhold, timer/10));
            yield return null;
        }

        StartCoroutine(changeValues());
    }
}
