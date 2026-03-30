using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float respawnTime = 5f;

    private GameObject currentEnemy;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        currentEnemy.GetComponent<Enemy_Health>().Ondeath += HandleDeath; //적이 죽었을때 호출되게 연결
    }

    private void HandleDeath()
    {
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        Spawn();
    }
}
