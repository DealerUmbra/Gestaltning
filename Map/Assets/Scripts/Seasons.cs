using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season
{
    Spring, Summer, Fall, Winter
}

public static class SeasonExtensions
{

    public static Season Next(this Season season)
    {
        return season == Season.Winter ? Season.Spring : (season + 1);
    }
}
