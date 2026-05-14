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
            return new Vector2(
                Mathf.Cos(rad),  //Componente horizontal | rotacion = 0 => 1 | rotacion = 90 => 0
                Mathf.Sin(rad)); //Componente vertical   | rotacion = 0 => 0 | rotacion = 90 => 1
            //rotacion = 0 => (1,0) | rotacion = 90 => (0,1)
        }
    }

    public Vector2 Up
    {
        get
        {
            float rad = rotation * Mathf.Deg2Rad;
            return new Vector2( 
                -Mathf.Sin(rad), //Componente horizontal | rad = 0 => 0 | rad = 90 => -1
                Mathf.Cos(rad)); //Componente vertical   | rad = 0 => 1 | rad = 90 => 0
            //rotacion = 0 => (0,1) | rotacion = 90 => (-1,0)
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
