using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private Animator anim;
    private new Collider2D collider;
    private GameObject objectCovered = null;
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    private void OnEnable() {
        if (GameData.wallPass == 1 || GameData.hackWallPass)
        {
            collider.isTrigger = true;
        }
        else {
            collider.isTrigger = false;
        }
    }
    public void Destroy()
    {
        anim.Play("Broken");
        StartCoroutine(PoolBrick.instance.Despawn(gameObject));
    }
    public void SetTrigger(bool isTrigger)
    {
        collider.isTrigger = isTrigger;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Explosion") {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.4f, LayerMask.GetMask("EnemyCanThrough"));
            foreach (Collider2D col in cols)
            {
                col.gameObject.GetComponent<Enemy>().Die();
            }
            Destroy();
        }
    }
    public void SetObjectCovered(GameObject obj) {
        objectCovered = obj;
    }
    private void OnDisable() {
        if (objectCovered != null)
            objectCovered.GetComponent<Items>().Appear();
    }
}
