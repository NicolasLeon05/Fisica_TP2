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

    public static bool CircleVsAABB(Circle circle, AABB box)
    {
        Vector2 closestPoint;

        closestPoint.x = Mathf.Clamp(circle.center.x, box.Min.x, box.Max.x);
        closestPoint.y = Mathf.Clamp(circle.center.y, box.Min.y, box.Max.y);

        Vector2 difference = circle.center - closestPoint;

        float distanceSquared = difference.sqrMagnitude;

        return distanceSquared <= circle.radius * circle.radius;
    }

    public static void ResolveBulletWall(Bullet bullet, Wall wall)
    {
        Vector2 velocity = bullet.Velocity;

        velocity.x *= -wall.RestitutionCoefficient;

        bullet.Velocity = velocity;

        Vector3 pos = bullet.transform.position;

        if (velocity.x > 0)
            pos.x = wall.Bounds.Max.x + bullet.Radius;
        else
            pos.x = wall.Bounds.Min.x - bullet.Radius;

        bullet.transform.position = pos;
    }
}