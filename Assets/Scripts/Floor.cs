using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private float friction;

    private Vector2 size;

    public float Friction => friction;
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
            size = Vector2.Scale(sr.sprite.bounds.size, transform.lossyScale);
    }
}