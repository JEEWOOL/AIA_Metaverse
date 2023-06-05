using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingInfo : MonoBehaviour
{
    public float currHp = 90;
    public float maxhp = 90;
    public float HpRate()
    {
        return currHp / maxhp;
    }

    public void PlayerDie()
    {
        if(GetComponent<CPlayerInfo>().IS_MINE)
        {
            RoomGameManager.Instance.gameEndPannel.SetActive(true);
        }
        RoomGameManager.Instance.playerList.Remove(this.gameObject);
        if (RoomGameManager.Instance.playerList.Count == 1)
        {
            CPacket pack = CPacket.create((short)PROTOCOL.GAME_END_REQ);
            pack.push(RoomGameManager.Instance.playerList[0].GetComponent<CPlayerInfo>().USER_ID);
            CNetworkManager.Instance.send(pack);
        }
    }
}
