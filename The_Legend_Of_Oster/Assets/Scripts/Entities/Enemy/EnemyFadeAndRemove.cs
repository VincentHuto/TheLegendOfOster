using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFadeAndRemove : MonoBehaviour
{

    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public List<Material> enemyMaterials;
    public float spawnTime;
    public EnemyStats enemyStats;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            enemyMaterials.Add(skinnedMeshRenderers[i].material);
        }
        spawnTime = Time.time;
    }

    void Update()
    {
        if (enemyStats.isDead && enemyMaterials[0] != null)
        {
            if (enemyMaterials[0].color.a <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                enemyMaterials.ForEach(mat =>
                {
                    if (mat.color.a >= 0)
                    {
                        Color newColor = mat.color;
                        newColor.a -= Time.deltaTime;
                        mat.color = newColor;
                    }
                });
            }
        }

    }

}