using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Provdeltagare_listan
    {
        public int Person_id { get; set; }
        public string Namn { get; set; }
        public DateTime Senaste_prov { get; set; }
        public int Senaste_provresultat { get; set; }
        public string Godkand_senast { get; set; }
        public string Licencierad { get; set; }
        public DateTime Licencierings_datum { get; set; }
        public string Provtyp { get; set; }
        public DateTime Nasta_prov_tidigast { get; set; }
        public DateTime Nasta_prov_senast { get; set; }
        public int Dagar_kvar {get; set;}
    }
}