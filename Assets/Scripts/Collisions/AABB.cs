using UnityEngine;

public class AABB
{
    public Vector2 center;
    public Vector2 halfSize;

    public AABB(Vector2 center, Vector2 size)
    {
        this.center = center;
        this.halfSize = size / 2f;
    }

    public Vector2 Min => center - halfSize;
    public Vector2 Max => center + halfSize;
}