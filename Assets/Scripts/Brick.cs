using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private Animator anim;
    private new Collider2D collider;
    public static Brick instance;
    private GameObject objectCovered = null;
    private void Start()
    {
        instance = this;
        collider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        if (GameData.wallPass == 1 || GameData.hackWallPass)
        {
            collider.isTrigger = true;
        }
    }
    public void Destroy()
    {
        anim.Play("Broken");
        Invoke("Disable", 0.3f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
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
            objectCovered.GetComponent<Items>().ActiveCollider();
    }
}
