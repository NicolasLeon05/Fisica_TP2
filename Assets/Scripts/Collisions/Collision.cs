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

    public static bool CircleVsCircle(Circle a, Circle b)
    {
        Vector2 delta = b.center - a.center;
        float radiusSum = a.radius + b.radius;
        return delta.sqrMagnitude <= radiusSum * radiusSum;
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

        //Velocidad vertical (normal al piso)
        float verticalSpeed = velocity.y;

        //Resolver solo si la bala cae hacia el piso
        if (verticalSpeed < 0f)
        {
            //Componente vertical (rebote)
            velocity.y = -verticalSpeed * bullet.Restitution;

            //Componente horizontal (friccion)
            float frictionFactor = Mathf.Clamp01(1f - floor.Friction);
            velocity.x *= frictionFactor;

            //Evitar micro rebotes infinitos
            if (Mathf.Abs(velocity.y) < 0.1f)
                velocity.y = 0f;

            bullet.Velocity = velocity;
        }

        //Correccion de penetracion
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

        //Normal de colision
        Vector2 normal = delta / distance;

        //Velocidad relativa
        Vector2 relativeVelocity = b.Velocity - a.Velocity;

        //Velocidad relativa sobre la normal
        float normalSpeed = Vector2.Dot(relativeVelocity, normal);

        //Ya se están separando
        if (normalSpeed > 0f)
            return;

        //Restitucion efectiva
        float restitution = Mathf.Min(a.Restitution, b.Restitution);

        //Impulso
        float impulseMagnitude = -(1f + restitution) * normalSpeed;
        impulseMagnitude /= (1f / a.Mass) + (1f / b.Mass);
        Vector2 impulse = impulseMagnitude * normal;

        //Aplicar impulso
        a.Velocity -= impulse / a.Mass;
        b.Velocity += impulse / b.Mass;

        //Corrección de penetracion
        float penetration = (a.Radius + b.Radius) - distance;

        if (penetration > 0f)
        {
            Vector2 correction = normal * (penetration * 0.5f);

            a.transform.position -= (Vector3)correction;
            b.transform.position += (Vector3)correction;
        }
    }
}