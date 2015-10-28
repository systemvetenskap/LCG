using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Lcg_provtillfalle
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public string Typ_av_test { get; set;}
        public int Fk_person_id { get; set; }
        public string Anvandarnamn { get; set; }
    }
}