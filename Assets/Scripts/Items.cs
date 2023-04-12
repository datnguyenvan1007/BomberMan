using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemysPrefab;
    private new Collider2D collider;
    int index;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }
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
            index = Random.Range(0, enemysPrefab.Count);
            PoolEnemy.instance.Spawn(enemysPrefab[index], transform.position);
        }
        if (gameObject.tag == "Items")
            gameObject.SetActive(false);
    }
    public void Appear()
    {
        collider.enabled = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3.5f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D col in colliders)
        {
            col.GetComponent<Enemy>().CheckImpediment("Items");
        }
    }
    public void Hide() {
        collider.enabled = false;
    }
}
