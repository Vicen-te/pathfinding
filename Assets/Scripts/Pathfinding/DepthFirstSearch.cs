// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// Implements Depth-First Search (DFS) for pathfinding.
    /// This algorithm explores as far as possible along each branch before backtracking.
    /// </summary>
    public class DepthFirstSearch : Pathfinding
    {
        /// <summary>
        /// Set of visited squares to avoid processing the same node multiple times.
        /// </summary>
        private HashSet<Square> visitedNodes;

        /// <summary>
        /// Initializes the DFS by setting up the visited squares and pushing the start node onto the stack.
        /// </summary>
        /// <param name="startNode">The starting node for the search.</param>
        /// <param name="endNode">The goal node for the search.</param>
        public override void Initialize(Square startNode, Square endNode) 
        {
            base.Initialize(startNode, endNode);  //< Calls the base class initialization.
            visitedNodes = new HashSet<Square>();  //< Initialize the set of visited nodes.
            PathStack.Push(startNode);  //< Start searching from the start node.
        }
        
        /// <summary>
        /// Explores the next node in the path and processes its neighbors.
        /// </summary>
        /// <param name="board">The game board containing all the nodes (squares).</param>
        /// <param name="visualBoard">A 2D array of game objects representing the visual representation of the board.</param>
        protected override void GetPath(Board.Board board, GameObject[,] visualBoard)
        {
            // If there are no nodes left to explore, exit the method (no solution).
            if (PathStack.Count == 0) return;
            
            // Pop the current node from the stack for processing.
            Square currentNode = PathStack.Pop();

            // Ensure the current node hasn't been visited before processing it.
            while (visitedNodes.Contains(currentNode))
            {
                currentNode = PathStack.Pop();  //< Pop the next node if already visited.
                
                // You might want to check if the stack is empty after popping.
                if (PathStack.Count == 0) return; // Exit if no more nodes to process.
            }

            // Changes the visual representation of the current node to indicate that it has been processed in the search.
            SetSearchingPathColor(visualBoard, currentNode);
            visitedNodes.Add(currentNode); //< Mark the current node as visited.

            // Check if we've reached the end node; if so, finish the search.
            if (IsAtEndNode(currentNode))
            {
                Finished();
                return;
            }
            
            // Get the neighboring nodes of the current node.
            Square[] neighbors = board.GetFloorNeighbours(currentNode);
            foreach (Square neighbor in neighbors)
            {
                // Skip null nodes and already visited ones to prevent loops.
                if (neighbor == null || visitedNodes.Contains(neighbor)) continue;
                
                // Set the parent of the neighbor for path reconstruction later.
                Parent[neighbor.Id] = currentNode;
                PathStack.Push(neighbor); //< Add the neighbor to the stack for further exploration.
            }
        }
    }
}