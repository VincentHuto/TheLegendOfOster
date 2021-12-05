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
    AudioSource music2D;
    public enum MusicState
    {
        Inside, Outside, Combat
    }

    private void Awake()
    {
        state = MusicState.Inside;
        music2D = GetComponent<AudioSource>();
        music2D.clip = lowIntensity_clip;
        music2D.Play();
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
        if (Input.GetKeyDown(KeyCode.F1))
            m_intensity = 1;

        if (Input.GetKeyDown(KeyCode.Tilde))
            m_intensity = 0;
    }
}
