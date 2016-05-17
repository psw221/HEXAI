using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    private static GameManager inst = null;
    MapManager mm;
    PlayerManager pm;
    Camera camera;
    GUIManager gm;
    BattleManager bm;
    SoundManager sm;
    EventManager em;

    public static GameManager GetInst()
    {
        return inst;
    }

    void Awake()
    {
        inst = this;
        mm = MapManager.GetInst();
        pm = PlayerManager.GetInst();
        gm = GUIManager.GetInst();
        bm = BattleManager.GetInst();
        sm = SoundManager.GetInst();
        em = EventManager.GetInst();
    }
	// Use this for initialization
	void Start () {
        mm.CreateMap();
        pm.GenPlayerTest();
        sm.PlayMusic(transform.position);

        camera = GetComponent<Camera>();
	}

    GameObject StageString;
	
	// Update is called once per frame
	void Update () {
        if (em.GameEnd)
        {
            ShowGameOver();
            return;
        }
        if (em.StageStarted)
        {
            CheckMouseZoom();
            CheckMouseButtonDown();
            bm.CheckBattle();
            pm.CheckTurnOver();
        }
        else if (!em.StageStartEvent)
        {
            StageString = GameObject.FindGameObjectWithTag("StageString");
            em.ShowStartEvent();
        }
	}

    void OnGUI()
    {
        if (em.StageStarted)
        {
            gm.UpdateTurnInfoPos(transform.position.x, transform.position.z);
            gm.DrawGUI();
        }
    }

    void CheckMouseZoom()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel");
        float mouseY = camera.transform.position.y + mouse * 5;

        if (mouseY < 5) mouseY = 5;
        else if (mouseY > 10) mouseY = 10; 

        Vector3 newPos = new Vector3(camera.transform.position.x, mouseY, camera.transform.position.z);
        camera.transform.position = newPos;
        
    }

    public void CheckMouseButtonDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Mouse 1 btn Down");
            pm.MouseInputPorc(1);
        }
    }

    public void MoveCamPosToHex(Hex hex)
    {
        float destX = hex.transform.position.x;
        float destZ = hex.transform.position.z;

      //  camera.transform.position = new Vector3(destX, camera.transform.position.y, destZ - 5);
        
    }

    public Hex damagedHex;
    public int damage;

    IEnumerator ShowDamage()
    {
        GameObject GO_Damage = (GameObject)Resources.Load("Prefabs/Effect/Damage");

        GameObject obj = (GameObject)GameObject.Instantiate(GO_Damage, damagedHex.transform.position, GameManager.GetInst().transform.rotation);
        TextMesh tm = obj.GetComponent<TextMesh>();
        tm.text = damage.ToString();
        tm.color = Color.red;

        yield return new WaitForSeconds(0.05f);

        for (float i = 1; i >= 0; i -= 0.01f)
        {
            tm.color = new Vector4(255, 0, 0, i);
            yield return new WaitForFixedUpdate();
        }

        GameObject.Destroy(obj);
    }

    IEnumerator ShowStageString()
    {
       
        yield return new WaitForSeconds(3f);

        em.StageStarted = true;
        StageString.SetActive(false);
    }

    void ShowGameOver()
    {

    }
}
