using DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class CPlayer
    {
        public CPlace GAME_ROOM { get; set; }  // 소속룸
        public CGameUser GAME_USER { get; set; }  // 연결된 CGameUser 객체
        public string USER_ID { get; set; }        // Player의 정보

        public byte spawnIdx { get; set; }      // Player가 생성된 초기위치의 index

        public CPlayer(CPlace gameRoom, CGameUser gameUser, string user_id)
        {
            this.GAME_ROOM = gameRoom;
            this.GAME_USER = gameUser;
            this.USER_ID = user_id;
            this.GAME_USER.PLAYER = this;
        }
    }
}
