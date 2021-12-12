using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    
    [SerializeField] int musicVolume;
    [SerializeField] AudioClip[] music;
    public static AudioSource musicsource;
    public static float fadeTime = 1.5f;
    public static float maxVol;
    public static AudioClip clip;
    int idx = 0;
    // Start is called before the first frame update
    void Awake()
    {
        musicsource = GetComponent<AudioSource>();
        maxVol = musicsource.volume;
        musicsource.clip = music[idx]; // hard wired the first track
        musicsource.loop = true;
        FadeToSong(music[0],fadeTime, maxVol);
        //musicsource.Play();
    }

    public static void FadeToSong(AudioClip song,float fadetime, float maxVol)
    {
        // Song changes should always invoke this and NOT be changing the clip variables


        while (musicsource.volume > 0)
        {
            musicsource.volume -= maxVol * Time.deltaTime / fadeTime;
        }

        musicsource.Stop();
        //swap out the audio clip here. At this point VOL = 0
        // now play it and while the volume is less than maxVol do the reverse 
        musicsource.clip = song;
        musicsource.Play();
        while (musicsource.volume < maxVol)
        {
            musicsource.volume += maxVol * Time.deltaTime / fadeTime;   
        }

        clip = musicsource.clip; // passes the clip upwards to the static var
    }
/*
    public static IEnumerator FadeToSong(AudioClip song, AudioSource musicsource, float fadeTime, float maxVol)
    {
        while (musicsource.volume > 0)
        {
            musicsource.volume -= maxVol * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicsource.Stop();
        //swap out the audio clip here. At this point VOL = 0
        // now play it and while the volume is less than maxVol do the reverse 
        musicsource.clip = song;
        musicsource.Play();
        while (musicsource.volume < maxVol)
        {
            musicsource.volume += maxVol * Time.deltaTime / fadeTime;
            yield return null;
        }

        yield return 0;
            
    }
*/
    // Update is called once per frame
    void FixedUpdate()
    {
        //AnalyzePlayerContext();

        if (false)
        {
            
         // Testing for music swapping mechanism   
            if (idx == music.Length - 1)
            {
                idx = 0;
            }
            else
            {
                idx++;
            }

            Debug.Log("Swapping Songs");
            //StartCoroutine(FadeToSong(music[idx], musicsource, fadeTime, maxVol));
        }

        if (false)
        {
            if (musicsource.volume > 0)
            {
                musicsource.volume -= 0.05f;
                maxVol = musicsource.volume;
                Debug.Log("- Music Vol: " + musicsource.volume);
            }
        }

        if (false)
        {
            if (musicsource.volume < 1)
            {
                musicsource.volume += 0.05f;
                maxVol = musicsource.volume;
                Debug.Log("+ Music Vol: " + musicsource.volume);
            }
        }
    }

    private void AnalyzePlayerContext()
    {
        throw new NotImplementedException();
    }
}
