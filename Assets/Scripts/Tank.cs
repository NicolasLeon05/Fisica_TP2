using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Cannon cannon;

    [Header("Standard")]
    [SerializeField] private Vector2 initialPosition;

    [Header("Fire")]
    [SerializeField] private float maxChargeTime = 2f;
    private float currentCharge = 0f;
    private bool chargingShot = false;

    [Header("Physics")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float mass;
    [SerializeField] private float frictionCoefficient;

    private float velocity = 0f;
    private float input = 0f;

    private void Start()
    {
        transform.position = initialPosition;
    }

    private void OnValidate()
    {
        transform.position = initialPosition;
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        float force = input * movementAcceleration;
        ClearInput();

        float friction = 0f;
        if (velocity != 0)
            friction = -Mathf.Sign(velocity) * frictionCoefficient;

        float acceleration = (force + friction) / mass;

        float deltaX = velocity * dt + 0.5f * acceleration * dt * dt;
        velocity += acceleration * dt;

        if (Mathf.Abs(velocity) < 0.01f)
            velocity = 0;

        transform.position += new Vector3(deltaX, 0, 0);


        if (chargingShot)
        {
            currentCharge += Time.deltaTime;

            if (currentCharge > maxChargeTime)
                currentCharge = maxChargeTime;
        }
    }

    public void SetInput(Direction dir)
    {
        if (dir == Direction.Left) input = -1;
        else if (dir == Direction.Right) input = 1;
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

        Debug.Log("Shot released");
        chargingShot = false;

        float powerPercent = Mathf.Clamp01(currentCharge / maxChargeTime);

        cannon.Fire(powerPercent);

        currentCharge = 0f;
    }
}