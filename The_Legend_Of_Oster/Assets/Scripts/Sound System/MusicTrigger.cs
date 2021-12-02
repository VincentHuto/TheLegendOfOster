using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public string triggerName;
    public AudioClip triggerSong;
    private static AudioClip song;

    private void Start()
    {
        song = triggerSong;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Debug.Log("Switching song to " + triggerName);
            StartCoroutine(MusicManager.FadeToSong(triggerSong, MusicManager.musicsource, MusicManager.fadeTime, MusicManager.maxVol));
        }
    }
}
