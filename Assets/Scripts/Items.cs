using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private new Collider2D collider;
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
            PoolEnemy.instance.Spawn(enemy, transform.position);
        }
        if (gameObject.tag == "Items")
            gameObject.SetActive(false);
    }

    public void ActiveCollider()
    {
        StartCoroutine(ActiveColliderDelay());
    }

    private IEnumerator ActiveColliderDelay()
    {
        yield return new WaitForSeconds(0.5f);
        collider.enabled = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3, LayerMask.GetMask("Enemy"));
        foreach (Collider2D col in colliders)
        {
            col.GetComponent<Enemy>().CheckImpediment("Items");
        }
    }
}
