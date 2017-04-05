using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirections
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionsExtensions
{
    public static HexDirections Opposite (this HexDirections direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
    public static HexDirections Previous (this HexDirections direction)
    {
        return direction == HexDirections.NE ? HexDirections.NW : (direction - 1);
    }
    public static HexDirections Next (this HexDirections direction)
    {
        return direction == HexDirections.NW ? HexDirections.NE : (direction + 1);
    }
}

