using UnityEngine;
using System.Collections.Generic;

public static class EnemyCreator
{
    public static GameObject enemyPrefab;
    public static int initialPoolSize = 5;

    private static List<GameObject> pooledEnemies = new List<GameObject>();
    private static GameObject prototypeEnemy;

    public static void Initialize()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab not set before initialization!");
            return;
        }

        prototypeEnemy = Object.Instantiate(enemyPrefab);
        prototypeEnemy.SetActive(false);

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject newEnemy = ClonePrototypeEnemy();
            pooledEnemies.Add(newEnemy);
        }
    }

    public static void SetEnemyPrefab(GameObject prefab)
    {
        enemyPrefab = prefab;
    }

    public static GameObject GetPooledEnemy()
    {
        foreach (GameObject enemy in pooledEnemies)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        GameObject newEnemy = ClonePrototypeEnemy();
        pooledEnemies.Add(newEnemy);
        return newEnemy;
    }

    private static GameObject ClonePrototypeEnemy()
    {
        GameObject newEnemy = Object.Instantiate(prototypeEnemy);
        newEnemy.SetActive(false);
        return newEnemy;
    }
}