using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NF
public class PlayerAudioAgent : MonoBehaviour
{
    enum Material
    {
        unsure,
        dirt,
        grass,
        wood,
        stone
    }

    public AudioSource playerSource;
    public AudioClip[] fs;
    public float fs_volscale = 0f;
    Color raycol;
    [SerializeField] float hcomp;
    [SerializeField] float raylen;
    [SerializeField] Material steppingOn;
    Animator anim;
    int fs_idx= 0;
    

    void Start()
    {
        anim = GetComponent<Animator>();
        raycol = Color.white;
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
        position.y += hcomp; // compensate height
        Debug.DrawRay(position, Vector3.down, raycol, raylen);
        if (Physics.Raycast(position, Vector3.down, out hit, raylen))
        {
            //Debug.Log(hit.transform.name);
            string groundtag = hit.transform.tag;
            //Debug.Log(groundtag);
            switch (groundtag)
            {
                case "dirt":
                    if (steppingOn.ToString() == "dirt")
                        break;
                    else
                        Debug.Log("On Dirt!");

                    raycol = Color.black;
                    steppingOn = Material.dirt;
                    break;
                case "grass":
                    if (steppingOn.ToString() == "grass")
                        break;
                    else
                        Debug.Log("On Grass!");
                        
                    raycol = Color.green;
                    steppingOn = Material.grass;
                    break;
                case "wood":
                    if (steppingOn.ToString() == "wood")
                        break;
                    else
                        Debug.Log("On Wood!");

                    raycol = Color.yellow;
                    steppingOn = Material.wood;
                    break;

                case "stone":
                    if (steppingOn.ToString() == "stone")
                        break;
                    else
                        Debug.Log("On Stone!");
                    raycol = Color.blue;
                    steppingOn = Material.stone;
                    break;

                default:
                    if (steppingOn.ToString() != "unsure")
                        Debug.Log("Unsure..");
                    raycol = Color.white;
                    steppingOn = Material.unsure;
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
