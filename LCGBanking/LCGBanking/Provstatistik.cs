using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Provstatistik
    {
        public int Person_id { get; set; }
        public string Namn { get; set; }
        public int Provtillfalle_id { get; set; }
        public DateTime Datum { get; set; }
        public string Typ_av_test { get; set; }
        public string Kategori { get; set; }
        public int Db_fraga_id { get; set; }
        public int Fraga_id { get; set; }
        public string Fraga { get; set; }
        public int Antal_fragor { get; set; }
        public int Antal_poang { get; set; }
        public double Antal_ratt { get; set; }

    }
}