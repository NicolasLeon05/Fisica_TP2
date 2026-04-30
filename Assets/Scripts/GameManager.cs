using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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

    }
}
