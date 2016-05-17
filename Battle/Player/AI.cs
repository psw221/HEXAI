using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI {
    private static AI inst = null;

    public static AI GetInst()
    {
        if (inst == null)
        {
            inst = new AI();
        }
        return inst;
    }

    public void MoveAIToNearUserPlayer(PlayerBase aiPlayer)
    {
        PlayerManager pm = PlayerManager.GetInst();
        MapManager mm = MapManager.GetInst();
        // 근접한 플레이어를 찾는다.

        PlayerBase nearUserPlayer = null;
        int nearDistance = 1000;

        foreach (PlayerBase up in pm.Players)
        {
            if (up is UserPlayer)
            {
                int distance = mm.GetDistance(up.CurHex, aiPlayer.CurHex); 
                if (distance < nearDistance)
                {
                    nearDistance = distance;
                    nearUserPlayer = up;
                }
            }
        }

        if (nearUserPlayer != null)
        {
            
            // 근접한 플레이어로 이동
            List<Hex> path = mm.GetPath(aiPlayer.CurHex, nearUserPlayer.CurHex);

            if (path.Count > aiPlayer.status.MoveRange)
            {
                path.RemoveRange(aiPlayer.status.MoveRange, path.Count - aiPlayer.status.MoveRange);
            }
            aiPlayer.MoveHexes = path;

            if(nearUserPlayer.CurHex.MapPos == aiPlayer.MoveHexes[aiPlayer.MoveHexes.Count - 1].MapPos)
                aiPlayer.MoveHexes.RemoveAt(aiPlayer.MoveHexes.Count - 1);


            if (aiPlayer.MoveHexes.Count == 0)
                return;

            aiPlayer.act = ACT.MOVING;
            MapManager.GetInst().ResetMapColor(aiPlayer.CurHex.MapPos);
        }
        

        // 만약 근점 후 공격이 가능하면 공격
    }

    public void AtkAItoUser(PlayerBase aiPlayer)
    {
        PlayerManager pm = PlayerManager.GetInst();
        MapManager mm = MapManager.GetInst();
        // 근접한 플레이어를 찾는다.

        PlayerBase nearUserPlayer = null;
        int nearDistance = 1000;

        foreach (PlayerBase up in pm.Players)
        {
            if (up is UserPlayer)
            {
                int distance = mm.GetDistance(up.CurHex, aiPlayer.CurHex);
                if (distance < nearDistance)
                {
                    nearDistance = distance;
                    nearUserPlayer = up;
                }
            }
        }

        if (nearUserPlayer != null)
        {
            BattleManager.GetInst().AttackAtoB(aiPlayer, nearUserPlayer);
            aiPlayer.act = ACT.ATTACKING;

            return;
        }

        pm.TurnOver();
    }

}
