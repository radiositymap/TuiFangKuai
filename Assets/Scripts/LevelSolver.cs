using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSolver : MonoBehaviour
{
    int boardSize;

    public void SolveLevel(BoardState state, int size) {
        // Build graph
        boardSize = size;
        Graph graph = new Graph(size);
        // Create nodes
        for (int i=0; i<boardSize; i++) {
            for (int j=0; j<boardSize; j++) {
                if (!state.treePos.Contains(new Vector2(i,j))) {
                    Node node = new Node(i, j);
                    graph.AddNode(i, j, node);
                }
            }
        }
        // Make edges
        Vector2 goalPos = state.goalPos;
        foreach (KeyValuePair<int, Node> item in graph.nodes) {
            Node node = item.Value;
            if (node.xPos == 0 || node.xPos == boardSize-1 ||
                node.yPos == 0 || node.yPos == boardSize-1)
                continue;

            // border limit
            int leftLimit =
                node.yPos == goalPos.y && goalPos.x == 0 ? (int)goalPos.x: 1;
            int rightLimit =
                node.yPos == goalPos.y && goalPos.x == boardSize-1 ?
                boardSize-1: boardSize-2;
            int topLimit =
                node.xPos == goalPos.x && goalPos.y == 0 ? (int)goalPos.y : 1;
            int bottomLimit =
                node.xPos == goalPos.x && goalPos.y == boardSize-1 ?
                boardSize-1: boardSize-2;

            // left
            int xPos = node.xPos;
            while (xPos > leftLimit &&
                !state.treePos.Contains(new Vector2(xPos-1, node.yPos))) {
                    xPos--;
                    if (xPos == goalPos.x && node.yPos == goalPos.y)
                        break;
            }
            if (xPos != node.xPos) {
                graph.GetNode(xPos, node.yPos)?.neighbours.Add(node);
            }

            // right
            xPos = node.xPos;
            while (xPos < rightLimit &&
                !state.treePos.Contains(new Vector2(xPos+1, node.yPos))) {
                    xPos++;
                    if (xPos == goalPos.x && node.yPos == goalPos.y)
                        break;
            }
            if (xPos != node.xPos)
                graph.GetNode(xPos, node.yPos)?.neighbours.Add(node);
            // up
            int yPos = node.yPos;
            while (yPos > topLimit &&
                !state.treePos.Contains(new Vector2(node.xPos, yPos-1))) {
                    yPos--;
                    if (yPos == goalPos.y && node.xPos == goalPos.x)
                        break;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos)?.neighbours.Add(node);
            // down
            yPos = node.yPos;
            while (yPos < bottomLimit &&
                !state.treePos.Contains(new Vector2(node.xPos, yPos+1))) {
                    yPos++;
                    if (yPos == goalPos.y && node.xPos == goalPos.x)
                        break;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos)?.neighbours.Add(node);
        }

        //foreach (KeyValuePair<int, Node> item in graph.nodes) {
        //    Node node = item.Value;
        //    foreach (Node neighbour in node.neighbours) {
        //        Debug.Log(node.xPos + " " + node.yPos +
        //        " , " + neighbour.xPos + " " + neighbour.yPos);
        //    }
        //}

        // Solve using BFS - reverse search from goal to user
        Dictionary<Vector2, bool> visited = new Dictionary<Vector2, bool>();
        Queue<Vector2> queue = new Queue<Vector2>();

        visited[goalPos] = true;
        queue.Enqueue(goalPos);

        bool foundPlayer = false;
        Node currentNode;
        while (queue.Count > 0) {
            Vector2 nodePos = queue.Dequeue();
            if (nodePos == state.selfPos) {
                foundPlayer = true;
                Debug.Log("Found self!");
                break;
            }
            currentNode = graph.GetNode((int)nodePos.x, (int)nodePos.y);
            foreach (Node neighbour in currentNode.neighbours) {
                Vector2 pos = new Vector2(neighbour.xPos, neighbour.yPos);
                if (!visited.ContainsKey(pos) || !visited[pos]) {
                    neighbour.prevNode = currentNode;
                    visited[pos] = true;
                    queue.Enqueue(pos);
                }
            }
        }

        if (foundPlayer) {
            Node selfNode =
                graph.GetNode((int)state.selfPos.x, (int)state.selfPos.y);
            currentNode = selfNode;
            while (currentNode != null &&
                !(currentNode.xPos == goalPos.x && currentNode.yPos == goalPos.y)) {
                Debug.Log("Path: " + currentNode.xPos + " " + currentNode.yPos);
                if (currentNode.prevNode != null) {
                    currentNode = currentNode.prevNode;
                }
            }
        }
        else {
            Debug.Log("No solution");
        }
    }
}

public class Graph {
    public Dictionary<int, Node> nodes;
    int boardSize;

    public Graph(int dimensions) {
        nodes =  new Dictionary<int, Node>();
        boardSize = dimensions;
    }

    public void AddNode(int x, int y, Node node) {
        nodes.Add(y*boardSize + x, node);
    }

    public Node GetNode(int x, int y) {
        int index = y*boardSize + x;
        if (nodes.ContainsKey(index))
            return nodes[index];
        else
            return null;
    }
}

public class Node {
    // Store all possible previous positions
    public int xPos;
    public int yPos;
    public Node prevNode = null;

    public Node(int x, int y) {
        xPos = x;
        yPos = y;
    }

    public List<Node> neighbours = new List<Node>();
}
