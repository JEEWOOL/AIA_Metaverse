using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ICE_Gamemanager : MonoBehaviour
{
    public GameObject playerPref;
    public List<GameObject> playerList = new List<GameObject>();
    public GameObject cameraPref;

    public GameObject icePrefab;
    public GameObject[,,] iceBlock = new GameObject[9, 3, 7];

    public TMP_Text gameEndText;
    public GameObject gameEndPannel;

    public bool isGameOn;

    private static ICE_Gamemanager _instance = null;
    Vector3 startPoint;
    public static ICE_Gamemanager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Start()
    {
        _instance = this;
        startPoint = new Vector3(8, 21, 6);
        //GameObject.Instantiate(player, startPoint.position, startPoint.rotation);
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        //Time.timeScale = 0;
        CreateLocalPlayer();
        SendReqSpawnedPlayer();
        CreateMap();

    }


    void CreateLocalPlayer()
    {
        //Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //int idx = Random.Range(1, points.Length);


        //GameObject localPlayer = GameObject.Instantiate(playerPref, points[idx].position, points[idx].rotation);
        GameObject localPlayer = GameObject.Instantiate(playerPref, startPoint, Quaternion.identity);

        string user_id = CProcessPacket.Instance.USER_ID;
        localPlayer.AddComponent<CPlayerInfo>().USER_ID = user_id;
        localPlayer.GetComponent<CPlayerInfo>().IS_MINE = true;
        GameObject cameraObj = Instantiate(cameraPref, localPlayer.transform);
        localPlayer.GetComponent<FP_Move>().theCamera = cameraObj;
        // Player List에 등록
        playerList.Add(localPlayer);
    }

    // 서버에 Local Player 생성정보를 전송
    void SendReqSpawnedPlayer()
    {
        CPacket msg = CPacket.create((short)PROTOCOL.SPAWN_PLAYER_REQ);
        string user_id = CProcessPacket.Instance.USER_ID;
        msg.push(user_id);

        CNetworkManager.Instance.send(msg);
    }

    // 다른 사람의 Local Player의 Remote Player를 내 공간에 생성
    public void CreateFriendPlayer(CPacket msg)
    {
        short count = msg.pop_int16();

        for (short i = 0; i < count; i++)
        {
            string user_id = msg.pop_string();
            bool isCreate = true;
            foreach (var player in playerList)
            {
                // 이미 동일 Player가 생성되어 존재한다.
                if (player.GetComponent<CPlayerInfo>().USER_ID == user_id)
                    isCreate = false;
            }
            // Player가 존재하지 않을 때만 생성
            if (isCreate)
            {
                Vector3 pos = startPoint;
                Quaternion rot = Quaternion.identity;
                GameObject playerObj = Instantiate(playerPref, pos, rot);
                playerObj.AddComponent<CPlayerInfo>().USER_ID = user_id;
                playerObj.GetComponent<CPlayerInfo>().IS_MINE = false;

                // Player List에 등록
                playerList.Add(playerObj);
            }
        }
    }
    void CreateMap()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 7; k++)
                {
                    GameObject map = Instantiate(icePrefab, new Vector3(i * 2, j * 10, k * 2), this.transform.rotation);
                    iceBlock[i, j, k] = map;

                    map.GetComponent<IceFloor>().x = i;
                    map.GetComponent<IceFloor>().y = j;
                    map.GetComponent<IceFloor>().z = k;
                }
            }
        }
    }
    public void IceBreakAck(int x, int y, int z)
    {
        iceBlock[x,y,z].GetComponent<IceFloor>().TriggerIncome();
    }
    public void GameOverUIEnable(string id)
    {
        gameEndText.text = id + " WIN!!";
        gameEndPannel.SetActive(true);
        Cursor.visible = true;
    }
}