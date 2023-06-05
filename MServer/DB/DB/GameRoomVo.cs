using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class GameRoomVo
    {
        public short GAME_NO { get; set; }
        public string GAME_NAME { get; set; }        
        public string USER_COUNT { get; set; }

        public override string ToString()
        {
            return $"game_no={GAME_NO}, " +
                   $"game_name={GAME_NAME}, " +                   
                   $"user_count={USER_COUNT}";
        }
    }
}
