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
        CheckBulletBulletCollisions();
        CheckBulletTankCollisions();
        CheckTankTankCollision();
        CheckTankWallCollisions();
    }

    public void RegisterBullet(Bullet bullet)
    {
        activeBullets.Add(bullet);
    }

    // Bullet-Wall
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
        Collision.CollisionInfo info = Collision.CircleVsAABB(bullet.Bounds, wall.Bounds);

        if (!info.collision)
            return;

        Collision.ResolveBulletWall(bullet, info, wall.RestitutionCoefficient);
    }

    // Bullet-Floor
    private void CheckBulletFloorCollisions()
    {
        foreach (Bullet bullet in activeBullets)
            CheckBulletFloor(bullet, floor);
    }

    private void CheckBulletFloor(Bullet bullet, Floor floor)
    {
        Collision.CollisionInfo info = Collision.CircleVsAABB(bullet.Bounds, floor.Bounds);

        if (!info.collision)
            return;

        Collision.ResolveBulletFloor(bullet, floor, info);
    }

    // Bullet-Bullet
    private void CheckBulletBulletCollisions()
    {
        for (int i = 0; i < activeBullets.Count; i++)
        {
            for (int j = i + 1; j < activeBullets.Count; j++)
            {
                Bullet a = activeBullets[i];
                Bullet b = activeBullets[j];

                CheckBulletBullet(a, b);
            }
        }
    }

    private void CheckBulletBullet(Bullet a, Bullet b)
    {
        Collision.CollisionInfo info = Collision.CircleVsCircle(a.Bounds, b.Bounds);

        if (!info.collision)
            return;

        Collision.ResolveBulletBullet(a, b, info);
    }

    // Bullet-Tank
    private void CheckBulletTankCollisions()
    {
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            Bullet bullet = activeBullets[i];

            CheckBulletTank(bullet, tank1);
            CheckBulletTank(bullet, tank2);
        }
    }

    private void CheckBulletTank(Bullet bullet, Tank tank)
    {
        if (bullet == null || tank == null)
            return;

        if (bullet.OriginGO == tank.Cannon.gameObject)
            return;

        Collision.CollisionInfo info = Collision.CircleVsAABB(bullet.Bounds, tank.Bounds);
        if (!info.collision)
        {
            info = Collision.CircleVsOBB(bullet.Bounds, tank.Cannon.Bounds);

            if (!info.collision)
                return;
        }

        Collision.ResolveBulletTank(bullet, tank, info);

        activeBullets.Remove(bullet);
        Destroy(bullet.gameObject);
    }

    // Tank-Tank
    private void CheckTankTankCollision()
    {
        bool bodyVsBody = Collision.AABBvsAABB(tank1.Bounds, tank2.Bounds);
        bool bodyVsCannon = Collision.OBBvsAABB(tank1.Cannon.Bounds, tank2.Bounds);
        bool cannonVsBody = Collision.OBBvsAABB(tank2.Cannon.Bounds, tank1.Bounds);
        bool cannonVsCannon = Collision.OBBvsOBB(tank1.Cannon.Bounds, tank2.Cannon.Bounds);

        if (!bodyVsBody &&
            !bodyVsCannon &&
            !cannonVsBody &&
            !cannonVsCannon)
            return;

        Collision.ResolveTankTank(tank1, tank2);
    }

    // Tank-Wall
    private void CheckTankWallCollisions()
    {
        CheckTankWall(tank1, wall1);
        CheckTankWall(tank1, wall2);

        CheckTankWall(tank2, wall1);
        CheckTankWall(tank2, wall2);
    }

    private void CheckTankWall(Tank tank, Wall wall)
    {
        if (!Collision.AABBvsAABB(tank.Bounds, wall.Bounds))
            return;

        Collision.ResolveTankWall(tank, wall);
    }
}