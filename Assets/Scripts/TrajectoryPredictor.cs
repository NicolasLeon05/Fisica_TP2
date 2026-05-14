using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tank tank;
    [SerializeField] private Floor floor;
    [SerializeField] private Wall wall1;
    [SerializeField] private Wall wall2;

    [Header("Simulation")]
    [SerializeField] private int maxIterations = 500;
    [SerializeField] private float gravity = 9.8f;

    [Header("Visual")]
    [SerializeField] private Transform marker;

    private float SimulationStep => Time.fixedDeltaTime;

    private void Update()
    {
        if (!tank.IsChargingShot)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        marker.gameObject.SetActive(true);

        Vector2 finalPosition = SimulateTrajectory();
        marker.position = finalPosition;
    }

    private Vector2 SimulateTrajectory()
    {
        Cannon cannon = tank.Cannon;

        Bullet bulletData = cannon.BulletPrefab;

        float bulletRadius = bulletData.Radius;
        float bulletRestitution = bulletData.Restitution;
        float bulletMass = bulletData.Mass;

        Vector2 position = cannon.GetSpawnPosition();
        float force =cannon.GetLaunchSpeed(tank.CurrentChargePercent);
        float acceleration = force / bulletMass;
        Vector2 velocity =cannon.GetFireDirection() *acceleration;

        for (int i = 0; i < maxIterations; i++)
        {
            Vector2 accel = Vector2.down * gravity;
            Vector2 deltaPos = velocity * SimulationStep + 0.5f * accel * SimulationStep * SimulationStep;

            velocity += accel * SimulationStep;
            position += deltaPos;

            Circle simulatedBullet = new Circle(position, bulletRadius);

            // FLOOR
            Collision.CollisionInfo floorInfo = Collision.CircleVsAABB(simulatedBullet, floor.Bounds);

            if (floorInfo.collision)
            {
                position += floorInfo.normal * floorInfo.penetration;

                float velocityAlongNormal = Collision.DotProduct(velocity, floorInfo.normal);
                if (velocityAlongNormal < 0f)
                {
                    velocity -= (1f + bulletRestitution) * velocityAlongNormal * floorInfo.normal;

                    float frictionFactor = 1f - floor.Friction;
                    velocity.x *= frictionFactor;

                    if (Mathf.Abs(velocity.y) < 0.1f)
                        velocity.y = 0f;
                }
            }


            // LEFT WALL
            Collision.CollisionInfo leftWallInfo = Collision.CircleVsAABB(simulatedBullet, wall1.Bounds);

            if (leftWallInfo.collision)
            {
                position += leftWallInfo.normal * leftWallInfo.penetration;

                float velocityAlongNormal = Collision.DotProduct(velocity, leftWallInfo.normal);
                if (velocityAlongNormal < 0f)
                    velocity -= (1f + wall1.RestitutionCoefficient) * velocityAlongNormal * leftWallInfo.normal;
            }


            // RIGHT WALL
            Collision.CollisionInfo rightWallInfo = Collision.CircleVsAABB(simulatedBullet, wall2.Bounds);

            if (rightWallInfo.collision)
            {
                position += rightWallInfo.normal * rightWallInfo.penetration;

                float velocityAlongNormal = Collision.DotProduct(velocity, rightWallInfo.normal);
                if (velocityAlongNormal < 0f)
                    velocity -= (1f + wall2.RestitutionCoefficient) * velocityAlongNormal * rightWallInfo.normal;
            }


            bool stoppedOnFloor =
                Mathf.Abs(velocity.x) < 0.05f &&
                Mathf.Abs(velocity.y) < 0.05f &&
                position.y <= floor.Bounds.Max.y + bulletRadius + 0.01f;

            if (stoppedOnFloor)
                return position;
        }

        return position;
    }
}