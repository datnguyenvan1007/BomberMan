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
    protected List<int> distanceCanWalk = new List<int>();
    private RaycastHit2D hit;
    List<Vector2> directions = new List<Vector2>();
    private int MoveHash = Animator.StringToHash("Move");
    private bool isStart;

    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        nextPosition = transform.position;
        oldPosition = transform.position;    
        isStart = true;    
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }
    protected Vector2 GetNextPosition()
    {
        oldPosition = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        nextPosition = oldPosition;
        limitedDistance = Random.Range(1, 6);

        directions.Clear();
        distanceCanWalk.Clear();
        CheckDirection(Vector2.up, limitedDistance);
        CheckDirection(Vector2.down, limitedDistance);
        CheckDirection(Vector2.right, limitedDistance);
        CheckDirection(Vector2.left, limitedDistance);
        isStart = false;

        if (directions.Count > 0)
        {
            int index = Random.Range(0, directions.Count);
            SetAnimationOfMovement(directions[index]);
            direction = directions[index];
            nextPosition += distanceCanWalk[index] * directions[index];
            return nextPosition;
        }
        return nextPosition;
    }
    private void CheckDirection(Vector2 direction, int distance)
    {
        if (!isThroughBrick)
            if (isStart) {
                hit = Physics2D.Raycast(oldPosition, direction, distance, LayerMask.GetMask("Wall", "Brick", "Bomb"));
            }
            else
                hit = Physics2D.Raycast(oldPosition, direction, distance, LayerMask.GetMask("Wall", "Brick", "Bomb", "Items"));
        else
        {
            hit = Physics2D.Raycast(oldPosition, direction, distance, LayerMask.GetMask("Wall", "Bomb"));
        }
        
        if (hit.collider)
        {
            if (Mathf.FloorToInt(hit.distance) == 0)
                return;
            else
            {
                distanceCanWalk.Add(Mathf.FloorToInt(hit.distance));
                directions.Add(direction);
            }
        }
        else
        {
            distanceCanWalk.Add(limitedDistance);
            directions.Add(direction);
        }
    }
    protected void Move()
    {
        if (isDead)
            return;
        if (nextPosition == (Vector2)transform.position)
        {
            nextPosition = GetNextPosition();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.fixedDeltaTime);
        }
    }
    public virtual void CheckImpediment(string impediment)
    {
        float distanceToNextPosition = Vector2.Distance(transform.position, nextPosition);
        RaycastHit2D h = Physics2D.Raycast(transform.position, direction, distanceToNextPosition, LayerMask.GetMask(impediment));
        if (h.collider)
        {
            nextPosition = new Vector2(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y)) - direction;
        }
    }

    protected void SetAnimationOfMovement(Vector2 dir)
    {
        if (dir == Vector2.zero)
            return;
        anim.SetFloat(MoveHash, dir.x + dir.y);
    }

    public void Die()
    {
        if (isDead)
            return;
        isDead = true;
        collider.enabled = false;
        anim.Play("Die");
        UIManager.instance.SetGameScore(score);
        StartCoroutine(PoolEnemy.instance.Despawn(gameObject));
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion")
            Die();
    }

    protected void OnEnable()
    {
        direction = Vector2.zero;
        isDead = false;
        isStart = true;
        oldPosition = transform.position;
        nextPosition = transform.position;
    }
    protected void OnDisable()
    {
        collider.enabled = true;
    }
}