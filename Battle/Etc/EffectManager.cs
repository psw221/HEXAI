using UnityEngine;
using System.Collections;

public class EffectManager  {
    private static EffectManager inst = null;
    public GameObject GO_AttackEffect;
    public GameObject GO_Damage;

    public static EffectManager GetInst()
    {
        if (inst == null)
        {
            inst = new EffectManager();
            inst.Init();
        }
        return inst;
    }

    public void Init()
    {
        GO_AttackEffect = (GameObject)Resources.Load("Effect/Lightning Spark");
        GO_Damage = (GameObject)Resources.Load("Prefabs/Effect/Damage");
    }



    // todo : 이펙트를 화면에 보여주는데 , 차후 버젼에서는 이펙트 종류나 타입등을 설정할 수 있도록 바꿔야 한다.
    public void ShowEffect(GameObject hex)
    {
        GameObject go = (GameObject)GameObject.Instantiate(GO_AttackEffect, hex.transform.position, hex.transform.rotation);
    }

    public void ShowDamage(Hex hex, int damage)
    {
        GameManager.GetInst().damagedHex = hex;
        GameManager.GetInst().damage = damage;

        GameManager.GetInst().StartCoroutine("ShowDamage");
    }
}
