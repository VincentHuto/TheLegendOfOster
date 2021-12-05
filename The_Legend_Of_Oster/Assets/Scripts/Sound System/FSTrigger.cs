using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FSTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioClip[] fs_clips;
    AudioSource asrc;

    private void Awake()
    {
        asrc = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        
        asrc.PlayOneShot(fs_clips[UnityEngine.Random.Range(0,fs_clips.Length)]);
    }
}
