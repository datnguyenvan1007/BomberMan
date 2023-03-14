using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemy : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies ;

    private static PoolEnemy instance;
    public static PoolEnemy Instance { get => instance; }
    void Awake()
    {
        instance = this;
    }
    public IEnumerator Despawn(GameObject enemy)
    {
        yield return new WaitForSeconds(2f);
        this.enemies.Add(enemy);
        enemy.SetActive(false);
    }
    public void Spawn(GameObject enemyPrefab, Vector2 position)
    {
        GameObject enemy = GetEnemyByName(enemyPrefab.name);
        if (enemy == null)
        {
            GameObject e = Instantiate(enemyPrefab, position, Quaternion.identity);
            e.transform.parent = transform;
            e.name = enemyPrefab.name;
        }
        else
        {
            enemy.transform.position = position;
            enemy.SetActive(true);
        }
    }
    private GameObject GetEnemyByName(string name)
    {
        foreach (GameObject e in enemies)
        {
            if (e.name == name)
            {
                this.enemies.Remove(e);
                return e;
            }
        }
        return null;
    }
}
