using JEEWOO.NET;
using FreeNet;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkatingGameManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> playerList = new List<GameObject>();
    public GameObject cameraPref;
    public GameObject gameOverPanel;
    string my_id;
    private static SkatingGameManager _instance = null;
    Transform startPoint;
    public static SkatingGameManager Instance
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
    }


    void CreateLocalPlayer()
    {
        GameObject localPlayer = GameObject.Instantiate(player, startPoint.position, startPoint.rotation);

        string user_id = CProcessPacket.Instance.USER_ID;
        localPlayer.AddComponent<CPlayerInfo>().USER_ID = user_id;
        localPlayer.GetComponent<CPlayerInfo>().IS_MINE = true;
        GameObject cameraObj = Instantiate(cameraPref, localPlayer.transform);
        //cameraObj.AddComponent<FollowCam>().target = localPlayer.transform;
        if (localPlayer.GetComponent<SP_Move>() != null)
            localPlayer.GetComponent<SP_Move>().theCamera = cameraObj;

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

    public void GameOverRecv(string uid)
    {
        gameOverPanel.SetActive(true);
        CGameOverPannel pannel = gameOverPanel.GetComponent<CGameOverPannel>();
        if (CProcessPacket.Instance.USER_ID == uid)
        {
            pannel.gameOverText.text = "GAME WIN!!";
        }
        else
        {
            pannel.gameOverText.text = "GAME LOSE :(";
        }
        pannel.ID_TEXT.text += uid;

        Cursor.visible = true;
    }
}