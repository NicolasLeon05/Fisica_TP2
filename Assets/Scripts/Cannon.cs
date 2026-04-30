using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Standard")]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Header("Physics")]
    private const float MAX_CANNON_ROTATION_ANGLE = 90f;
    private float acceleration;
    private float rotationAngle;

}
