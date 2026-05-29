using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType
{
    SMOOTH,
    IRREGULAR,
    ROCKY,
}

public enum VisibilityType
{
    NOMINAL,
    POOR,
}

public enum MapSize
{
    SMALL,
    MEDIUM,
    LARGE,
}

public enum EnvironType
{
    FOREST,
    URBAN,
    MOUNTAIN,
}

[CreateAssetMenu(fileName = "New Battlefield", menuName = "Battlefield")]
public class Battlefield : ScriptableObject
{
    public string map_name;
    public EnvironType environ;
    public MapSize map_size;
    public VisibilityType visibility;
    public TerrainType terrain;

    public Sprite map_sprite;
}
