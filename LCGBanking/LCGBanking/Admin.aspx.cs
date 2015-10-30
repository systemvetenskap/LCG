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
                    nyDeltagare.Person_id = (int)dr["person_id"];
                    nyDeltagare.Namn = (string)dr["namn"];
                    if (dr["senaste_prov"] != DBNull.Value)
                    {
                        nyDeltagare.Senaste_prov = (DateTime)dr["senaste_prov"];
                    }
                    if (dr["senaste_provresultat"] != DBNull.Value)
                    {
                        nyDeltagare.Senaste_provresultat = (int)dr["senaste_provresultat"];
                    }
                    if (dr["godkand_senast"] != DBNull.Value)
                    {
                        nyDeltagare.Godkand_senast = (string)dr["godkand_senast"];
                    }
                    if (dr["licencierad"] != DBNull.Value)
                    {
                        nyDeltagare.Licencierad = (string)dr["licencierad"];
                    }
                    if (dr["licencierings_datum"] != DBNull.Value)
                    {
                        nyDeltagare.Licencierings_datum = (DateTime)dr["licencierings_datum"];                    
                    }
                    if (dr["provtyp"] != DBNull.Value)
                    {
                        nyDeltagare.Provtyp = (string)dr["provtyp"];                    
                    }
                    if (dr["nasta_prov_tidigast"] != DBNull.Value)
                    {
                        nyDeltagare.Nasta_prov_tidigast = (DateTime)dr["nasta_prov_tidigast"];
                    }
                    if (dr["nasta_prov_senast"] != DBNull.Value)
                    {
                        nyDeltagare.Nasta_prov_senast = (DateTime)dr["nasta_prov_senast"];
                    }
                    if (dr["dagar_kvar"] != DBNull.Value)
                    {
                        nyDeltagare.Dagar_kvar = (int)dr["dagar_kvar"];
                    }
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