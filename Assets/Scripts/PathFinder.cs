using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    static List<Vector2> pathToTarget;
    static List<Node> checkedNodes;
    static List<Node> waitingNodes;
    static bool addStartingPosition = false;

    public static List<Vector2> GetPath(Vector2 start, Vector2 target, bool isThroughBrick)
    {
        pathToTarget = new List<Vector2>();
        checkedNodes = new List<Node>();
        waitingNodes = new List<Node>();

        Vector2 startPosition = new Vector2(Mathf.Round(start.x), Mathf.Round(start.y));
        addStartingPosition = !startPosition.Equals(start);
        Vector2 targetPosition = new Vector2(Mathf.Round(target.x), Mathf.Round(target.y));
        if (startPosition == targetPosition)
            return pathToTarget;
        if (!isThroughBrick && Physics2D.OverlapCircle(targetPosition, 0.1f, LayerMask.GetMask("Brick")))
            return pathToTarget;

        Node startNode = new Node(0, startPosition, targetPosition, null);
        checkedNodes.Add(startNode);
        waitingNodes.AddRange(GetNeighbourNodes(startNode));

        while (waitingNodes.Count > 0)
        {
            Node nodeToCheck = waitingNodes.Where(x => x.F == waitingNodes.Min(y => y.F)).FirstOrDefault();
            if (nodeToCheck.Position == targetPosition)
            {
                return CalculatePathFromNode(nodeToCheck);
            }
            bool walkable;
            if (isThroughBrick)
            {
                walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, LayerMask.GetMask("Wall", "Bomb"));
            }
            else
            {
                walkable = !Physics2D.OverlapCircle(nodeToCheck.Position, 0.1f, LayerMask.GetMask("Wall", "Bomb", "Brick"));
            }
            if (!walkable)
            {
                waitingNodes.Remove(nodeToCheck);
                checkedNodes.Add(nodeToCheck);
            }
            else
            {
                waitingNodes.Remove(nodeToCheck);
                if (!checkedNodes.Where(x => x.Position == nodeToCheck.Position).Any())
                {
                    checkedNodes.Add(nodeToCheck);
                    waitingNodes.AddRange(GetNeighbourNodes(nodeToCheck));
                }
            }
        }

        return pathToTarget;
    }
    public static List<Vector2> CalculatePathFromNode(Node node)
    {
        var path = new List<Vector2>();
        Node currentNode = node;

        while (currentNode.PreviousNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.PreviousNode;
        }
        if (addStartingPosition)
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

    private void OnDrawGizmos()
    {
        if (checkedNodes == null)
            return;
        foreach (var item in checkedNodes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector2(item.Position.x, item.Position.y), 0.1f);
        }
        if (pathToTarget != null)
            foreach (var item in pathToTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector2(item.x, item.y), 0.2f);
            }
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
