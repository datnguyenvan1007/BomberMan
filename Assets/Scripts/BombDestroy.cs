using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroy : MonoBehaviour
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
        yield return new WaitForSeconds(2f);
        BombSpawner.Instance.Destroy(gameObject);
    }
}
