﻿using UnityEngine;
using System.Collections;

public static class HexMetrics
{

    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public const float solidFactor = 0.75f;

    public const float blendFactor = 1f - solidFactor;

    static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 GetFirstCorner(HexDirections direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirections direction)
    {
        return corners[(int)direction + 1];
    }
    public static Vector3 GetFirstSolidCorner(HexDirections direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirections direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirections direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }
}
