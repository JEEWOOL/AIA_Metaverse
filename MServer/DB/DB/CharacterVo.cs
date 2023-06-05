using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class CharacterVo
    {
        public string CHAR_NO { get; set; }                        
        public string COIN { get; set; }
        public string USER_ID { get; set; }       
        public string INVENT { get; set; }

        public override string ToString()
        {
            return $"char_no={CHAR_NO}, " +
                    $"coin={COIN}, " +
                    $"user_id={USER_ID}, " +
                    $"invent={INVENT}";
        }

    }
}
