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
        state.goalPos = new Vector2(3,3);
        state.treePos = new List<Vector2>() {
            new Vector2(2, 2)
        };
        SolveLevel(state, 5);
    }

    public void SolveLevel(BoardState state, int size) {
        // Build graph
        boardSize = size;
        Graph graph = new Graph(size);
        // Create nodes
        for (int i=1; i<boardSize-1; i++) {
            for (int j=1; j<boardSize-1; j++) {
                if (!state.treePos.Contains(new Vector2(i,j))) {
                    Node node = new Node(i, j);
                    graph.AddNode(i, j, node);
                    Debug.Log("Add " + i + " " + j);
                }
            }
        }
        // Make edges
        foreach (KeyValuePair<int, Node> item in graph.nodes) {
            Node node = item.Value;

            // left
            int xPos = node.xPos;
            while (xPos >= 1 &&
                !state.treePos.Contains(new Vector3(xPos, node.yPos))) {
                    xPos--;
            }
            if (xPos != node.xPos)
                graph.GetNode(xPos, node.yPos).neighbours.Add(node);
            // right
            xPos = node.xPos;
            while (xPos < boardSize - 1 &&
                !state.treePos.Contains(new Vector3(xPos, node.yPos))) {
                    xPos++;
            }
            if (xPos != node.xPos)
                graph.GetNode(xPos, node.yPos).neighbours.Add(node);
            // up
            int yPos = node.yPos;
            while (yPos >= 1 &&
                !state.treePos.Contains(new Vector3(node.xPos, yPos))) {
                    yPos--;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos).neighbours.Add(node);
            // down
            yPos = node.yPos;
            while (yPos < boardSize - 1 &&
                !state.treePos.Contains(new Vector3(node.xPos, yPos))) {
                    yPos++;
            }
            if (yPos != node.yPos)
                graph.GetNode(node.xPos, yPos).neighbours.Add(node);
        }

        foreach (KeyValuePair<int, Node> item in graph.nodes) {
            Node node = item.Value;
            foreach (Node neighbour in node.neighbours) {
                Debug.Log(node.xPos + " " + node.yPos +
                " , " + neighbour.xPos + " " + neighbour.yPos);
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
        Debug.Log("Get: " + x + " " + y);
        return nodes[y*boardSize + x];
    }
}

public class Node {
    // Store all possible previous positions
    public int xPos;
    public int yPos;

    public Node(int x, int y) {
        xPos = x;
        yPos = y;
    }

    public List<Node> neighbours = new List<Node>();
}
