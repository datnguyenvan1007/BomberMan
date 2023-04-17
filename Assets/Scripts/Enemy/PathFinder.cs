using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    static List<Vector2> pathToTarget;
    static List<Node> close;
    static List<Node> open;
    static Node nodeToCheck;

    // public static List<Vector2> GetPath(Vector2 start, Vector2 target, bool isThroughBrick)
    // {
    //     pathToTarget = new List<Vector2>();
    //     checkedNodes = new List<Node>();
    //     waitingNodes = new List<Node>();

    //     Vector2 startPosition = new Vector2(Mathf.Round(start.x), Mathf.Round(start.y));
    //     Vector2 targetPosition = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));
    //     addStartingPosition = !startPosition.Equals(start);

    //     if (startPosition == targetPosition)
    //         return pathToTarget;
    //     if (!isThroughBrick && Physics2D.OverlapCircle(targetPosition, 0.1f, LayerMask.GetMask("Brick")))
    //         return pathToTarget;
    //     if (Physics2D.OverlapCircle(targetPosition, 0.1f, LayerMask.GetMask("Bomb"))) {
    //         return pathToTarget;
    //     }
    //     if (!isThroughBrick && CheckAround(targetPosition)) 
    //         return pathToTarget;

    //     Node startNode = new Node(0, startPosition, targetPosition, null);
    //     checkedNodes.Add(startNode);
    //     waitingNodes.AddRange(GetNeighbourNodes(startNode));

    //     while (waitingNodes.Count > 0)
    //     {
    //         Node nodeToCheck = waitingNodes.Where(x => x.F == waitingNodes.Min(y => y.F)).FirstOrDefault();
    //         if (nodeToCheck.Position == targetPosition)
    //         {
    //             return CalculatePathFromNode(nodeToCheck);
    //         }
    //         bool walkable;
    //         if (isThroughBrick)
    //         {
    //             walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, LayerMask.GetMask("Wall", "Bomb"));
    //         }
    //         else
    //         {
    //             walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, LayerMask.GetMask("Wall", "Bomb", "Brick"));
    //         }
    //         if (!walkable)
    //         {
    //             waitingNodes.Remove(nodeToCheck);
    //             checkedNodes.Add(nodeToCheck);
    //         }
    //         else
    //         {
    //             waitingNodes.Remove(nodeToCheck);
    //             if (!checkedNodes.Where(x => x.Position == nodeToCheck.Position).Any())
    //             {
    //                 checkedNodes.Add(nodeToCheck);
    //                 waitingNodes.AddRange(GetNeighbourNodes(nodeToCheck));
    //             }
    //         }
    //     }

    //     return pathToTarget;
    // }
    public static List<Vector2> GetPath(Vector2 start, Vector2 target, bool isThroughBrick)
    {
        pathToTarget = new List<Vector2>();
        close = new List<Node>();
        open = new List<Node>();

        Vector2 startPosition = new Vector2(Mathf.Round(start.x), Mathf.Round(start.y));
        Vector2 targetPosition = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));

        if (startPosition == targetPosition)
            return pathToTarget;
        if (!isThroughBrick && Physics2D.OverlapCircle(startPosition, 0.1f, LayerMask.GetMask("Brick", "Items")))
            return pathToTarget;
        if (Physics2D.OverlapCircle(startPosition, 0.1f, LayerMask.GetMask("Bomb")))
        {
            return pathToTarget;
        }

        Node startNode = new Node(0, startPosition, targetPosition, null);
        close.Add(startNode);
        open.AddRange(GetNeighbourNodes(startNode));

        while (open.Count > 0)
        {
            nodeToCheck = open.Where(x => x.F == open.Min(y => y.F)).FirstOrDefault();
            if (nodeToCheck.Position == targetPosition)
            {
                return CalculatePathFromNode(nodeToCheck, target);
            }
            if (nodeToCheck.F >= 8)
                break;
            bool walkable;
            if (isThroughBrick)
            {
                walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, LayerMask.GetMask("Wall", "Bomb"));
            }
            else
            {
                walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, LayerMask.GetMask("Wall", "Bomb", "Brick", "Items"));
            }
            if (!walkable)
            {
                open.Remove(nodeToCheck);
                close.Add(nodeToCheck);
            }
            else
            {
                open.Remove(nodeToCheck);
                if (!close.Where(x => x.Position == nodeToCheck.Position).Any())
                {
                    close.Add(nodeToCheck);
                    open.AddRange(GetNeighbourNodes(nodeToCheck));
                }
            }
        }

        return pathToTarget;
    }
    public static List<Vector2> CalculatePathFromNode(Node node, Vector2 target)
    {
        var path = new List<Vector2>();
        Node currentNode = node;
        //Neu target nam ngoai khoang 2 diem thi them diem bat dau.
        if ((target.x > currentNode.Position.x && target.x > currentNode.PreviousNode.Position.x) ||
            (target.x < currentNode.Position.x && target.x < currentNode.PreviousNode.Position.x) ||
            (target.y > currentNode.Position.y && target.y > currentNode.PreviousNode.Position.y) ||
            (target.y < currentNode.Position.y && target.y < currentNode.PreviousNode.Position.y))
            {
                path.Add(currentNode.Position);
            }
        currentNode = currentNode.PreviousNode;

        while (currentNode.PreviousNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.PreviousNode;
        }
        path.Add(currentNode.Position);

        return path;
    }
    static List<Node> GetNeighbourNodes(Node node)
    {
        var neighbours = new List<Node>();
        neighbours.Add(new Node(node.G + 1, new Vector2(node.Position.x - 1, node.Position.y), node.TargetPosition, node));
        neighbours.Add(new Node(node.G + 1, new Vector2(node.Position.x + 1, node.Position.y), node.TargetPosition, node));
        neighbours.Add(new Node(node.G + 1, new Vector2(node.Position.x, node.Position.y - 1), node.TargetPosition, node));
        neighbours.Add(new Node(node.G + 1, new Vector2(node.Position.x, node.Position.y + 1), node.TargetPosition, node));
        return neighbours;
    }
}

public class Node
{
    public Vector2 Position;
    public Vector2 TargetPosition;
    public Node PreviousNode;
    public int F; //F = G + H
    public int G;
    public int H;
    public Node(int g, Vector2 nodePosition, Vector2 targetPosition, Node previousNode)
    {
        Position = nodePosition;
        TargetPosition = targetPosition;
        PreviousNode = previousNode;
        G = g;
        H = (int)Mathf.Abs(targetPosition.x - Position.x) + (int)Mathf.Abs(targetPosition.y - Position.y);
        F = G + H;
    }
}
