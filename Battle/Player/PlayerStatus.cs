using UnityEngine;
using System.Collections;

public class PlayerStatus {
    public string Name = "Test";
    public int MoveRange = 3;
    public int AtkRange = 1;
    public float MoveSpeed = 3f;
    public int CurHP = 30;

    public PlayerStatus()
    {
        this.Name = "Test";
        this.MoveRange = 2;
        this.AtkRange = 1;
        this.MoveSpeed = 3f;
        this.CurHP = 30;
    }
}
