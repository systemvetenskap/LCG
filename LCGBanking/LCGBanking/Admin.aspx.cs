using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;
using System.Data;

namespace LCGBanking
{
    public partial class Admin : System.Web.UI.Page
    {
        private const string conString = "cirkus";
        private int svarAntal = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Welcome.Text = "Tjenare " + Context.User.Identity.Name;
            List<Provdeltagare_listan> provdeltagareListan = new List<Provdeltagare_listan>();
            GridViewDeltagarLista.CssClass = "admin-tabell";

            provdeltagareListan = GeProvdeltagareListan();
            GridViewDeltagarLista.DataSource = provdeltagareListan;
            GridViewDeltagarLista.DataBind();

            GridViewIndividResultat.CssClass = "admin-tabell";
            //fyllGridViewIndividResultat();
        }

        /// <summary>
        /// returnerar en lista med personer 
        /// </summary>
        /// <returns></returns>
        public static List<Person> GeListaPersoner()
        {
            List<Person> listaPersoner = new List<Person>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT id, namn, roll_id, har_licens FROM lcg_person";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);

                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Person nyPerson = new Person();
                    nyPerson.Id = (int)(dr["id"]);
                    nyPerson.Namn = (string)(dr["namn"]);
                    nyPerson.Roll_id = (int)(dr["roll_id"]);
                    nyPerson.Har_licens = (bool)(dr["roll_id"]);
                    listaPersoner.Add(nyPerson);
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
            return listaPersoner;
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
                    Provdeltagare_listan nyDeltagare = new Provdeltagare_listan { };
                    
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

        /// <summary>
        /// Metod för att hämta senaste prov tillfälle för respektive person  
        /// </summary>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public static Provtillfalle GeSistaTillfalleId(int person_id)
        {   
            Provtillfalle nyProvtillfalle = new Provtillfalle();

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);

            try
            {
                conn.Open();
                string sql = "";
                sql = sql + "SELECT lcg_provtillfalle.id, datum, typ_av_test, provresultat, godkand, fk_person_id ";
                sql = sql + " FROM lcg_person ";
                sql = sql + "     LEFT JOIN lcg_provtillfalle AS lcg_provtillfalle ON lcg_provtillfalle.fk_person_id = lcg_person.id ";
                sql = sql + " WHERE lcg_provtillfalle.fk_person_id = :newFkPersonId";
                sql = sql + " AND ( (lcg_provtillfalle.datum = (SELECT MAX(c.datum) ";
                sql = sql + "                                   FROM lcg_provtillfalle c";
                sql = sql + "                                   WHERE c.fk_person_id = lcg_provtillfalle.fk_person_id)) OR lcg_provtillfalle.datum IS NULL ) ";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);                
                command.Parameters.Add(new NpgsqlParameter("newFkPersonId", NpgsqlDbType.Integer));
                command.Parameters["newFkPersonId"].Value = person_id;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    nyProvtillfalle.Id = (int)(dr["id"]);
                    nyProvtillfalle.Datum = (DateTime)(dr["datum"]);
                    nyProvtillfalle.Typ_av_test = (string)(dr["typ_av_test"]);
                    nyProvtillfalle.Provresultat = (int)(dr["provresultat"]);
                    nyProvtillfalle.Godkand = (bool)(dr["godkand"]);
                    nyProvtillfalle.Fk_person_id = (int)(dr["fk_person_id"]);
                    // nyProvtillfalle.Anvandar_id = (int)(dr["anvandar_id"]);
                    // nyProvtillfalle.Anvandarnamn = (string)(dr["anvandarnamn"]);
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
            return nyProvtillfalle;
        }
        
        /// <summary>
        /// Hämtar en lista med frågor för en specifikt prov tillfalle
        /// </summary>
        /// <param name="tillfalleid"></param>
        /// <returns></returns>
        public static List<Fraga> GeFrageListan(int tillfalleid)
        {
            List<Fraga> frageListan = new List<Fraga>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "";
                sql = sql + "SELECT id, fraga_id, fraga, information, flerval, kategori, fk_provtillfalle_id";
                sql = sql + " FROM lcg_fragor ";
                sql = sql + " WHERE fk_provtillfalle_id = :fk_provtillfalle_id";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("newFkProvtillfalleId", NpgsqlDbType.Integer));
                command.Parameters["newFkProvtillfalleId"].Value = tillfalleid;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Fraga nyFraga = new Fraga { };
                    nyFraga.id_db = (int)(dr["id"]);
                    nyFraga.id = (int)(dr["fraga_id"]);
                    nyFraga.fraga = (string)(dr["fraga"]);
                    nyFraga.information = (string)(dr["information"]);
                    nyFraga.flerVal = (bool)(dr["flerval"]);
                    nyFraga.kategori = (string)(dr["kategori"]);
                    nyFraga.fk_provtillfalle_id = (int)(dr["fk_provtillfalle_id"]);
                    frageListan.Add(nyFraga);
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
            return frageListan;
        }
        /// <summary>
        /// Hämtar en lista med svar som användare har lämnat på respektive fråga  
        /// </summary>
        /// <param name="frageid"></param>
        /// <returns></returns>
        public static List<Svar> GeSvarLista(int frageid)
        {
            List <Svar> svarLista = new List<Svar>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "";
                sql = sql + "SELECT id, svar, alt, facit, icheckad, fk_fraga_id";
                sql = sql + " FROM lcg_svar WHERE fk_fraga_id = :fk_fraga_id";
                
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("newFkFragaId", NpgsqlDbType.Integer));
                command.Parameters["newFkFragaId"].Value = frageid;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Svar nySvar = new Svar{};
                    nySvar.id_db = (int)(dr["id"]);
                    nySvar.svar = (string)(dr["svar"]);
                    nySvar.alt = (string)(dr["alt"]);
                    nySvar.facit = (string)(dr["facit"]);
                    nySvar.icheckad = (bool)(dr["icheckad"]);
                    nySvar.fk_fraga_id = (int)(dr["fk_fraga_id"]);
                    svarLista.Add(nySvar);
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
            return svarLista;
        }

        /// <summary>
        /// laddar GridViewIndividResultat med information
        /// </summary>
        protected void fyllGridViewIndividResultat()
        {
            List<Fraga> testlist = GlobalValues.Fragor;

            Fraga maxSvarFraga = new Fraga();

            foreach (Fraga fr in testlist)
            {
                if (fr.svarLista.Count > maxSvarFraga.svarLista.Count)
                {
                    maxSvarFraga = fr;
                }
            }
            svarAntal = maxSvarFraga.svarLista.Count();

            DataTable dt = new DataTable();

            //kolumner
            dt.Columns.Add("fraga", typeof( String ));

            foreach (Svar sv in maxSvarFraga.svarLista)
            {
               dt.Columns.Add("svar" + maxSvarFraga.svarLista.IndexOf(sv), typeof( String ));
            }

            dt.Columns.Add("poang", typeof( int ));

            //rader
            int summaPoang = 0;
            foreach (Fraga fr in testlist)
            {
                DataRow dr = dt.NewRow();

                dr["fraga"] = fr.fraga;

                foreach (Svar sv in fr.svarLista)
                {
                    if (CheckBoxSvarText.Checked)
                    {
                        dr["svar" + fr.svarLista.IndexOf(sv)] = sv.svar;
                    }
                    else
                    {
                        dr["svar" + fr.svarLista.IndexOf(sv)] = sv.alt;
                    }
                }

                //poänguträkning
                int poang = 0;
                int antalKorrektaSvar = 0;
                foreach (Svar sv in fr.svarLista)
                {
                    if (sv.facit == "true")
                    {
                        antalKorrektaSvar++;
                    }
                }

                int givnaKorrektaSvar = 0;
                foreach (Svar sv in fr.svarLista)
                {
                    if (sv.icheckad.ToString() == sv.facit && sv.facit == "true")
                    {
                        givnaKorrektaSvar++;
                    }
                }

                if (antalKorrektaSvar == givnaKorrektaSvar)
                {
                    poang++;
                    summaPoang++;
                }
                
                dr["poang"] = poang;

                dt.Rows.Add(dr);
            }

            GridViewIndividResultat.DataSource = dt;
            GridViewIndividResultat.DataBind();
        }

        /// <summary>
        /// färgkodar GridViewIndividResultat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewIndividResultat_DataBound(object sender, EventArgs e)
        {
            GridViewRow gvr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);

            TableHeaderCell cell = new TableHeaderCell();
            cell.Text = "Frågor";
            cell.ColumnSpan = 1;
            gvr.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.Text = "Svar";
            cell.ColumnSpan = svarAntal;
            gvr.Controls.Add(cell);

            cell = new TableHeaderCell();
            cell.Text = "Poäng";
            cell.ColumnSpan = 1;
            gvr.Controls.Add(cell);

            GridViewIndividResultat.HeaderRow.Controls.Clear();
            GridViewIndividResultat.HeaderRow.Parent.Controls.AddAt(0, gvr);

            //färgläggning
            List<Fraga> testlist = GlobalValues.Fragor;

            foreach (GridViewRow gr in GridViewIndividResultat.Rows)
            {
                foreach (Fraga fr in testlist)
                {
                    string cellFraga = Server.HtmlDecode(gr.Cells[0].Text);
                    if (cellFraga == fr.fraga)
                    {
                        //rad har kopplats till fråga
                        if (Convert.ToInt32(gr.Cells[gr.Cells.Count-1].Text) > 0)
                        {
                            //den är korrekt besvarad
                            foreach (TableCell tc in gr.Cells)
                            {
                                foreach (Svar sv in fr.svarLista)
                                {
                                    if (Server.HtmlDecode(tc.Text) == sv.alt && sv.facit == "true")
                                    {
                                        tc.CssClass = "GVIndRes_rattsvar";
                                    }
                                }
                            }
                        }
                        else
                        {
                            //DU HAR SVARAT FEL!!!
                            foreach (TableCell tc in gr.Cells)
                            {
                                foreach (Svar sv in fr.svarLista)
                                {
                                    if (Server.HtmlDecode(tc.Text) == sv.alt && sv.facit == "true")
                                    {
                                        tc.CssClass = "GVIndRes_korrektsvar";
                                    }
                                    else if (Server.HtmlDecode(tc.Text) == sv.alt && sv.icheckad == true)
                                    {
                                        tc.CssClass = "GVIndRes_felsvar";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// uppdaterar GridViewIndividResultat vid CheckBoxSvarText_CheckedChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckBoxSvarText_CheckedChanged(object sender, EventArgs e)
        {
            fyllGridViewIndividResultat();
        }
    }
}