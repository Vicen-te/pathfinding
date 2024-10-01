// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using UnityEngine;
using Type = Pathfinding.Heuristics.Type;

namespace Pathfinding
{
    /// <summary>
    /// Implements the A* algorithm combined with Dijkstra's algorithm for pathfinding.
    /// This class efficiently finds the shortest path from a start node to an end node
    /// on a grid-based board by evaluating neighboring nodes and updating their scores
    /// based on movement costs and heuristic estimates.
    /// </summary>
    public class AStarDijkstra : Pathfinding
    {
        /// <summary>
        /// Holds the g-scores (movement cost from start node) for each node.
        /// Key: Node, Value: Movement cost.
        /// </summary>
        private Dictionary<Square, float> gDictionary;
        
        /// <summary>
        /// Holds the f-scores (g + h) for each node.
        /// Key: Node, Value: Estimated total cost.
        /// </summary>
        private Dictionary<Square, float> fDictionary;
        
        /// <summary>
        /// Set of nodes that have been fully evaluated.
        /// </summary>
        private HashSet<Square> closedSet;

        /// <summary>
        /// Serialized field to choose the distance type (heuristic) for the algorithm.
        /// </summary>
        [SerializeField] private Type distance;

        /// <summary>
        /// Calculates the heuristic value h(n) based on the chosen distance type.
        /// This method helps estimate the cost to reach the goal from a given node.
        /// </summary>
        private float CalculateH(Square node)
        {
            return distance switch
            {
                Type.Manhattan => Heuristics.ManhattanDistance(node.GetPosition, EndNode.GetPosition),
                Type.Euclidean => Heuristics.EuclideanDistance(node.GetPosition, EndNode.GetPosition),
                Type.DiagonalChebyshev or Type.DiagonalOctile => 
                    Heuristics.DiagonalDistance(node.GetPosition, EndNode.GetPosition, distance),
                _ => throw new NotImplementedException() //< Throw exception for unsupported types.
            };
        }

        /// <summary>
        /// Initializes the algorithm, setting up the g, f dictionaries and the closed set.
        /// </summary>
        public override void Initialize(Square startNode, Square endNode)
        {
            base.Initialize(startNode, endNode);
            gDictionary = new Dictionary<Square, float>();
            fDictionary = new Dictionary<Square, float>();
            closedSet = new HashSet<Square>();

            // Initialize the g and f values for the start node.
            gDictionary.Add(startNode, 0);
            fDictionary.Add(startNode, CalculateH(startNode));
        }

        /// <summary>
        /// Retrieves the node with the lowest f-score from unvisited nodes.
        /// </summary>
        /// <returns>A KeyValuePair containing the node and its f-value.</returns>
        private KeyValuePair<Square, float> GetLowestFScoreNode()
        {
            float minFScore = float.PositiveInfinity; //< Initialize to a large number.
            KeyValuePair<Square, float> minNode = new(null, float.PositiveInfinity); //< Default node.

            // Filter the gDictionary to get nodes that have not been visited.
            IEnumerable<KeyValuePair<Square,float>> unvisitedNodes = gDictionary.Where(node => !closedSet.Contains(node.Key));

            // Iterate through the not visited nodes to find the one with the minimum f-score.
            foreach (KeyValuePair<Square, float> node in unvisitedNodes)
            {
                float fScore = fDictionary[node.Key];
                if (fScore < minFScore)
                {
                    minFScore = fScore;
                    minNode = node;
                }
                
                // Log current node information for debugging.
                Debug.Log($"getLeastFCell: {node.Key.GetPosition}, f:{fScore}, minFScore:{minFScore}");
            }

            return minNode; //< Return the node with the lowest f-value.
        }
        
        /// <summary>
        /// Core method that executes the pathfinding logic using the A* & Dijkstra's algorithm.
        /// It explores neighboring nodes and updates their scores based on the current node.
        /// </summary>
        protected override void GetPath(Board.Board board, GameObject[,] visualBoard)
        {
            KeyValuePair<Square, float> currentNode = GetLowestFScoreNode(); //< Get the node with the minimum distance.
            if (currentNode.Key == null) //< If no valid node is found, finish the algorithm.
            {
                Finished();
                return;
            }

            Debug.Log(currentNode.Key.GetPosition); //< Log the current node's position.
            closedSet.Add(currentNode.Key); //< Mark the current node as visited.
            SetSearchingPathColor(visualBoard, currentNode.Key); //< Update the visual representation of the path.

            // Check if the current node is the goal node.
            if (IsAtEndNode(currentNode.Key))
            {
                Finished(); //< If reached the goal, finish the search.
                return;
            }
            
            // Retrieve the neighboring nodes of the current node.
            Square[] neighbors =
                (from Square node in board.GetFloorNeighbours(currentNode.Key)
                    where node != null select node).ToArray();

            // Update scores for each neighboring node based on the current node.
            foreach (Square neighbor in neighbors)
            {
                float dist = float.MaxValue; //< Initialize to a large value.
                if (gDictionary.ContainsKey(neighbor)) dist = gDictionary[neighbor]; //< Get current g-value if exists.

                // Skip nodes that have already been visited or if the current path is not better.
                if (closedSet.Contains(neighbor) || dist <= currentNode.Value + 1) continue;
                
                // Update g and f values for the neighbor.
                float gScore = currentNode.Value + 1; //< Increment cost for moving to this node

                gDictionary[neighbor] = gScore; // Increment cost for moving to this node.
                fDictionary[neighbor] = gScore + CalculateH(neighbor); // Update f value.
                Parent[neighbor.Id] = currentNode.Key; // Set the parent node for path reconstruction.
            }
        }
    }
}
