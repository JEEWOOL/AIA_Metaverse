using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class GameSetVo
    {
        public string GAME_NAME { get; set; }
        public string USER_MIN { get; set; }
        public string USER_MAX { get; set; }

        public override string ToString()
        {
            return $"game_name={GAME_NAME}, " +
                   $"user_min={USER_MIN}, " +
                   $"user_max={USER_MAX}";
        }
    }
}
