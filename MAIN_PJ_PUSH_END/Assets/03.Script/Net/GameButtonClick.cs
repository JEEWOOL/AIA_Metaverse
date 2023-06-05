using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GameButtonClick : MonoBehaviour
{
    public void ExitToMainClick()
    {
        CPacket pack = CPacket.create((short)PROTOCOL.LEAVE_GAME_ROOM_REQ);
        pack.push(CProcessPacket.Instance.USER_ID);
        CNetworkManager.Instance.send(pack);
    }
}
