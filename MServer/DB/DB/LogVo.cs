using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class LogVo
    {
        public string LOG_SEQ { get; set; }
        public string LOG_TIME { get; set; }
        public string LOG_IP { get; set; }
        public string LOG_PORT { get; set; }
        public string LOG_INFO { get; set; }
        public string USER_ID { get; set; }

        public override string ToString()
        {
            return $"log_seq={LOG_SEQ}, " +
                $"log_time={LOG_TIME}, " +
                $"log_ip={LOG_IP}, " +
                $"log_port={LOG_PORT}, " +
                $"log_info={LOG_INFO}, " +
                $"user_id={USER_ID}";
        }
    }
}
