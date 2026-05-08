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

    private List<Bullet> activeBullets = new();

    public List<Bullet> Bullets => activeBullets;

    private void OnEnable()
    {
        Cannon.OnBulletSpawned += RegisterBullet;
    }

    private void OnDisable()
    {
        Cannon.OnBulletSpawned -= RegisterBullet;
    }

    private void Update()
    {
        // Tank 1

        if (Input.GetKey(KeyCode.A))
            tank1.SetInput(Direction.Left);
        else if (Input.GetKey(KeyCode.D))
            tank1.SetInput(Direction.Right);
        else
            tank1.ClearInput();

        if (Input.GetKey(KeyCode.Q))
            tank1.SetCannonInput(Direction.Left);
        else if (Input.GetKey(KeyCode.E))
            tank1.SetCannonInput(Direction.Right);
        else
            tank1.ClearCannonInput();

        if (Input.GetKeyDown(KeyCode.W))
            tank1.StartChargingShot();

        if (Input.GetKeyUp(KeyCode.W))
            tank1.ReleaseShot();

        // Tank 2

        if (Input.GetKey(KeyCode.J))
            tank2.SetInput(Direction.Left);
        else if (Input.GetKey(KeyCode.L))
            tank2.SetInput(Direction.Right);
        else
            tank2.ClearInput();

        if (Input.GetKey(KeyCode.U))
            tank2.SetCannonInput(Direction.Left);
        else if (Input.GetKey(KeyCode.O))
            tank2.SetCannonInput(Direction.Right);
        else
            tank2.ClearCannonInput();

        if (Input.GetKeyDown(KeyCode.I))
            tank2.StartChargingShot();

        if (Input.GetKeyUp(KeyCode.I))
            tank2.ReleaseShot();
    }

    private void FixedUpdate()
    {
        CheckBulletWallCollisions();
        CheckBulletFloorCollisions();

        // Bullet vs Bullet
        // Bullet vs Tank
        // Tank vs Tank
        // Tank vs Wall
    }

    public void RegisterBullet(Bullet bullet)
    {
        activeBullets.Add(bullet);
    }

    private void CheckBulletWallCollisions()
    {
        foreach (Bullet bullet in activeBullets)
        {
            CheckBulletWall(bullet, wall1);
            CheckBulletWall(bullet, wall2);
        }
    }

    private void CheckBulletWall(Bullet bullet, Wall wall)
    {
        if (!Collision.CircleVsAABB(bullet.Bounds, wall.Bounds))
            return;

        Collision.ResolveBulletWall(bullet, wall);
    }

    private void CheckBulletFloorCollisions()
    {
        foreach (Bullet bullet in activeBullets)
            CheckBulletFloor(bullet, floor);
    }

    private void CheckBulletFloor(Bullet bullet, Floor floor)
    {
        if (!Collision.CircleVsAABB(bullet.Bounds, floor.Bounds))
            return;

        Collision.ResolveBulletFloor(bullet, floor);
    }
}