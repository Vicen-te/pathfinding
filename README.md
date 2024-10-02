# Pathfinding
This project implements several pathfinding algorithms used to navigate a grid-based game board. 
It emphasizes demonstrating how these algorithms explore and compute the most efficient paths 
between a starting point and a goal. 
The project provides a visual representation of the board, allowing users to observe the real-time
process as the algorithms search for paths, evaluate nodes, and make decisions. 
Designed for educational purposes, it can also be used in game development or AI simulations.

![previewimg]

## Algorithms
The project includes the following algorithms:

1. Depth-First Search (DFS)
2. Breadth-First Search (BFS)
3. Bidirectional BFS
4. Dijkstra
5. AStarDijkstra (Combination of AStar and Dijkstra)
6. AStar with different heuristic types:
    1. Manhattan
    2. Euclidean
    3. Diagonal Chebyshev
    4. Diagonal Octile

## Visualization
- The board features walls (dark blocks) and floors (light-colored blocks).
- The character is represented as a blue square, while the goal is indicated by a pink square.
- Squares considered during the pathfinding process.
- Squares selected as the final route to reach the goal.

## Customization
Customizations can be made to instances without modifying the code.

![customization0]

### Character
- Movement Speed
- Starting position (if the starting position is a wall, the game will stop, 
and an error message will be displayed).
- Delay between consecutive pathfinding calculations.
- Ability to control the character: when the keyboardControl boolean is activated,
 the path disappears. If deactivated, path generation restarts from the character's
 current position.
 
![customization1]

### PathFindingAlgorithm
- A GameObject to which you can attach the desired pathfinding algorithm script.

![customization2]

### Loader
- Seed for generating different maps.
- Number of columns and rows of the board.
- Minimum and maximum number of walls.
- Prefabs for walls, floor, and goal.

![customization3]

## Preview - AStar
![previewgif]

[previewimg]: Images/Preview.png
[customization0]: Images/Customization0.png
[customization1]: Images/Customization1.png
[customization2]: Images/Customization2.png
[customization3]: Images/Customization3.png
[previewgif]: Images/Preview.gif