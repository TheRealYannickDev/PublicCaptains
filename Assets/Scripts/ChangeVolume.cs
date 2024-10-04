using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class ChangeVolume : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    public TextMeshProUGUI masterText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI musicText;
    private float audioValue;
    private AudioManager audioManager;
    private int initialUpdate;

    private void Start()
    {
        initialUpdate = 0;
        audioManager = AudioManager.Instance;
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 0.6f);
        masterText.text = (masterSlider.value * 100).ToString("0") + "%";
        UpdateMasterVolume(PlayerPrefs.GetFloat("MasterVol", 0.6f));

        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1);
        UpdateSfxVolume(PlayerPrefs.GetFloat("SFXVol", 1));
        sfxText.text = (sfxSlider.value * 100).ToString("0") + "%";

        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1);
        UpdateSongVolume(PlayerPrefs.GetFloat("MusicVol", 1));
        musicText.text = (musicSlider.value * 100).ToString("0") + "%";


    }
    // Start is called before the first frame update
    public void UpdateSongVolume(float Value)
    {
        audioValue = Mathf.Log10(Value) * 20;
        masterMixer.SetFloat("MusicVol", audioValue);
        PlayerPrefs.SetFloat("MusicVol", Value);
        musicText.text = (Value * 100).ToString("0") + "%";
    }

    public void UpdateSfxVolume(float Value)
    {
        audioValue = Mathf.Log10(Value) * 20;
        masterMixer.SetFloat("SFXVol", audioValue);
        PlayerPrefs.SetFloat("SFXVol", Value);
        sfxText.text = (Value * 100).ToString("0") + "%";
    }

    public void UpdateMasterVolume(float Value)
    {
        audioValue = Mathf.Log10(Value) * 20;
        masterMixer.SetFloat("MasterVol", audioValue);
        PlayerPrefs.SetFloat("MasterVol", Value);
        masterText.text = (Value * 100).ToString("0") + "%";
    }

}
