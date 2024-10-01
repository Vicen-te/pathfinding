// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using Board;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Responsible for loading and initializing the game board and character.
/// This class sets up the game scene by creating the board, generating its visual
/// representation, and configuring the character with the necessary references to
/// the board and its visuals. It manages the random generation of the board layout
/// and ensures that the game environment is properly configured before gameplay starts.
/// </summary>
public class Loader : MonoBehaviour
{
    [FormerlySerializedAs("characterManager")]
    [Header("References"), Tooltip("Reference to the character object")] 
    [SerializeField] private Character.Character character;
        
    [Header("Board Configuration")]
    
    [SerializeField, Tooltip("Seed for random number generation, ensuring reproducible board configurations across runs")]
    private int seed = 2;
    
    [SerializeField, Tooltip("Number of columns in the game board")]
    private int columns = 16;
    
    [SerializeField, Tooltip("Number of rows in the game board")] 
    private int rows = 9; 
    
    [SerializeField, Tooltip("Range for the number of walls on the board")] 
    private Range wallRange = new(10, 50);
        
    [Header("Prefabs")]
    [SerializeField, Tooltip("Prefab for the goal object")] 
    private GameObject goalPrefab;
    
    [SerializeField, Tooltip("Prefab for the floor object")] 
    private GameObject floorPrefab;
    
    [SerializeField, Tooltip("Prefab for the wall object")] 
    private GameObject wallPrefab;
    

    /// <summary>
    /// Instance of the Board class representing the game board.
    /// </summary>
    private Board.Board board;
    
    /// <summary>
    /// 2D array to hold visual representations (GameObjects) of the board's squares.
    /// </summary>
    private GameObject[,] visualBoard;
        
    private void Awake()
    {
        InitGame(seed);
    }
    
    /// <summary>
    /// Initializes the game by setting up the board and generating the map.
    /// </summary>
    /// <param name="generationSeed">The seed used for random number generation.</param>
    private void InitGame(int generationSeed)
    {
        InitializeBoard(generationSeed); // Set up the board and its configuration.
        GenerateMap(); // Create visual representations for the board.
        SetCharacter(); // Configure the character with the board and visual information.
    }

    /// <summary>
    /// Sets up the game scene by initializing the board and visual array.
    /// </summary>
    /// <param name="generationSeed">The seed for the board setup.</param>
    private void InitializeBoard(int generationSeed)
    {
        board = new Board.Board(columns, rows); // Create a new instance of the Board.
        board.SetUpBoard(generationSeed, wallRange); // Set up the board with walls and goal.

        visualBoard = new GameObject[columns, rows]; // Initialize the visual board array.
    }

    /// <summary>
    /// Generates the visual map based on the types of squares on the board.
    /// </summary>
    private void GenerateMap()
    {
        // Iterate through each square in the board to instantiate visual representations.
        foreach (Square square in board.Squares)
        {
            // Choose the appropriate prefab based on the square's type (wall or floor).
            GameObject prefab = square.TypeId == Square.Type.Wall ? wallPrefab : floorPrefab;
            
            // Instantiate the prefab at the square's position in the world space.
            visualBoard[square.ColumnId, square.RowId] = Instantiate(prefab, new Vector3(square.ColumnId, square.RowId, 0f), Quaternion.identity);

            // If the square is the goal, instantiate the goal prefab as a child of the square's visual representation.
            if (square.TypeId != Square.Type.Goal) continue;
            
            // Instantiate the goal prefab at the square's position.
            GameObject itemLogicGameObject = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity, visualBoard[square.ColumnId, square.RowId].transform);
            itemLogicGameObject.transform.localPosition = Vector3.zero; // Reset local position to zero to align with the square.
        }
    }
    
    /// <summary>
    /// Initializes the character's starting values
    /// </summary>
    private void SetCharacter()
    {
        character.SetBoard(board);
        character.SetVisualBoard(visualBoard);
        character.Init(); 
    }
}