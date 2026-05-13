using UnityEngine;

public static class Collision
{
    public static bool OBBvsOBB(OBB a, OBB b)
    {
        Vector2[] axes = new Vector2[]
        {
            a.Right,
            a.Up,
            b.Right,
            b.Up
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

            if (proj < min)
                min = proj;

            if (proj > max)
                max = proj;
        }
    }

    public static bool AABBvsAABB(AABB a, AABB b)
    {
        return
           (a.Min.x <= b.Max.x &&
            a.Max.x >= b.Min.x &&
            a.Min.y <= b.Max.y &&
            a.Max.y >= b.Min.y);
    }

    public static bool OBBvsAABB(OBB obb, AABB aabb)
    {
        OBB converted = new OBB(aabb.center, aabb.halfSize * 2f, 0f);

        return OBBvsOBB(obb, converted);
    }

    public static bool CircleVsAABB(Circle circle, AABB box)
    {
        Vector2 closestPoint;
        closestPoint.x = Mathf.Clamp(circle.center.x, box.Min.x, box.Max.x);
        closestPoint.y = Mathf.Clamp(circle.center.y, box.Min.y, box.Max.y);

        Vector2 difference = circle.center - closestPoint;

        return difference.sqrMagnitude <= circle.radius * circle.radius;
    }

    public static bool CircleVsCircle(Circle a, Circle b)
    {
        Vector2 delta = b.center - a.center;
        float radiusSum = a.radius + b.radius;

        return delta.sqrMagnitude <= radiusSum * radiusSum;
    }

    public static bool CircleVsOBB(Circle circle, OBB box)
    {
        Vector2 localCirclePosition = circle.center - box.center;
        float rad = -box.rotation * Mathf.Deg2Rad;

        Vector2 local;
        local.x = localCirclePosition.x * Mathf.Cos(rad) - localCirclePosition.y * Mathf.Sin(rad);
        local.y = localCirclePosition.x * Mathf.Sin(rad) + localCirclePosition.y * Mathf.Cos(rad);

        Vector2 closestPoint;
        closestPoint.x = Mathf.Clamp(local.x, -box.halfSize.x, box.halfSize.x);
        closestPoint.y = Mathf.Clamp(local.y, -box.halfSize.y, box.halfSize.y);

        Vector2 difference = local - closestPoint;

        return difference.sqrMagnitude <= circle.radius * circle.radius;
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

    public static void ResolveBulletFloor(Bullet bullet, Floor floor)
    {
        Vector2 velocity = bullet.Velocity;
        float verticalSpeed = velocity.y;

        if (verticalSpeed < 0f)
        {
            velocity.y = -verticalSpeed * bullet.Restitution;

            float frictionFactor = Mathf.Clamp01(1f - floor.Friction);
            velocity.x *= frictionFactor;

            if (Mathf.Abs(velocity.y) < 0.1f)
                velocity.y = 0f;

            bullet.Velocity = velocity;
        }

        Vector3 pos = bullet.transform.position;
        pos.y = floor.Bounds.Max.y + bullet.Radius;
        bullet.transform.position = pos;
    }

    public static void ResolveBulletBullet(Bullet a, Bullet b)
    {
        Vector2 delta = (Vector2)(b.transform.position - a.transform.position);
        float distance = delta.magnitude;

        if (distance == 0f)
            return;

        Vector2 normal = delta / distance;
        Vector2 relativeVelocity = b.Velocity - a.Velocity;
        float normalSpeed = Vector2.Dot(relativeVelocity, normal);

        if (normalSpeed > 0f)
            return;

        float restitution = Mathf.Min(a.Restitution, b.Restitution);
        float impulseMagnitude = -(1f + restitution) * normalSpeed;
        impulseMagnitude /= (1f / a.Mass) + (1f / b.Mass);

        Vector2 impulse = impulseMagnitude * normal;
        a.Velocity -= impulse / a.Mass;
        b.Velocity += impulse / b.Mass;

        float penetration = (a.Radius + b.Radius) - distance;

        if (penetration > 0f)
        {
            Vector2 correction = normal * (penetration * 0.5f);
            a.transform.position -= (Vector3)correction;
            b.transform.position += (Vector3)correction;
        }
    }

    public static void ResolveTankTank(Tank a, Tank b)
    {
        float distance = b.transform.position.x - a.transform.position.x;
        if (distance == 0f)
            return;

        float normal = Mathf.Sign(distance);

        float relativeVelocity = b.Velocity - a.Velocity;

        if (relativeVelocity * normal > 0f)
            return;

        float restitution = Mathf.Min(a.Restitution, b.Restitution);

        float impulse = -(1f + restitution) * relativeVelocity;
        impulse /= (1f / a.Mass) + (1f / b.Mass);

        a.Velocity -= impulse / a.Mass;
        b.Velocity += impulse / b.Mass;

        float overlap = (a.Bounds.halfSize.x + b.Bounds.halfSize.x) - Mathf.Abs(distance);

        if (overlap > 0f)
        {
            float correction = overlap * 0.5f;

            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;

            posA.x -= correction * normal;
            posB.x += correction * normal;

            a.transform.position = posA;
            b.transform.position = posB;
        }
    }

    public static void ResolveTankWall(Tank tank, Wall wall)
    {
        Vector3 pos = tank.transform.position;

        if (tank.Velocity > 0)
            pos.x = wall.Bounds.Min.x - tank.Bounds.halfSize.x;
        else
            pos.x = wall.Bounds.Max.x + tank.Bounds.halfSize.x;

        tank.transform.position = pos;
        tank.Velocity = 0f;
    }
}