using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDie : MonoBehaviour
{
    Transform tr;
    GameObject gameoverPanel;

    bool isDie = false;

    private void Awake()
    {
        gameoverPanel = GameObject.Find("Canvas/Panel-Gameover");
        gameoverPanel.SetActive(false);
    }

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    
    void Update()
    {
        if(tr.position.y < -20 && !isDie)
        {
            isDie = true;
            GameOverUIEnable();
        }
    }
    private void GameOverUIEnable()
    {
        gameoverPanel.SetActive(true);
        Cursor.visible = true;
        ICE_Gamemanager.Instance.playerList.Remove(this.gameObject);
        if(ICE_Gamemanager.Instance.playerList.Count == 1)
        {
            CPacket pack = CPacket.create((short)PROTOCOL.GAME_END_REQ);
            pack.push(ICE_Gamemanager.Instance.playerList[0].GetComponent<CPlayerInfo>().USER_ID);
            CNetworkManager.Instance.send(pack);
        }
        //gameObject.SetActive(false);
        //Cursor.lockState = CursorLockMode.Confined;
    }
}
