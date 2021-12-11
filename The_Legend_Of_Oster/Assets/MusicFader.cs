using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFader : MonoBehaviour
{
    // Start is called before the first frame update
    // WARNING: I don't feel like putting the effort into fixing this design but the audio source on the camera holder will only go back up to 0.49
    // WARNING: In order to keep the entire audio mix from changing, the unity mixer is mixed from the perspective of having the audio source vol of a music emitter
    // set at 0.5 from the inspector instead of starting at 1. This is including sources both in world-space and 2D music player.
    AudioSource asrc;
    AudioSource mussrc; // the source in charge of playing overworld music
    [SerializeField] float maxdist; // max distance at which this script will begin to fade out music
    [SerializeField] float mindist; // after getting closer than this, the main music source vol should be 0
    Transform pt;
    void Awake()
    {
        
        asrc = GetComponent<AudioSource>();
        pt = GameObject.Find("PlayerModel").GetComponent<Transform>();
        mussrc = GameObject.Find("CameraHolder").GetComponent<AudioSource>();
        maxdist = mussrc.maxDistance; // sync with the audio source
    }

    private void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, pt.position);
       
        
        // if dist is less than maxdist, first find the ratio that represents the player distance from the fader source
        if (dist < maxdist)
        {
            float ratio = mindist / dist; // ratio becomes 1 as player approaches min dist
            Debug.Log(ratio);
            float newvol = 1 - ratio; // invert the ratio so it becomes smaller as the player approaches and set it to newvol
            mussrc.volume = newvol;
        }
    }
}
