using JEEWOO.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;

public class ArtManager : MonoBehaviour
{
    //bool isart = false;

    //public GameObject menualText;
    //public GameObject gallery_Menual_Panel;

    private void Start()
    {
        //gallery_Menual_Panel.SetActive(false);
        //menualText.SetActive(true);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    isart = !isart;

        //    if (isart)
        //    {
        //        gallery_Menual_Panel.SetActive(true);
        //        menualText.SetActive(false);
        //    }
        //    else
        //    {
        //        gallery_Menual_Panel.SetActive(false);
        //        menualText.SetActive(true);
        //    }
        //}
    }

    public void ArtExit()
    {
        CPacket pack = CPacket.create((short)PROTOCOL.LEAVE_GAME_ROOM_REQ);
        pack.push(CProcessPacket.Instance.USER_ID);
        CNetworkManager.Instance.send(pack);
    }
}
