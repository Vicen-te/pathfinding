// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;
using Board;
using UnityEngine;

namespace Character
{
    /// <summary>
    /// Manages the locomotion of the character, handling movement between squares on the board.
    /// </summary>
    public class Locomotion : MonoBehaviour
    {
        /// <summary>
        /// Represents possible movement directions for the character.
        /// </summary>
        public enum MovementDirection
        {
            Up,    //< Move upwards.
            Right, //< Move to the right.
            Down,  //< Move downwards.
            Left,  //< Move to the left.
            None   //< No movement.
        }

        /// <summary>
        /// Reference to the Rigidbody2D component for physics movement.
        /// </summary>
        private Rigidbody2D rb; 
        
        /// <summary>
        /// The current square the character is occupying.
        /// </summary>
        public Square CurrentSquare { get; private set; }
        
        /// <summary>
        /// The next square the character will move to.
        /// </summary>
        private Square TargetSquare { get; set; }
        
        /// <summary>
        /// Distance the character has traveled toward the target square.
        /// </summary>
        private float distanceTraveled;
        
        /// <summary>
        /// The speed at which the character moves between squares on the board.
        /// </summary>
        [SerializeField, Range(0f, 10f)] private float movementSpeed = 3;
        
        /// <summary>
        /// Initializes the Rigidbody2D component.
        /// </summary>
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to this GameObject.
        }
        
        /// <summary>
        /// Initializes the locomotion system with the starting square.
        /// </summary>
        /// <param name="start">The square where the character starts its movement.</param>
        public void Init(Square start)
        {
            distanceTraveled = 1; // Set distance traveled to 1 to ensure immediate positioning.
            CurrentSquare = TargetSquare = start; // Set the current and target squares to the starting square.
        }

        /// <summary>
        /// Updates the target square based on the given movement direction.
        /// </summary>
        /// <param name="movementDirection">The direction to move towards.</param>
        /// <param name="board">The board to reference for square positions.</param>
        public void UpdateTargetSquare(MovementDirection movementDirection, Board.Board board)
        {
            switch (movementDirection)
            {
                case MovementDirection.Up:
                    TargetSquare = board.Squares[TargetSquare.ColumnId, TargetSquare.RowId + 1]; //< Move up.
                    break;
            
                case MovementDirection.Left: 
                    TargetSquare = board.Squares[TargetSquare.ColumnId - 1, TargetSquare.RowId]; //< Move left.
                    break;
                
                case MovementDirection.Down: 
                    TargetSquare = board.Squares[TargetSquare.ColumnId, TargetSquare.RowId - 1]; //< Move down.
                    break;
                
                case MovementDirection.Right: 
                    TargetSquare = board.Squares[TargetSquare.ColumnId + 1, TargetSquare.RowId]; //< Move right.
                    break;
                
                case MovementDirection.None:
                    break; //< Do nothing if direction is None.
                
                default:
                    // Handle invalid direction.
                    throw new ArgumentOutOfRangeException($"NewDirection: {movementDirection}");
            }
        }
        
        /// <summary>
        /// Checks if the character has reached the target square.
        /// </summary>
        /// <returns>True if the character is on the target square; otherwise, false.</returns>
        public bool IsOnTargetSquare() => distanceTraveled >= 1;
        
        /// <summary>
        /// Resets the position tracking and updates the current square to the target square.
        /// </summary>
        public void ResetPosition()
        {
            distanceTraveled = 0; //< Reset distance traveled.
            CurrentSquare = TargetSquare; 
        }

        /// <summary>
        /// Moves the character to the target square position based on the distance traveled.
        /// </summary>
        public void MoveToTargetSquarePosition()
        {
            if (distanceTraveled < 1)
            {
                distanceTraveled += Time.deltaTime * movementSpeed; //< Increment the distance traveled.
                // Smoothly move to the target square position.
                rb.MovePosition(Vector2.Lerp(CurrentSquare.GetPosition, TargetSquare.GetPosition, distanceTraveled)); 
            }
            else
            {
                distanceTraveled = 1; //< Ensure distance is capped at 1.
                rb.MovePosition(TargetSquare.GetPosition); //< Move to the target square position.
            }
        }

    }
}
