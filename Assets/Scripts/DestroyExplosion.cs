using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExplosion : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DestroyByTime());
        }
    }
    private IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(0.5f);
        Explosion.Instance.Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Player")
            collision.gameObject.GetComponent<Player>().Destroy();
        if (tag == "Enemy")
            collision.gameObject.GetComponent<Enemy>().Destroy();
    }
    
}