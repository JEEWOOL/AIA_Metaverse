using DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class CPlaceManager
    {
        GameRoomDao roomDao = new GameRoomDao();

        public List<CPlace> roomList = new List<CPlace>();

        public static CPlaceManager instance = new CPlaceManager();
        public CPlaceManager()
        {
            // 시작하자마자 DB에 있는 게임룸 정보를 가져와 CGameRoom객체를 생성해
            // 리스트에 저장한다.

            roomList.Add(new CPlace());

            //IList<RoomVo> voList = roomDao.GetRoomList();

            //foreach (var vo in voList)
            //{
            //    CGameRoom gameRoom = new CGameRoom(vo.ROOM_ID, vo.ROOM_NAME);
            //    roomList.Add(gameRoom);
            //}
        }

        //public void AddRoomList(CGameRoom gameRoom)
        //{
        //    lock (roomList)
        //    {
        //        roomList.Add(gameRoom);
        //    }
        //}

        //public void RemoveRoomList(CGameRoom gameRoom)
        //{
        //    lock (roomList)
        //    {
        //        roomList.Remove(gameRoom);
        //    }
        //}

        public void FindRoom(short roomId, CPlayer player)
        {
            foreach (var room in roomList)
            {
                if (room is CGameRoom &&(room as CGameRoom).vo.GAME_NO == roomId && (room as CGameRoom).RoomUserCount() < 6)
                {
                    JoinRoom(player, (room as CGameRoom));
                    return;
                }
            }
            CGameRoom tempRoom = new CGameRoom(roomId, "");
            //roomDao.InsertGameRoom();
            roomList.Add(tempRoom);
            JoinRoom(player, tempRoom);
        }

        private void JoinRoom(CPlayer player, CGameRoom room)
        {
            room.AddPlayerList(player);
            roomList[0].RemovePlayerList(player);
            player.GAME_ROOM = room;
        }

        // 게임방에 진입하면 CPlayer객체를 만들어서 해당 GameRoom에 등록시킨다
        public void AddRoomCreatePlayer(string gameroom_id, string gameroom_name,
                                        CGameUser gameUser, string user_id)
        {
            CPlayer player;

            lock (this.roomList)
            {
                foreach (var room in roomList)
                {
                    // 게임룸id, name이 일치하는 room객체를 찾아서 
                    // 해당 room에 CPlayer객체를 등록한다.
                    if (room is CGameRoom)
                    {
                        if (((CGameRoom)room).vo.GAME_NO.Equals(gameroom_id) &&
                                ((CGameRoom)room).vo.GAME_NAME.Equals(gameroom_name))
                        {
                            player = gameUser.PLAYER;
                            room.AddPlayerList(player);
                            roomList[0].RemovePlayerList(player);
                            return;
                        }
                    }
                }
            }

        }
    }
}
