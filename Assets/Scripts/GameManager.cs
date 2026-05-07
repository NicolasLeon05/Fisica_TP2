using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
    None
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Tank tank1;
    [SerializeField] private Tank tank2;

    [SerializeField] private Floor floor;
    [SerializeField] private Wall wall1;
    [SerializeField] private Wall wall2;

    private List<Bullet> activeBullets;
    public List<Bullet> Bullets => activeBullets;

    private void Update()
    {
        //Bullets collision with walls check
        //Bullets collision with other bullets
        //Bullets collision with tanks
        //Tank collision with tank

        //Tank 1
        if (Input.GetKey(KeyCode.A))
            tank1.SetInput(Direction.Left);
       
        if (Input.GetKey(KeyCode.D))
            tank1.SetInput(Direction.Right);

        if (Input.GetKey(KeyCode.Q))
            tank1.SetCannonInput(Direction.Left);
        else if (Input.GetKey(KeyCode.E))
            tank1.SetCannonInput(Direction.Right);
        else
            tank1.ClearCannonInput();

        if (Input.GetKeyDown(KeyCode.W)) // Fire
        {

        }

        //Tank 2
        if (Input.GetKeyDown(KeyCode.J)) // Move Left
        {

        }
        if (Input.GetKeyDown(KeyCode.L)) // Move Right
        {

        }
        if (Input.GetKeyDown(KeyCode.U)) // Rotate Left
        {

        }
        if (Input.GetKeyDown(KeyCode.O)) // Rotate Right
        {

        }
        if (Input.GetKeyDown(KeyCode.I)) // Fire
        {

        }





    }
}
