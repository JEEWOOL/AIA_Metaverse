using JEEWOO.NET;
using FreeNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NLoginManager : MonoBehaviour
{

    public InputField id;
    public InputField password;
    public Text notify;

    private static NLoginManager _instance = null;
    public static NLoginManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }


    private void Start()
    {
        notify.text = "";
        id.Select();
    }

    private void Update()
    {
        if (id.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                password.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckUserData();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CheckUserData();
        }
    }

    public void SaveUserData()
    {
        if (!CheckInput(id.text, password.text))
        {
            return;
        }
        SendRegReq();
    }

    private void SendRegReq()
    {
        CPacket msg = CPacket.create((short)PROTOCOL.REG_MEMBER_REQ);
        msg.push(id.text);
        msg.push(password.text);

        CNetworkManager.Instance.send(msg);
    }

    public void CheckUserData()
    {
        if (!CheckInput(id.text, password.text))
        {
            return;
        }

        SendLoginReq();
    }

    private void SendLoginReq()
    {
        CPacket msg = CPacket.create((short)PROTOCOL.LOGIN_REQ);
        msg.push(id.text);
        msg.push(password.text);

        CNetworkManager.Instance.send(msg);
    }

    bool CheckInput(string id, string pwd)
    {
        if (id == "" || pwd == "")
        {
            notify.text = "아이디 / 비밀번호를 확인해주세요.";
            return false;
        }
        else
        {
            return true;
        }
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
