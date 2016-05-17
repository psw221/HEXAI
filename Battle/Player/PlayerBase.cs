using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ACT
{
    IDLE,
    MOVEHIGHLIGHT,
    MOVING,
    ATTACKHIGHLIGHT,
    ATTACKING,
    DIYING
}

public class PlayerBase : MonoBehaviour {
    public Animator anim;
    public PlayerStatus status;
    public Hex CurHex;
    public ACT act;
    public List<Hex> MoveHexes;

    public float removeTime = 0f;

    public float damagedTime = 0f;

    void Awake()
    {
        
    }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (act == ACT.MOVING)
        {
            Hex nextHex = MoveHexes[0];

            float distance = Vector3.Distance(transform.position, nextHex.transform.position);
            if (distance > 0.1f)
            {
                transform.position += (nextHex.transform.position - transform.position).normalized * Time.smoothDeltaTime * status.MoveSpeed;
            }
            else // 다음 헥사에 도착
            {
                transform.position = nextHex.transform.position;
                MoveHexes.RemoveAt(0);
                if (MoveHexes.Count == 0)   // 최종도착
                {
                    act = ACT.IDLE;
                    CurHex = nextHex;
                    PlayerManager.GetInst().TurnOver();
                }
               
            }
        }
         * */
	}

    public void GetDamage(int damage)
    {
        status.CurHP -= damage;
        if (status.CurHP <= 0)
        {
            Debug.Log("Died");
            anim.SetTrigger("Die");
            act = ACT.DIYING;
            removeTime += Time.smoothDeltaTime;
            //PlayerManager.GetInst().RemovePlayer(this);
        }
        else
        {
            Debug.Log("Hited");
            anim.SetTrigger("Hited");
        }
    }
}
