using UnityEngine;
using UnityEngine.InputSystem;

public class Tank : MonoBehaviour
{
    //INPUTS PLAYER 1:
    //Movement: A - D
    //Cannon rotation: Q - E
    //Fire: W (longer pressed, more acceleration for the bullet)

    //INPUTS PLAYER 2:
    //Movement: J - L
    //Cannon rotation: U - O
    //Fire: I (longer pressed, more acceleration for the bullet)

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Standard")]
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private Vector2 initialPosition;

    [Header("Physics")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float mass;
    [SerializeField] private float restitutionCoefficient;


    public void Move(Direction dir)
    {
        //Placeholder
        float mov = 0;
        if (dir == Direction.Left)
            mov = -1;
        if (dir == Direction.Right)
            mov = 1;

        Vector3 newPos = transform.position;
        newPos.x += mov;
        transform.position = newPos;
    }
}
