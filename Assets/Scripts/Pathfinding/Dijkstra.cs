// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// Dijkstra pathfinding algorithm: A simpler version of A* algorithm that
    /// doesn't use heuristics and focuses only on finding the shortest path 
    /// from a start node to an end node by expanding nodes in order of distance.
    /// </summary>
    public class Dijkstra : Pathfinding 
    {
        /// <summary>
        /// Set of nodes that have already been visited (processed).
        /// </summary>
        private HashSet<Square> visitedNodes;
        
        /// <summary>
        /// Dictionary to store the distance (cost) from the start node to each node.
        /// Key: Node, Value: Distance from start node.
        /// </summary>
        private Dictionary<Square, float> nodeDistances;

        /// <summary>
        /// Initializes the Dijkstra algorithm by setting up the distance dictionary and visited nodes set.
        /// </summary>
        /// <param name="startNode">The start node of the pathfinding algorithm.</param>
        /// <param name="endNode">The goal node of the pathfinding algorithm.</param>
        public override void Initialize(Square startNode, Square endNode)
        {
            base.Initialize(startNode, endNode);
            visitedNodes = new HashSet<Square>();
            nodeDistances = new Dictionary<Square, float>
            {
                { startNode, 0 }
            };
        }
        
        /// <summary>
        /// Retrieves the node with the smallest distance that hasn't been visited yet.
        /// This function mimics a priority queue by manually checking all nodes.
        /// </summary>
        /// <returns>A KeyValuePair containing the node with the minimum distance and its associated distance.</returns>
        private KeyValuePair<Square, float> GetClosestUnvisitedNode() //< Simulates priority queue functionality.
        {
            KeyValuePair<Square, float> minNode = new(null, float.MaxValue); //< Initialize minNode with a large value.
            
            // Iterate over all nodes in the distance dictionary to find the unvisited node with the smallest distance.
            foreach (KeyValuePair<Square, float> node in nodeDistances)
            {
                if(node.Value < minNode.Value && !visitedNodes.Contains(node.Key))
                {
                    minNode = node; //< Update the minimum node.
                }
            }
            return minNode; //< Return the node with the minimum distance value.
        }
        
        /// <summary>
        /// The main pathfinding logic that explores neighbors and updates their distances.
        /// This method is executed in each step of the pathfinding algorithm.
        /// </summary>
        /// <param name="board">The game board containing all the nodes (squares).</param>
        /// <param name="visualBoard">A 2D array of game objects representing the visual representation of the board.</param>
        protected override void GetPath(Board.Board board, GameObject[,] visualBoard)
        {
            // Get the node with the smallest  distance (next node to explore).
            KeyValuePair<Square, float> currentNode = GetClosestUnvisitedNode();
            
            // If no node is returned (i.e., no solution found), exit the method.
            if(currentNode.Key == null) return;

            // Mark the current node as visited.
            visitedNodes.Add(currentNode.Key);
            
            // Update the visual representation of the current node to show it's being explored.
            SetSearchingPathColor(visualBoard, currentNode.Key);

            // Check if the current node is the goal node.
            if (IsAtEndNode(currentNode.Key))
            {
                Finished();  //< Finish the search if the goal has been reached.
                return;
            }
        
            // Retrieve neighboring nodes of the current node from the board.
            Square[] neighbors = board.GetFloorNeighbours(currentNode.Key);

            // Iterate over each neighboring node.
            foreach (Square neighbor in neighbors)
            {
                // Skip null nodes (no node or obstacle).
                if (neighbor == null) continue;
            
                // Initialize distance value to a large number if the node is not already in the distance dictionary.
                float dist = float.MaxValue;
                if (nodeDistances.ContainsKey(neighbor)) dist = nodeDistances[neighbor];

                // If the node has already been visited or the current path is not better, skip it.
                if (visitedNodes.Contains(neighbor) || dist <= currentNode.Value + 1) continue;
                
                // Update the distance for this neighbor (current node's distance + 1 step).
                nodeDistances[neighbor] = currentNode.Value + 1;
                
                // Update the parent of this node for path reconstruction later.
                Parent[neighbor.Id] = currentNode.Key;
            }
        }
    }
}
