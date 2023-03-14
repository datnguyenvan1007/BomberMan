using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
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
}
