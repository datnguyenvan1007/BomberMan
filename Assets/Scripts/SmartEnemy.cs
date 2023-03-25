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
        ReadyAI();
    }

    protected override void Update()
    {
        if (isDead)
            return;
        ReadyAI();
        if (pathToPlayer.Count == 0)
        {
            if (hasJustSwitchedToNormal)
            {
                /*direction = GetDirection();*/
                nextPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                oldPosition = nextPosition;
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
    }
    void ReadyAI()
    {
        if (!player.activeSelf)
            return;
        if (!isMoving && Vector2.Distance(transform.position, player.transform.position) <= limitedDistanceCanFindPlayer)
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
}
