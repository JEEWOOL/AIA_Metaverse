using FreeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class CGameUser : IPeer
    {
        CProcessPacket procPacket;
        CUserToken token;
        public CPlayer PLAYER { get; set; } // 게임룸내에서의 클라이언트

        public delegate void Remove_user(CGameUser user);
        public Remove_user remove_user_callback { get; set; }

        public CGameUser(CProcessPacket procPacket, CUserToken token)
        {
            this.procPacket = procPacket;
            this.token = token;
            this.token.set_peer(this);
        }

        public void disconnect()
        {
            this.token.socket.Disconnect(false);
        }

        public void on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);

            process_user_operation(msg);
        }

        public void on_removed()
        {
            if (remove_user_callback != null)
                remove_user_callback(this);
            if(PLAYER?.GAME_ROOM != null)
                PLAYER.GAME_ROOM.RemovePlayerList(PLAYER);
        }

        public void process_user_operation(CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();

            switch (protocol)
            {
                case PROTOCOL.REG_MEMBER_REQ:
                    procPacket.Process_REG_MEMBER_REQ(msg);
                    break;
                case PROTOCOL.LOGIN_REQ:
                    procPacket.Process_LOGIN_REQ(msg);
                    break;
                case PROTOCOL.JOIN_ROOM_REQ:
                    procPacket.Process_JOIN_ROOM_REQ(msg);
                    break;
                case PROTOCOL.LEAVE_GAME_ROOM_REQ:
                    procPacket.Process_LEAVE_GAME_ROOM_REQ(msg);
                    break;
                case PROTOCOL.SPAWN_PLAYER_REQ:
                    procPacket.Process_SPAWN_PLAYER_REQ(msg);
                    break;
                case PROTOCOL.TRANSFORM_PLAYER_REQ:
                    procPacket.Process_TRANSFORM_PLAYER_REQ(msg);
                    break;
                case PROTOCOL.SHOT_PLAYER_REQ:
                    procPacket.Process_SHOT_PLAYER_REQ(msg);
                    break;
                case PROTOCOL.ICE_BROKE_REQ:
                    procPacket.Process_ICE_BROKE_REQ(msg);
                    break;
                case PROTOCOL.GAME_END_REQ:
                    procPacket.Process_GAME_END_REQ(msg);
                    break;
                case PROTOCOL.MYROOM_SAVE_REQ:
                    procPacket.Process_MYROOM_SAVE_REQ(msg);
                    break;
                case PROTOCOL.MYROOM_LOAD_REQ:
                    procPacket.Process_MYROOM_LOAD_REQ(msg);
                    break;
                case PROTOCOL.GET_MONEY_REQ:
                    procPacket.Process_GET_MONEY_REQ(msg);
                    break;
                case PROTOCOL.UPDATE_MONEY_REQ:
                    procPacket.Process_UPDATE_MONEY_REQ(msg);
                    break;
            }
        }

        public void send(CPacket msg)
        {
            this.token.send(msg);
        }
    }
}
