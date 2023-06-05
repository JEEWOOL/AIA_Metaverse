using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoExit : MonoBehaviour
{
    public GameObject exitButton;
    private bool isOn = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOn = !isOn;
            exitButton.SetActive(isOn);

            Cursor.visible = isOn;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void ExitToMainClick()
    {
        CPacket pack = CPacket.create((short)PROTOCOL.LEAVE_GAME_ROOM_REQ);
        pack.push(CProcessPacket.Instance.USER_ID);
        CNetworkManager.Instance.send(pack);
    }
}
