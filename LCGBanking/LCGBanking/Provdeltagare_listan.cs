using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Provdeltagare_listan
    {
        public string Person_id { get; set; }
        public string Namn { get; set; }
        public string Senaste_prov { get; set; }
        public string Senaste_provresultat { get; set; }
        public string Godkand_senast { get; set; }
        public string Licencierad { get; set; }
        public string Licencierings_datum { get; set; }
        public string Provtyp { get; set; }
        public string Nasta_prov_tidigast { get; set; }
        public string Nasta_prov_senast { get; set; }
        public string Dagar_kvar { get; set; }
    }
}