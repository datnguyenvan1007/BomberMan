using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private Animator anim;
    private new Collider2D collider;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        if (GameData.wallPass == 1)
        {
            collider.isTrigger = true;
        }
    }
    private void Update()
    {
        if (collider.isTrigger)
            return;
        if (GameData.wallPass == 1)
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
}
