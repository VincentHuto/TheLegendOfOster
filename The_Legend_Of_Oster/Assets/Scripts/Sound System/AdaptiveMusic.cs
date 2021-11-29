using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveMusic : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AudioClip lowIntensity_clip;
    [SerializeField] AudioClip highIntensity_clip;
    public int m_intensity;
    MusicState state;
    
    public enum MusicState
    {
        Inside, Outside, Combat
    }

    void Start()
    {
        state = MusicState.Inside;   
    }

    void TransitionTo(AudioClip c)
    {

    }

    public void ChangeMusicZone(AudioClip lc, AudioClip hc)
    {
        lowIntensity_clip = lc;
        highIntensity_clip = hc;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
