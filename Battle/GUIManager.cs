using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager
{
    private static GUIManager inst = null;
    private PlayerManager pm = null;

    public static GUIManager GetInst()
    {
        if (inst == null)
        {
            inst = new GUIManager();
            inst.Init();
        }
        return inst;
    }

    public void Init()
    {
        pm = PlayerManager.GetInst();
        InitTurnInfo();
    }


    public void DrawGUI()
    {
        if (pm.Players.Count > 0)
        {
            Debug.Log("index : " + pm.CurTurnIdx);
            PlayerBase pb = pm.Players[pm.CurTurnIdx];
            if (pb is UserPlayer)
            {
                if (pb.act == ACT.IDLE)
                {
                    DrawCommand(pm.Players[pm.CurTurnIdx]);
                    DrawStatus(pm.Players[pm.CurTurnIdx]);
                }
            }
        }
        DrawTurnInfo();
    }

    public void DrawStatus(PlayerBase pb)
    {
        GUILayout.BeginArea(new Rect(0, Screen.height / 2, 150f, Screen.height / 2), "Player Info", GUI.skin.window);
        GUILayout.Label("Name : " + pb.status.Name);
        GUILayout.Label("HP : " + pb.status.CurHP);
        GUILayout.Label("MoveRange : " + pb.status.MoveRange);
        GUILayout.Label("AtkRange : " + pb.status.AtkRange);
        GUILayout.EndArea();
    }

    public void DrawCommand(PlayerBase pb)
    {
        float cmdW = 150f;
        float btnW = 100f;
        float btnH = 50f;
        int cmdCount = 3;

        GUILayout.BeginArea(new Rect(Screen.width - cmdW, Screen.height - cmdCount * btnH, cmdW, cmdCount * btnH), "Command", GUI.skin.window);
        if (GUILayout.Button("Move"))
        {
            Debug.Log("Move");
            if (MapManager.GetInst().HighLightMoveRange(pb.CurHex, pb.status.MoveRange))
                pb.act = ACT.MOVEHIGHLIGHT;
        }
        if (GUILayout.Button("Attack"))
        {
            Debug.Log("Attack");

            if (MapManager.GetInst().HighLightAtkRange(pb.CurHex, pb.status.AtkRange))
                pb.act = ACT.ATTACKHIGHLIGHT;
        }
        if (GUILayout.Button("TurnOver"))
        {
            Debug.Log("Turn Over");

            PlayerManager.GetInst().TurnOver();
        }

        GUILayout.EndArea();

    }

    List<GameObject> players;

    public void InitTurnInfo()
    {
        players = new List<GameObject>();
    }

    public void AddTurnPlayer(PlayerBase pb)
    {
        GameObject userPlayer = pm.GO_userPlayer;
        GameObject aiPlayer = pm.GO_aiPlayer;

        if (pb is UserPlayer)
        {
            players.Add((GameObject)GameObject.Instantiate(userPlayer, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)));
        }
        else if (pb is AIPlayer)
        {
            players.Add((GameObject)GameObject.Instantiate(aiPlayer, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)));
        }
    }

    public void RemoveTurnPlayer(int turnIndex)
    {
        GameObject pb = players[turnIndex];
        players.RemoveAt(turnIndex);
        GameObject.Destroy(pb);
    }

    float camX;
    float camZ;

    public void DrawTurnInfo()
    {
        int maxDraw = 5;
        int curDraw = pm.CurTurnIdx;

        GUILayout.BeginArea(new Rect(0, 0, 320f, 110f), "Turn Info", GUI.skin.window);
        GUILayout.EndArea();

        if (maxDraw > players.Count)
        {
            maxDraw = players.Count;
        }

        for (int i = 0; i < maxDraw; i++)
        {
            players[curDraw].transform.position = new Vector3(-7f + i, 1.4f, 0f);
            players[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            curDraw++;
            if (curDraw == pm.Players.Count)
            {
                curDraw = 0;
            }
        }
    }

    public void UpdateTurnInfoPos(float x, float z)
    {
        camX = x;
        camZ = z;
    }
}
