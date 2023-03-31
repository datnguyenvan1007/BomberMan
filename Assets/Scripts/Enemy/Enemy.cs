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
    private int GoLeftHash = Animator.StringToHash("GoLeft");
    private int GoRightHash = Animator.StringToHash("GoRight");

    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        collider = gameObject.GetComponent<Collider2D>();
        nextPosition = transform.position;
        oldPosition = transform.position;
    }

    protected virtual void FixedUpdate()
    {
        if (isDead)
            return;
        Move();
    }
    protected Vector2 GetNextPosition()
    {
        oldPosition = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //oldPosition = transform.position;
        nextPosition = oldPosition;
        limitedDistance = Random.Range(1, 6);
        string[] layerMask;
        if (!isThroughBrick)
            layerMask = new string[] { "Wall", "Brick", "Bomb" };
        else
        {
            layerMask = new string[] { "Wall", "Bomb" };
        }
        directions.Clear();
        distanceCanWalk.Clear();
        CheckDirection(Vector2.up, limitedDistance, layerMask);
        CheckDirection(Vector2.down, limitedDistance, layerMask);
        CheckDirection(Vector2.right, limitedDistance, layerMask);
        CheckDirection(Vector2.left, limitedDistance, layerMask);

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
    private void CheckDirection(Vector2 direction, int distance, string[] layerMask)
    {
        hit = Physics2D.Raycast(oldPosition, direction, distance, LayerMask.GetMask(layerMask));
        if (hit.collider)
        {
            if (Mathf.RoundToInt(hit.distance) == 0)
                return;
            else
            {
                distanceCanWalk.Add(Mathf.RoundToInt(hit.distance));
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
        if (nextPosition == (Vector2)transform.position) {
            nextPosition = GetNextPosition();
        } 
        else {
            CheckBomb();
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.fixedDeltaTime);
        }
    }
    protected bool CheckBomb()
    {
        if (!Player.hasJustPutBomb)
            return false;
        float distanceToNextPosition = Vector2.Distance(transform.position, nextPosition);
        RaycastHit2D h = Physics2D.Raycast(transform.position, direction, distanceToNextPosition, LayerMask.GetMask("Bomb"));
        if (h.collider) {
            Debug.Log(h.distance);
            nextPosition = new Vector2(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y)) - direction;
            Player.hasJustPutBomb = false;
            return true;
        }
        return false;
    }
    public void ReverseDirection()
    {
        SetAnimationOfMovement(direction * -1);
        direction = -1 * direction;
        nextPosition = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        nextPosition += direction;
    }

    protected void SetAnimationOfMovement(Vector2 dir)
    {
        if (dir == direction || dir == Vector2.zero)
            return;
        if (dir == Vector2.up || dir == Vector2.left)
            anim.Play(GoLeftHash);
        else
            anim.Play(GoRightHash);
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion")
            Die();
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