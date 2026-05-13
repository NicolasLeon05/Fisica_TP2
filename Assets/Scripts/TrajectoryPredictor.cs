using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tank tank;
    [SerializeField] private Floor floor;
    [SerializeField] private Wall wall1;
    [SerializeField] private Wall wall2;

    [Header("Simulation")]
    [SerializeField] private float simulationStep = 0.02f;
    [SerializeField] private int maxIterations = 500;
    [SerializeField] private float gravity = 9.8f;

    [Header("Bullet")]
    [SerializeField] private float bulletRadius = 0.2f;
    [SerializeField]
    [Range(0f, 1f)]
    private float bulletRestitution = 0.8f;

    [Header("Visual")]
    [SerializeField] private Transform marker;

    private void Update()
    {
        if (!tank.IsChargingShot)
        {
            //marker.gameObject.SetActive(false);
            return;
        }

        marker.gameObject.SetActive(true);

        Vector2 finalPosition = SimulateTrajectory();
        marker.position = finalPosition;
    }

    private Vector2 SimulateTrajectory()
    {
        Cannon cannon = tank.Cannon;

        Vector2 position = cannon.GetSpawnPosition();

        Vector2 velocity = cannon.GetFireDirection() * cannon.GetLaunchSpeed(tank.CurrentChargePercent);

        for (int i = 0; i < maxIterations; i++)
        {
            // MISMA integración que Bullet.cs
            Vector2 acceleration = Vector2.down * gravity;
            Vector2 deltaPos = velocity * simulationStep + 0.5f * acceleration * simulationStep * simulationStep;

            velocity += acceleration * simulationStep;
            position += deltaPos;

            Circle simulatedBullet = new Circle(position, bulletRadius);

            // FLOOR
            if (Collision.CircleVsAABB(simulatedBullet, floor.Bounds))
            {
                if (velocity.y < 0f)
                {
                    velocity.y = -velocity.y * bulletRestitution;

                    float frictionFactor = Mathf.Clamp01(1f - floor.Friction);

                    velocity.x *= frictionFactor;

                    if (Mathf.Abs(velocity.y) < 0.1f)
                        velocity.y = 0f;
                }

                position.y = floor.Bounds.Max.y + bulletRadius;
            }

            // LEFT WALL

            if (Collision.CircleVsAABB(simulatedBullet, wall1.Bounds))
            {
                velocity.x *= -1f;

                position.x = wall1.Bounds.Max.x + bulletRadius;
            }

            // RIGHT WALL

            if (Collision.CircleVsAABB(simulatedBullet, wall2.Bounds))
            {
                velocity.x *= -1f;

                position.x = wall2.Bounds.Min.x - bulletRadius;
            }

            // TERMINÓ

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