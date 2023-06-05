using JEEWOO.NET;
using FreeNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CProcessPacket : MonoBehaviour
{
    private static CProcessPacket _instance = null;

    public static CProcessPacket Instance
    {
        get
        {
            return _instance;
        }
    }
    public string USER_ID { get; set; }
    public short GAMEROOM_ID { get; set; }
    private void Awake()
    {
        _instance = this;
    }
    public delegate void DispatchTransform(string uid, Vector3 moveDir, Vector3 pos, Vector3 euler, int aniState);
    public DispatchTransform OnDispatchTransform = null;

    public delegate void DispatchFire(string uid, short shot, Vector3 pos, Vector3 dir);
    public DispatchFire OnDispatchFire = null;

    public void on_process_packet(CPacket msg)
    {
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
        switch (protocol_id)
        {
            case PROTOCOL.REG_MEMBER_ACK:
                {
                    byte result = msg.pop_byte();
                    if (result == (byte)1)
                    {
                        NLoginManager.Instance.notify.text = "아이디 생성이 완료되었습니다.";
                    }
                    else if (result == (byte)0)
                    {
                        NLoginManager.Instance.notify.text = "이미 존재하는 아이디입니다.";
                    }
                }
                break;
            case PROTOCOL.LOGIN_ACK:
                {
                    string id = msg.pop_string();
                    byte result = msg.pop_byte();
                    if (result == (byte)1)
                    {
                        NLoginManager.Instance.notify.text = $"{id} Login Success";
                        USER_ID = id;
                        Loading.LoadScene("Main_Protype");
                    }
                    else if (result == (byte)0)
                    {
                        NLoginManager.Instance.notify.text = $"{id} / 비밀번호가 일치하지 않습니다.";
                    }
                }
                break;
            case PROTOCOL.JOIN_ROOM_ACK:
                {
                    string user_id = msg.pop_string();
                    short gameroom_id = msg.pop_int16();
                    this.GAMEROOM_ID = gameroom_id;
                    ChangeBattleFieldScene(gameroom_id);
                }
                break;
            case PROTOCOL.SPAWN_PLAYER_ACK:
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.CreateFriendPlayer(msg);
                        return;
                    }
                    if (RoomGameManager.Instance != null)
                    {
                        RoomGameManager.Instance.CreateFriendPlayer(msg);
                        return;
                    }
                    if (ICE_Gamemanager.Instance != null)
                    {
                        ICE_Gamemanager.Instance.CreateFriendPlayer(msg);
                        return;
                    }
                    if (SkatingGameManager.Instance != null)
                    {
                        SkatingGameManager.Instance.CreateFriendPlayer(msg);
                    }
                    //StartCoroutine(coSendTransform());
                }
                break;
            case PROTOCOL.TRANSFORM_PLAYER_ACK:
                {
                    if (OnDispatchTransform != null)
                    {
                        string uid = msg.pop_string();
                        float move_x = msg.pop_Float();
                        float move_y = msg.pop_Float();
                        float move_z = msg.pop_Float();
                        Vector3 moveDir = new Vector3(move_x, move_y, move_z);
                        float pos_x = msg.pop_Float();
                        float pos_y = msg.pop_Float();
                        float pos_z = msg.pop_Float();
                        Vector3 position = new Vector3(pos_x, pos_y, pos_z);
                        float euler_x = msg.pop_Float();
                        float euler_y = msg.pop_Float();
                        float euler_z = msg.pop_Float();
                        Vector3 euler = new Vector3(euler_x, euler_y, euler_z);
                        int aniState = msg.pop_int32();

                        OnDispatchTransform(uid, moveDir, position, euler, aniState);
                    }

                }
                break;
            case PROTOCOL.SHOT_PLAYER_ACK:
                {
                    if (OnDispatchFire != null)
                    {
                        string uid = msg.pop_string();
                        short fire = msg.pop_int16();
                        Vector3 pos = new Vector3();
                        Vector3 dir = new Vector3();
                        if (fire == 1)
                        {
                            pos.x = msg.pop_Float();
                            pos.y = msg.pop_Float();
                            pos.z = msg.pop_Float();
                            dir.x = msg.pop_Float();
                            dir.y = msg.pop_Float();
                            dir.z = msg.pop_Float();
                        }
                        OnDispatchFire(uid, fire, pos, dir);
                    }
                }
                break;
            case PROTOCOL.ICE_BROKE_ACK:
                {
                    if (ICE_Gamemanager.Instance != null)
                    {
                        ICE_Gamemanager.Instance.IceBreakAck(msg.pop_int32(), msg.pop_int32(), msg.pop_int32());
                    }
                }
                break;
            case PROTOCOL.LEAVE_GAME_ROOM_ACK:
                {
                    Loading.LoadScene("Main_Protype");
                }
                break;
            case PROTOCOL.GAME_END_ACK:
                {
                    if (SkatingGameManager.Instance != null)
                    {
                        string id = msg.pop_string();
                        SkatingGameManager.Instance.GameOverRecv(id);
                    }
                    if (ICE_Gamemanager.Instance != null)
                    {
                        string id = msg.pop_string();
                        ICE_Gamemanager.Instance.GameOverUIEnable(id);
                    }
                    if (RoomGameManager.Instance != null)
                    {
                        string id = msg.pop_string();
                        RoomGameManager.Instance.GameEnd(id);
                    }
                }
                break;
            case PROTOCOL.MYROOM_SAVE_ACK:
                {
                    short isSuc = msg.pop_int16();
                }
                break;
            case PROTOCOL.MYROOM_LOAD_ACK:
                {
                    string data = msg.pop_string();
                    BuildMenu.Instance.LoadObjects(data);
                }
                break;
            case PROTOCOL.GET_MONEY_ACK:
                {
                    SetMoney(msg);
                }
                break;
            case PROTOCOL.UPDATE_MONEY_ACK:
                {
                    
                }
                break;
        }
    }

    private static void SetMoney(CPacket msg)
    {
        int money = msg.pop_int32();
        PlayListManager.gold = money;
    }

    private void ChangeBattleFieldScene(short roomID)
    {
        Loading.LoadScene(Enum.GetName(typeof(RoomID), roomID));
    }

    void ChangeLobbyScene()
    {
        Loading.LoadScene("Lobby");
    }

}
