// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Board;
using Character;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pathfinding
{
    /// <summary>
    /// Abstract base class for pathfinding algorithms in a grid-based board.
    /// This class provides methods to calculate and retrieve paths from a start node to an end node.
    /// </summary>
    public abstract class Pathfinding : MonoBehaviour
    {
        protected Square EndNode, StartNode; // The end and start nodes for pathfinding.
        
        /// <summary>
        /// Maps each square to its parent in the path.
        /// </summary>
        protected Dictionary<string, Square> Parent;
        
        /// <summary>
        /// Stack to hold the path.
        /// </summary>
        protected Stack<Square> PathStack;

        /// <summary>
        /// Indicates if the pathfinding process is complete.
        /// </summary>
        private bool isFinished; 
        
        /// <summary>
        /// It tracks the most recently visited square to determine the movement direction for the character,
        /// ensuring smooth transitions between nodes during navigation.
        /// </summary>
        private Square lastSquare;

        /// <summary>
        /// Stopwatch for measuring performance.
        /// </summary>
        private Stopwatch stopWatch;
    
        /// <summary>
        /// Checks if the pathfinding process has finished.
        /// </summary>
        /// <returns>True if finished, otherwise false.</returns>
        public bool IsFinished() => isFinished;
        
        /// <summary>
        /// Abstract method to get the path based on the specific pathfinding algorithm.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="visualBoard">The visual representation of the board.</param>
        protected abstract void GetPath(Board.Board board, GameObject[,] visualBoard);

        /// <summary>
        /// Calculates the path from the start node to the end node.
        /// This method logs the time taken to solve the algorithm and the length of the path found.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="visualBoard">The visual representation of the board.</param>
        public void CalculatePath(Board.Board board, GameObject[,] visualBoard)
        {
            // Call the abstract GetPath method to compute the path.
            GetPath(board, visualBoard);

            // If the pathfinding process is not finished, return early.
            if (!isFinished) return;
            
            // Generate the path and display the results.
            GeneratePath(visualBoard);
            
            stopWatch.Stop(); // Stop the stopwatch.
            
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            
            // Format and display the elapsed time in minutes, seconds, and milliseconds.
            string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds / 10:00}";
            Debug.Log($"Time taken to solve the algorithm: {elapsedTime} - Path Length: {PathStack.Count}");
        }

        /// <summary>
        /// Initializes the pathfinding with the start and end nodes.
        /// </summary>
        /// <param name="startNode">The starting square.</param>
        /// <param name="endNode">The ending square.</param>
        public virtual void Initialize(Square startNode, Square endNode)
        {
            StartNode = startNode;
            EndNode = endNode;
            lastSquare = startNode;

            Parent = new Dictionary<string, Square>();
            PathStack = new Stack<Square>();
            stopWatch = new Stopwatch();
            stopWatch.Start(); // Start measuring time.
        }

        /// <summary>
        /// Checks the direction of movement from one square to another.
        /// </summary>
        /// <param name="currentSquare">The current square.</param>
        /// <param name="lastSquare">The last square visited.</param>
        /// <returns>The direction of movement as a <see cref="Locomotion.MovementDirection"/>.</returns>
        private static Locomotion.MovementDirection CheckDirection(Square currentSquare, Square lastSquare)
        {
            float x = currentSquare.GetPosition.x - lastSquare.GetPosition.x; // Calculate the x difference.
            float y = currentSquare.GetPosition.y - lastSquare.GetPosition.y; // Calculate the y difference.
    
            // Determine the direction based on the greater distance.
            return Mathf.Abs(x) > Mathf.Abs(y)
                ? (x > 0 ? Locomotion.MovementDirection.Right : Locomotion.MovementDirection.Left)
                : (y > 0 ? Locomotion.MovementDirection.Up : Locomotion.MovementDirection.Down);
        }
    
        /// <summary>
        /// Gets the next movement direction based on the path stack.
        /// </summary>
        /// <returns>The next movement direction as a <see cref="Locomotion.MovementDirection"/>.</returns>
        public Locomotion.MovementDirection GetNextMove()
        {
            // Return None if there are no more squares in the stack (no solution).
            if (PathStack.Count <= 0) return Locomotion.MovementDirection.None;

            Square currentSquare = PathStack.Pop(); // Get the current square from the stack.
            Locomotion.MovementDirection movementDirection = CheckDirection(currentSquare, lastSquare); // Determine the movement direction.
            lastSquare = currentSquare; // Update the last square to the current square.
            return movementDirection;
        }

        /// <summary>
        /// Generates the path from the start node to the end node by tracing back through the parent nodes.
        /// </summary>
        /// <param name="visualBoard">The visual representation of the board.</param>
        protected virtual void GeneratePath(GameObject[,] visualBoard)
        {
            Square node = EndNode; // Start with the end node.
            SetSelectedPathColor(visualBoard, node); // Set the color for the end node.

            // Trace back through the parent nodes to generate the full path.
            while (node != null && node != StartNode)
            {
                PathStack.Push(node); // Push the current node onto the stack.
                node = Parent[node.Id]; // Move to the parent node.
                SetSelectedPathColor(visualBoard, node); // Set the color for the current node.
            }
        }

        /// <summary>
        /// Marks the pathfinding process as finished.
        /// </summary>
        protected void Finished() => isFinished = true;

        /// <summary>
        /// Restarts the pathfinding process and resets the visual board colors.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="visualBoard">The visual representation of the board.</param>
        public void Restart(Board.Board board, GameObject[,] visualBoard) 
        {
            isFinished = false; // Reset the finish flag.
            
            const float grey = 122.0f / 255; // Define a grey color value.
            foreach (var square in board.Squares) // Loop through each square on the board.
            {
                // Change the color of non-wall squares to grey.
                if (square.TypeId != Square.Type.Wall)
                {
                    ChangeColor(visualBoard, square, new Color(grey, grey, grey));
                }
            }
        }
        
        /// <summary>
        /// Checks if the current node is the end node, indicating the pathfinding is complete.
        /// </summary>
        /// <param name="currentNode">The current node being checked.</param>
        /// <returns>True if the current node is the end node, otherwise false.</returns>
        protected bool IsAtEndNode(Square currentNode) => currentNode == EndNode; // Return true if the current node is the end node.
        
        /// <summary>
        /// Changes the color of the visual representation of a square.
        /// </summary>
        /// <param name="visualBoard">The visual representation of the board.</param>
        /// <param name="square">The square to change the color of.</param>
        /// <param name="color">The new color to apply.</param>
        private static void ChangeColor(GameObject[,] visualBoard, Square square, Color color)
        {
            GameObject squareGameObject = visualBoard[square.ColumnId, square.RowId]; // Get the GameObject from the visual board 
            // Change the color of the square's sprite renderer if it exists.
            if (squareGameObject.transform.GetChild(0).TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.color = color; // Apply the new color.
        }

        /// <summary>
        /// Sets the color of the selected path square in the visual representation.
        /// </summary>
        /// <param name="visualBoard">The visual representation of the board.</param>
        /// <param name="square">The square to set the color for.</param>
        protected static void SetSelectedPathColor(GameObject[,] visualBoard, Square square)
        {
            ChangeColor(visualBoard, square, new Color(1f, 1f, 0.72f)); //< Set color to a light yellow for the selected path.
        }

        /// <summary>
        /// Sets the color of the searching path square in the visual representation.
        /// </summary>
        /// <param name="visualBoard">The visual representation of the board.</param>
        /// <param name="square">The square to set the color for.</param>
        protected static void SetSearchingPathColor(GameObject[,] visualBoard, Square square)
        {
            ChangeColor(visualBoard, square, new Color(0.72f, 1f, 0.72f)); //< Set color to light green for the searching path.
        }

    }
}
