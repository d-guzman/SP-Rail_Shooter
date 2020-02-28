using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoadingScript : MonoBehaviour {
    public GameObject[] enemiesToLoad;

    public void LoadEnemies()
    {
        foreach (GameObject enemy in enemiesToLoad)
        {
            enemy.SetActive(true);
        }
    }
}
