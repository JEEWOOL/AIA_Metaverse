using FreeNet;
using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayListManager : MonoBehaviour
{
    static public int gold = 5000;
    public Text goldText;
    public Slot slot;

    public void Start()
    {
        SendGetMoneyReq();
    }

    private static void SendGetMoneyReq()
    {
        CPacket pack = CPacket.create((short)PROTOCOL.GET_MONEY_REQ);
        pack.push(CProcessPacket.Instance.USER_ID);
        CNetworkManager.Instance.send(pack);
    }

    public void ItemSale()
    {
        gold += slot.itemCount * 1000;
        goldText.text = gold.ToString();
        slot.ClearSlot();
    }

    public void IceBreak()
    {
        Time.timeScale = 1;
        //Loading.LoadScene("FallGuys_Proto Type");
        SendJoinRoomReq(RoomID.FallGuys_ProtoType);

    }

    public void MiniFighter()
    {
        Time.timeScale = 1;
        //Loading.LoadScene("Shooting_ProtoType");
        SendJoinRoomReq(RoomID.Shooting_ProtoType);
    }

    public void IceSkating()
    {
        Time.timeScale = 1;
        //Loading.LoadScene("Skating_ProtoType");
        SendJoinRoomReq(RoomID.Skating_ProtoType);
    }

    public void Casino()
    {
        Time.timeScale = 1;
        //Loading.LoadScene("Casino_ProtoType");
        SendJoinRoomReq(RoomID.Casino_ProtoType);
    }

    public void Art_Gallery()
    {
        Time.timeScale = 1;
        //Loading.LoadScene("Shooting_ProtoType");
        SendJoinRoomReq(RoomID.Art_gallery_ProtoType);
    }

    public void MyRoom()
    {
        Time.timeScale = 1;
        //Loading.LoadScene("MyRoom_ProtoType");
        SendJoinRoomReq(RoomID.MyRoom_ProtoType);
    }


    private static void SendJoinRoomReq(RoomID roomID)
    {
        CPacket packet = CPacket.create((short)PROTOCOL.JOIN_ROOM_REQ);
        packet.push(CProcessPacket.Instance.USER_ID);
        packet.push((short)roomID);

        CNetworkManager.Instance.send(packet);
    }
}
