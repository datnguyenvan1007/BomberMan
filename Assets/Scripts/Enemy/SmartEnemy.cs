using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : Enemy
{
    [Range(1, 2)]
    [SerializeField] private int smart = 1;
    private float limitedDistanceCanFindPlayer;
    private List<Vector2> pathToPlayer = new List<Vector2>();
    private bool isMoving = false;
    private bool hasJustSwitchedToNormal = false;
    protected override void Start()
    {
        base.Start();
        limitedDistanceCanFindPlayer = smart == 2 ? 6f : 2.5f;
    }

    protected override void FixedUpdate()
    {
        if (isDead)
            return;
        ReadyAI();
        AI();
        if (pathToPlayer.Count == 0)
        {
            if (hasJustSwitchedToNormal)
            {
                nextPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                //oldPosition = nextPosition;
                Vector2 dir = nextPosition - (Vector2)transform.position;
                dir.Normalize();
                SetAnimationOfMovement(dir);
                hasJustSwitchedToNormal = false;
            }
            // ChangeDirectionByDistance();
            Move();
        }
    }
    void AI()
    {
        if (!player.activeSelf || !isMoving)
            return;
        if (Vector2.Distance(transform.position, pathToPlayer[pathToPlayer.Count - 1]) > 0.0f)
        {
            if (CheckBomb())
            {
                isMoving = false;
                return;
            }
            transform.position = Vector2.MoveTowards(transform.position, pathToPlayer[pathToPlayer.Count - 1], speed * Time.fixedDeltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    void ReadyAI()
    {
        if (!player.activeSelf)
        {
            pathToPlayer.Clear();
            return;
        }
        if (!isMoving && Vector2.Distance(transform.position, player.transform.position) <= limitedDistanceCanFindPlayer)
        {
            pathToPlayer = PathFinder.GetPath(transform.position, player.transform.position, isThroughBrick);
            if (pathToPlayer.Count > 0) {
                isMoving = true;
                //set animation
                Vector2 dir = pathToPlayer[pathToPlayer.Count - 1] - (Vector2)transform.position;
                dir.Normalize();
                SetAnimationOfMovement(dir);
                direction = dir;
            }
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
}
