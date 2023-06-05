using FreeNet;
using FreeNetUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNetworkManager : MonoBehaviour
{
    private static CNetworkManager _instance = null;

    public static CNetworkManager Instance { get {
            return _instance;
        } set {
            _instance = value;
        } 
    }

    public CFreeNetUnityService gameServer;
    CProcessPacket processPacket;

    [SerializeField] 
    string IP = "127.0.0.1";
    [SerializeField]
    int PORT = 7979;

    private void Awake()
    {
        _instance = this;

        gameServer = this.gameObject.AddComponent<CFreeNetUnityService>();
        processPacket = this.gameObject.AddComponent<CProcessPacket>();
        gameServer.appcallback_on_status_changed += on_status_changed;
        gameServer.appcallback_on_message += processPacket.on_process_packet;
    }

    void on_status_changed(NETWORK_EVENT status)
    {
        switch (status)
        {
            case NETWORK_EVENT.connected:
                Debug.Log("On Connected");
                break;
            case NETWORK_EVENT.disconnected:
                Debug.Log("On Disconnected");
                break;
        }
    }

    void Start()
    {
        while(gameServer.service == null)
        {
            gameServer.connect(IP, PORT);
        }
    }
    public void send(CPacket msg)
    {
        gameServer.send(msg);
    }
}
