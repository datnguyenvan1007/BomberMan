using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int score;
    [SerializeField] private bool isThroughBrick = false;
    [SerializeField] private int limitDistance = 0;
    [Range(0, 2)]
    [SerializeField] private int smart = 0;
    [SerializeField] private GameObject player;
    private PathFinder pathFinder;
    private Rigidbody2D rig;
    private Animator anim;
    [SerializeField] private List<Vector2> pathToPlayer = new List<Vector2>();
    private bool isMoving;
    [SerializeField] private Vector2 direction;
    private Vector2 oldPosition;

    void Start()
    {
        player = GameObject.Find("Player");
        direction = Vector2.zero;
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (player != null && smart != 0)
        {
            pathFinder = GetComponent<PathFinder>();
            if (smart == 1 && Vector2.Distance(transform.position, player.transform.position) <= 2f)
            {
                pathToPlayer = pathFinder.GetPath(player.transform.position, isThroughBrick);
                ChangeDirectionForAI();
            }
            if (smart == 2 && Vector2.Distance(transform.position, player.transform.position) <= 5f)
            {
                pathToPlayer = pathFinder.GetPath(player.transform.position, isThroughBrick);
                ChangeDirectionForAI();
            }
        }
        if (smart == 0 || !isMoving)
        {
            direction = GetDirection();
            ChangeDirection(direction);
            oldPosition = transform.position;
        }
    }

    void Update()
    {
        if (player.activeSelf)
        {
            ReadyAI();
        }
        else
        {
            pathToPlayer.Clear();
        }
        
        if (smart == 0 || pathToPlayer.Count == 0)
        {
            Ready();
            /*Move();*/
            ChangeDirectionByDistance();
        }
        else
        {
            AI();
        }
    }

    void AI()
    {
        if (player == null || !player.activeSelf)
            return;
        /*if (pathToPlayer.Count == 0)
        {
            pathToPlayer = pathFinder.GetPath(player.transform.position, isThroughBrick);
            isMoving = true;
        }*/
        if (isMoving)
        {
            /*if (Vector2.Distance(transform.position, pathToPlayer[pathToPlayer.Count - 1]) > 0.1f)
            {
                //transform.position = Vector2.MoveTowards(transform.position, pathToPlayer[pathToPlayer.Count - 1], speed * Time.deltaTime);
                
                isMoving = true;
            }*/
            if (Vector2.Distance(transform.position, pathToPlayer[pathToPlayer.Count - 1]) <= 0.09f)
            {
                ChangeDirection(Vector2.zero);
                isMoving = false;
            }
        }
        else
        {
            pathToPlayer = pathFinder.GetPath(player.transform.position, isThroughBrick);
            ChangeDirectionForAI();
        }
    }
    void ReadyAI()
    {
        if (smart == 2)
        {
            if (pathToPlayer.Count == 0 && Vector2.Distance(transform.position, player.transform.position) <= 5f)
            {
                pathToPlayer = pathFinder.GetPath(player.transform.position, isThroughBrick);
                ChangeDirectionForAI();
            }
            if (Vector2.Distance(transform.position, player.transform.position) > 5f)
                pathToPlayer.Clear();
        }
        if (smart == 1)
        {
            if (pathToPlayer.Count == 0 && Vector2.Distance(transform.position, player.transform.position) <= 2f)
            {
                pathToPlayer = pathFinder.GetPath(player.transform.position, isThroughBrick);
                ChangeDirectionForAI();
            }
            if (Vector2.Distance(transform.position, player.transform.position) > 2f)
                pathToPlayer.Clear();
        }
    }
    void ChangeDirectionForAI()
    {
        if (pathToPlayer.Count == 0)
            return;
        Vector2 dir = pathToPlayer[pathToPlayer.Count - 1] - new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        dir.Normalize();
        if (dir != direction)
        {
            ChangeDirection(dir);
        }
        isMoving = true;
    }

    void Ready()
    {
        if (direction != Vector2.zero)
            return;
        ChangeDirection(GetDirection());
    }
    void ChangeDirectionByDistance()
    {
        if (Vector2.Distance(oldPosition, transform.position) >= limitDistance)
        {
            ChangeDirection(GetDirection());
            oldPosition = transform.position;
        }
    }
    Vector2 GetDirection()
    {
        limitDistance = Random.Range(1, 6);
        string[] layerMask;
        if (!isThroughBrick)
            layerMask = new string[] {"Wall", "Brick", "Bomb"};
        else
        {
            layerMask = new string[] { "Wall", "Bomb"};
        }
        List<Vector2> directions = new List<Vector2>();
        if (!Physics2D.Raycast(transform.position, Vector2.up, 0.6f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.up);
        }
        if (!Physics2D.Raycast(transform.position, Vector2.down, 0.6f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.down);
        }
        if (!Physics2D.Raycast(transform.position, Vector2.right, 0.6f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.right);
        }
        if (!Physics2D.Raycast(transform.position, Vector2.left, 0.6f, LayerMask.GetMask(layerMask)))
        {
            directions.Add(Vector2.left);
        }

        if (directions.Count > 0)
        {            
            return directions[Random.Range(0, directions.Count)];
        }
        return Vector2.zero;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (smart == 2 && pathToPlayer.Count != 0)
        {
            return;
        }
        if (smart == 1 && pathToPlayer.Count != 0)
        {
            return;
        }
        if (tag == "Bomb" || tag == "Wall" || (tag == "Brick" && !isThroughBrick))
        {
            ChangeDirection(GetDirection());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Bomb" || (tag == "Brick" && !isThroughBrick))
        {
            ChangeDirection(GetDirection());
        }
        if (tag == "Player" && GameData.mystery == 0)
        {
            collision.gameObject.GetComponent<Player>().Destroy();
            return;
        }
    }
    void ChangeDirection(Vector2 dir)
    {
        //transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
        direction = dir;
        rig.velocity = direction * speed;
        if (dir == Vector2.left || dir == Vector2.right)
            transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), 0);
        else
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, 0);
        SetAnimationOfMovement(dir);
    }
    void SetAnimationOfMovement(Vector2 dir)
    {
        if (dir == Vector2.zero)
            return;
        if (dir == Vector2.up || dir == Vector2.left)
            anim.Play("GoLeft");
        else
            anim.Play("GoRight");
    }

    public void Destroy()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        rig.velocity = Vector2.zero;
        anim.Play("Die");
        UIManager.Instance.SetGameScore(score);
        StartCoroutine(PoolEnemy.Instance.Despawn(gameObject));
    }
}