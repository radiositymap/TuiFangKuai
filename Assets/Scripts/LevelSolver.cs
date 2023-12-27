using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSolver : MonoBehaviour
{
    int boardSize;

    public bool SolveLevel(BoardState state, int size) {
        // Build graph
        boardSize = size;
        Graph graph = new Graph(size);
        // Create nodes
        for (int i=0; i<boardSize; i++) {
            for (int j=0; j<boardSize; j++) {
                if (!state.HasTree(i,j)) {
                    Node node = new Node(i, j);
                    graph.AddNode(i, j, node);
                }
            }
        }
        // Make edges
        Vector2Int goalPos = Vector2Int.RoundToInt(state.goalPos);
        foreach (KeyValuePair<int, Node> item in graph.nodes) {
            Node node = item.Value;
            if (node.xPos == 0 || node.xPos == boardSize-1 ||
                node.yPos == 0 || node.yPos == boardSize-1)
                continue;

            // border limit
            int leftLimit = goalPos == new Vector2(0, node.yPos) ? goalPos.x: 1;
            int rightLimit = goalPos == new Vector2(boardSize-1, node.yPos) ?
                boardSize-1: boardSize-2;
            int topLimit = goalPos == new Vector2(node.xPos, 0) ? goalPos.y: 1;
            int bottomLimit = goalPos == new Vector2(node.xPos, boardSize-1) ?
                boardSize-1: boardSize-2;

            // left
            int xPos = node.xPos;
            while (xPos > leftLimit && !state.HasTree(xPos-1, node.yPos)) {
                    xPos--;
                    if (xPos == goalPos.x && node.yPos == goalPos.y)
                        break;
            }
            if (xPos != node.xPos) {
                graph.GetNode(xPos, node.yPos)?.neighbours.Add(node);
            }

            // right
            xPos = node.xPos;
            while (xPos < rightLimit && !state.HasTree(xPos+1, node.yPos)) {
                    xPos++;
                    if (xPos == goalPos.x && node.yPos == goalPos.y)
                        break;
            }
            if (xPos != node.xPos)
                graph.GetNode(xPos, node.yPos)?.neighbours.Add(node);

            // up
            int yPos = node.yPos;
            while (yPos > topLimit && !state.HasTree(node.xPos, yPos-1)) {
                    yPos--;
                    if (yPos == goalPos.y && node.xPos == goalPos.x)
                        break;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos)?.neighbours.Add(node);

            // down
            yPos = node.yPos;
            while (yPos < bottomLimit && !state.HasTree(node.xPos, yPos+1)) {
                    yPos++;
                    if (yPos == goalPos.y && node.xPos == goalPos.x)
                        break;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos)?.neighbours.Add(node);
        }

        // Solve using BFS - reverse search from goal to user
        Dictionary<Vector2Int, bool> visited =
            new Dictionary<Vector2Int, bool>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        visited[goalPos] = true;
        queue.Enqueue(goalPos);

        bool foundPlayer = false;
        Node currentNode;
        Vector2Int selfPos = Vector2Int.RoundToInt(state.selfPos);
        while (queue.Count > 0) {
            Vector2Int nodePos = queue.Dequeue();
            if (nodePos == selfPos) {
                foundPlayer = true;
                break;
            }
            currentNode = graph.GetNode(nodePos.x, nodePos.y);
            foreach (Node neighbour in currentNode.neighbours) {
                Vector2Int pos = neighbour.GetPos();
                if (!visited.ContainsKey(pos) || !visited[pos]) {
                    neighbour.prevNode = currentNode;
                    visited[pos] = true;
                    queue.Enqueue(pos);
                }
            }
        }

        if (foundPlayer) {
            Node selfNode = graph.GetNode(selfPos.x, selfPos.y);
            currentNode = selfNode;
            List<Vector2Int> path = new List<Vector2Int>();
            while (currentNode != null && currentNode.GetPos() != goalPos) {
                path.Add(currentNode.GetPos());
                if (currentNode.prevNode != null) {
                    currentNode = currentNode.prevNode;
                }
            }
            path.Add(goalPos);
            PlayBackSolution(path);
        }
        return foundPlayer;
    }

    void PlayBackSolution(List<Vector2Int> path) {
        CubeController cubeController = FindObjectOfType<CubeController>();
        for (int i=1; i<path.Count; i++) {
            if (path[i].x < path[i-1].x)
                cubeController.SimulateMotion(-1, 0);
            else if (path[i].x > path[i-1].x)
                cubeController.SimulateMotion(1, 0);
            else if (path[i].y < path[i-1].y)
                cubeController.SimulateMotion(0, -1);
            else if (path[i].y > path[i-1].y)
                cubeController.SimulateMotion(0, 1);
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

    public Vector2Int GetPos() {
        return new Vector2Int(xPos, yPos);
    }

    public List<Node> neighbours = new List<Node>();
}
