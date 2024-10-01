// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using UnityEngine;

namespace Pathfinding
{
    
    /// <summary>
    /// Provides methods to calculate different heuristic distances used in the A* search algorithm.
    /// </summary>
    public static class Heuristics
    {
        /// <summary>
        /// Enum representing different types of heuristic calculations for h(n).
        /// </summary>
        public enum Type { Manhattan, Euclidean, DiagonalChebyshev, DiagonalOctile }
        
        /// <summary>
        /// Calculates the Manhattan distance between two points.
        /// </summary>
        public static float ManhattanDistance(Vector2 from, Vector2 to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }

        /// <summary>
        /// Calculates the Euclidean distance between two points.
        /// </summary>
        public static float EuclideanDistance(Vector2 from, Vector2 to)
        {
            return Mathf.Sqrt(Mathf.Pow(from.x - to.x, 2) + Mathf.Pow(from.y - to.y, 2));
        }

        /// <summary>
        /// Calculates the diagonal distance between two points using either Chebyshev or Octile metrics.
        /// </summary>
        public static float DiagonalDistance(Vector2 from, Vector2 to, Type type)
        {
            float dx = Mathf.Abs(from.x - to.x);
            float dy = Mathf.Abs(from.y - to.y);
            const float d = 1;

            // Choose between Octile and Chebyshev diagonal distances.
            float d2 = type == Type.DiagonalOctile ? Mathf.Sqrt(2) : 1;

            return d * (dx + dy) + (d2 - 2 * d) * Mathf.Min(dx, dy);
        }
    }

}