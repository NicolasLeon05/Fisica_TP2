using UnityEngine;

public class OBB
{
    public Vector2 center;
    public Vector2 halfSize;
    public float rotation; //Degrees

    public OBB(Vector2 center, Vector2 size, float rotation)
    {
        this.center = center;
        this.halfSize = size / 2f;
        this.rotation = rotation;
    }

    public Vector2 Right
    {
        get
        {
            float rad = rotation * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    }

    public Vector2 Up
    {
        get
        {
            float rad = rotation * Mathf.Deg2Rad;
            return new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
        }
    }

    public Vector2[] GetCorners()
    {
        Vector2 right = Right * halfSize.x;
        Vector2 up = Up * halfSize.y;

        return new Vector2[]
        {
            center + right + up,
            center + right - up,
            center - right - up,
            center - right + up
        };
    }
}
