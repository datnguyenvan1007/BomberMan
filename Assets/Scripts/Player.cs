using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    private Animator animator;
    private Collider2D coll;
    void Start()
    {
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        BombSpawner.Instance.AddBomb();
    }
    void FixedUpdate()
    {
        Move();
        Bomb();
    }
    void Move()
    {
        float move_x = Input.GetAxisRaw("Horizontal");
        transform.Translate(move_x * speed * Time.fixedDeltaTime, 0, 0);
        if (move_x != 0 && !animator.isActiveAndEnabled)
        {
            animator.enabled = true;
        }
            animator.SetFloat("MoveX", move_x);

        float move_y = Input.GetAxisRaw("Vertical");
        transform.Translate(0, move_y * speed * Time.fixedDeltaTime, 0);
        if (move_y != 0 && !animator.isActiveAndEnabled)
        {
            animator.enabled = true;
        }
            animator.SetFloat("MoveY", move_y);

        if (move_x == 0f && move_y == 0f)
            animator.enabled = false;
    }
    void Bomb()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            coll.isTrigger = true;
            BombSpawner.Instance.Spawn(pos);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bomb"))
            coll.isTrigger = false;
    }
}
