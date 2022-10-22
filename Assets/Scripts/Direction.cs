using UnityEngine;

public enum Dir
{
    E,
    NE,
    N,
    NW,
    W,
    SW,
    S,
    SE,
    None
}

public static class Direction
{
    public static Vector3 GetDirectionVector3(Dir dir)
    {
        switch (dir)
        {
            case Dir.E:
                return Vector3.right;
            case Dir.N:
                return Vector3.up;
            case Dir.W:
                return Vector3.left;
            case Dir.S:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }
}

