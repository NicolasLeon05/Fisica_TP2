using System;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Fire")]
    [SerializeField] private float maxLaunchSpeed = 15f;

    [Header("Visual / Collision")]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 100f;

    private const float MAX_ANGLE = 90f;

    private float currentAngle = 0f;
    private float rotationInput = 0f;

    public static Action<Bullet> OnBulletSpawned;
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
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.Initialize(initialVelocity, gameObject);

        OnBulletSpawned?.Invoke(bulletComponent);
    }

    public Vector2 GetFireDirection()
    {
        return pivot.up;
    }

    public Vector2 GetSpawnPosition()
    {
        Vector2 direction = GetFireDirection();
        return (Vector2)transform.position + direction * (height / 2f);
    }

    public float GetLaunchSpeed(float powerPercent)
    {
        return maxLaunchSpeed * Mathf.Clamp01(powerPercent);
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