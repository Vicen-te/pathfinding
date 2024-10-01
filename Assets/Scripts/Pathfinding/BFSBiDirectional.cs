// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// Implements the Bi-Directional Breadth-First Search (BFS) algorithm for pathfinding.
    /// This algorithm simultaneously searches from both the start and end nodes, aiming
    /// to meet in the middle to reduce the search space and improve efficiency.
    /// </summary>
    public class BfsBiDirectional : Pathfinding
    {
        /// <summary>
        /// Queue to store the nodes being explored from the start node.
        /// </summary>
        private Queue<Square> startQueue; 
        
        /// <summary>
        /// Queue to store the nodes being explored from the end node.
        /// </summary>
        private Queue<Square> endQueue;
        
        /// <summary>
        /// Dictionary to track the parent nodes of nodes explored from the start node.
        /// Key: Node Id, Value: Parent node.
        /// </summary>
        private Dictionary<string, Square> startParents; 
        
        /// <summary>
        /// Dictionary to track the parent nodes of nodes explored from the end node.
        /// Key: Node Id, Value: Parent node.
        /// </summary>
        private Dictionary<string, Square> endParents; 
        
        /// <summary>
        /// Node where the two searches (from start and end) meet, forming a complete path.
        /// </summary>
        private Square meetingNode;

        /// <summary>
        /// Initializes the Bi-Directional BFS by setting up the queues and parent dictionaries.
        /// </summary>
        /// <param name="startNode">The starting node for the search.</param>
        /// <param name="endNode">The target (end) node for the search.</param>
        public override void Initialize(Square startNode, Square endNode)
        {
            base.Initialize(startNode, endNode); //< Calls the base class initialization.
            
            // Initialize the queues for BFS search from both start and end.
            startQueue = new Queue<Square>();
            endQueue = new Queue<Square>();
            
            // Initialize the dictionaries to keep track of parent nodes from start and end searches.
            endParents = new Dictionary<string, Square>();
            startParents = new Dictionary<string, Square>();
            
            // Enqueue the start and end nodes to their respective queues.
            startQueue.Enqueue(startNode);
            endQueue.Enqueue(endNode);
        }

        /// <summary>
        /// Generates the path after the two searches meet, by combining paths from both directions.
        /// </summary>
        /// <param name="visualBoard">The visual representation of the board for displaying the final path.</param>
        protected override void GeneratePath(GameObject[,] visualBoard)
        {
            if (meetingNode == null) return; //< Ensure that a valid meeting node exists before proceeding.

            Queue<Square> pathA = new(); //< Queue to hold the path from start to the meeting node.
            Square node = meetingNode;

            // Traverse backwards from the meeting node to the start node, building the path.
            while(node != StartNode)
            {
                pathA.Enqueue(node);
                node = startParents[node.Id];
            }

            Stack<Square> pathB = new(); //< Stack to hold the path from the meeting node to the end node.
            node = meetingNode;

            // Traverse backwards from the meeting node to the end node, building the reverse path.
            while (node != null && node != EndNode)
            {
                pathB.Push(node);
                node = endParents[node.Id];
            }

            // Add the end node to the final path.
            PathStack.Push(EndNode);
            
            // Merge both paths by alternating nodes from the two directions.
            while (pathA.Count > 0) 
            {
                Square nodeOut;
                
                // If there are nodes in pathB, push them to the final path.
                if (pathB.Count > 1)
                {
                    nodeOut = pathB.Pop();
                    PathStack.Push(nodeOut);
                }
                else
                {
                    // Otherwise, add nodes from pathA to the final path.
                    nodeOut = pathA.Dequeue();
                    PathStack.Push(nodeOut);
                }
                
                // Update the visual board with the selected path color.
                SetSelectedPathColor(visualBoard, nodeOut);
            }
        }

        /// <summary>
        /// Performs a single iteration of the BFS search from both the start and end nodes.
        /// If the two searches meet, the pathfinding is completed.
        /// </summary>
        /// <param name="board">The board containing the nodes to search through.</param>
        /// <param name="visualBoard">The visual representation of the board for path visualization.</param>
        protected override void GetPath(Board.Board board, GameObject[,] visualBoard)
        {
            // If either queue is empty, no further search can be done.
            if (startQueue.Count == 0 || endQueue.Count == 0) return;
            
            // Dequeue a node from the start queue for exploration.
            Square startNode = startQueue.Dequeue();
            if (startNode != StartNode) SetSearchingPathColor(visualBoard, startNode); //< Mark the node being explored from the start.
            
            // Dequeue a node from the end queue for exploration.
            Square endNode = endQueue.Dequeue();
            if (endNode != EndNode) SetSearchingPathColor(visualBoard, endNode); //< Mark the node being explored from the end.

            // Check if the paths have met, meaning a solution has been found.
            if (startParents.ContainsKey(endNode.Id) || endParents.ContainsKey(startNode.Id))
            {
                // Set the meeting node as either the startNode or endNode where the two paths meet.
                meetingNode = startParents.ContainsKey(endNode.Id) ? endNode : startNode;
                Finished(); //< End the search since the paths have met.
                return;
            }
            
            // Get the neighboring nodes of the startNode.
            Square[] startNeighbors = board.GetFloorNeighbours(startNode); 
            foreach (Square startNeighbor in startNeighbors)
            {
                // If the node has not been visited by the start search, mark its parent and enqueue it.
                if (startNeighbor != null && !startParents.ContainsKey(startNeighbor.Id))
                {
                    startParents[startNeighbor.Id] = startNode;
                    startQueue.Enqueue(startNeighbor);
                }
            }

            // Get the neighboring nodes of the endNode.
            Square[] endNeighbors = board.GetFloorNeighbours(endNode);
            foreach (Square endNeighbor in endNeighbors)
            {
                // If the node has not been visited by the end search, mark its parent and enqueue it.
                if (endNeighbor != null && !endParents.ContainsKey(endNeighbor.Id))
                {
                    endParents[endNeighbor.Id] = endNode;
                    endQueue.Enqueue(endNeighbor);
                }
            }
        }
    }
}