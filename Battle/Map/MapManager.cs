using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path
{
    public Path parent;
    public Hex curHex;
    public int F, H, G;

    public Path(Path parent, Hex hex, int H, int G)
    {
        this.parent = parent;
        this.H = H;
        this.G = G;
        this.F = H + G;
        this.curHex = hex;
    }
}

public class MapManager { 
    private static MapManager inst = null; 
    public GameObject GO_hex;

    public float hexW;
    public float hexH;

    public int mapSizeX = 4;
    public int mapSizeY = 4;
    public int mapSizeZ = 3;

    public Point [] Dirs;

    Hex[][][] Map;

    public static MapManager GetInst()
    {
        if (inst == null)
        {
            inst = new MapManager();
            inst.Init();
        }

        return inst;
    }

    public void Init()
    {
        GO_hex = (GameObject)Resources.Load("Prefabs/Map/Hex");

        SetHexSize();
        initDirs();
    }

    public void initDirs(){
        Dirs = new Point[6];
        Dirs[0] = new Point(+1, -1, 0); // right
        Dirs[1] = new Point(+1, 0, -1); // up right
        Dirs[2] = new Point(0, +1, -1); // up left
        Dirs[3] = new Point(-1, +1, 0); // left
        Dirs[4] = new Point(-1, 0, +1); // down left
        Dirs[5] = new Point(0, -1, +1); // down right
    }

    public void SetHexSize()
    {
        hexW = GO_hex.transform.GetComponent<Renderer>().bounds.size.x;
        hexH = GO_hex.transform.GetComponent<Renderer>().bounds.size.z;
    }

    public Vector3 GetWorldPos(int x, int y, int z)
    {
        float X = 0f;
        float Z = 0f;

        X = x * hexW + (z * hexW * 0.5f);
        Z = (-z) * hexH * 0.75f;

        return new Vector3(X, 0, Z);
    }

    public void CreateMap()
    {
        Map = new Hex[mapSizeX * 2 + 1][][];
        GameObject map = new GameObject("Map");
        for (int x = -mapSizeX; x <= mapSizeX; x++)
        {
            Map[x + mapSizeX] = new Hex[mapSizeY * 2 + 1][];
            for (int y = -mapSizeY; y <= mapSizeY; y++)
            {
                Map[x + mapSizeX][y + mapSizeY] = new Hex[mapSizeZ * 2 + 1];
                for (int z = -mapSizeZ; z <= mapSizeZ; z++)
                {
                    if (x + y + z == 0)
                    {
                        GameObject hex = (GameObject)GameObject.Instantiate(GO_hex);
                        hex.transform.parent = map.transform;

                        Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ] = hex.GetComponent<Hex>();
                        Vector3 pos = GetWorldPos(x, y, z);
                        Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].transform.position = pos;
                        Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].SetMapPos(x, y, z);
                    }
                }
            }
        }
    }

    public Hex GetPlayerHex(int x, int y, int z)
    {
        return Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ];
    }

    public bool HighLightMoveRange(Hex start, int moveRange)
    {
        int highLightCount = 0;

        for (int x = -mapSizeX; x <= mapSizeX; x++)
        {
            for (int y = -mapSizeY; y <= mapSizeY; y++)
            {
                for (int z = -mapSizeZ; z <= mapSizeZ; z++)
                {
                    if (x + y + z == 0 )
                    {
                        int distance = GetDistance(start, Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ]);
                        if (distance <= moveRange && distance != 0)
                        {
                            if (Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].Passable)
                            {
                                if (IsReachAble(start, Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ], moveRange))
                                {
                                    Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].transform.GetComponent<Renderer>().material.color = Color.green;
                                    highLightCount++;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (highLightCount == 0)
            return false;
        else
            return true;
    }

    public bool HighLightAtkRange(Hex start, int atkRange)
    {
        PlayerManager pm = PlayerManager.GetInst();
        int highLightCount = 0;

        for (int x = -mapSizeX; x <= mapSizeX; x++)
        {
            for (int y = -mapSizeY; y <= mapSizeY; y++)
            {
                for (int z = -mapSizeZ; z <= mapSizeZ; z++)
                {
                    if (x + y + z == 0)
                    {
                        int distance = GetDistance(start, Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ]);
                        if (distance <= atkRange && distance != 0)
                        {
                            if (Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].Passable)
                            {
                                bool isExit = false;
                                foreach (PlayerBase pb in pm.Players)
                                {
                                    if (pb is AIPlayer)
                                    {
                                        if (pb.CurHex.MapPos == Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].MapPos)
                                        {
                                            isExit = true;
                                            break;
                                        }
                                    }
                              
                                }
                                if (isExit)
                                {
                                    if (IsReachAble(start, Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ], atkRange))
                                    {
                                        Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].transform.GetComponent<Renderer>().material.color = Color.red;
                                        highLightCount++;
                                    }
                                }  
                            }
                        }
                    }
                }
            }
        }
        if (highLightCount == 0)
            return false;
        else
            return true;
    }

    public void ResetMapColor()
    {
        for (int x = -mapSizeX; x <= mapSizeX; x++)
        {
            for (int y = -mapSizeY; y <= mapSizeY; y++)
            {
                for (int z = -mapSizeZ; z <= mapSizeZ; z++)
                {
                    if (x + y + z == 0)
                    {
                        if (Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].Passable)
                            Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ].transform.GetComponent<Renderer>().material.color = Color.white;
                        
                    }
                }
            }
        }
    }

    public void ResetMapColor(Point pos)
    {
        Map[pos.X + mapSizeX][pos.Y + mapSizeY][pos.Z + mapSizeZ].transform.GetComponent<Renderer>().material.color = Color.white;
                   
    }

    public int GetDistance(Hex h1, Hex h2)
    {
        Point pos1 = h1.MapPos;
        Point pos2 = h2.MapPos;
        return (Mathf.Abs(pos1.X - pos2.X) + Mathf.Abs(pos1.Y - pos2.Y) + Mathf.Abs(pos1.Z - pos2.Z)) / 2;

    }

    public bool IsReachAble(Hex start, Hex dest, int moveRange)
    {
        List<Hex> path = GetPath(start, dest);
        if (path.Count == 0 || path.Count > moveRange)
            return false;
        return true;
    }

    List<Path> openList;
    List<Path> closedList;

    public List<Hex> GetPath(Hex start, Hex dest)
    {
        openList = new List<Path>();
        closedList = new List<Path>();
         List<Hex> rtnVal = new List<Hex>();

        int H = GetDistance(start, dest);
        Path startPath = new Path(null, start, 0, H);

        closedList.Add(startPath);

        Path result = Recursive_FindPath(startPath, dest);

        if (result == null)
            return rtnVal;

       
        while (result.parent != null)
        {
            rtnVal.Insert(0, result.curHex);
            result = result.parent;
        }

        return rtnVal;
    }

    public Path Recursive_FindPath(Path parent, Hex dest)
    {
        if (parent.curHex.MapPos == dest.MapPos)
            return parent;

        List<Hex> neibhors = GetNeibhors(parent.curHex);

        foreach (Hex h in neibhors)
        {
            Path newP = new Path(parent, h,  parent.G+1 , GetDistance(h, dest));
            AddToOpenList(newP);
        }

        Path bestP = openList[0];

        if (openList.Count == 0)
            return null;

        foreach (Path p in openList)
        {
            if (p.F < bestP.F)
                bestP = p;
        }

        openList.Remove(bestP);
        closedList.Add(bestP);

        return Recursive_FindPath(bestP, dest);
    }

    public void AddToOpenList(Path p){
        foreach (Path inP in closedList)
        {
            if (p.curHex.MapPos == inP.curHex.MapPos)
                return;
        }

        foreach(Path inP in openList){
            if(p.curHex.MapPos == inP.curHex.MapPos){
                if (p.F < inP.F)
                {
                    openList.Remove(inP);
                    openList.Add(p);

                    return;
                }
            }
        }
        openList.Add(p);
    }

    public List<Hex> GetNeibhors(Hex pos)
    {
        List<Hex> rtn = new List<Hex>();
        Point cur = pos.MapPos;

        foreach (Point p in Dirs)
        {
            Point tmp = p + cur;
            if (Mathf.Abs(tmp.X) <= mapSizeX && Mathf.Abs(tmp.Y) <= mapSizeY && Mathf.Abs(tmp.Z) <= mapSizeZ)
            {
                if (tmp.X + tmp.Y + tmp.Z == 0 && Map[tmp.X + mapSizeX][tmp.Y + mapSizeY][tmp.Z + mapSizeZ].Passable == true)
                {
                    rtn.Add(GetHex(tmp.X, tmp.Y, tmp.Z));
                }
            }
        }

        return rtn;
    }

    public Hex GetHex(int x, int y, int z)
    {
        return Map[x + mapSizeX][y + mapSizeY][z + mapSizeZ];
    }

    public void SetHexColor(Hex hex, Color color)
    {
        hex.transform.GetComponent<Renderer>().material.color = color;
    }
}
