using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DestroyByTime());
    }
    private IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(0.4f);
        ExplosionSpawner.Instance.Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Player" && (GameData.flamePass == 0 && GameData.mystery == 0))
        {
            collision.gameObject.GetComponent<Player>().Die();
        }
        if (tag == "Enemy")
            collision.gameObject.GetComponent<Enemy>().Die();
    }
    
}
