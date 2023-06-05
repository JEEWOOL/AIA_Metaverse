using DB;
using FreeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class CProcessPacket
    {
        public CPlaceManager GAMEROOM_MGR
        {
            get
            {
                return CPlaceManager.instance;
            }
        }
        RoomDao roomDao = new RoomDao();
        GameRoomDao gameDao = new GameRoomDao();
        UserInfoDao memDao = new UserInfoDao();
        CharacterDao charDao = new CharacterDao();

        public void Process_REG_MEMBER_REQ(CPacket msg)
        {
            // DB에 적용
            UserInfoVo vo = new UserInfoVo();
            vo.USER_ID = msg.pop_string();
            vo.USER_PASS = msg.pop_string();
            int row = memDao.InsertUser(vo);

            // Unity Client에 응답
            CPacket ack = CPacket.create((short)PROTOCOL.REG_MEMBER_ACK);
            if (row == 1)
                ack.push((byte)1);  // success
            else
                ack.push((byte)0);  // fail

            msg.owner.send(ack);
        }

        public void Process_LOGIN_REQ(CPacket msg)
        {
            string id = msg.pop_string();
            string pass = msg.pop_string();
            UserInfoVo vo = memDao.GetUserData(id, pass);

            // Unity Client에 결과를 응답
            CPacket ack = CPacket.create((short)PROTOCOL.LOGIN_ACK);
            ack.push(id);
            if (id.Equals(vo.USER_ID))
            {
                CPlayer player = new CPlayer(GAMEROOM_MGR.roomList[0], (CGameUser)msg.owner, id);
                ((CGameUser)msg.owner).PLAYER = player;
                GAMEROOM_MGR.roomList[0].AddPlayerList(player);
                ack.push((byte)1);
            }
            else
                ack.push((byte)0);  // fail        
            msg.owner.send(ack);
        }
        public void Process_JOIN_ROOM_REQ(CPacket msg)
        {
            string user_id = msg.pop_string();
            short gameroom_id = msg.pop_int16();
            //string gameroom_name = msg.pop_string();

            //int row = charDao.UpdateUserJoin(user_id, gameroom_id);


            GAMEROOM_MGR.FindRoom(gameroom_id, (msg.owner as CGameUser).PLAYER);

            //CPacket start = CPacket.create((short)PROTOCOL.GAME_START_ACK);


            //if((msg.owner as CGameUser).PLAYER.GAME_ROOM.RoomUserCount()>=2)
            //    (msg.owner as CGameUser).PLAYER.GAME_ROOM.BroadcastPacket(msg);

            CPacket ack = CPacket.create((short)PROTOCOL.JOIN_ROOM_ACK);
            ack.push(user_id);
            ack.push(gameroom_id);

            msg.owner.send(ack);
        }

        public void Process_SPAWN_PLAYER_REQ(CPacket msg)
        {
            string user_id = msg.pop_string();
            //byte idx = msg.pop_byte();

            // 현재 패킷 -> CGameUser -> CPlayer 로 접근
            CPlayer player = ((CGameUser)msg.owner).PLAYER;
            //player.spawnIdx = idx;

            // CPlayer가 소속된 CGameRoom
            CPlace gameRoom = player.GAME_ROOM;
            CPacket ack = gameRoom.GetSpawnedPlayers();

            // GameRoom에 소속된 모든 CPlayer한테 전송
            gameRoom.BroadcastPacket(ack);
        }

        public void Process_TRANSFORM_PLAYER_REQ(CPacket msg)
        {
            CPlace gameRoom = ((CGameUser)msg.owner).PLAYER.GAME_ROOM;
            CPacket ack = gameRoom.GetTransformPlayers(msg);
            gameRoom.BroadcastPacket(ack);
        }

        public void Process_SHOT_PLAYER_REQ(CPacket msg)
        {
            CPlace gameRoom = ((CGameUser)msg.owner).PLAYER.GAME_ROOM;
            CPacket ack = gameRoom.GetFire(msg);
            gameRoom.BroadcastPacket(ack);
        }

        public void Process_ICE_BROKE_REQ(CPacket msg)
        {
            CPlace gameRoom = ((CGameUser)msg.owner).PLAYER.GAME_ROOM;
            CPacket ack = gameRoom.GetIceNum(msg);
            gameRoom.BroadcastPacket(ack);
        }

        public void Process_GAME_END_REQ(CPacket msg)
        {
            CPlace gameRoom = ((CGameUser)msg.owner).PLAYER.GAME_ROOM;
            CPacket ack = CPacket.create((short)PROTOCOL.GAME_END_ACK);
            ack.push(msg.pop_string());
            gameRoom.BroadcastPacket(ack);
        }

        public void Process_MYROOM_SAVE_REQ(CPacket msg)
        {
            string id = msg.pop_string();
            string roomData = msg.pop_string();
            int row = roomDao.UpdateRoom(id, roomData);
            CPacket ack = CPacket.create((short)PROTOCOL.MYROOM_SAVE_ACK);
            if(row == 1)    ack.push((short)1);
            else ack.push((short)0);
            msg.owner.send(ack);
        }
        public void Process_MYROOM_LOAD_REQ(CPacket msg)
        {
            string id = msg.pop_string();
            RoomVo vo = roomDao.GetRoomData(id);
            CPacket ack = CPacket.create((short)PROTOCOL.MYROOM_LOAD_ACK);
            if (vo.USER_ID != " ")
            {
                ack.push(vo.ROOM_DATA);
            }
            else
            {
                ack.push("{}");
            }
            msg.owner.send(ack);
        }

        public void Process_LEAVE_GAME_ROOM_REQ(CPacket msg)
        {
            CPlace gameRoom = ((CGameUser)msg.owner).PLAYER.GAME_ROOM;
            CPacket ack = gameRoom.GetLeaveRoom(msg);
            GAMEROOM_MGR.roomList[0].AddPlayerList(((CGameUser)msg.owner).PLAYER);
            ((CGameUser)msg.owner).PLAYER.GAME_ROOM = GAMEROOM_MGR.roomList[0];
            msg.owner.send(ack);
        }

        public void Process_GET_MONEY_REQ(CPacket msg)
        {
            CPacket ack = CPacket.create((short)PROTOCOL.GET_MONEY_ACK);
            int money =  memDao.LoadCoin(msg.pop_string());
            ack.push(money);
            msg.owner.send(ack);
        }

        public void Process_UPDATE_MONEY_REQ(CPacket msg)
        {
            string id = msg.pop_string();
            short pl = msg.pop_int16();
            int amount = msg.pop_int32();

            short result = 1;
            if(pl == 1)
            {
                memDao.AddCoin(id, amount);
            }
            else if(pl == 0) {
                memDao.RemoveCoin(id, amount);
            }
            else
                result = 0;
            CPacket ack = CPacket.create((short)PROTOCOL.UPDATE_MONEY_ACK);
            ack.push(result);
            msg.owner.send(ack);
        }
    }
}
