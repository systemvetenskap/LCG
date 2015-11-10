using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Fraga
    {
        public int id_db { get; set; }
        public int id { get; set; }
        public string fraga { get; set; }
        public string kategori { get; set; }
        public string information { get; set; }
        public bool flerVal {get; set;}
        public int fk_provtillfalle_id { get; set; }
        public string bildURL { get; set; }
        public List<Svar> svarLista = new List<Svar>();
    }
}