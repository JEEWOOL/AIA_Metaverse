using DB;
using FreeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class CGameRoom : CPlace
    {
        public GameRoomVo vo = new GameRoomVo();
        public bool isGameRunning = false;
        List<CPlayer> playerList = new List<CPlayer>(); // 진입한 Player들(Client)

        public CGameRoom(short gameRoom_id, string gameRoom_name)// : base(gameRoom_id)
        {            
            this.vo.GAME_NO = gameRoom_id;
            this.vo.GAME_NAME = gameRoom_name;
        }
    }
}
