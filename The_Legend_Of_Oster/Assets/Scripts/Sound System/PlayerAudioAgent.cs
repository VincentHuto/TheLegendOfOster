using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NF
public class PlayerAudioAgent : MonoBehaviour
{
    enum Material
    {
        dirt,
        grass
    }

    public AudioSource playerSource;
    public AudioClip[] fs;
    public float fs_volscale = 0f;
    [SerializeField] Material steppingOn;
    Animator anim;
    int fs_idx= 0;
    

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void PlayFS()
    {
        fs_idx = Random.Range(0, fs.Length );
        playerSource.PlayOneShot(fs[fs_idx], fs_volscale);
        Debug.Log("Played FS " + fs_idx);
    }
    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 position = anim.transform.position;
        position.y += 2; // compensate height
        Debug.DrawRay(position, Vector3.down, Color.green);
        if (Physics.Raycast(position, Vector3.down, out hit, 5f))
        {
            //Debug.Log(hit.transform.name);
            string groundtag = hit.transform.tag;
            //Debug.Log(groundtag);
            switch (groundtag)
            {
                case "dirt":
                    Debug.Log("On Dirt!");
                    break;
                case "grass":
                    Debug.Log("On Grass!");
                    break;
            }

            // switch the states for area and fs sound
        }

        //Debug.Log(anim.velocity.magnitude);
        if (anim.velocity.magnitude > 1)
        {
            fs_volscale = anim.velocity.magnitude / 6; 
            // the velocity . magnitude never goes past 6 when sprinting so result will be between 0-1
        }
        else
        {
            fs_volscale = 0;
        }
    }
}
