using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : Enemy
{
    private List<Vector2> pathToPlayer = new List<Vector2>();
    public bool isMoving = false;
    private Vector2 oldPositionOfPlayer;
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        if (isDead)
            return;
        if (!isMoving || pathToPlayer.Count == 0)
        {
            Move();
        }
        else
        {
            AI();
        }
    }
    void AI()
    {
        FollowPlayerPosition();
        if (pathToPlayer.Count == 0)
            return;
        if (Vector2.Distance(transform.position, pathToPlayer[0]) > 0.0f)
        {
            nextPosition = pathToPlayer[0]; 
            //set animation
            Vector2 dir = nextPosition - (Vector2)transform.position;
            dir.Normalize();
            SetAnimationOfMovement(dir);
            direction = dir;
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.fixedDeltaTime);
        }
        else
        {
            pathToPlayer.RemoveAt(0);
        }
    }
    public override void CheckImpediment(string impediment)
    {
        if (pathToPlayer.Count == 0) {
            base.CheckImpediment(impediment);
            return;
        }
        Vector2 bombPosition = new Vector2(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        if (pathToPlayer.IndexOf(bombPosition) != -1) {
            if (pathToPlayer.IndexOf(bombPosition) == 0)
                nextPosition = nextPosition - direction;
            pathToPlayer.Clear();
            FindPath();
        }
    }
    // void ReadyAI()
    // {
    //     // if (!player.activeSelf)
    //     // {
    //     //     pathToPlayer.Clear();
    //     //     return;
    //     // }
    //     if (!isMoving && Vector2.Distance(transform.position, player.transform.position) <= limitedDistanceCanFindPlayer)
    //     {
    //         pathToPlayer = PathFinder.GetPath(player.transform.position, transform.position, isThroughBrick);
    //         if (pathToPlayer.Count > 0)
    //         {
    //             isMoving = true;
    //             //set animation
    //             Vector2 dir = pathToPlayer[0] - (Vector2)transform.position;
    //             nextPosition = pathToPlayer[0];
    //             dir.Normalize();
    //             SetAnimationOfMovement(dir);
    //             direction = dir;
    //         }
    //     }
    //     if (Vector2.Distance(transform.position, player.transform.position) > limitedDistanceCanFindPlayer)
    //     {
    //         pathToPlayer.Clear();
    //         isMoving = false;
    //     }
    // }
    public void FindPath()
    {
        if (pathToPlayer.Count > 0)
            return;
        pathToPlayer = PathFinder.GetPath(player.transform.position, transform.position, isThroughBrick);
        if (pathToPlayer.Count > 0)
        {
            isMoving = true;
            oldPositionOfPlayer = pathToPlayer[pathToPlayer.Count - 1];
        }
        else {
            isMoving = false;
        }
    }
    public void FollowPlayerPosition()
    {
        if (Mathf.Abs(oldPositionOfPlayer.x - player.transform.position.x) >= 0.5f ||
        Mathf.Abs(oldPositionOfPlayer.y - player.transform.position.y) >= 0.5f)
        {
            if (player.transform.position.x - oldPositionOfPlayer.x >= 0.5f)
                oldPositionOfPlayer.x = oldPositionOfPlayer.x + 1;
            if (player.transform.position.x - oldPositionOfPlayer.x >= -0.5f)
                oldPositionOfPlayer.x = oldPositionOfPlayer.x - 1;
            if (player.transform.position.y - oldPositionOfPlayer.y >= 0.5f)
                oldPositionOfPlayer.y = oldPositionOfPlayer.y + 1;
            if (player.transform.position.y - oldPositionOfPlayer.y >= -0.5f)
                oldPositionOfPlayer.y = oldPositionOfPlayer.y - 1;
            if (pathToPlayer.IndexOf(oldPositionOfPlayer) == -1)
            {
                if (CanWalk(oldPositionOfPlayer))
                    pathToPlayer.Add(oldPositionOfPlayer);
                else
                {
                    pathToPlayer.Clear();
                    isMoving = false;
                }
            }
            else
            {
                pathToPlayer.RemoveAt(pathToPlayer.Count - 1);
            }
        }
    }
    public bool CanWalk(Vector2 pos)
    {
        if (isThroughBrick)
        {
            return !Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("Wall", "Bomb"));
        }
        else
        {
            return !Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("Wall", "Bomb", "Brick"));
        }
    }
    public void ClearPath()
    {
        pathToPlayer.Clear();
    }
    public Vector2 GetPlayerPosition() {
        return player.transform.position;
    }
}
