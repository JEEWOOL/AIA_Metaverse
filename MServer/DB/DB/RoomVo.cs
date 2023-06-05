using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class RoomVo
    {
        public string USER_ID { get; set; }
        public string ROOM_DATA { get; set; }        

        public override string ToString()
        {
            return $"user_id={USER_ID}, " +
                   $"room_data={ROOM_DATA}";                    
        }
    }
}
