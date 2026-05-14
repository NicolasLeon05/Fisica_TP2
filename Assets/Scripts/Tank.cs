using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Cannon cannon;
    [SerializeField] private Floor floor;

    [Header("Standard")]
    [SerializeField] private Vector2 initialPosition;

    [Header("Visual")]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Header("Fire")]
    [SerializeField] private float maxChargeTime = 2f;

    [Header("Physics")]
    [SerializeField] private float movementForce;
    [SerializeField] private float mass;
    [SerializeField][Range(0f, 1f)] private float restitution = 0.2f;

    public float Restitution => restitution;

    private float currentCharge = 0f;
    private bool chargingShot = false;
    public bool IsChargingShot => chargingShot;
    public float CurrentChargePercent =>
        Mathf.Clamp01(currentCharge / maxChargeTime);

    private float velocity = 0f;
    private float input = 0f;

    private const float GRAVITY = 9.8f;

    public Cannon Cannon => cannon;
    public float Mass => mass;
    public float Velocity
    {
        get => velocity;
        set => velocity = value;
    }
    public AABB Bounds => new AABB(transform.position, new Vector2(width, height));

    private void Start()
    {
        transform.position = initialPosition;
        UpdateVisual();
    }

    private void OnValidate()
    {
        transform.position = initialPosition;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        transform.localScale = new Vector3(width, height, 1);
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        float appliedForce = input * movementForce;
        float normalForce = mass * GRAVITY;

        float frictionForce = 0f;

        if (Mathf.Abs(velocity) > 0.001f)
        {
            //Friccion cinetica
            frictionForce = -Mathf.Sign(velocity) * floor.Friction * normalForce;
        }
        else if (input != 0)
        {
            //Friccion estatica:
            //impide movimiento si la fuerza aplicada no supera la friccion
            float maxStaticFriction = floor.Friction * normalForce;

            if (Mathf.Abs(appliedForce) < maxStaticFriction)
            {
                velocity = 0f;
                return;
            }

            frictionForce = -Mathf.Sign(appliedForce) * maxStaticFriction;
        }

        //Fuerza neta
        float totalForce = appliedForce + frictionForce;

        //Segunda ley de Newton
        float acceleration = totalForce / mass;

        //Integracion
        velocity += acceleration * dt; //vf = vi + a*T

        if (input == 0 && Mathf.Abs(velocity) < 0.01f)
            velocity = 0f;

        transform.position += Vector3.right * velocity * dt; //MRU xf = xi + v*T

        ResolveFloorConstraint();

        if (chargingShot)
        {
            currentCharge += Time.deltaTime;

            if (currentCharge > maxChargeTime)
                currentCharge = maxChargeTime;
        }
    }

    private void ResolveFloorConstraint()
    {
        Vector3 pos = transform.position;
        pos.y = floor.Bounds.Max.y + Bounds.halfSize.y;
        transform.position = pos;
    }

    public void SetInput(Direction dir)
    {
        if (dir == Direction.Left)
            input = -1;

        else if (dir == Direction.Right)
            input = 1;
    }

    public void ClearInput()
    {
        input = 0;
    }

    public void SetCannonInput(Direction dir)
    {
        if (cannon != null)
            cannon.SetRotationInput(dir);
    }

    public void ClearCannonInput()
    {
        if (cannon != null)
            cannon.ClearRotationInput();
    }

    public void StartChargingShot()
    {
        chargingShot = true;
    }

    public void ReleaseShot()
    {
        if (!chargingShot)
            return;

        chargingShot = false;

        float powerPercent = Mathf.Clamp01(currentCharge / maxChargeTime);
        cannon.Fire(powerPercent);

        currentCharge = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        AABB box = Bounds;

        Gizmos.DrawWireCube(box.center, box.halfSize * 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}