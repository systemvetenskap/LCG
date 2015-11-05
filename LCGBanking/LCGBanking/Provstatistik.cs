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
        public int Db_fråga_id { get; set; }
        public int Fråga_id { get; set; }
        public string Fråga { get; set; }
        public int Antal_frågor { get; set; }
        public int Antal_poäng { get; set; }
        public double Antal_rätt { get; set; }

    }
}