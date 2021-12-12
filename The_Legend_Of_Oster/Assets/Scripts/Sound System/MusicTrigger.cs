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
        if (other.CompareTag("Player") && !triggerSong.Equals(MusicManager.clip))
        {
            
            Debug.Log("Switching song to " + triggerName);
            MusicManager.FadeToSong(triggerSong, MusicManager.fadeTime, MusicManager.maxVol);
        }
    }
}
