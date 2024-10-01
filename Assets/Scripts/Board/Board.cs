// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Board
{
    /// <summary>
    /// Represents a game board composed of squares, allowing for the creation of
    /// walls and goals, and providing functionality to manage the state of the board.
    /// It supports operations such as initializing the board, randomly placing walls 
    /// and a goal, and retrieving neighboring squares for pathfinding logic.
    /// </summary>
    public class Board : ICloneable
    {
        /// <summary>
        /// Number of rows in the board.
        /// </summary>
        public int NumRows { get; }
        
        /// <summary>
        /// Number of columns in the board.
        /// </summary>
        public int NumColumns { get; }
        
        /// <summary>
        /// 2D array of squares that represent the board.
        /// </summary>
        public Square[,] Squares { get; private set; }
        
        /// <summary>
        /// Total number of squares in the board.
        /// </summary>
        private int TotalSquares => NumRows * NumColumns;

        /// <summary>
        /// Initializes a new instance of the Board class.
        /// </summary>
        /// <param name="columns">Number of columns.</param>
        /// <param name="rows">Number of rows.</param>
        public Board(int columns, int rows)
        {
            NumColumns = columns;
            NumRows = rows;
            Squares = new Square[columns, rows];
        }

        /// <summary>
        /// Retrieves the goal square from the board.
        /// </summary>
        public Square Goal => 
            (from Square square in Squares where square is { TypeId: Square.Type.Goal } select square).First();

        /// <summary>
        /// Retrieves all squares that are of type Floor.
        /// </summary>
        private List<Square> FloorSquares => 
            (from Square square in Squares where square is { TypeId: Square.Type.Floor } select square).ToList();

        /// <summary>
        /// Resets the board to its initial state.
        /// </summary>
        private void InitializeBoard()
        {
            Squares = new Square[NumColumns, NumRows];

            // Instantiate Board and set squares with their coordinates.
            // O(n) loop over all squares.
            for (int i = 0; i < TotalSquares; ++i)
            {
                int y = i % NumRows;
                int x = i / NumRows;

                // Create a new square at position (x, y)
                Squares[x, y] = new Square(x, y);
            }
        }

        /// <summary>
        /// Randomly places walls on the board.
        /// </summary>
        /// <param name="minWalls">Minimum number of walls to place.</param>
        /// <param name="maxWalls">Maximum number of walls to place.</param>
        private void PlaceWallsRandomly(int minWalls, int maxWalls)
        {
            int numWalls = Random.Range(minWalls, maxWalls);

            // Keep placing walls until numWalls is 0.
            while (numWalls > 0)
            {
                // Randomize a position on the board.
                int toTestCol = Random.Range(0, NumColumns);
                int toTestRow = Random.Range(0, NumRows);
                
                // If the square is already a wall, skip it.
                if (Squares[toTestCol, toTestRow].TypeId == Square.Type.Wall) continue;
                
                // Set the square to a wall.
                Squares[toTestCol, toTestRow].SetType(Square.Type.Wall);
                numWalls--;
            }
        }

        /// <summary>
        /// Randomly places the goal on a floor square.
        /// </summary>
        private void PlaceGoalRandomly()
        {
            // Choose a random floor square.
            Square goalSquare = FloorSquares[Random.Range(0, FloorSquares.Count)];
            goalSquare.SetType(Square.Type.Goal);
        }

        /// <summary>
        /// Sets up the board, placing walls and the goal based on a seed.
        /// </summary>
        /// <param name="seed">The random seed to ensure reproducibility.</param>
        /// <param name="wallMinMax">The range for the minimum and maximum number of walls.</param>
        public void SetUpBoard(int seed, Range wallMinMax)
        {
            // Initialize the random generator with a seed.
            Random.InitState(seed);

            // Reset the board (create outer walls and floor).
            InitializeBoard();

            // Randomly place walls between the min and max values.
            PlaceWallsRandomly(wallMinMax.min, wallMinMax.max);
            
            // Randomly place the goal.
            PlaceGoalRandomly();
        }
        
        /// <summary>
        /// Returns the neighboring floor squares of a given square.
        /// </summary>
        /// <param name="square">The square to check its neighbors.</param>
        /// <returns>An array of neighboring floor squares.</returns>
        public Square[] GetFloorNeighbours(Square square)
        {
            Square[] neighbours = new Square[4];
            
            // UP: Check if the square above is within bounds and not a wall.
            neighbours[0] = square.RowId < NumRows - 1 && Squares[square.ColumnId, square.RowId + 1].TypeId != Square.Type.Wall
                ? Squares[square.ColumnId, square.RowId + 1]
                : null;
            
            // LEFT: Check if the square to the left is within bounds and not a wall.
            neighbours[1] = square.ColumnId > 0 && Squares[square.ColumnId - 1, square.RowId].TypeId != Square.Type.Wall
                ? Squares[square.ColumnId - 1, square.RowId]
                : null;
            
            // DOWN: Check if the square below is within bounds and not a wall.
            neighbours[2] = square.RowId > 0 && Squares[square.ColumnId, square.RowId - 1].TypeId != Square.Type.Wall
                ? Squares[square.ColumnId, square.RowId - 1]
                : null;
            
            // RIGHT: Check if the square to the right is within bounds and not a wall.
            neighbours[3] = square.ColumnId < NumColumns - 1 && Squares[square.ColumnId + 1, square.RowId].TypeId != Square.Type.Wall
                ? Squares[square.ColumnId + 1, square.RowId]
                : null;
            
            return neighbours;
        }
        
        /// <summary>
        /// Creates a deep clone of the board.
        /// </summary>
        /// <returns>A clone of the current board.</returns>
        public object Clone()
        {
            // Clone the board with the same dimensions and a copy of the squares.
            return new Board(NumColumns, NumRows)
            {
                Squares = (Square[,])Squares.Clone()
            };
        }
    }
}
