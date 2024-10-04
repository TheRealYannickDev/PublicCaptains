using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup audioMixerSounds;
    public AudioMixerGroup audioMixerMusic;
    public Sound[] sounds;
    public List<Sound> music = new List<Sound>();
    public List<Sound> music2 = new List<Sound>();
    private List<AudioSource> audiosources = new List<AudioSource>();
    private List<AudioSource> activeSources = new List<AudioSource>();
    private static AudioManager _instance;
    private bool musicSource = false;
    private AudioSource currentSong;
    private Coroutine fading;


    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();
            }

            return _instance;
        }

    }



    // Start is called before the first frame update
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        foreach (Sound s in music)
        {
            s.volume = 0.606f;
            s.pitch = 1;
        }


        for (int i = 0; i < 22; i++)
        {
            audiosources.Add(gameObject.AddComponent<AudioSource>());
            audiosources[i].outputAudioMixerGroup = audioMixerSounds;
        }

    }


    public void Play(string name, float volume = 0)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            s = music.Find(sound => sound.name == name);
            //s = List.Find(music, sound => sound.name == name);
            if (s != null)
            {
                musicSource = true;
            }
        }
        else
        {
            musicSource = false;

        }
        if (s == null)
        {
            Debug.Log("Couldn't find sound: " + name);
            return;
        }

        foreach (AudioSource audioSource in audiosources)
        {
            if (audioSource.isPlaying == false)
            {
                audioSource.clip = s.clip;
                if (volume != 0)
                {
                    audioSource.volume = volume;
                }
                else
                {
                    audioSource.volume = s.volume;
                }
                audioSource.pitch = s.pitch;
                audioSource.panStereo = s.stereoPan;
                audioSource.loop = s.loop;
                if (musicSource)
                {
                    if (currentSong)
                    {
                        if (audioSource.clip != currentSong.clip || !currentSong.isPlaying)
                        {
                            /*
                            if (fading != null || !currentSong.isPlaying)
                            {
                                StopAllCoroutines();
                                StopAllMusic();
                                currentSong = audioSource;
                                audioSource.outputAudioMixerGroup = audioMixerMusic;
                                audioSource.Play();
                                return;
                            }
                            fading = StartCoroutine(FadeSongs(audioSource));
                            */
                            StartCoroutine(FadeSongs(audioSource));
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }

                    audioSource.outputAudioMixerGroup = audioMixerMusic;
                    currentSong = audioSource;
                }
                else
                {
                    audioSource.outputAudioMixerGroup = audioMixerSounds;
                }
                audioSource.Play();
                return;
            }
        }

    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            s = music2.Find(sound => sound.name == name);
        }
        if (s == null)
        {
            Debug.Log("Couldn't find sound: " + name);
            return;
        }



        foreach (AudioSource audioSource in audiosources)
        {
            if (audioSource.clip == s.clip)
            {
                audioSource.Stop();
            }
        }
    }

    private void StopAllMusic()
    {
        Sound s;
        foreach (AudioSource audioSource in audiosources)
        {
            if (audioSource.isPlaying)
            {
                s = music2.Find(sound => sound.name == name);
                if (s != null)
                {
                    audioSource.Stop();
                }

            }

        }

    }




    public void PauseAudio()
    {
        Debug.Log("PauseAudio");
        foreach (AudioSource audiosource in audiosources)
        {
            if (audiosource.isPlaying)
            {
                activeSources.Add(audiosource);
                audiosource.Pause();
            }
        }

    }

    public void ContinueAudio()
    {
        Debug.Log("ContinueAudio");
        foreach (AudioSource audioSource in activeSources)
        {
            audioSource.Play();
        }
        activeSources.Clear();
        activeSources = new List<AudioSource>();

    }


    private IEnumerator FadeSongs(AudioSource newSong)
    {

        float i = 0;
        float targetvolume = newSong.volume;
        float startVolume = currentSong.volume;
        AudioSource oldsong = currentSong;
        currentSong = newSong;
        currentSong.volume = 0;
        currentSong.outputAudioMixerGroup = audioMixerMusic;
        currentSong.Play();

        while (i < 4)
        {
            currentSong.volume = Mathf.Lerp(0, targetvolume, i / 4);
            oldsong.volume = Mathf.Lerp(startVolume, 0, i / 4);
            i += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        oldsong.Stop();
        fading = null;
    }

    /*
    public void setVolume(float volume, string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.volume = volume;
    }


    public void setPitch(float pitch, string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.pitch = pitch;
    }

    
    */


}
