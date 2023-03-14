using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBomb : MonoBehaviour
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
        Explosion.Instance.Explode(transform);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.GetComponent<Collider2D>().isTrigger = false;
    }
}
