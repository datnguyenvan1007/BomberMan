using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DestroyByTime());
        }
    }
    private IEnumerator DestroyByTime()
    {
        /*gameObject.GetComponent<Collider2D>().isTrigger = true;*/
        yield return new WaitForSeconds(2f);
        BombSpawner.Instance.Destroy(gameObject);
        AudioManager.Instance.PlayAudioBoom();
        ExplosionSpawner.Instance.Explode(transform);
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "player")
            gameObject.GetComponent<Collider2D>().isTrigger = false;
    }*/
}
