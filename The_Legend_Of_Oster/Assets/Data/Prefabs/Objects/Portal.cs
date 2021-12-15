using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Portal : MonoBehaviour
{
    [SerializeField] string nextLevel;
    bool loading = false;
    
    public void LoadLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !loading)
        {
            loading = true;
            LoadLevel();
        }
    }
}
