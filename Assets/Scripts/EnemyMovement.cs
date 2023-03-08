using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Animator anim;
    private Vector2 direction;
    void Start()
    {
        anim = GetComponent<Animator>();
        direction = GetDirection();
    }

    void FixedUpdate()
    {
        Ready();
        Move();
    }
    void Ready()
    {
        if (direction != Vector2.zero)
            return;
        direction = GetDirection();
    }
    void Move()
    {
        transform.Translate(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime, 0);
    }
    Vector2 GetDirection()
    {
        List<Vector2> directions = new List<Vector2>();
        if (!Physics2D.Raycast(transform.position, Vector2.up, 1.1f, LayerMask.GetMask("Wall")))
        {
            directions.Add(Vector2.up);
        }
        if (!Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Wall")))
        {
            directions.Add(Vector2.down);
        }
        if (!Physics2D.Raycast(transform.position, Vector2.right, 1.1f, LayerMask.GetMask("Wall")))
        {
            directions.Add(Vector2.right);
        }
        if (!Physics2D.Raycast(transform.position, Vector2.left, 1.1f, LayerMask.GetMask("Wall")))
        {
            directions.Add(Vector2.left);
        }

        if (directions.Count > 0)
        {
            Vector2 dir = directions[Random.Range(0, directions.Count)];
            SetAnimation(dir);
            return dir;
        }
        return Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            direction = GetDirection();
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        }
    }
    void SetAnimation(Vector2 dir)
    {
        if (dir == Vector2.up || dir == Vector2.left)
            anim.SetFloat("MoveX", -1f);
        else
            anim.SetFloat("MoveX", 1f);
    }
}
