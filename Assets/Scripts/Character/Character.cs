// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Character
{
    /// <summary>
    /// Represents a character in the game, responsible for movement and pathfinding
    /// based on user input or automated decisions. It interacts with the game board 
    /// and can operate in manual or automatic control modes.
    /// </summary>
    [RequireComponent(typeof(Locomotion))] //< Ensures that the Locomotion component is present on the GameObject.
    public sealed class Character : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Locomotion controller for handling movement.
        /// </summary>
        private Locomotion locomotion;
        
        /// <summary>
        /// Reference to the Pathfinding algorithm for movement decisions.
        /// </summary>
        private Pathfinding.Pathfinding pathfinding;
        
        /// <summary>
        /// Reference to the game board.
        /// </summary>
        private Board.Board board;
        
        /// <summary>
        /// 2D array holding visual representations (GameObjects) of the board's squares.
        /// </summary>
        private GameObject[,] visualBoard;
        
        /// <summary>
        /// Stores the current input key for manual control.
        /// </summary>
        private KeyCode currentInputKey;
        
        /// <summary>
        /// Cooldown timer for pathfinding.
        /// </summary>
        private float cooldown;

        [Space(20)]
        [SerializeField, Tooltip("Starting position of the character on the board")] 
        private Vector2Int startingPosition; 

        [SerializeField, Range(0f, 1f), Tooltip("Interval delay between consecutive pathfinding calculations for the character")] 
        private float pathGenerationDelay = 0.02f;
        
        [SerializeField, Tooltip("Indicates whether the character is controlled by keyboard input")] private bool keyboardControl;
        
        /// <summary>
        /// Indicates if the character is in manual control mode.
        /// </summary>
        private bool isManualControl;
        
        /// <summary>
        /// Gets or sets the current state of manual control.
        /// </summary>
        public bool IsManualControl
        {
            get => isManualControl;
            set
            {
                isManualControl = value; //< Sets the state of manual control.
                
                // Restart the pathfinding algorithm.
                if (keyboardControl)
                {
                    pathfinding?.Restart(board, visualBoard);
                }
                // Initialize the character's position based on its current transform.
                else
                {
                    if(locomotion && pathfinding)
                        InitializePosition(new Vector2Int((int)transform.position.x, (int)transform.position.y));
                }
            }
        }

        /// <summary>
        /// Sets the board for the character.
        /// </summary>
        /// <param name="givenBoard">The board instance to be assigned.</param>
        public void SetBoard(Board.Board givenBoard) =>  board = givenBoard;

        /// <summary>
        /// Sets the visual representation of the board for the character.
        /// </summary>
        /// <param name="givenVisualBoard">The visual board to be assigned.</param>
        public void SetVisualBoard(GameObject[,] givenVisualBoard) => visualBoard = givenVisualBoard;
        
        /// <summary>
        /// Validates properties during PlayMode, setting the control mode based on keyControl.
        /// </summary>
        private void OnValidate()
        {
            if(!Application.isPlaying) return;
            IsManualControl = keyboardControl;
        }
        
        /// <summary>
        /// Handles key input events for manual control.
        /// </summary>
        private void OnGUI()
        {
            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                // Capture key down and up events to update input state.
                currentInputKey = Event.current.type switch
                {
                    EventType.KeyDown => Event.current.keyCode, //< Store key pressed.
                    EventType.KeyUp => KeyCode.None, //< Reset input on key release.
                    _ => currentInputKey //< Maintain current input for other event types.
                };
            }
        }
        
        /// <summary>
        /// Initializes the character by setting up its locomotion and pathfinding components.
        /// It checks if the starting position is valid (not a wall). 
        /// If the starting position is a wall, an error is logged and execution is halted.
        /// </summary>
        public void Init()
        {
            locomotion = GetComponent<Locomotion>();
            pathfinding = GetComponentInChildren<Pathfinding.Pathfinding>();

            if (board.Squares[startingPosition.x, startingPosition.y].TypeId == Square.Type.Wall)
            {
                Debug.LogError("It can't start on a wall, change startPosition.");
                Debug.Break();
            }
            
            InitializePosition(startingPosition);
        }

        /// <summary>
        /// Initializes the character's position on the board.
        /// </summary>
        /// <param name="position">The position to initialize the character at.</param>
        private void InitializePosition(Vector2Int position)
        {
            locomotion.Init(new Square(position.x, position.y));
            pathfinding.Initialize(locomotion.CurrentSquare, board.Goal); //< Initialize pathfinding with current position and goal.
        }

        /// <summary>
        /// Updates the character's state every frame.
        /// </summary>
        private void Update()
        {
            if (!keyboardControl) //< If not in key control mode, update pathfinding.
            {
                if (!pathfinding.IsFinished()) //< Check if the pathfinding has completed.
                {
                    SearchPath(); //< Perform pathfinding.
                }
                else
                {
                    MoveAutomatically(); //< Move the character automatically along the calculated path.
                }
            }
            else
            {
                MoveManually(); // Move the character based on manual input.
            }
        }
        
        /// <summary>
        /// Moves the character automatically based on the calculated path.
        /// </summary>
        private void MoveAutomatically()
        {
            locomotion.MoveToTargetSquarePosition(); //< Move the character to the next position in the path.
            ExecuteNextMovement(ReturnPathPosition); //< Execute movement based on path position.
        }

        /// <summary>
        /// Moves the character based on manual input.
        /// </summary>
        private void MoveManually()
        {
            locomotion.MoveToTargetSquarePosition(); //< Move the character to the next position.
            if (currentInputKey != KeyCode.None) //< If there is input, execute the movement.
            {
                ExecuteNextMovement(ReturnInputPosition); //< Execute movement based on input.
            }
        }

        /// <summary>
        /// Searches for a path on the board if the cooldown has expired.
        /// </summary>
        private void SearchPath()
        {
            cooldown -= Time.deltaTime; //< Decrease the cooldown by the time passed since the last frame.
                
            if (!(cooldown < 0)) return; //< Exit if the cooldown hasn't expired.
            cooldown = pathGenerationDelay; //< Reset cooldown for the next path calculation.
                
            pathfinding.CalculatePath(board, visualBoard); //< Calculate a path for the character.
        }
        
        /// <summary>
        /// Executes the next movement based on the provided movement strategy.
        /// </summary>
        /// <param name="getNextPosition">Function that provides the next direction to move.</param>
        private void ExecuteNextMovement(Func<Locomotion.MovementDirection> getNextPosition)
        {
            if (!locomotion.IsOnTargetSquare()) return; //< Exit if not at the next position.
            
            locomotion.ResetPosition(); 
            Locomotion.MovementDirection movementDirection = getNextPosition.Invoke(); //< Get the next movement direction.
            locomotion.UpdateTargetSquare(movementDirection, board); //< Set the target square to move to.
            Debug.Log($"{movementDirection}");  //< Log the direction for debugging purposes.
        }
        
        /// <summary>
        /// Returns the next movement direction based on the calculated path.
        /// </summary>
        /// <returns>The direction to move next.</returns>
        private Locomotion.MovementDirection ReturnPathPosition()
        {
            return pathfinding.GetNextMove(); 
        }
        
        /// <summary>
        /// Returns the next movement direction based on the current input.
        /// </summary>
        /// <returns>The direction to move next based on input.</returns>
        private Locomotion.MovementDirection ReturnInputPosition()
        {
            // Map keys to their corresponding movement directions.
            Dictionary<KeyCode,(int x, int y, Locomotion.MovementDirection direction)> directionMap = 
            new Dictionary<KeyCode, (int x, int y, Locomotion.MovementDirection direction)>
            {
                {KeyCode.UpArrow, (0, 1, Locomotion.MovementDirection.Up)},
                {KeyCode.W, (0, 1, Locomotion.MovementDirection.Up)},
                {KeyCode.DownArrow, (0, -1, Locomotion.MovementDirection.Down)},
                {KeyCode.S, (0, -1, Locomotion.MovementDirection.Down)},
                {KeyCode.RightArrow, (1, 0, Locomotion.MovementDirection.Right)},
                {KeyCode.D, (1, 0, Locomotion.MovementDirection.Right)},
                {KeyCode.LeftArrow, (-1, 0, Locomotion.MovementDirection.Left)},
                {KeyCode.A, (-1, 0, Locomotion.MovementDirection.Left)}
            };

            // Return None if the input key is not in the direction map.
            if (!directionMap.ContainsKey(currentInputKey)) return Locomotion.MovementDirection.None;
            
            var (dx, dy, direction) = directionMap[currentInputKey]; //< Extract movement deltas and direction.
            var currentPosition = new Vector2Int(locomotion.CurrentSquare.ColumnId, locomotion.CurrentSquare.RowId);
            // Check if the move is valid; if yes, return the direction, otherwise return None.
            return CanMoveToPosition(currentPosition.x + dx, currentPosition.y + dy) ? direction : Locomotion.MovementDirection.None;
        }
        
        /// <summary>
        /// Checks if the character can move to the specified position.
        /// </summary>
        /// <param name="column">The target column index.</param>
        /// <param name="row">The target row index.</param>
        /// <returns>True if the character can move to the position; otherwise, false.</returns>
        private bool CanMoveToPosition(int column, int row)
        {
            return column >= 0 && column < board.NumColumns && row >= 0 && row < board.NumRows &&
                   board.Squares[column, row].TypeId != Square.Type.Wall; //< Ensure the position is within bounds and not a wall.
        }
    }
}