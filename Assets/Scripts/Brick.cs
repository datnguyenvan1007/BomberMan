using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private Animator anim;
    private new Collider2D collider;
    public static Brick instance;
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
        Invoke("Disable", 0.35f);
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
            Collider2D col = Physics2D.OverlapCircle(transform.position, 0.4f, LayerMask.GetMask("EnemyCanThrough"));
            if (col)
            {
                col.gameObject.GetComponent<Enemy>().Die();
            }
            Destroy();
        }
    }
}
