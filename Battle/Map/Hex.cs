using UnityEngine;
using System.Collections;

public class Point
{
    public int X, Y, Z;

    public Point(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public override string ToString()
    {
        return "[" + X + " " + Y + " " + Z + "]";
    }

    public static Point operator + (Point p1, Point p2){
        return new Point(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z);
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z);
    }
}

public class Hex : MonoBehaviour {
    public Point MapPos;
    public bool Passable = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMapPos(Point pos)
    {
        MapPos = pos;
    }

    public void SetMapPos(int x, int y, int z)
    {
        MapPos = new Point(x, y, z);
    }

    void OnMouseDown()
    {
        PlayerManager pm = PlayerManager.GetInst();
        PlayerBase pb = pm.Players[pm.CurTurnIdx];

        Debug.Log(MapPos.ToString() + "OnMouseDown");

        if (pb.act == ACT.IDLE)
        {
            if (Passable == true)
            {
                Passable = false;
                transform.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else
            {
                Passable = true;
                transform.GetComponent<Renderer>().material.color = Color.white;
            }
 
        }
        else if (pb.act == ACT.MOVEHIGHLIGHT)
        {
            pm.MovePlayer(pm.Players[pm.CurTurnIdx].CurHex, this);
        }
    }
}
