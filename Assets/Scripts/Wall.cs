using UnityEngine;

public class Wall : MonoBehaviour
{
    private const float RESTITUTION_COEFFICIENT = 1f;

    private Vector2 size;

    public float RestitutionCoefficient => RESTITUTION_COEFFICIENT;

    public AABB Bounds => new AABB(transform.position, size);

    private void OnValidate()
    {
        UpdateSize();
    }

    private void Awake()
    {
        UpdateSize();
    }

    private void UpdateSize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            size.x = sr.sprite.bounds.size.x * transform.lossyScale.x;
            size.y = sr.sprite.bounds.size.y * transform.lossyScale.y;
        }
    }
}