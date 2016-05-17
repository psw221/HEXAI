using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserPlayer : PlayerBase
{
    void Awake()
    {
        anim = GetComponent<Animator>();
        act = ACT.IDLE;
        status = new PlayerStatus();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerManager pm = PlayerManager.GetInst();

        if (removeTime != 0)
        {
            removeTime += Time.smoothDeltaTime;
            if (removeTime >= 2f)
            {
                pm.RemovePlayer(this);
                pm.TurnOver();
            }
        }

        if (act == ACT.IDLE)
        {

            if (pm.Players[pm.CurTurnIdx] == this)
                MapManager.GetInst().SetHexColor(CurHex, Color.grey);
        }
        if (act == ACT.MOVING)
        {
            anim.SetBool("Run", true);
            Hex nextHex = MoveHexes[0];

            float distance = Vector3.Distance(transform.position, nextHex.transform.position);
            if (distance > 0.1f)
            {
                transform.position += (nextHex.transform.position - transform.position).normalized * Time.smoothDeltaTime * status.MoveSpeed;
                transform.rotation = Quaternion.LookRotation((nextHex.transform.position - transform.position).normalized);
            }
            else // 다음 헥사에 도착
            {
                transform.position = nextHex.transform.position;
                MoveHexes.RemoveAt(0);
                if (MoveHexes.Count == 0)   // 최종도착
                {
                    act = ACT.IDLE;
                    CurHex = nextHex;
                    anim.SetBool("Run", false);
                    PlayerManager.GetInst().TurnOver();
                }

            }
        }
    }
}
