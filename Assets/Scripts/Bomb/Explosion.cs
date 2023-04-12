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
        yield return new WaitForSeconds(0.3f);
        PoolExplosion.instance.Despawn(gameObject);
    }
}
