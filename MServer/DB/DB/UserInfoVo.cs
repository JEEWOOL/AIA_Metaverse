using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class UserInfoVo
    {
        public string USER_ID { get; set; }
        public string USER_PASS { get; set; }
        public string GAME_NO { get; set; }
        public string COIN { get; set; }
        public string INVENT { get; set; }
        public override string ToString()
        {
            return $"user_id={USER_ID}, " +
                    $"user_pass={USER_PASS}, " +
                    $"coin={COIN}, " +
                    $"invent={INVENT}, " +
                    $"game_no={GAME_NO}";
        }
    }
}
