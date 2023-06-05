using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class ItemVo
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }

        public override string ToString()
        {
            return $"item_code={ITEM_CODE}, " +
                    $"item_name={ITEM_NAME}";
        }
    }
}
