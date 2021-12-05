using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    float tick;
    float tickmax = 5; // delay between each playoneshot. 
    [SerializeField] AudioClip [] badsounds;
    [SerializeField] AudioSource asrc;
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent<AudioSource>(out asrc);
    }

    // Update is called once per frame
    void Update()
    {
        if (tick < tickmax)
            tick += Time.deltaTime;
        else
        {
            Debug.Log("Playing enemy idle");
            AudioClip chosen = badsounds[Random.Range(0, badsounds.Length)];
            tick = 0; // reset timer
            asrc.PlayOneShot(chosen);
        }
    }
}
