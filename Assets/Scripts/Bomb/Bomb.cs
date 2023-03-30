using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
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
        BombSpawner.Instance.Destroy(gameObject);
        AudioManager.Instance.PlayAudioBoom();
        ExplosionSpawner.Instance.Explode(transform);
    }
}
