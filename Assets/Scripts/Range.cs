// Copyright (c) 2024 Vicente Brisa Saez 
// Github: Vicen-te
// License: MIT

using System;

/// <summary>
/// Represents a range with minimum and maximum values.
/// This structure is serializable, allowing it to be used as a property in the Unity inspector.
/// It is particularly useful for defining ranges in configurations, such as for the number of walls 
/// on a game board. The <c>Range</c> struct encapsulates the minimum and maximum limits and provides 
/// a constructor for easy initialization.
/// </summary>
[Serializable]
public struct Range
{
    public int min; //< Minimum value in the range.
    public int max; //< Maximum value in the range.
        
    // Constructor for the Range struct.
    public Range(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}