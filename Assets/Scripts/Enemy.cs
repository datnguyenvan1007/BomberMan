using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int score;
    [SerializeField] private bool isThroughBrick = false;
    [Range(0, 2)]
    [SerializeField] private int smart = 0;
    private int limitDistance = 0;
    private GameObject player;
    private float limitedDistanceCanFindPlayer;
    private Animator anim;
    private new Collider2D collider;
    private List<Vector2> pathToPlayer = new List<Vector2>();
    private bool isMoving = false;
    private bool isDead = false;
    private bool hasJustSwitchedToNormal = false;
    private Vector2 direction;
    private Vector2 oldPosition;
    private Vector2 nextPosition;

    void Start()
    {
        player = GameObject.Find("Player");
        nextPosition = transform.position;
        direction = Vector2.zero;
        anim = GetComponent<Animator>();
        collider = gameObject.GetComponent<Collider2D>();
        if (smart != 0)
        {
            limitedDistanceCanFindPlayer = smart == 2 ? 6f : 2.5f;
            if (Vector2.Distance(transform.position, player.transform.position) <= limitedDistanceCanFindPlayer)
            {
                pathToPlayer = PathFinder.GetPath(transform.position, player.transform.position, isThroughBrick);
            }
            if (pathToPlayer.Count > 0)
                isMoving = true;
        }
        if (smart == 0 || !isMoving)
        {
            direction = GetDirection();
            oldPosition = transform.position;
        }
    }

    void Update()
    {
        if (isDead)
            return;
        if (player.activeSelf && smart != 0)
        {
            ReadyAI();
        }
        
        if (smart == 0 || pathToPlayer.Count == 0)
        {
            if (hasJustSwitchedToNormal)
            {
                direction = GetDirection();
                hasJustSwitchedToNormal = false;
            }
            Ready();
            ChangeDirectionByDistance();
        }
        else
        {
            AI();
        }
    }

    void Ready()
    {
        if (direction != Vector2.zero)
            return;
        direction = GetDirection();
    }
    void ChangeDirectionByDistance()
    {
        if (Vector2.Distance(oldPosition, transform.position) >= limitDistance)
        {
            direction = GetDirection();
        }
        Move();
    }
    Vector2 GetDirection()
    {
        nextPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        oldPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        limitDistance = Random.Range(1, 6);
        string[] layerMask;
        if (!isThroughBrick)
            layerMask = new string[] {"Wall", "Brick", "Bomb"};
        else
        {
            layerMask = new string[] { "Wall", "Bomb"};
        }
        Vector2 pos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        List<Vector2> directions = new List<Vector2>();
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
        return Vector2.zero;
    }
    void Move()
    {
        if (isDead)
            return;
        if (Vector2.Distance(nextPosition, transform.position) == 0f)
        {
            bool walkable;
            nextPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            nextPosition += direction;
            if (isThroughBrick)
            {
                walkable = !Physics2D.OverlapCircle(nextPosition, 0.1f, LayerMask.GetMask("Wall", "Bomb"));
            }
            else
            {
                walkable = !Physics2D.OverlapCircle(nextPosition, 0.1f, LayerMask.GetMask("Wall", "Bomb", "Brick"));
            }
            if (!walkable)
            {
                direction = GetDirection();
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
    }
    void AI()
    {
        if (!player.activeSelf)
            return;
        if (isMoving)
        {
            if (Vector2.Distance(transform.position, pathToPlayer[pathToPlayer.Count - 1]) > 0.0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, pathToPlayer[pathToPlayer.Count - 1], speed * Time.deltaTime);
                Vector2 dir = pathToPlayer[pathToPlayer.Count - 1] - new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                dir.Normalize();
                SetAnimationOfMovement(dir);
                direction = dir;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            pathToPlayer = PathFinder.GetPath(transform.position, player.transform.position, isThroughBrick);
            isMoving = true;
        }
    }
    void ReadyAI()
    {
        if (pathToPlayer.Count == 0 && Vector2.Distance(transform.position, player.transform.position) <= limitedDistanceCanFindPlayer)
        {
            pathToPlayer = PathFinder.GetPath(transform.position, player.transform.position, isThroughBrick);
            if (pathToPlayer.Count > 0)
                isMoving = true;
        }
        if (Vector2.Distance(transform.position, player.transform.position) > limitedDistanceCanFindPlayer)
        {
            if (isMoving)
            {
                hasJustSwitchedToNormal = true;
            }
            pathToPlayer.Clear();
            isMoving = false;
        }
    }
    void SetAnimationOfMovement(Vector2 dir)
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
        isDead = true;
        collider.enabled = false;
        anim.Play("Die");
        UIManager.Instance.SetGameScore(score);
        StartCoroutine(PoolEnemy.Instance.Despawn(gameObject));
    }

    private void OnEnable()
    {
        direction = Vector2.zero;
        isDead = false;
    }
    private void OnDisable()
    {
        collider.enabled = true;
    }
}