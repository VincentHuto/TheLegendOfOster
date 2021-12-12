using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NF
public class PlayerAudioAgent : MonoBehaviour
{
    enum GroundMaterial
    {
        unsure,
        dirt,
        grass,
        wood,
        stone
    }

    public AudioSource playerSource;
    public AudioClip[] fs_grass;
    public AudioClip[] fs_stone;
    public AudioClip[] fs_wood;
    public AudioClip[] fs_dirt;
    public float fs_volscale;

    Color raycol;
    [SerializeField] float hcomp; // Height compensation for the raycast origin point
    [SerializeField] float raylen; // length of the raycast
    [SerializeField] GroundMaterial steppingOn;
    Animator anim;
    
    

    void Start()
    {
        anim = GetComponent<Animator>();
        raycol = Color.white;
    }
    public void PlayFS()
    {
        // only invoked via animation
        AudioClip[] fsarr; // reference variable for the correct list of footstepts which are stored in this class variables per each material
        int fs_idx = 0;
        switch (steppingOn)
        {

            case GroundMaterial.grass:
                fsarr = fs_grass;
                fs_idx = UnityEngine.Random.Range(0, fsarr.Length); // after setting fs dataset to the correct one, pick a random fs
                break;
            case GroundMaterial.wood:
                fsarr = fs_wood;
                fs_idx = UnityEngine.Random.Range(0, fsarr.Length); // after setting fs dataset to the correct one, pick a random fs
                break;

            case GroundMaterial.dirt:
                fsarr = fs_dirt; 
                fs_idx = UnityEngine.Random.Range(0, fsarr.Length); // after setting fs dataset to the correct one, pick a random fs
                break;

            case GroundMaterial.stone:
                fsarr = fs_stone;
                fs_idx = UnityEngine.Random.Range(0, fsarr.Length); // after setting fs dataset to the correct one, pick a random fs
                break;

            default:
                fsarr = fs_stone;
                fs_idx = UnityEngine.Random.Range(0, fsarr.Length); // after setting fs dataset to the correct one, pick a random fs
                break;
        }

            playerSource.PlayOneShot(fsarr[fs_idx], fs_volscale);
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
                    steppingOn = GroundMaterial.dirt;
                    break;
                case "grass":
                    if (steppingOn.ToString() == "grass")
                        break;
                    else
                        Debug.Log("On Grass!");
                        
                    raycol = Color.green;
                    steppingOn = GroundMaterial.grass;
                    break;
                case "wood":
                    if (steppingOn.ToString() == "wood")
                        break;
                    else
                        Debug.Log("On Wood!");

                    raycol = Color.yellow;
                    steppingOn = GroundMaterial.wood;
                    break;

                case "stone":
                    if (steppingOn.ToString() == "stone")
                        break;
                    else
                        Debug.Log("On Stone!");
                    raycol = Color.blue;
                    steppingOn = GroundMaterial.stone;
                    break;

                default:
                    if (steppingOn.ToString() != "unsure")
                        Debug.Log("Unsure..");
                    raycol = Color.white;
                    steppingOn = GroundMaterial.unsure;
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
