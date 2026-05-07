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

        //Cannon rotation
        if (Input.GetKey(KeyCode.Q))
            tank1.SetCannonInput(Direction.Left);
        else if (Input.GetKey(KeyCode.E))
            tank1.SetCannonInput(Direction.Right);
        else
            tank1.ClearCannonInput();

        //Fire
        if (Input.GetKeyDown(KeyCode.W))
            tank1.StartChargingShot();
        if (Input.GetKeyUp(KeyCode.W))
            tank1.ReleaseShot();


        //Tank 2
        if (Input.GetKey(KeyCode.J)) //Move Left
            tank2.SetInput(Direction.Left);

        if (Input.GetKey(KeyCode.L)) //Move Right
            tank2.SetInput(Direction.Right);

        //Cannon rotation
        if (Input.GetKey(KeyCode.U)) //Rotate Left
            tank2.SetCannonInput(Direction.Left);
        else if (Input.GetKey(KeyCode.O)) //Rotate Right
            tank2.SetCannonInput(Direction.Right);
        else
            tank2.ClearCannonInput();

        //Fire
        if (Input.GetKeyDown(KeyCode.I))
            tank2.StartChargingShot();

        if (Input.GetKeyUp(KeyCode.I))
            tank2.ReleaseShot();
    }
}
