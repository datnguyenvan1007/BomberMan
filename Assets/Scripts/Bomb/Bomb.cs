using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private bool isExploded = false;
    private new Collider2D collider;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }
    private void OnEnable()
    {
        isExploded = false;
        if (GameData.detonator == 0 && !GameData.hackDetonator)
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
        isExploded = true;
        PoolBomb.instance.Despawn(gameObject);
        AudioManager.instance.PlayAudioBoom();
        PoolExplosion.instance.Explode(transform);
        collider.isTrigger = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (GameData.bombPass == 0 && !GameData.hackBombPass && !isExploded)
                collider.isTrigger = false;
        }

    }
}
