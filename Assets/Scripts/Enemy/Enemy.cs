using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected int score;
    [SerializeField] protected bool isThroughBrick = false;
    protected Animator anim;
    protected new Collider2D collider;
    protected GameObject player;
    protected int limitedDistance = 0;
    protected bool isDead = false;
    protected Vector2 direction;
    protected Vector2 oldPosition;
    protected Vector2 nextPosition;
    List<Vector2> directions = new List<Vector2>();

    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        collider = gameObject.GetComponent<Collider2D>();
        direction = Vector2.zero;
        nextPosition = transform.position;
        oldPosition = transform.position;
    }

    protected virtual void FixedUpdate()
    {
        if (isDead)
            return;
        /*Ready();*/
        ChangeDirectionByDistance();
    }

    /*protected void Ready()
    {
        if (direction != Vector2.zero)
            return;
        direction = GetDirection();
    }*/
    protected void ChangeDirectionByDistance()
    {
        if (Vector2.Distance(oldPosition, transform.position) >= limitedDistance)
        {
            direction = GetDirection();
        }
        Move();
    }
    protected Vector2 GetDirection()
    {
        Vector2 pos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        nextPosition = pos;
        oldPosition = pos;
        limitedDistance = Random.Range(1, 6);
        string[] layerMask;
        if (!isThroughBrick)
            layerMask = new string[] {"Wall", "Brick", "Bomb"};
        else
        {
            layerMask = new string[] { "Wall", "Bomb"};
        }
        directions.Clear();
        if (!Physics2D.OverlapCircle(pos + Vector2.up, 0.1f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.up);
        }
        if (!Physics2D.OverlapCircle(pos + Vector2.down, 0.1f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.down);
        }
        if (!Physics2D.OverlapCircle(pos + Vector2.right, 0.1f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.right);
        }
        if (!Physics2D.OverlapCircle(pos + Vector2.left, 0.1f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.left);
        }
        if (directions.Count > 0)
        {
            Vector2 dir = directions[Random.Range(0, directions.Count)];
            SetAnimationOfMovement(dir);
            return dir;
        }
        limitedDistance = 0;
        return Vector2.zero;
    }
    protected void Move()
    {
        if (isDead || direction == Vector2.zero)
            return;
        bool walkable = true;
        if (nextPosition == (Vector2)transform.position)
        {
            nextPosition += direction;
            if (isThroughBrick)
            {
                walkable = !Physics2D.OverlapCircle(nextPosition, 0.1f, LayerMask.GetMask("Wall", "Bomb"));
            }
            else
            {
                walkable = !Physics2D.OverlapCircle(nextPosition, 0.1f, LayerMask.GetMask("Wall", "Bomb", "Brick"));
            }
        }
        if (!walkable)
        {
            direction = GetDirection();
        }
        else {
            CheckBomb();
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.fixedDeltaTime);
        }
    }
    protected bool CheckBomb()
    {
        if (Physics2D.Raycast(transform.position, direction, 0.6f, LayerMask.GetMask("Bomb")))
        {
            ReverseDirection();
            return true;
        }
        return false;
    }
    public void ReverseDirection()
    {
        SetAnimationOfMovement(direction * -1);
        direction = -1 * direction;
        nextPosition += direction;
    }
    
    protected void SetAnimationOfMovement(Vector2 dir)
    {
        if (dir == direction)
            return;
        if (dir == Vector2.zero)
            return;
        if (dir == Vector2.up || dir == Vector2.left)
            anim.Play("GoLeft");
        else
            anim.Play("GoRight");
    }

    public void Die()
    {
        if (isDead)
            return;
        isDead = true;
        collider.enabled = false;
        anim.Play("Die");
        UIManager.Instance.SetGameScore(score);
        StartCoroutine(PoolEnemy.Instance.Despawn(gameObject));
    }

    protected void OnEnable()
    {
        direction = Vector2.zero;
        isDead = false;
    }
    protected void OnDisable()
    {
        collider.enabled = true;
    }
}