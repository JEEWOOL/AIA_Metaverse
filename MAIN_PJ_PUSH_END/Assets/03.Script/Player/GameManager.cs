using JEEWOO.NET;
using FreeNet;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> playerList = new List<GameObject>();
    public GameObject cameraPref;
    public Camera maincam; // 지뢰찾기용 카메라할당

    public Text actionText;
    public GameObject inventory;
    private static GameManager _instance = null;
    Transform startPoint;

    public static GameObject slotMachineUI;   // 슬롯머신 UI
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        startPoint = GameObject.Find("StartPos").GetComponent<Transform>();
        //GameObject.Instantiate(player, startPoint.position, startPoint.rotation);

        CreateLocalPlayer();
        SendReqSpawnedPlayer();

        slotMachineUI = GameObject.Find("Canvas - SlotMachine");
        if (slotMachineUI != null)
        {
            slotMachineUI.SetActive(false);
        }
    }


    void CreateLocalPlayer()
    {
        GameObject localPlayer = GameObject.Instantiate(player, startPoint.position, startPoint.rotation);

        string user_id = CProcessPacket.Instance.USER_ID;
        localPlayer.AddComponent<CPlayerInfo>().USER_ID = user_id;
        localPlayer.GetComponent<CPlayerInfo>().IS_MINE = true;
        GameObject cameraObj = Instantiate(cameraPref, localPlayer.transform);
        maincam = cameraObj.GetComponent<Camera>(); // 지뢰찾기 카메라할당
        //cameraObj.AddComponent<FollowCam>().target = localPlayer.transform;
        if (localPlayer.GetComponent<SP_Move>() != null)
            localPlayer.GetComponent<SP_Move>().theCamera = cameraObj;
        ActionController ac = cameraObj.GetComponent<ActionController>();
        if(ac != null)
        {
            ac.actionText = this.actionText;
            ac.theInventory = this.inventory.GetComponent<Inventory>();
        }

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
        //if(count >= 2) {
        //    CPacket pack = CPacket.create((short)PROTOCOL.GAME_START_REQ);

        //    CNetworkManager.Instance.send();
        //}
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