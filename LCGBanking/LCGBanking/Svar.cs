using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Svar
    {
        public int id_db { get; set; }
        public string alt { get; set; }
        public string svar { get; set; }
        public string facit { get; set; }
        public bool icheckad { get; set; }
        public int fk_fraga_id { get; set; }
    }
}