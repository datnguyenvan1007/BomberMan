using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemy : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies = null ;
    public int enemyAlive;
    public static PoolEnemy instance;
    void Awake()
    {
        enemyAlive = transform.childCount;
        enemies = new List<GameObject>();
        instance = this;
    }
    public IEnumerator Despawn(GameObject enemy)
    {
        yield return new WaitForSeconds(2f);
        this.enemies.Add(enemy);
        enemy.SetActive(false);
        enemyAlive--;
        
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
            enemy.GetComponent<Collider2D>().enabled = true;
            enemy.transform.position = position;
            enemy.SetActive(true);
        }
        enemyAlive++;
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
