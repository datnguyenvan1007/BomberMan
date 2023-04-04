using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGate : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Explosion" && !Player.isCompleted)
        {
            StartCoroutine(SpawnEnemy());
        }
    }
    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 1; i <= 4; i++)
        {
            PoolEnemy.instance.Spawn(enemy, transform.position);
        }
    }
}
