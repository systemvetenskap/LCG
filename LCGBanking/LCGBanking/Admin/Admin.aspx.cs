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
        public int anvandare;

        protected void Page_Load(object sender, EventArgs e)
        {
            anvandare = Convert.ToInt32(Session["lcg_roll"]);

            if (!Page.IsPostBack)
            {
                if (anvandare == 2)
                { 
                    ((Label)Master.FindControl("headertext")).Visible = true;
                    ((HyperLink)Master.FindControl("HyperLinkLicens")).Visible = true;
                    ((HyperLink)Master.FindControl("HyperLinkAdmin")).Visible = true;
                    Welcome.Text = "Välkommen tillbaka till Kunskapsportalens administrationssida. " + " Du är inloggad som: " + GlobalValues.namn ;
                }

                CheckBoxSvarText.Visible = false;
                ButtonIndResGeLicens.Visible = false;
                LabelLicensGiven.Text = "";
            }
            List<Provdeltagare_listan> provdeltagareListan = new List<Provdeltagare_listan>();
            GridViewDeltagarLista.CssClass = "admin-tabell";

            provdeltagareListan = GeProvdeltagareListan();
            GridViewDeltagarLista.DataSource = provdeltagareListan;
            GridViewDeltagarLista.DataBind();

            if (!IsPostBack)
            {
                populeraListBoxGVIndRes();
                fyllGridViewStatistik("Licenseringstest");
                fyllGridViewStatistik("Kunskapstest");
                IndividuellaResultat.Visible = false;
                OvergripandeStatistik.Visible = false;
            }

        }

        /// <summary>
        /// Returnerar en lista med personer 
        /// </summary>
        /// <returns></returns>
        public List<Person> GeListaPersoner()
        {
            List<Person> listaPersoner = new List<Person>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT id, namn, fk_roll_id, har_licens FROM lcg_person";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);

                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Person nyPerson = new Person();
                    nyPerson.Id = dr["id"] != DBNull.Value ? (int)(dr["id"]) : 0;
                    nyPerson.Namn = (string)(dr["namn"]);
                    nyPerson.Roll_id = dr["fk_roll_id"] != DBNull.Value ? (int)(dr["fk_roll_id"]) : 0;
                    nyPerson.Har_licens = dr["har_licens"] != DBNull.Value ? (bool)(dr["har_licens"]) : false;
                    listaPersoner.Add(nyPerson);
                }
            }
            catch (NpgsqlException ex)
            {
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av person relaterad information. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");                
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
        public List<Provdeltagare_listan> GeProvdeltagareListan()
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
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN (lcg_provtillfalle.datum + interval '365 day') ::timestamp::date ";
                sql = sql + "            WHEN lcg_provtillfalle.godkand = FALSE THEN (lcg_provtillfalle.datum + '7 DAYS') ::timestamp::date";
                sql = sql + "            ELSE NULL END AS nasta_prov_tidigast,";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN (lcg_provtillfalle.datum + interval '455 day') ::timestamp::date ";
                sql = sql + "            ELSE (lcg_provtillfalle.datum + interval '90 day') ::timestamp::date END AS nasta_prov_senast, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN (lcg_provtillfalle.datum + interval '455 day') ::timestamp::date - CURRENT_DATE ::timestamp::date ";
                sql = sql + "            ELSE NULL END AS dagar_kvar ";
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
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av information till provdeltagarlistan. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");                
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
        public Provtillfalle GeSistaTillfalleId(int person_id)
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
                    nyProvtillfalle.Provresultat = dr["provresultat"] != DBNull.Value ? (int)(dr["provresultat"]) : 0;
                    nyProvtillfalle.Godkand = dr["godkand"] != DBNull.Value ? (bool)(dr["godkand"]) : false;
                    nyProvtillfalle.Fk_person_id = (int)(dr["fk_person_id"]);
                    // nyProvtillfalle.Anvandar_id = (int)(dr["anvandar_id"]);
                    // nyProvtillfalle.Anvandarnamn = (string)(dr["anvandarnamn"]);
                }
            }
            catch (NpgsqlException ex)
            {
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av senaste provtillfälle. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return nyProvtillfalle;
        }
        
        /// <summary>
        /// Metoden returnerar statiskt data per prov fråga.  
        /// </summary>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public List<Provstatistik> GeStatistikPerFraga(int person_id)
        {
            List<Provstatistik> listaPerFraga = new List<Provstatistik>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT person_id, namn, provtillfalle_id, datum, typ_av_test, kategori, db_fraga_id, fraga_id, fraga, ";
                sql = sql + "SUM(antal_fragor) antal_fragor, ";
                sql = sql + "SUM(antal_poang) antal_poang, ";
                sql = sql + "round(SUM(antal_poang)/SUM(antal_fragor),2)*100 antal_ratt ";
                sql = sql + "FROM lcg_provstatistik ";
                sql = sql + "WHERE person_id = :person_id ";
                sql = sql + "GROUP BY person_id, namn, provtillfalle_id, datum, typ_av_test, kategori, db_fraga_id, fraga_id, fraga ";
                sql = sql + "ORDER BY person_id, namn, provtillfalle_id, datum, typ_av_test, kategori, db_fraga_id, fraga_id, fraga";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("person_id", NpgsqlDbType.Integer));
                command.Parameters["person_id"].Value = person_id;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Provstatistik nyProv = new Provstatistik();
                    nyProv.Person_id = (int)(dr["person_id"]);
                    nyProv.Namn = (string)(dr["namn"]);
                    nyProv.Provtillfalle_id = (int)(dr["provtillfalle_id"]);
                    nyProv.Datum = (DateTime)(dr["datum"]);
                    nyProv.Typ_av_test = (string)(dr["typ_av_test"]);
                    nyProv.Kategori = (string)(dr["kategori"]);
                    // nyProv.Db_fråga_id = (int)(dr["db_fraga_id"]);
                    nyProv.Fraga_id = (int)(dr["fraga_id"]);
                    nyProv.Fraga = (string)(dr["fraga"]);
                    nyProv.Antal_fragor = Convert.ToInt32(dr["antal_fragor"]);
                    nyProv.Antal_poang = Convert.ToInt32(dr["antal_poang"]);
                    nyProv.Antal_ratt = Convert.ToDouble(dr["antal_ratt"]);
                    listaPerFraga.Add(nyProv);
                }
            }
            catch (NpgsqlException ex)
            {
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av statistik. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return listaPerFraga;
        }
        /// <summary>
        /// Hämtar prov datat för respektive person och provtillfälle fördelat på de olika fråge kategorier. 
        /// </summary>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public List<Provstatistik> GeStatistikPerKategori(int person_id)
        {
            List<Provstatistik> listaPerKategori = new List<Provstatistik>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT person_id, namn, provtillfalle_id, datum, typ_av_test, kategori, ";
                sql = sql + "SUM(antal_fragor) antal_fragor, ";
                sql = sql + "SUM(antal_poang) antal_poang, ";
                sql = sql + "round(SUM(antal_poang)/SUM(antal_fragor),2)*100 antal_ratt ";
                sql = sql + "FROM lcg_provstatistik ";
                sql = sql + "WHERE person_id = :person_id ";
                // sql = sql + " AND datum = (SELECT max(datum) FROM lcg_provtillfalle WHERE fk_person_id = person_id)";
                sql = sql + "GROUP BY person_id, namn, provtillfalle_id, datum, typ_av_test, kategori ";
                sql = sql + "ORDER BY person_id, namn, provtillfalle_id, datum, typ_av_test, kategori ";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("person_id", NpgsqlDbType.Integer));
                command.Parameters["person_id"].Value = person_id;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Provstatistik nyProv = new Provstatistik();
                    nyProv.Person_id = (int)(dr["person_id"]);
                    nyProv.Namn = (string)(dr["namn"]);
                    nyProv.Provtillfalle_id = (int)(dr["provtillfalle_id"]);
                    nyProv.Datum = (DateTime)(dr["datum"]);
                    nyProv.Typ_av_test = (string)(dr["typ_av_test"]);
                    nyProv.Kategori = (string)(dr["kategori"]);
                    nyProv.Antal_fragor = Convert.ToInt32(dr["antal_fragor"]);
                    nyProv.Antal_poang = Convert.ToInt32(dr["antal_poang"]);
                    nyProv.Antal_ratt = Convert.ToDouble(dr["antal_ratt"]);
                    listaPerKategori.Add(nyProv);
                }
            }
            catch (NpgsqlException ex)
            {
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av statistik. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return listaPerKategori;
        }
        /// <summary>
        /// Hämtar prov datat för respektive person och provtillfälle. 
        /// </summary>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public List<Provstatistik> GeStatistikPerProv(int person_id)
        {
            List<Provstatistik> listaPerProv = new List<Provstatistik>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT person_id, namn, provtillfalle_id, datum, typ_av_test, ";
                sql = sql + "SUM(antal_fragor) antal_fragor, ";
                sql = sql + "SUM(antal_poang) antal_poang, ";
                sql = sql + "round(SUM(antal_poang)/SUM(antal_fragor),2)*100 antal_ratt ";
                sql = sql + "FROM lcg_provstatistik ";
                sql = sql + "WHERE person_id = :person_id ";
                // sql = sql + " AND datum = (SELECT max(datum) FROM lcg_provtillfalle WHERE fk_person_id = person_id)";
                sql = sql + "GROUP BY person_id, namn, provtillfalle_id, datum, typ_av_test ";
                sql = sql + "ORDER BY person_id, namn, provtillfalle_id, datum, typ_av_test ";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("person_id", NpgsqlDbType.Integer));
                command.Parameters["person_id"].Value = person_id;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Provstatistik nyProv = new Provstatistik();
                    nyProv.Person_id = (int)(dr["person_id"]);
                    nyProv.Namn = (string)(dr["namn"]);
                    nyProv.Provtillfalle_id = (int)(dr["provtillfalle_id"]);
                    nyProv.Datum = (DateTime)(dr["datum"]);
                    nyProv.Typ_av_test = (string)(dr["typ_av_test"]);
                    nyProv.Antal_fragor = Convert.ToInt32(dr["antal_fragor"]);
                    nyProv.Antal_poang = Convert.ToInt32(dr["antal_poang"]);
                    nyProv.Antal_ratt = Convert.ToDouble(dr["antal_ratt"]);
                    listaPerProv.Add(nyProv);
                }
            }
            catch (NpgsqlException ex)
            {
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av statistik. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return listaPerProv;
        }

        /// <summary>
        /// Hämtar en lista med frågor för en specifikt prov tillfalle
        /// </summary>
        /// <param name="tillfalleid"></param>
        /// <returns></returns>
        public List<Fraga> GeFrageListan(int tillfalleid)
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
                sql = sql + " WHERE fk_provtillfalle_id = :newFkProvtillfalleId";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("newFkProvtillfalleId", NpgsqlDbType.Integer));
                command.Parameters["newFkProvtillfalleId"].Value = tillfalleid;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Fraga nyFraga = new Fraga();
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
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av statistik. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
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
        public List<Svar> GeSvarLista(int frageid)
        {
            List<Svar> svarLista = new List<Svar>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "";
                sql = sql + "SELECT id, svar, alt, facit, icheckad, fk_fraga_id";
                sql = sql + " FROM lcg_svar WHERE fk_fraga_id = :newFkFragaId";
                
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("newFkFragaId", NpgsqlDbType.Integer));
                command.Parameters["newFkFragaId"].Value = frageid;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Svar nySvar = new Svar { };
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
                string felmeddelande = "Ett fel har uppstått i samband med hämtning av statistik. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return svarLista;
        }

        /// <summary>
        /// skickar information till gridview för översiktliga individuella resultat
        /// </summary>
        /// <param name="personId"></param>
        protected void fyllGridViewIndResOversikt(int personId)
        {
            List<Provstatistik> provStatistik = GeStatistikPerProv(personId);
            List<Provstatistik> kategorier = GeStatistikPerKategori(personId);
            Provstatistik prov = new Provstatistik();
            Provtillfalle senasteProv = GeSistaTillfalleId(personId);

            foreach (Provstatistik pr in provStatistik)
            {
                if (pr.Provtillfalle_id == senasteProv.Id)
                {
                    prov = pr;
                }
            }

            foreach (Provstatistik pr in kategorier)
            {
                if (pr.Provtillfalle_id != senasteProv.Id)
                {
                    kategorier.Remove(pr);
                }
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Kategori", typeof(String));
            dt.Columns.Add("Antal frågor", typeof(int));
            dt.Columns.Add("Poäng", typeof(int));
            dt.Columns.Add("Procent rätt", typeof(String));
            dt.Columns.Add("Godkänd", typeof(String));

            int godkandaKategorier = 0;
            foreach (Provstatistik pr in kategorier)
            {
                DataRow dr = dt.NewRow();
                dr["Kategori"] = pr.Kategori;
                dr["Antal frågor"] = pr.Antal_fragor;
                dr["Poäng"] = pr.Antal_poang;
                dr["Procent rätt"] = Convert.ToInt32(pr.Antal_ratt) + "%";
                if (Convert.ToInt32(pr.Antal_ratt) >= 60)
                {
                    dr["Godkänd"] = "Ja";
                    godkandaKategorier++;
                }
                else
                {
                    dr["Godkänd"] = "Nej";
                }
                dt.Rows.Add(dr);
            }

            DataRow drTotal = dt.NewRow();
            drTotal["Kategori"] = "Totalt";
            drTotal["Antal frågor"] = prov.Antal_fragor;
            drTotal["Poäng"] = prov.Antal_poang;
            drTotal["Procent rätt"] = Convert.ToInt32(prov.Antal_ratt) + "%";
            if (Convert.ToInt32(prov.Antal_ratt) >= 70 && godkandaKategorier == 3)
            {
                drTotal["Godkänd"] = "Ja";
            }
            else
            {
                drTotal["Godkänd"] = "Nej";
            }
            dt.Rows.Add(drTotal);

            GridViewIndResOversikt.DataSource = dt;
            GridViewIndResOversikt.DataBind();
        }

        /// <summary>
        /// laddar GridViewIndividResultat med information
        /// </summary>
        protected void fyllGridViewIndividResultat(GridView gridview, string kategori)
        {
            if (GlobalValues.GVIndResLista.Count == 0)
            {
                Fraga dummy = new Fraga
                {
                    id = 0,
                    fraga = "Ingen data hittades",
                    kategori = "Ingen data hittades",
                    information = "Ingen data hittades",
                    flerVal = false
                };

                Svar dummysvar = new Svar
                {
                    alt = "Ingen data hittades",
                    svar = "Ingen data hittades",
                    facit = "Ingen data hittades",
                    icheckad = false
                };
                dummy.svarLista.Add(dummysvar);
                GlobalValues.GVIndResLista.Add(dummy);
            }

            Fraga maxSvarFraga = new Fraga();

            foreach (Fraga fr in GlobalValues.GVIndResLista)
            {
                if (fr.svarLista.Count > maxSvarFraga.svarLista.Count)
                {
                    maxSvarFraga = fr;
                }
            }
            svarAntal = maxSvarFraga.svarLista.Count();

            DataTable dt = new DataTable();

            //kolumner
            dt.Columns.Add("fraga", typeof(String));

            foreach (Svar sv in maxSvarFraga.svarLista)
            {
                dt.Columns.Add("svar" + maxSvarFraga.svarLista.IndexOf(sv), typeof(String));
            }

            dt.Columns.Add("poang", typeof(int));

            //rader
            int summaPoang = 0;
            foreach (Fraga fr in GlobalValues.GVIndResLista)
            {
                if (fr.kategori == kategori)
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
                        string icheckad = sv.icheckad.ToString().ToLower();
                        if (icheckad == sv.facit && sv.facit == "true")
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
            }

            gridview.DataSource = dt;
            gridview.DataBind();
        }

        /// <summary>
        /// färgkodar GridViewIndividResultat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewIndividResultat_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridView gridview = (GridView)sender;
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

                gridview.HeaderRow.Controls.Clear();
                gridview.HeaderRow.Parent.Controls.AddAt(0, gvr);

                //färgläggning
                foreach (GridViewRow gr in gridview.Rows)
                {
                    gr.Cells[0].CssClass = "GVIndRes_fraga";
                    gr.Cells[gr.Cells.Count - 1].CssClass = "GVIndRes_poang";
                    foreach (Fraga fr in GlobalValues.GVIndResLista)
                    {
                        string cellFraga = Server.HtmlDecode(gr.Cells[0].Text);
                        if (cellFraga == fr.fraga)
                        {
                            //rad har kopplats till fråga
                            foreach (TableCell tc in gr.Cells)
                            {
                                foreach (Svar sv in fr.svarLista)
                                {
                                    if ((Server.HtmlDecode(tc.Text) == sv.alt || Server.HtmlDecode(tc.Text) == sv.svar) && sv.facit == "true" && sv.icheckad == true)
                                    {
                                        tc.CssClass = "GVIndRes_rattsvar";
                                    }
                                    else if ((Server.HtmlDecode(tc.Text) == sv.alt || Server.HtmlDecode(tc.Text) == sv.svar) && sv.facit == "true" && sv.icheckad == false)
                                    {
                                        tc.CssClass = "GVIndRes_korrektsvar";
                                    }
                                    else if ((Server.HtmlDecode(tc.Text) == sv.alt || Server.HtmlDecode(tc.Text) == sv.svar) && sv.facit == "false" && sv.icheckad == true)
                                    {
                                        tc.CssClass = "GVIndRes_felsvar";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// uppdaterar GridViewIndividResultat vid CheckBoxSvarText_CheckedChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckBoxSvarText_CheckedChanged(object sender, EventArgs e)
        {
            LabelIndResKategori1.Text = "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi";
            LabelIndResKategori2.Text = "Produkter och hantering av kundens affärer";
            LabelIndResKategori3.Text = "Etik och regelverk";
            fyllGridViewIndividResultat(GridViewIndividResultat1, "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi");
            fyllGridViewIndividResultat(GridViewIndividResultat2, "Produkter och hantering av kundens affärer ");
            fyllGridViewIndividResultat(GridViewIndividResultat3, "Etik och regelverk");
        }

        /// <summary>
        /// visar eller döljer delar av admin-sidan beroende på checkboxval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckBoxAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxDeltagare.Checked)
            {
                Deltagarlista.Visible = true;
            }
            else
            {
                Deltagarlista.Visible = false;
            }
            if (CheckBoxIndRes.Checked)
            {
                IndividuellaResultat.Visible = true;
            }
            else
            {
                IndividuellaResultat.Visible = false;
            }
            if (CheckBoxOversikt.Checked)
            {
                OvergripandeStatistik.Visible = true;
            }
            else
            {
                OvergripandeStatistik.Visible = false;
            }
        }

        /// <summary>
        /// populerar listbox med personer
        /// </summary>
        private void populeraListBoxGVIndRes()
        {
            List<Person> ursprungligLista = GeListaPersoner();
            List<ListItem> nyLista = new List<ListItem>();

            foreach (Person pe in ursprungligLista)
            {
                nyLista.Add(new ListItem(pe.Namn, pe.Id.ToString()));
            }

            ListBoxGVIndRes.DataTextField = "Text";
            ListBoxGVIndRes.DataValueField = "Value";
            ListBoxGVIndRes.DataSource = nyLista;
            ListBoxGVIndRes.DataBind();
        }

        /// <summary>
        /// uppdaterar admin-sidan med information rörande vald person
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListBoxGVIndRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            ListItem li = lb.SelectedItem;

            int personId = Convert.ToInt32(li.Value);
            int senasteProv = GeSistaTillfalleId(personId).Id;
            List<Fraga> fragor = GeFrageListan(senasteProv);
            foreach (Fraga fr in fragor)
            {
                fr.svarLista = GeSvarLista(fr.id_db);
            }

            GlobalValues.GVIndResLista = fragor;

            LabelIndResKategori1.Text = "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi";
            LabelIndResKategori2.Text = "Produkter och hantering av kundens affärer";
            LabelIndResKategori3.Text = "Etik och regelverk";
            fyllGridViewIndividResultat(GridViewIndividResultat1, "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi");
            fyllGridViewIndividResultat(GridViewIndividResultat2, "Produkter och hantering av kundens affärer ");
            fyllGridViewIndividResultat(GridViewIndividResultat3, "Etik och regelverk");

            fyllGridViewIndResOversikt(personId);

            List<Provdeltagare_listan> plLista = GeProvdeltagareListan();
            Provdeltagare_listan deltagare = new Provdeltagare_listan();
            foreach (Provdeltagare_listan pl in plLista)
            {
                if (pl.Person_id == personId.ToString())
                {
                    deltagare = pl;
                }
            }
            LabelIndResNamn.Text = deltagare.Namn;
            LabelIndResDatum.Text = deltagare.Senaste_prov;

            string godkand = "Nej";
            try
            {
                godkand = GridViewIndResOversikt.Rows[3].Cells[4].Text;
            }
            catch (Exception)
            {
                
            }

            if (godkand == "Ja")
            {
                ButtonIndResGeLicens.Enabled = true;
            }
            else
            {
                ButtonIndResGeLicens.Enabled = false;
            }

            CheckBoxSvarText.Visible = true;
            ButtonIndResGeLicens.Visible = true;
            LabelLicensGiven.Text = "";
        }

        /// <summary>
        /// färgkodar gridview med översiktliga individuella testresultat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewIndResOversikt_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridView gridview = (GridView)sender;
                foreach (GridViewRow row in gridview.Rows)
                {
                    if (row.Cells[4].Text == "Ja")
                    {
                        row.Cells[4].CssClass = "GVIndRes_rattsvar";
                    }
                    else
                    {
                        row.Cells[4].CssClass = "GVIndRes_felsvar";
                    }
                }

                foreach (TableCell tc in gridview.Rows[3].Cells)
                {
                    tc.Font.Bold = true;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// ger licens till vald provtagare
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonIndResGeLicens_Click(object sender, EventArgs e)
        {
            ListItem valdPerson = ListBoxGVIndRes.SelectedItem;
            int personId = Convert.ToInt32(valdPerson.Value);

            tilldelaLicens(personId);
            LabelLicensGiven.Text = valdPerson.Text + " är nu licensierad";
        }

        /// <summary>
        /// tilldelar värdet TRUE på har_licens för vald person
        /// </summary>
        /// <param name="personId"></param>
        private void tilldelaLicens(int personId)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            NpgsqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                NpgsqlCommand command = new NpgsqlCommand(@"UPDATE lcg_person 
                                                            SET har_licens = TRUE
                                                            WHERE id = :id;", conn);

                command.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
                command.Parameters["id"].Value = personId;

                command.Transaction = tran;
                command.ExecuteNonQuery();
                tran.Commit();
            }
            catch (Exception ex)
            {
                string felmeddelande = "Ett fel har uppstått i samband med tilldelning av licencs. Mer information: " + ex.Message.ToString();
                Response.Write("<script>alert('" + felmeddelande + "')</script>");
                tran.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// skickar information till gridviews för översiktlig statistik
        /// </summary>
        /// <param name="provtyp"></param>
        private void fyllGridViewStatistik(string provtyp)
        {
            List<Provdeltagare_listan> deltagarlista = GeProvdeltagareListan();
            DataTable dt = new DataTable();

            dt.Columns.Add("Namn", typeof(string));
            if (provtyp == "Licenseringstest")
            {
                for (int i = 1; i <= 25; i++)
                {
                    dt.Columns.Add(i.ToString(), typeof(int));
                }
            }
            else
            {
                for (int i = 1; i <= 15; i++)
                {
                    dt.Columns.Add(i.ToString(), typeof(int));
                }
            }
            dt.Columns.Add("Ekonomi", typeof(string));
            dt.Columns.Add("Etik", typeof(string));
            dt.Columns.Add("Produkter", typeof(string));
            dt.Columns.Add("Totalt", typeof(string));

            foreach (Provdeltagare_listan deltagare in deltagarlista)
            {
                if (deltagare.Provtyp == provtyp)
                {
                    DataRow dr = dt.NewRow();
                    dr["Namn"] = deltagare.Namn;

                    //frågor
                    List<Provstatistik> fragor = GeStatistikPerFraga(Convert.ToInt32(deltagare.Person_id));
                    foreach (Provstatistik ps in fragor)
                    {
                        if (ps.Datum.ToString().Substring(0, 10) == deltagare.Senaste_prov)
                        {
                            foreach (DataColumn dc in dt.Columns)
                            {
                                if (dc.ColumnName == ps.Fraga_id.ToString())
                                {
                                    dr[dc.ColumnName] = Convert.ToInt32(ps.Antal_poang);
                                }
                            }
                        }
                    }

                    //kategorier
                    List<Provstatistik> kategorier = GeStatistikPerKategori(Convert.ToInt32(deltagare.Person_id));
                    foreach (Provstatistik ps in kategorier)
                    {
                        if (ps.Datum.ToString().Substring(0, 10) == deltagare.Senaste_prov)
                        {
                            if (ps.Kategori == "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi")
                            {
                                dr["Ekonomi"] = Convert.ToInt32(ps.Antal_ratt) + "%";
                            }
                            else if (ps.Kategori == "Etik och regelverk")
                            {
                                dr["Etik"] = Convert.ToInt32(ps.Antal_ratt) + "%";
                            }
                            else if (ps.Kategori == "Produkter och hantering av kundens affärer ")
                            {
                                dr["Produkter"] = Convert.ToInt32(ps.Antal_ratt) + "%";
                            }
                        }
                    }

                    //totalt
                    List<Provstatistik> prov = GeStatistikPerProv(Convert.ToInt32(deltagare.Person_id));
                    foreach (Provstatistik ps in prov)
                    {
                        if (ps.Datum.ToString().Substring(0, 10) == deltagare.Senaste_prov)
                        {
                            dr["Totalt"] = Convert.ToInt32(ps.Antal_ratt) + "%";
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }

            if (provtyp == "Licenseringstest")
            {
                GridViewOvergripandeLicens.DataSource = dt;
                GridViewOvergripandeLicens.DataBind();
            }
            else
            {
                GridViewOvergripandeKunskap.DataSource = dt;
                GridViewOvergripandeKunskap.DataBind();
            }
        }

        /// <summary>
        /// färgkodar gridviews för översiktlig statistik
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewOvergripandeStatistik_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridView gridview = (GridView)sender;
                foreach (GridViewRow row in gridview.Rows)
                {
                    foreach (TableCell tc in row.Cells)
                    {
                        if (tc.Text.Contains("%"))
                        {
                            string[] value = tc.Text.Split('%');
                            if (Convert.ToInt32(value[0]) >= 60)
                            {
                                tc.CssClass = "GVIndRes_rattsvar";
                            }
                            else
                            {
                                tc.CssClass = "GVIndRes_felsvar";
                            }
                        }
                        else if (tc.Text == "1")
                        {
                            tc.CssClass = "GVIndRes_rattsvar";
                        }
                        else if (tc.Text == "0")
                        {
                            tc.CssClass = "GVIndRes_felsvar";
                        }
                    }
                    string[] totalVal = row.Cells[row.Cells.Count - 1].Text.Split('%');
                    if (Convert.ToInt32(totalVal[0]) >= 70)
                    {
                        row.Cells[row.Cells.Count - 1].CssClass = "GVIndRes_rattsvar";
                    }
                    else
                    {
                        row.Cells[row.Cells.Count - 1].CssClass = "GVIndRes_felsvar";
                    }
                }
            }
            catch (Exception ex)
            {
                //Response.Write("<script>alert('"+ex.Message.ToString()+"')</script>");
            }
        }
    }
}