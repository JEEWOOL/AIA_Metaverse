using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomManager : MonoBehaviour
{
    private static MyRoomManager _instance = null;
    public static MyRoomManager Instance
    {
        get
        {
            return _instance;
        }
    }


    public GameObject player;
    public List<GameObject> playerList = new List<GameObject>();
    public GameObject cameraPref;
    public GameObject cameraPosPref;
    Transform startPoint;

    void Start()
    {
        _instance = this;
        CPacket packet = CPacket.create((short)PROTOCOL.MYROOM_LOAD_REQ);
        packet.push(CProcessPacket.Instance.USER_ID);
        CNetworkManager.Instance.send(packet);
        startPoint = GameObject.Find("StartPos").GetComponent<Transform>();

        CreateLocalPlayer();
        SendReqSpawnedPlayer();
    }


    void CreateLocalPlayer()
    {
        GameObject localPlayer = GameObject.Instantiate(player, startPoint.position, startPoint.rotation);

        string user_id = CProcessPacket.Instance.USER_ID;
        localPlayer.AddComponent<CPlayerInfo>().USER_ID = user_id;
        localPlayer.GetComponent<CPlayerInfo>().IS_MINE = true;
        GameObject cameraObj = Instantiate(cameraPref, localPlayer.transform);
        localPlayer.GetComponent<SP_Move>().theCamera = cameraObj;
        BuildMenu.Instance.player = cameraObj.transform;
        ActionController ac = cameraObj.GetComponent<ActionController>();
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
                Vector3 pos = startPoint.position;
                Quaternion rot = startPoint.rotation;
                GameObject playerObj = Instantiate(player, pos, rot);
                playerObj.AddComponent<CPlayerInfo>().USER_ID = user_id;
                playerObj.GetComponent<CPlayerInfo>().IS_MINE = false;

                // Player List에 등록
                playerList.Add(playerObj);
            }
        }
    }
}
