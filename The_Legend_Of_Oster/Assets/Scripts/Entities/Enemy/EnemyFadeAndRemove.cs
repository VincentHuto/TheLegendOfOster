using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFadeAndRemove : MonoBehaviour
{

    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public List<Material> enemyMaterials;
    public float deathDelay = 2.5f;
     EnemyStats enemyStats;
    [Header("Particle FX")]
    public GameObject deathFX;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            enemyMaterials.Add(skinnedMeshRenderers[i].material);
        }
    }

    void Update()
    {
        if (enemyStats.isDead && enemyMaterials[0] != null)
        {
            deathFX.gameObject.SetActive(true);
            if (enemyMaterials[0].color.a <= 0)
            {
                deathFX.gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                enemyMaterials.ForEach(mat =>
                {
                    if (mat.color.a >= 0)
                    {
                        Color newColor = mat.color;
                        newColor.a -= Time.deltaTime/deathDelay;
                        mat.color = newColor;
                    }
                });
            }
        }

    }

}