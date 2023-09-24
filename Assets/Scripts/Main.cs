using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayedEnemySpawn());
    }

    IEnumerator DelayedEnemySpawn()
    {
        yield return new WaitForSeconds(5f);
        MapCreator.Instance.SpawnEnemyAndSendPath();
    }
}