using FreeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class CPlace
    {
        List<CGameUser> placeUsers = new List<CGameUser>();

        public CPlace() {  }

        public void AddPlayerList(CPlayer player)
        {
            lock (this.placeUsers)
            {
                this.placeUsers.Add(player.GAME_USER);
            }
        }

        public void RemovePlayerList(CPlayer player)
        {
            lock (this.placeUsers)
            {
                this.placeUsers.Remove(player.GAME_USER);
                if (this is CGameRoom && placeUsers.Count == 0)
                {
                    (this as CGameRoom).isGameRunning = false;
                }
            }
        }

        public CPacket GetSpawnedPlayers()
        {
            CPacket ack = CPacket.create((short)PROTOCOL.SPAWN_PLAYER_ACK);
            ack.push_int16((short)placeUsers.Count);    // 현재 게임룸의 플레이어 수

            foreach (var player in placeUsers)
            {
                ack.push(player.PLAYER.USER_ID);
                //ack.push((byte)player.PLAYER.spawnIdx);
            }

            return ack;
        }

        public CPacket GetTransformPlayers(CPacket msg)
        {
            CPacket ack = CPacket.create((short)PROTOCOL.TRANSFORM_PLAYER_ACK);

            string uid = msg.pop_string();
            float moveX = msg.pop_Float();
            float moveY = msg.pop_Float();
            float moveZ = msg.pop_Float();
            float posX = msg.pop_Float();
            float posY = msg.pop_Float();
            float posZ = msg.pop_Float();
            float eulerX = msg.pop_Float();
            float eulerY = msg.pop_Float();
            float eulerZ = msg.pop_Float();
            float forward = msg.pop_Float();
            float strafe = msg.pop_Float();

            //SDebug.WriteLog($"uid:{uid}, mx:{moveX}, my:{moveY}, mz:{moveZ}, "
            //    + $"posX:{posX}, posY:{posY}, posZ:{posZ}, "
            //    + $"eulerX:{eulerX}, eulerY:{eulerY}, eulerZ:{eulerZ}, " +
            //      $"forward:{forward}, strafe:{strafe}");

            ack.push(uid);
            ack.push(moveX);
            ack.push(moveY);
            ack.push(moveZ);
            ack.push(posX);
            ack.push(posY);
            ack.push(posZ);
            ack.push(eulerX);
            ack.push(eulerY);
            ack.push(eulerZ);
            ack.push(forward);
            ack.push(strafe);

            return ack;
        }

        public CPacket GetFire(CPacket msg)
        {
            CPacket ack = CPacket.create((short)PROTOCOL.SHOT_PLAYER_ACK);

            ack.push(msg.pop_string());
            short shot = msg.pop_int16();
            ack.push_int16(shot);
            if (shot == 1)
            {
                ack.push(msg.pop_Float());
                ack.push(msg.pop_Float());
                ack.push(msg.pop_Float());
                ack.push(msg.pop_Float());
                ack.push(msg.pop_Float());
                ack.push(msg.pop_Float());
            }


            return ack;
        }

        public CPacket GetLeaveRoom(CPacket msg)
        {
            string uid = msg.pop_string();

            RemovePlayerList(((CGameUser)msg.owner).PLAYER);
            ((CGameUser)msg.owner).PLAYER.GAME_ROOM = null;
            CPacket ack = CPacket.create((short)PROTOCOL.LEAVE_GAME_ROOM_ACK);

            ack.push(uid);

            return ack;
        }

        // 게임룸에 진입한 모든 Player들한테 전송
        public void BroadcastPacket(CPacket msg)
        {
            /*
            msg를 send하면 첫번째 전송되고 메모리에서 delete되므로
            두번째부터는 전송할 수 없다.
            그래서 msg를 복제하여 전송해야 한다.
            */
            lock (this.placeUsers)
            {
                foreach (var player in placeUsers)
                {
                    CPacket clone = new CPacket();
                    msg.copy_to(clone);     // 패킷 복제
                    player.send(clone);
                }
            }
        }

        public int RoomUserCount()
        {
            return placeUsers.Count;
        }

        public CPacket GetIceNum(CPacket msg)
        {
            CPacket ack = CPacket.create((short)PROTOCOL.ICE_BROKE_ACK);

            ack.push(msg.pop_int32());
            ack.push(msg.pop_int32());
            ack.push(msg.pop_int32());

            return ack;
        }
    }
}
