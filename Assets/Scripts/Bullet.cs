using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float mass;
    [Range(0, 1)]
    [SerializeField] private float restitutionCoefficient;
    [SerializeField] private float radius = 0.2f;

    [SerializeField] private float gravity = 9.8f;

    private GameObject originGO;
    public GameObject OriginGO => originGO;

    private Vector2 velocity;

    public float Mass => mass;
    public float Restitution => restitutionCoefficient;
    public float Radius => radius;

    public void Initialize(Vector2 initialVelocity, GameObject origin)
    {
        velocity = initialVelocity;
        originGO = origin;
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector2 acceleration = new Vector2(0, -gravity);

        Vector2 deltaPos = velocity * dt + 0.5f * acceleration * dt * dt;

        velocity += acceleration * dt;

        transform.position += (Vector3)deltaPos;
    }
}