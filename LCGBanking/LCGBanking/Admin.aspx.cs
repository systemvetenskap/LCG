using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;

namespace LCGBanking
{
    public partial class Admin : System.Web.UI.Page
    {
        private const string conString = "cirkus";

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Provdeltagare_listan> provdeltagareListan = new List<Provdeltagare_listan>();
            GridViewDeltagarLista.CssClass = "admin-tabell";

            provdeltagareListan = GeProvdeltagareListan();
            GridViewDeltagarLista.DataSource = provdeltagareListan;
            GridViewDeltagarLista.DataBind();

        }
        /// <summary>
        /// Metod för att hämta datat och presentera deltagar listan
        /// </summary>
        /// <returns></returns>
        public static List<Provdeltagare_listan> GeProvdeltagareListan()
        {
            List<Provdeltagare_listan> provdeltagareListan = new List<Provdeltagare_listan>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);

            try
            {
                conn.Open();
                string sql = "";
                sql = sql + "SELECT lcg_person.id AS person_id, ";
                sql = sql + "       lcg_person.namn AS namn, ";
                sql = sql + "       lcg_provtillfalle.datum ::timestamp::date AS senaste_prov, ";
                sql = sql + "       lcg_provtillfalle.provresultat AS senaste_provresultat, ";
                sql = sql + "       CASE WHEN lcg_provtillfalle.godkand = TRUE THEN 'Ja' ";
                sql = sql + "            WHEN lcg_provtillfalle.godkand = FALSE THEN 'Nej' ";
                sql = sql + "            ELSE NULL END AS godkand_senast, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN 'Ja' ";
                sql = sql + "            ELSE 'Nej' END AS licencierad, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = FALSE THEN NULL ";
                sql = sql + "            ELSE lcg_provtillfalle.datum ::timestamp::date END AS licencierings_datum, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN 'Kunskapstest' ";
                sql = sql + "            ELSE 'Licenseringstest' END AS provtyp, ";
                sql = sql + "       CASE WHEN lcg_provtillfalle.godkand = FALSE THEN (lcg_provtillfalle.datum + '7 DAYS') ::timestamp::date ";
                sql = sql + "          ELSE NULL END AS nasta_prov_tidigast, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN (lcg_provtillfalle.datum + interval '365 day') ::timestamp::date ";
                sql = sql + "          ELSE NULL END AS nasta_prov_senast, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN (lcg_provtillfalle.datum + interval '365 day') ::timestamp::date - CURRENT_DATE ::timestamp::date ";
                sql = sql + "          ELSE NULL END AS dagar_kvar ";
                sql = sql + "FROM lcg_person ";
                sql = sql + "     LEFT JOIN lcg_roll AS lcg_roll ON lcg_roll.id = lcg_person.fk_roll_id ";
                sql = sql + "     LEFT JOIN lcg_provtillfalle AS lcg_provtillfalle ON lcg_provtillfalle.fk_person_id = lcg_person.id ";
                sql = sql + "WHERE ( lcg_provtillfalle.DATUM = (SELECT MAX(c.DATUM) ";
                sql = sql + "                                    FROM lcg_provtillfalle c ";
                sql = sql + "                                     WHERE c.fk_person_id = lcg_provtillfalle.fk_person_id) ";
                sql = sql + "      OR lcg_provtillfalle.DATUM IS NULL) ";
                sql = sql + "ORDER BY lcg_provtillfalle.datum ASC;";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Provdeltagare_listan nyDeltagare = new Provdeltagare_listan{};
                    
                    nyDeltagare.Person_id = Convert.ToString(dr["person_id"]);
                    nyDeltagare.Person_id = Convert.ToString(dr["person_id"]);
                    nyDeltagare.Namn = Convert.ToString(dr["namn"]);
                    nyDeltagare.Senaste_prov = Convert.ToString(dr["senaste_prov"]);
                    if (nyDeltagare.Senaste_prov.Length > 10)
                    {
                        nyDeltagare.Senaste_prov = nyDeltagare.Senaste_prov.Substring(0, 10);
                    }
                    nyDeltagare.Senaste_provresultat = Convert.ToString(dr["senaste_provresultat"]);
                    nyDeltagare.Godkand_senast = Convert.ToString(dr["godkand_senast"]);
                    nyDeltagare.Licencierad = Convert.ToString(dr["licencierad"]);
                    nyDeltagare.Licencierings_datum = Convert.ToString(dr["licencierings_datum"]);
                    if (nyDeltagare.Licencierings_datum.Length > 10)
                    {
                        nyDeltagare.Licencierings_datum = nyDeltagare.Licencierings_datum.Substring(0, 10);
                    }                    
                    nyDeltagare.Provtyp = Convert.ToString(dr["provtyp"]);
                    nyDeltagare.Nasta_prov_tidigast = Convert.ToString(dr["nasta_prov_tidigast"]);
                    if (nyDeltagare.Nasta_prov_tidigast.Length > 10)
                    {
                        nyDeltagare.Nasta_prov_tidigast = nyDeltagare.Nasta_prov_tidigast.Substring(0, 10);
                    }
                    nyDeltagare.Nasta_prov_senast = Convert.ToString(dr["nasta_prov_senast"]);
                    if (nyDeltagare.Nasta_prov_senast.Length > 10)
                    {
                        nyDeltagare.Nasta_prov_senast = nyDeltagare.Nasta_prov_senast.Substring(0, 10);
                    }
                    nyDeltagare.Dagar_kvar = Convert.ToString(dr["dagar_kvar"]);

                    provdeltagareListan.Add(nyDeltagare);
                } 
            }
            
            catch (NpgsqlException ex)
            {
                //MessageBox.Show("Ett fel uppstod:\n" + ex.Message); OBS! Lämlig medellande?
            }
            finally
            {
                conn.Close();
            }
            return provdeltagareListan;
        }
    }
}