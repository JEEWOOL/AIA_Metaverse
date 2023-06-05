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
        RoomDao roomDao = new RoomDao();

        List<CPlace> roomList = new List<CPlace>();

        public CPlaceManager()
        {
            // 시작하자마자 DB에 있는 게임룸 정보를 가져와 CGameRoom객체를 생성해
            // 리스트에 저장한다.
            roomList.Add(new CPlace());
            IList<RoomVo> voList = roomDao.GetRoomList();

            foreach (var vo in voList)
            {
                CGameRoom gameRoom = new CGameRoom(vo.ROOM_ID, vo.ROOM_NAME);
                roomList.Add(gameRoom);
            }
        }

        public void AddRoomList(CGameRoom gameRoom)
        {
            lock (roomList)
            {
                roomList.Add(gameRoom);
            }
        }

        public void RemoveRoomList(CGameRoom gameRoom)
        {
            lock (roomList)
            {
                roomList.Remove(gameRoom);
            }
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
                        if (((CGameRoom)room).vo.ROOM_ID.Equals(gameroom_id) &&
                                ((CGameRoom)room).vo.ROOM_NAME.Equals(gameroom_name))
                        {
                            player = new CPlayer(room, gameUser, user_id);
                            gameUser.PLAYER = player;
                            room.AddPlayerList(player);
                            return;
                        }
                    }
                }
            }
            player = new CPlayer(roomList[0], gameUser, user_id);
            gameUser.PLAYER = player;
            roomList[0].AddPlayerList(player);
        }
    }
}
