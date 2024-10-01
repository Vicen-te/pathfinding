// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// Represents a single square on the game board, with properties for its position,
    /// type (e.g., Goal, Floor, Wall), and functionality to manage its state.
    /// Each square can be identified by its row and column indices, and can be cloned 
    /// for use in pathfinding or game logic.
    /// </summary>
    public class Square : ICloneable
    {
        #region Attributes
        
        /// <summary>
        /// Gets the row index of the square.
        /// </summary>
        public int RowId { get; }
        
        /// <summary>
        /// Gets the column index of the square.
        /// </summary>
        public int ColumnId { get; }
        
        /// <summary>
        /// Gets a unique identifier for the square based on its column and row.
        /// </summary>
        public string Id => $"{ColumnId},{RowId}";
        
        /// <summary>
        /// Gets the position of the square as a 2D vector (column, row).
        /// </summary>
        public Vector2 GetPosition => new(ColumnId, RowId);
        
        /// <summary>
        /// Enum representing the type of the square (Goal, Floor, or Wall).
        /// </summary>
        public enum Type
        {
            Goal,
            Floor,
            Wall
        }
        
        /// <summary>
        /// Gets the type of the square (Goal, Floor, or Wall).
        /// </summary>
        public Type TypeId { get; private set; }
        
        #endregion

        /// <summary>
        /// Sets the type of the square.
        /// </summary>
        /// <param name="type">The new type for the square.</param>
        public void SetType(Type type) => TypeId = type;

        /// <summary>
        /// Constructor for creating a square with specified column and row indices.
        /// </summary>
        /// <param name="col">The column index.</param>
        /// <param name="row">The row index.</param>
        public Square(int col, int row)
        {
            RowId = row;
            ColumnId = col;
            TypeId = Type.Floor;
        }

        /// <summary>
        /// Creates a shallow copy of the current square instance.
        /// </summary>
        /// <returns>A new Square instance with the same column and row indices.</returns>
        public object Clone()
        {
            Square result = new (ColumnId, RowId);
            return result;
        }
    }
}