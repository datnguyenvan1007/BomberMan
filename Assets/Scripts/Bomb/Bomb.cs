using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private new Collider2D collider;
    private void Start() {
        collider = GetComponent<Collider2D>();
    }
    private void OnEnable()
    {
        if (GameData.detonator == 0)
        {
            StartCoroutine(DestroyByTime());
        }
    }
    private IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(2f);
        Explode();
    }
    public void Explode()
    {
        collider.isTrigger = true;
        BombSpawner.Instance.Destroy(gameObject);
        AudioManager.Instance.PlayAudioBoom();
        ExplosionSpawner.Instance.Explode(transform);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (collision.gameObject.tag == "Player")
        {
            if (GameData.bombPass == 0)
                collider.isTrigger = false;
        }

    }
}
