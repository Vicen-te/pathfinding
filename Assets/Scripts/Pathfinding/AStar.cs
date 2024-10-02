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
    /// A* (A-star) pathfinding algorithm: An extension of Dijkstra's search algorithm.
    /// Off-line calculation using the formula f(n) = g(n) + h(n), where:
    /// g(n) = the minimum cost from the start node to node n,
    /// h(n) = the heuristic estimate of the minimum cost from node n to the goal node.
    /// </summary>
    public class AStar : Pathfinding
    {
        /// <summary>
        /// Holds the g-scores (movement cost from start node) for each node.
        /// Key: Node ID, Value: Movement cost.
        /// </summary>
        private Dictionary<string, float> gDictionary;
        
        /// <summary>
        /// Holds the f-scores (g + h) for each node.
        /// Key: Node ID, Value: Estimated total cost.
        /// </summary>
        private Dictionary<string, float> fDictionary;
        
        /// <summary>
        /// Serialized field to choose the distance type (heuristic) for the algorithm.
        /// </summary>
        [SerializeField] private Type distance;

        /// <summary>
        /// Set of nodes to be evaluated
        /// </summary>
        private HashSet<Square> openSet; 
        
        /// <summary>
        /// Set of nodes already evaluated
        /// </summary>
        private HashSet<Square> closedSet;

        /// <summary>
        /// Calculates the heuristic value h(n) based on the chosen distance type.
        /// This value estimates the cost from the current node to the goal.
        /// </summary>
        private float CalculateH(Square node)
        {
            return distance switch
            {
                Type.Manhattan => Heuristics.ManhattanDistance(node.GetPosition, EndNode.GetPosition),
                Type.Euclidean => Heuristics.EuclideanDistance(node.GetPosition, EndNode.GetPosition),
                Type.DiagonalChebyshev or Type.DiagonalOctile => 
                    Heuristics.DiagonalDistance(node.GetPosition, EndNode.GetPosition, distance),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Initializes the A* algorithm, setting up open and closed sets,
        /// and starting values for g and f dictionaries.
        /// Adds the start node to the open set and initializes g and f values.
        /// </summary>
        public override void Initialize(Square startNode, Square endNode)
        {
            base.Initialize(startNode, endNode);

            gDictionary = new Dictionary<string, float>();
            fDictionary = new Dictionary<string, float>();
            
            openSet = new HashSet<Square>();
            closedSet = new HashSet<Square>();

            // Add the start node to the open set and initialize g and f values.
            openSet.Add(startNode);
            gDictionary.Add(startNode.Id, 0);
            fDictionary.Add(startNode.Id, CalculateH(startNode));
        }

        /// <summary>
        /// Retrieves the node with the lowest f-value from the open set,
        /// indicating the most promising node to explore next.
        /// </summary>
        private Square GetNodeWithLowestFScore()
        {
            float minFScore = float.PositiveInfinity;
            Square minFScoreSquare = null;

            foreach (Square node in openSet)
            {
                float fScore = fDictionary[node.Id];
                if (fScore < minFScore)
                {
                    minFScore = fScore;
                    minFScoreSquare = node;
                }
            }

            return minFScoreSquare;
        }

        /// <summary>
        /// Updates the g and f values for a given node.
        /// </summary>
        private void UpdateNodeScores(Square node, float gScore)
        {
            gDictionary[node.Id] = gScore;
            fDictionary[node.Id] = gScore + CalculateH(node);
        }

        /// <summary>
        /// Executes the core of the A* algorithm to find the optimal path.
        /// It explores neighboring nodes and updates their scores.
        /// </summary>
        protected override void GetPath(Board.Board board, GameObject[,] visualBoard)
        {
            if (openSet.Count == 0) return;
            
            Square lowestFNode = GetNodeWithLowestFScore();
            SetSearchingPathColor(visualBoard, lowestFNode);

            // Check if the current node is the goal node.
            if (IsAtEndNode(lowestFNode))
            {
                Finished();
                return;
            }
                
            closedSet.Add(lowestFNode);
            openSet.Remove(lowestFNode);
            
            // Retrieve the neighboring nodes of the current node.
            Square[] neighbors = (   
                from Square square in board.GetFloorNeighbours(lowestFNode)
                where square != null && !closedSet.Contains(square)
                select square
            ).ToArray();

            foreach (Square neighbor in neighbors)
            {
                float gScore = gDictionary[lowestFNode.Id] + 1; //< Increment cost for moving to this node.
                if (!openSet.Contains(neighbor))
                {
                    UpdateNodeScores(neighbor, gScore);
                    Parent.Add(neighbor.Id, lowestFNode);
                    openSet.Add(neighbor);
                }
                else if (gScore < gDictionary[neighbor.Id])
                {
                    UpdateNodeScores(neighbor, gScore);
                    Parent[neighbor.Id] = lowestFNode;
                }
            }
        }
    }
}
