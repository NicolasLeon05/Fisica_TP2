using UnityEngine;

public static class Collision
{
    public static bool OBBvsOBB(OBB a, OBB b)
    {
        Vector2[] axes = new Vector2[]
        {
            a.Right, a.Up,
            b.Right, b.Up
        };

        foreach (var axis in axes)
        {
            if (!OverlapOnAxis(a, b, axis))
                return false;
        }

        return true;
    }

    private static bool OverlapOnAxis(OBB a, OBB b, Vector2 axis)
    {
        Project(a, axis, out float minA, out float maxA);
        Project(b, axis, out float minB, out float maxB);

        return !(maxA < minB || maxB < minA);
    }

    private static void Project(OBB box, Vector2 axis, out float min, out float max)
    {
        Vector2[] corners = box.GetCorners();

        min = max = Vector2.Dot(corners[0], axis);

        for (int i = 1; i < corners.Length; i++)
        {
            float proj = Vector2.Dot(corners[i], axis);
            if (proj < min) min = proj;
            if (proj > max) max = proj;
        }
    }
}