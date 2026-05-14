using UnityEngine;

public static class Collision
{
    public struct CollisionInfo
    {
        public bool collision;
        public Vector2 normal;
        public float penetration;
    }

    public static float DotProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    //========================================================
    // OBB vs OBB
    //========================================================

    public static bool OBBvsOBB(OBB a, OBB b)
    {
        //Toma los 2 ejes de los 2 objetos
        Vector2[] axes = new Vector2[]
        {
            a.Right,
            a.Up,
            b.Right,
            b.Up
        };

        foreach (var axis in axes)
        {
            if (!OverlapOnAxis(a, b, axis)) //Si en algun eje no se overlapean, no colisionan
                return false;
        }

        return true;
    }

    private static bool OverlapOnAxis(OBB a, OBB b, Vector2 axis)
    {
        Project(a, axis, out float minA, out float maxA);
        Project(b, axis, out float minB, out float maxB);

        //Si hay una direccion donde no se tocan, no hay colision
        return !(maxA < minB || maxB < minA); 
    }

    private static void Project(OBB box, Vector2 axis, out float min, out float max)
    {
        Vector2[] corners = box.GetCorners();
        max = DotProduct(corners[0], axis);
        min = DotProduct(corners[0], axis);

        for (int i = 1; i < corners.Length; i++)
        {
            float proj = DotProduct(corners[i], axis); //Que tan lejos esta este punto en esa direccion

            if (proj < min)
                min = proj;

            if (proj > max)
                max = proj;
        }
    }

    //========================================================
    // AABB vs AABB
    //========================================================

    public static bool AABBvsAABB(AABB a, AABB b)
    {
        return
            a.Min.x <= b.Max.x &&
            a.Max.x >= b.Min.x &&
            a.Min.y <= b.Max.y &&
            a.Max.y >= b.Min.y;
    }

    public static bool OBBvsAABB(OBB obb, AABB aabb)
    {
        OBB converted = new OBB(aabb.center, aabb.halfSize * 2f, 0f);

        return OBBvsOBB(obb, converted);
    }

    //========================================================
    // Circle vs Circle
    //========================================================

    public static CollisionInfo CircleVsCircle(Circle a, Circle b)
    {
        CollisionInfo info = new CollisionInfo();

        Vector2 delta = b.center - a.center;
        float distance = delta.magnitude;
        float radiusSum = a.radius + b.radius;

        if (distance >= radiusSum)
        {
            info.collision = false;
            return info;
        }

        info.collision = true;

        if (distance > 0.0001f)
            info.normal = delta.normalized;
        else
            info.normal = Vector2.right;

        info.penetration = radiusSum - distance;

        return info;
    }

    //========================================================
    // Circle vs AABB
    //========================================================

    public static CollisionInfo CircleVsAABB(Circle circle, AABB box)
    {
        CollisionInfo info = new CollisionInfo();

        Vector2 closestPoint;
        closestPoint.x = Mathf.Clamp(circle.center.x, box.Min.x, box.Max.x);
        closestPoint.y = Mathf.Clamp(circle.center.y, box.Min.y, box.Max.y);

        Vector2 difference = circle.center - closestPoint;
        float distance = difference.magnitude;

        if (distance > circle.radius)
        {
            info.collision = false;
            return info;
        }

        info.collision = true;

        if (distance > 0.0001f)
        {
            info.normal = difference.normalized;
        }
        else
        {
            float left = Mathf.Abs(circle.center.x - box.Min.x);
            float right = Mathf.Abs(box.Max.x - circle.center.x);
            float bottom = Mathf.Abs(circle.center.y - box.Min.y);
            float top = Mathf.Abs(box.Max.y - circle.center.y);

            float min = Mathf.Min(left, right, bottom, top);

            if (min == left)
                info.normal = Vector2.left;
            else if (min == right)
                info.normal = Vector2.right;
            else if (min == bottom)
                info.normal = Vector2.down;
            else
                info.normal = Vector2.up;
        }

        info.penetration = circle.radius - distance;

        return info;
    }

    //========================================================
    // Circle vs OBB
    //========================================================

    public static CollisionInfo CircleVsOBB(Circle circle, OBB box)
    {
        CollisionInfo info = new CollisionInfo();

        Vector2 localCirclePosition = circle.center - box.center;

        float rad = -box.rotation * Mathf.Deg2Rad;

        Vector2 local;
        local.x = localCirclePosition.x * Mathf.Cos(rad) - localCirclePosition.y * Mathf.Sin(rad);
        local.y = localCirclePosition.x * Mathf.Sin(rad) + localCirclePosition.y * Mathf.Cos(rad);

        Vector2 closestPoint;
        closestPoint.x = Mathf.Clamp(local.x, -box.halfSize.x, box.halfSize.x);
        closestPoint.y = Mathf.Clamp(local.y, -box.halfSize.y, box.halfSize.y);

        Vector2 difference = local - closestPoint;
        float distance = difference.magnitude;

        if (distance > circle.radius)
        {
            info.collision = false;
            return info;
        }

        info.collision = true;

        if (distance > 0.0001f)
        {
            Vector2 localNormal = difference.normalized;

            float cos = Mathf.Cos(-rad);
            float sin = Mathf.Sin(-rad);

            Vector2 worldNormal;
            worldNormal.x = localNormal.x * cos - localNormal.y * sin;
            worldNormal.y = localNormal.x * sin + localNormal.y * cos;

            info.normal = worldNormal.normalized;
        }
        else
        {
            info.normal = Vector2.up;
        }

        info.penetration = circle.radius - distance;

        return info;
    }

    //========================================================
    // Resolve Bullet Wall
    //========================================================

    public static void ResolveBulletWall(Bullet bullet, CollisionInfo info, float restitution)
    {
        //Separa la bala de la pared
        bullet.transform.position += (Vector3)(info.normal * info.penetration);

        //Calcula la velocidad en la normal de la colision
        Vector2 velocity = bullet.Velocity;
        float velocityAlongNormal = DotProduct(velocity, info.normal);

        if (velocityAlongNormal > 0f)
            return;

        //Refleja la velocidad modificada por la restitucion respecto a la normal
        velocity -= (1f + restitution) * velocityAlongNormal * info.normal;
        bullet.Velocity = velocity;
    }

    //========================================================
    // Resolve Bullet Floor
    //========================================================

    public static void ResolveBulletFloor(Bullet bullet, Floor floor, CollisionInfo info)
    {
        //Separa la bala del piso
        bullet.transform.position += (Vector3)(info.normal * info.penetration);

        //Calcula la velocidad en la normal de la colision
        Vector2 velocity = bullet.Velocity;
        float velocityAlongNormal = DotProduct(velocity, info.normal);

        if (velocityAlongNormal > 0f)
            return;

        //Refleja la velocidad modificada por la restitucion respecto a la normal
        velocity -= (1f + bullet.Restitution) * velocityAlongNormal * info.normal;

        //Calcula cuanta velocidad horizontal se pierde por la friccion
        float frictionFactor = 1f - floor.Friction;
        velocity.x *= frictionFactor;

        //Evita rebote infinito
        if (Mathf.Abs(velocity.y) < 0.1f)
            velocity.y = 0f;

        bullet.Velocity = velocity;
    }

    //========================================================
    // Resolve Bullet Bullet
    //========================================================

    public static void ResolveBulletBullet(Bullet a, Bullet b, CollisionInfo info)
    {
        float totalMass = a.Mass + b.Mass;

        Vector2 separation = info.normal * info.penetration;
        a.transform.position -= (Vector3)(separation * (b.Mass / totalMass));
        b.transform.position += (Vector3)(separation * (a.Mass / totalMass));

        Vector2 relativeVelocity = b.Velocity - a.Velocity;
        float velocityAlongNormal = DotProduct(relativeVelocity, info.normal);

        if (velocityAlongNormal > 0f)
            return;

        float restitution = Mathf.Min(a.Restitution, b.Restitution);
        float impulseMagnitude = -(1f + restitution) * velocityAlongNormal;
        impulseMagnitude /= (1f / a.Mass) + (1f / b.Mass);

        Vector2 impulse = impulseMagnitude * info.normal;
        a.Velocity -= impulse / a.Mass;
        b.Velocity += impulse / b.Mass;
    }

    //========================================================
    // Resolve Tank Tank
    //========================================================

    public static void ResolveTankTank(Tank a, Tank b)
    {
        float distance = b.transform.position.x - a.transform.position.x;
        if (distance == 0f)
            return;

        float normal = Mathf.Sign(distance);
        float overlap = (a.Bounds.halfSize.x + b.Bounds.halfSize.x) - Mathf.Abs(distance);

        if (overlap > 0f)
        {
            float totalMass = a.Mass + b.Mass;
            float moveA = overlap * (b.Mass / totalMass);
            float moveB = overlap * (a.Mass / totalMass);

            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;

            posA.x -= moveA * normal;
            posB.x += moveB * normal;

            a.transform.position = posA;
            b.transform.position = posB;
        }

        float relativeVelocity = b.Velocity - a.Velocity;

        if (relativeVelocity * normal > 0f)
            return;

        float restitution = Mathf.Min(a.Restitution, b.Restitution);

        float impulse = -(1f + restitution) * relativeVelocity;

        impulse /= (1f / a.Mass) + (1f / b.Mass);

        a.Velocity -= impulse / a.Mass;
        b.Velocity += impulse / b.Mass;
    }

    //========================================================
    // Resolve Tank Wall
    //========================================================

    public static void ResolveTankWall(Tank tank, Wall wall)
    {
        Vector3 pos = tank.transform.position;

        if (tank.transform.position.x < wall.Bounds.center.x)
            pos.x = wall.Bounds.Min.x - tank.Bounds.halfSize.x;
        else
            pos.x = wall.Bounds.Max.x + tank.Bounds.halfSize.x;

        tank.transform.position = pos;
        tank.Velocity = 0f;
    }

    //========================================================
    // Resolve Bullet Tank
    //========================================================

    public static void ResolveBulletTank(Bullet bullet, Tank tank, CollisionInfo info)
    {
        Vector2 separation = info.normal * info.penetration;
        bullet.transform.position += (Vector3)separation;

        Vector2 velocity = bullet.Velocity;
        float velocityAlongNormal = DotProduct(velocity, info.normal);

        if (velocityAlongNormal < 0f)
        {
            Vector2 reflected = velocity - (1f + bullet.Restitution) * velocityAlongNormal * info.normal;
            bullet.Velocity = reflected;
        }
    }
}