using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSolver : MonoBehaviour
{
    int boardSize;

    // Test
    void Start() {
        BoardState state = new BoardState();
        state.selfPos = new Vector2(1,1);
        state.goalPos = new Vector2(3,2);
        //state.goalPos = new Vector2(4,2); // TODO doesn't work with wall
        state.treePos = new List<Vector2>() {
            new Vector2(3, 1),
            new Vector2(2, 3),
        };
        SolveLevel(state, 5);
    }

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
        foreach (KeyValuePair<int, Node> item in graph.nodes) {
            Node node = item.Value;
            if (node.xPos == 0 || node.xPos == boardSize-1 ||
                node.yPos == 0 || node.yPos == boardSize-1)
                continue;

            // left
            int xPos = node.xPos;
            while (xPos >= 2 &&
                !state.treePos.Contains(new Vector2(xPos-1, node.yPos))) {
                    xPos--;
            }
            if (xPos != node.xPos)
                graph.GetNode(xPos, node.yPos)?.neighbours.Add(node);

            // right
            xPos = node.xPos;
            while (xPos < boardSize - 2 &&
                !state.treePos.Contains(new Vector2(xPos+1, node.yPos))) {
                    xPos++;
            }
            if (xPos != node.xPos)
                graph.GetNode(xPos, node.yPos)?.neighbours.Add(node);
            // up
            int yPos = node.yPos;
            while (yPos >= 2 &&
                !state.treePos.Contains(new Vector2(node.xPos, yPos-1))) {
                    yPos--;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos)?.neighbours.Add(node);
            // down
            yPos = node.yPos;
            while (yPos < boardSize - 2 &&
                !state.treePos.Contains(new Vector2(node.xPos, yPos+1))) {
                    yPos++;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos)?.neighbours.Add(node);
        }

        // Handle goal in wall
        Vector2 goalPos = state.goalPos;
        if ((goalPos.x == 0 || goalPos.x == boardSize) &&
            (goalPos.y == 0 || goalPos.y == boardSize)) {
            if (goalPos.x == 0) {
                if (goalPos.y == 0 || goalPos.y == boardSize)
                    Debug.Log("No solution");
            }
            else if (goalPos.x == boardSize) {
                if (goalPos.y == 0 || goalPos.y == boardSize)
                    Debug.Log("No solution");
            }
            Node goalNode = graph.GetNode((int)goalPos.x, (int)goalPos.y);
            if (goalPos.x == 0) {
                if (state.treePos.Contains(new Vector2(1, goalPos.y)))
                    Debug.Log("No solution");
                graph.GetNode(1, (int)goalPos.y).neighbours.Add(goalNode);
            }
            if (goalPos.y == 0) {
                if (state.treePos.Contains(new Vector2(goalPos.x, 1)))
                    Debug.Log("No solution");
                graph.GetNode((int)goalPos.x, 1).neighbours.Add(goalNode);
            }
            if (goalPos.x == boardSize-1) {
                if (state.treePos.Contains(new Vector2(boardSize-2, goalPos.y)))
                    Debug.Log("No solution");
                graph.GetNode(boardSize-2, (int)goalPos.y).neighbours.Add(goalNode);
            }
            if (goalPos.y == boardSize-1) {
                if (state.treePos.Contains(new Vector2(goalPos.x, boardSize-2)))
                    Debug.Log("No solution");
                graph.GetNode((int)goalPos.x, boardSize-2).neighbours.Add(goalNode);
            }
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
