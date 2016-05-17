using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager {
    private static PlayerManager inst = null;
    public GameObject GO_userPlayer;
    public GameObject GO_aiPlayer;
    public List<PlayerBase> Players = new List<PlayerBase>();
    public int CurTurnIdx = 0;

    private float turnOverTime = 0f;
    private float curTurnOverTime = 0f;

    public static PlayerManager GetInst()
    {
        if (inst == null)
        {
            inst = new PlayerManager();
            inst.Init();
        }

        return inst;
    }

    public void Init()
    {
        turnOverTime = 0f;
        curTurnOverTime = 0f;

        GO_userPlayer = (GameObject)Resources.Load("Prefabs/Players/UserPlayer_Skel");
        GO_aiPlayer = (GameObject)Resources.Load("Prefabs/Players/AI_Skel");
    }


    public void CheckTurnOver()
    {
        if (curTurnOverTime != 0)
        {
            curTurnOverTime += Time.smoothDeltaTime;
            if (curTurnOverTime >= turnOverTime)
            {
                curTurnOverTime = 0;
                TurnOver();
            }
        }
    }

    public void GenPlayerTest()
    {
        // Top King
        UserPlayer player = ((GameObject)GameObject.Instantiate(GO_userPlayer)).GetComponent<UserPlayer>();
        Hex hex = MapManager.GetInst().GetPlayerHex(3, -3, 0);
        player.CurHex = hex;
        player.transform.position = player.CurHex.transform.position;
        Players.Add(player);
        GUIManager.GetInst().AddTurnPlayer(player);

        // Bottom King
        AIPlayer ai = ((GameObject)GameObject.Instantiate(GO_aiPlayer)).GetComponent<AIPlayer>();
        hex = MapManager.GetInst().GetPlayerHex(-3, 3, 0);
        ai.CurHex = hex;
        ai.transform.position = ai.CurHex.transform.position;
        Players.Add(ai);
        GUIManager.GetInst().AddTurnPlayer(ai);

 

    }

    public void MovePlayer(Hex start, Hex dest)
    {
        PlayerBase pb = Players[CurTurnIdx];
        
        if (!MapManager.GetInst().IsReachAble(start, dest, pb.status.MoveRange))
            return;

        if (pb.act == ACT.MOVEHIGHLIGHT)
        {
           
            int distance = MapManager.GetInst().GetDistance(start, dest);
            if (dest.Passable)
            {
                if (distance <= pb.status.MoveRange && distance != 0)
                {
                    pb.MoveHexes = MapManager.GetInst().GetPath(start, dest);
                    if (pb.MoveHexes.Count == 0)
                        return;
                    pb.act = ACT.MOVING;
                
                    pb.CurHex = dest;
                    

                    //      pb.transform.position = dest.transform.position;
                    //       TurnOver();

                    MapManager.GetInst().ResetMapColor();
                }
            }
        }
    }

    public void SetTurnOverTime(float time)
    {
        turnOverTime = time;
        curTurnOverTime = Time.smoothDeltaTime;
    }

    public void TurnOver()
    {
        MapManager.GetInst().ResetMapColor();
        PlayerBase pb = Players[CurTurnIdx];
        pb.act = ACT.IDLE;

        CurTurnIdx++;
        if (CurTurnIdx >= Players.Count)
            CurTurnIdx = 0;

        GameManager.GetInst().MoveCamPosToHex(Players[CurTurnIdx].CurHex);

    }

    public void RemovePlayer(PlayerBase pb)
    {
        int pos = Players.IndexOf(pb);
        Players.Remove(pb);
        GUIManager.GetInst().RemoveTurnPlayer(pos);
        GameObject.Destroy(pb.gameObject);

        int enemyCnt = 0;
        int userCnt = 0;
        foreach (PlayerBase pb2 in Players)
        {
            if (pb2 is AIPlayer)
                enemyCnt++;
            else if (pb2 is UserPlayer)
                userCnt++;
        }

        if (enemyCnt == 0 || userCnt == 0)
            EventManager.GetInst().GameEnd = true;
    }

    public void MouseInputPorc(int btn)
    {
        if (btn == 1)
        {
            PlayerBase pb = Players[CurTurnIdx];
            if (pb is AIPlayer)
                return;

            ACT act = Players[CurTurnIdx].act;
            if (act == ACT.IDLE)
                return;
            if (act == ACT.MOVEHIGHLIGHT || act == ACT.ATTACKHIGHLIGHT)
            {
                Players[CurTurnIdx].act = ACT.IDLE;
                MapManager.GetInst().ResetMapColor();
            }
        }
    }
}
