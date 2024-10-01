// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// Implements the Breadth-First Search (BFS) algorithm for pathfinding.
    /// This algorithm explores all possible nodes level by level from the starting node.
    /// </summary>
    public class BreadthFirstSearch : Pathfinding
    {
        /// <summary>
        /// Queue used to store the nodes being explored in BFS order.
        /// </summary>
        private Queue<Square> queue;

        /// <summary>
        /// Initializes the BFS pathfinding by enqueuing the start node.
        /// </summary>
        /// <param name="startNode">The starting node of the pathfinding.</param>
        /// <param name="endNode">The target (end) node of the pathfinding.</param>
        public override void Initialize(Square startNode, Square endNode)
        {
            base.Initialize(startNode, endNode); //< Calls the base class initialization.
            
            // Initialize the queue and enqueue the starting node for exploration.
            queue = new Queue<Square>();
            queue.Enqueue(startNode);
        }
        
        /// <summary>
        /// Performs one iteration of the BFS algorithm, exploring neighboring nodes
        /// and expanding the search space level by level.
        /// </summary>
        /// <param name="board">The board containing the nodes to search through.</param>
        /// <param name="visualBoard">The visual representation of the board for path visualization.</param>
        protected override void GetPath(Board.Board board, GameObject[,] visualBoard)
        {
            // If the queue is empty, there is no more path to explore (no solution found).
            if (queue.Count == 0) return;
            
            // Dequeue the current node for exploration.
            Square currentNode = queue.Dequeue();
            
            // Visually mark the current node as being explored.
            SetSearchingPathColor(visualBoard, currentNode);

            // Check if the current node is the end node. If so, the search is complete.
            if (IsAtEndNode(currentNode))
            {
                Finished();
                return;
            }
            
            // Retrieve the neighboring nodes of the current node (adjacent floor squares).
            Square[] neighbors = board.GetFloorNeighbours(currentNode);
            
            // Explore each neighboring node.
            foreach (Square neighbor in neighbors)
            {
                // Skip if the neighbor is null or has already been processed (i.e., has a parent).
                if (neighbor is not { Id: var cellId } || Parent.ContainsKey(cellId)) continue;
                
                // Record the parent of this neighbor as the current node (to reconstruct the path later).
                Parent[cellId] = currentNode;
                
                // Enqueue the neighbor for further exploration in subsequent iterations.
                queue.Enqueue(neighbor);
            }
        }
    }
}