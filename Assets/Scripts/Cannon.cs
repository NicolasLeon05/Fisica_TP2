using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Fire")]
    [SerializeField] private float maxLaunchSpeed = 5f;

    [Header("Visual / Collision")]
    [SerializeField] private float width = 1.5f;
    [SerializeField] private float height = 0.3f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 100f;

    private const float MAX_ANGLE = 90f;

    private float currentAngle = 0f;
    private float rotationInput = 0f;

    public OBB Bounds { get; private set; }

    private void OnValidate()
    {
        UpdateVisual();
        UpdateTransform();
        UpdateBounds();
    }

    private void FixedUpdate()
    {
        HandleRotation();
        UpdateTransform();
        UpdateBounds();

        rotationInput = 0f;
    }

    public void Fire(float powerPercent)
    {
        if (bulletPrefab == null)
            return;

        Vector2 direction = pivot.up;

        float speed = maxLaunchSpeed * Mathf.Clamp01(powerPercent);

        Vector2 initialVelocity = direction * speed;
        Vector2 spawnPosition = (Vector2)transform.position + direction * (height / 2f);
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(initialVelocity, gameObject);
    }

    public void SetRotationInput(Direction dir)
    {
        if (dir == Direction.Left)
            rotationInput = 1f;

        if (dir == Direction.Right)
            rotationInput = -1f;
    }

    public void ClearRotationInput()
    {
        rotationInput = 0f;
    }

    private void HandleRotation()
    {
        currentAngle += rotationInput * rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, -MAX_ANGLE, MAX_ANGLE);

        if (pivot != null)
            pivot.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }

    private void UpdateTransform()
    {
        if (pivot == null) return;

        transform.position = pivot.position;
        transform.localPosition = new Vector3(0, height / 2f, 0);
        transform.localScale = new Vector3(width, height, 1);
    }

    private void UpdateVisual()
    {
        transform.localScale = new Vector3(width, height, 1);
    }

    private void UpdateBounds()
    {
        Bounds = new OBB(transform.position, new Vector2(width, height), pivot != null ? pivot.eulerAngles.z : 0f);
    }
}