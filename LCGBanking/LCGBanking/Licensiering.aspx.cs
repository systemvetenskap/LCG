using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Web.UI.WebControls;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using System.Configuration;
using System.Web.Security;
using System.Data;

namespace LCGBanking
{
    public partial class Licensiering : System.Web.UI.Page
    {
        private const string conString = "cirkus";
        public int anvandare;
        private int svarAntal = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            ButtonPrevious.Enabled = false;
            anvandare = Convert.ToInt32(Session["lcg_roll"]);

            if (!Page.IsPostBack)
            {
                IndividuellaResultat.Visible = false;

                if (Context.User.Identity.IsAuthenticated)
                {
                  
                    Welcome.Text = "Välkommen tillbaka till Kunskapsportalen: " + "Inloggad som: " + Context.User.Identity.Name;
                }
                else
                {
                   
                    Welcome.Text = "Lycka till på ditt första licenseringstest " + Context.User.Identity.Name;
                }

                if (anvandare == 1)
                {
                    ((Label)Master.FindControl("headertext")).Visible = true;
                    ((HyperLink)Master.FindControl("HyperLinkLicens")).Visible = true;
                }
                else if (anvandare == 2)
                {
                    ((Label)Master.FindControl("headertext")).Visible = true;
                    ((HyperLink)Master.FindControl("HyperLinkLicens")).Visible = true;
                    ((HyperLink)Master.FindControl("HyperLinkAdmin")).Visible = true;
                }

                int personid = GePersonId(GlobalValues.anvandarid);
                bool har_licens = Licencierad(personid);
                DateTime nasta_prov_tidigast;

                if (har_licens == false)
                {
                    if (BehorigForProv(personid, out nasta_prov_tidigast) == true)
                    {
                        GlobalValues.testtyp = "Licenseringstest";
                        GlobalValues.xmlfilename = "APP_CODE/XML_Query.xml";
                        loadXML(GlobalValues.xmlfilename, "/Licenseringstest");
                        ButtonStart.Text = "Starta licenseringstest";
                        LabelKategori.Text = "Välkommen att göra licenseringstestet";
                        LabelKategori.Visible = true;
                    }
                  else
                    {
                        ButtonStart.Visible = false;
                        LabelQuestion.Visible = true;
                        LabelInfo.Visible = true;
                        LabelQuestion.Text = "Du kan inte genomföra några test just nu. Välkommen åter!";
                        LabelInfo.Text = "Datum för nästa prov: " + nasta_prov_tidigast.ToShortDateString();
                    }
                }

                else if (har_licens == true)
                {
                    if (BehorigForProv(personid, out nasta_prov_tidigast) == true)
                    {
                        GlobalValues.testtyp = "Kunskapstest";
                        GlobalValues.xmlfilename = "APP_CODE/XML_QueryKunskap.xml";
                        loadXML(GlobalValues.xmlfilename, "/Kunskapstest");
                        ButtonStart.Text = "Starta kunskapsprov";
                        LabelKategori.Text = "Nu är det dags att göra kunskapsprov";
                        LabelKategori.Visible = true;
                    }
                    else
                    {
                        ButtonStart.Visible = false;
                        LabelQuestion.Visible = true;
                        LabelInfo.Visible = true;
                        LabelQuestion.Text = "Du kan inte genomföra några prov just nu. Välkommen åter!";
                        LabelInfo.Text = "Datum för nästa prov: " + nasta_prov_tidigast.ToShortDateString();
                    }
                }
            }
            else
            {
                //fråga & svar återskapas temporärt så att valda svar kan registreras
                loadQuestion();
            }
        }

        /// <summary>
        /// laddar in alla frågor och deras svar och lägger dem i en global lista
        /// svaren innehåller info om svaret är icheckat eller inte
        /// </summary>
        /// <param name="path"></param>
        /// <param name="level"></param>
        private void loadXML(string path, string level)
        {
            GlobalValues.Fragor.Clear();

            string xmlfil = Server.MapPath(path);
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfil);

            XmlNodeList noder = doc.SelectNodes(level + "/Question");

            foreach (XmlNode nod in noder)
            {
                Fraga fr = new Fraga
                {
                    id = Convert.ToInt32(nod.Attributes["id"].Value),
                    kategori = nod["Kategori"].InnerText,
                    fraga = nod["Fraga"].InnerText,
                    information = nod["Information"].InnerText,
                    flerVal = false
                };

                XmlNodeList subNoder = nod.ChildNodes;

                foreach (XmlNode subNod in subNoder)
                {
                    if (subNod.Name == "Svar")
                    {
                        Svar sv = new Svar
                        {
                            alt = subNod.Attributes["alt"].Value,
                            svar = subNod.InnerText,
                            facit = subNod.Attributes["facit"].Value,
                            icheckad = false
                        };
                        fr.svarLista.Add(sv);
                    }
                }

                //kontrollera om frågan är en flervalsfråga
                int rattSvar = 0;
                foreach (Svar sv in fr.svarLista)
                {
                    if (sv.facit == "true")
                    {
                        rattSvar += 1;
                    }
                }

                if (rattSvar > 1)
                {
                    fr.flerVal = true;
                }

                GlobalValues.Fragor.Add(fr);
            }
        }

        /// <summary>
        /// skapar ett dictionary som skickas som datasource till RepeaterQuestNav
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> createQuestNavData()
        {
            Dictionary<int, string> questNo = new Dictionary<int, string>();

            foreach (Fraga fr in GlobalValues.Fragor)
            {
                string selectStatus = "qNavUnselected";
                foreach (Svar sv in fr.svarLista)
                {
                    if (sv.icheckad)
                    {
                        selectStatus = "qNavSelected";
                    }
                }

                int index = GlobalValues.Fragor.IndexOf(fr) + 1;
                questNo.Add(index, selectStatus);
            }
            return questNo;
        }

        /// <summary>
        /// laddar in en fråga och dess svar från den globala frågelistan
        /// </summary>
        private void loadQuestion()
        {
            int index = GlobalValues.FrageNr - 1;
            Fraga question = GlobalValues.Fragor[index];

            LabelKategori.Text = question.kategori;
            LabelQuestion.Text = question.fraga;
            LabelInfo.Text = question.information;

            //generera radioknappar/checkboxar
            if (question.flerVal)
            {
                foreach (Svar sv in question.svarLista)
                {
                    CheckBox cb = new CheckBox();
                    cb.Text = "  " + sv.svar + "<br /><br />";
                    cb.ID = sv.alt + GlobalValues.FrageNr;
                    PanelSvar.Controls.Add(cb);
                }
            }
            else
            {
                foreach (Svar sv in question.svarLista)
                {
                    RadioButton rb = new RadioButton();
                    rb.Text = "  " + sv.svar + "<br /><br />";
                    rb.ID = sv.alt + GlobalValues.FrageNr;
                    rb.GroupName = "gr" + GlobalValues.FrageNr;
                    PanelSvar.Controls.Add(rb);
                }
            }
        }

        /// <summary>
        /// registrerar valda svarsalternativ i global lista
        /// </summary>
        private void registreraVal()
        {
            int index = GlobalValues.FrageNr - 1;
            Fraga question = GlobalValues.Fragor[index];

            foreach (Svar sv in question.svarLista)
            {
                //om radioknappar
                if (!question.flerVal)
                {
                    foreach (RadioButton rb in PanelSvar.Controls)
                    {
                        if (rb.ID == sv.alt + GlobalValues.FrageNr)
                        {
                            sv.icheckad = rb.Checked;
                        }
                    }
                }
                //om checkboxar
                else
                {
                    foreach (CheckBox cb in PanelSvar.Controls)
                    {
                        if (cb.ID == sv.alt + GlobalValues.FrageNr)
                        {
                            sv.icheckad = cb.Checked;
                        }
                    }
                }
            }
            //rensa bort radioknapparna
            PanelSvar.Controls.Clear();
        }

        /// <summary>
        /// bockar i tidigare valda svarsalternativ
        /// </summary>
        private void laddaVal()
        {
            int index = GlobalValues.FrageNr - 1;
            Fraga question = GlobalValues.Fragor[index];

            foreach (Svar sv in question.svarLista)
            {
                //om radioknappar
                if (!question.flerVal)
                {
                    foreach (RadioButton rb in PanelSvar.Controls)
                    {
                        if (rb.ID == sv.alt + GlobalValues.FrageNr)
                        {
                            rb.Checked = sv.icheckad;
                        }
                    }
                }
                //om checkboxar
                else
                {
                    foreach (CheckBox cb in PanelSvar.Controls)
                    {
                        if (cb.ID == sv.alt + GlobalValues.FrageNr)
                        {
                            cb.Checked = sv.icheckad;
                        }
                    }
                }
            }
        }

        private void taframknappar()
        {
            ButtonPrevious.Visible = true;
            ButtonNext.Visible = true;
            ButtonSparaProv.Visible = true;
            ButtonStart.Visible = false;
        }

        /// <summary>
        /// läser in fråga och svarsalternativ från XML-fil
        /// </summary>
        /// <param name="path"></param>
        /// <param name="level"></param>
        /// <param name="index"></param>
        /*private void XML(string path, string level, int index)
        {

            string xmlfil = Server.MapPath(path);
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfil);

            XmlNodeList fraga = doc.SelectNodes(level + "/Question[@id=" + index + "]");

            // Hämtar vissa info i element
            foreach (XmlNode nod in fraga)
            {
                LabelQuestion.Text = nod["Fraga"].InnerText + "<br /> ";
            }

            // Hämta noder utifrån namn
            XmlNodeList svar = doc.SelectNodes(level + "/Question[@id=" + index + "]/Svar");

            //kontrollera om frågan är en flervalsfråga
            int rattSvar = 0;
            foreach (XmlNode nod in svar)
            {
                if (nod.Attributes["facit"].Value == "true")
                {
                    rattSvar += 1;
                }
            }

            //generera radioknappar/checkboxar för svarsalternativ
            if (rattSvar > 1)
            {
                foreach (XmlNode nod in svar)
                {
                    CheckBox cb = new CheckBox();
                    cb.Text = nod.InnerText;
                    PanelSvar.Controls.Add(cb);
                    PanelSvar.Controls.Add(new LiteralControl("<br />"));
                }
            }
            else
            {
                foreach (XmlNode nod in svar)
                {
                    RadioButton rb = new RadioButton();
                    rb.Text = nod.InnerText;
                    rb.GroupName = "gr" + index;
                    PanelSvar.Controls.Add(rb);
                    PanelSvar.Controls.Add(new LiteralControl("<br />"));
                }
            }
        }

        private void visaXML()
        {
            string xmlfil = Server.MapPath("APP_CODE/XML_Query.xml");
            XmlTextReader reader = new XmlTextReader(xmlfil);
            StringBuilder str = new StringBuilder();

            reader.ReadStartElement("Test/Licenseringstest");

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        str.Append("Element: ");
                        str.Append(reader.Name);
                        str.Append("<br />");

                        if (reader.AttributeCount > 1)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                str.Append("Attributnamn: ");
                                str.Append(reader.Name);
                                str.Append(": ");
                                str.Append(reader.Value);
                                str.Append("<br />");
                            }

                        }
                        break;

                    case XmlNodeType.Text:
                        str.Append("Fråga ");
                        str.Append(reader.Value);
                        str.Append("<br />");
                        break;
                }
            }
            LabelQuestion.Text = str.ToString();
        }*/

        protected void ButtonStart_Click(object sender, EventArgs e)
        {
            GlobalValues.FrageNr = 1;
            PanelSvar.Controls.Clear();

            //rensa bort tidigare valda svar
            foreach (Fraga fr in GlobalValues.Fragor)
            {
                foreach (Svar sv in fr.svarLista)
                {
                    sv.icheckad = false;
                }
            }

            loadQuestion();
            updateQuestNav();
            taframknappar();
        }

        protected void Move(int maxNr)
        {
            if (GlobalValues.FrageNr == 1)
            {
                ButtonNext.Enabled = true;
                ButtonPrevious.Enabled = false;

                loadQuestion();
                laddaVal();
            }
            else if ((GlobalValues.FrageNr > 1) && (GlobalValues.FrageNr < maxNr))
            {
                ButtonNext.Enabled = true;
                ButtonPrevious.Enabled = true;

                loadQuestion();
                laddaVal();
            }
            else if (GlobalValues.FrageNr == maxNr)
            {
                ButtonNext.Enabled = false;
                ButtonPrevious.Enabled = true;

                loadQuestion();
                laddaVal();
            }

            updateQuestNav();
        }

        /// <summary>
        /// uppdaterar frågenavigeringsmenyn
        /// </summary>
        private void updateQuestNav()
        {
            //data till RepeaterQuestNav
            Dictionary<int, string> DSDictionary = createQuestNavData();
            RepeaterQuestNav.DataSource = DSDictionary;
            RepeaterQuestNav.DataBind();

            //finn den aktuella knappen och märk den som aktiv
            foreach (RepeaterItem item in RepeaterQuestNav.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    try
                    {
                        LinkButton lb = (LinkButton)item.FindControl("LinkButtonQuestNav");

                        if(lb.Text == GlobalValues.FrageNr.ToString())
                        {
                            lb.CssClass = "qNavActive";
                        }                        
                    }
                    catch
                    {

                    }
                }
            }
        }

        /// <summary>
        /// anropas när användare väljer fråga i frågenavigeringsmenyn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButtonQuestNav_Click(object sender, EventArgs e)
        {
            registreraVal();

            int maxNr = GetNodeCount();
            LinkButton lb = (LinkButton)sender;
            GlobalValues.FrageNr = Convert.ToInt32(lb.Text);
            Move(maxNr);
        }

        protected void ButtonNext_Click(object sender, EventArgs e)
        {
            registreraVal();

            int maxNr = GetNodeCount();
            GlobalValues.FrageNr += 1;
            Move(maxNr);
        }

        protected void ButtonPrevious_Click(object sender, EventArgs e)
        {
            registreraVal();

            int maxNr = GetNodeCount();
            GlobalValues.FrageNr -= 1;
            Move(maxNr);
        }

        private int GetNodeCount()
        {
            string node = "/" + GlobalValues.testtyp + "/Question"; 
            string xmlfil = Server.MapPath(GlobalValues.xmlfilename);
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfil);

            XmlNodeList nodeList = doc.SelectNodes(node);
            return nodeList.Count;
        }

        public void SparaProvtillfalle()
        {
            registreraVal();
            Provtillfalle provtillfalle = new Provtillfalle();
            provtillfalle.Datum = DateTime.Now;
            provtillfalle.Typ_av_test = GlobalValues.testtyp; 
            provtillfalle.Anvandar_id = GlobalValues.anvandarid; 
            nyProvtillfalle(provtillfalle);
        }

        /// <summary>
        /// Metod som används för att skapa ett ny Provtillfalle
        /// </summary>
        /// <param name="provtillfalle"></param>
        /// <returns></returns>
        public void nyProvtillfalle(Provtillfalle provtillfalle)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            NpgsqlTransaction tran = null;

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                command.Connection = conn;
                command.Transaction = tran;

                string plsql = string.Empty;
                plsql = plsql + "INSERT INTO lcg_provtillfalle (datum, typ_av_test, fk_person_id)";
                plsql = plsql + "VALUES (:newDatum, :newTypAvTest, (SELECT fk_person_id FROM lcg_konto WHERE id = :newAnvandarId))";
                plsql = plsql + " RETURNING id";

                command.Parameters.Add(new NpgsqlParameter("newDatum", NpgsqlDbType.Timestamp));
                command.Parameters["newDatum"].Value = provtillfalle.Datum;
                command.Parameters.Add(new NpgsqlParameter("newTypAvTest", NpgsqlDbType.Varchar));
                command.Parameters["newTypAvTest"].Value = provtillfalle.Typ_av_test;
                command.Parameters.Add(new NpgsqlParameter("newAnvandarId", NpgsqlDbType.Integer));
                command.Parameters["newAnvandarId"].Value = provtillfalle.Anvandar_id;
                
                command.CommandText = plsql;
                int provtillfalleid = Convert.ToInt32(command.ExecuteScalar());                
                
                int dbfragaid = 0;
                int dbsvarid = 0;

                foreach (Fraga nyfraga in GlobalValues.Fragor)
	            {
                    dbfragaid = 0;
                    plsql = string.Empty;
                    plsql = plsql + "INSERT INTO lcg_fragor (fraga_id, fraga, information, flerval, kategori, fk_provtillfalle_id)";
                    plsql = plsql + " VALUES (:newFragaId, :newFraga, :newInformation, :newFlerval, :newKategori, :newProvtillfalleId)";
                    plsql = plsql + " RETURNING id";

                    command.Parameters.Add(new NpgsqlParameter("newFragaId", NpgsqlDbType.Integer));
                    command.Parameters["newFragaId"].Value = nyfraga.id;
                    command.Parameters.Add(new NpgsqlParameter("newFraga", NpgsqlDbType.Varchar));
                    command.Parameters["newFraga"].Value = nyfraga.fraga;
                    command.Parameters.Add(new NpgsqlParameter("newInformation", NpgsqlDbType.Varchar));
                    command.Parameters["newInformation"].Value = nyfraga.information;
                    command.Parameters.Add(new NpgsqlParameter("newFlerval", NpgsqlDbType.Boolean));
                    command.Parameters["newFlerval"].Value = nyfraga.flerVal;
                    command.Parameters.Add(new NpgsqlParameter("newKategori", NpgsqlDbType.Varchar));
                    command.Parameters["newKategori"].Value = nyfraga.kategori;
                    command.Parameters.Add(new NpgsqlParameter("newProvtillfalleId", NpgsqlDbType.Integer));
                    command.Parameters["newProvtillfalleId"].Value = provtillfalleid;

                    command.CommandText = plsql;
                    dbfragaid = Convert.ToInt32(command.ExecuteScalar());

                    foreach (Svar nysvar in nyfraga.svarLista)
                    {
                        dbsvarid = 0;
                        plsql = string.Empty;
                        plsql = plsql + "INSERT INTO lcg_svar (svar, alt, facit, icheckad, fk_fraga_id)";
                        plsql = plsql + " VALUES (:newSvar, :newAlt, :newFacit, :newIcheckad, :newFkFragaId)";
                        plsql = plsql + " RETURNING id";

                        command.Parameters.Add(new NpgsqlParameter("newSvar", NpgsqlDbType.Varchar));
                        command.Parameters["newSvar"].Value = nysvar.svar;
                        command.Parameters.Add(new NpgsqlParameter("newAlt", NpgsqlDbType.Varchar));
                        command.Parameters["newAlt"].Value = nysvar.alt;
                        command.Parameters.Add(new NpgsqlParameter("newFacit", NpgsqlDbType.Varchar));
                        command.Parameters["newFacit"].Value = nysvar.facit;
                        command.Parameters.Add(new NpgsqlParameter("newIcheckad", NpgsqlDbType.Boolean));
                        command.Parameters["newIcheckad"].Value = nysvar.icheckad;
                        command.Parameters.Add(new NpgsqlParameter("newFkFragaId", NpgsqlDbType.Integer));
                        command.Parameters["newFkFragaId"].Value = dbfragaid;
                        command.CommandText = plsql;
                        dbsvarid = Convert.ToInt32(command.ExecuteScalar());
                    }
                    dbsvarid = 0;
                }
                dbfragaid = 0;
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }

        protected void ButtonSparaProv_Click(object sender, EventArgs e)
        {
            SparaProvtillfalle();
            laddaResultat();
            main.Visible = false;
            IndividuellaResultat.Visible = true;
        }

        /// <summary>
        /// Metoden använd för att hämta anvandarid med hjälp av anvandarnamn (i samband med inloggning). 
        /// Anvandarid används för person identifiering och styrning. Bland annat används detta för att 
        /// bestämma vilken typ av test användaren bör göra - ha tillgång till.
        /// </summary>
        /// <param name="anvandarnamn"></param>
        /// <returns></returns>
        public static int GeAnvandarId(string anvandarnamn)
        {
            int anvandarid = 0;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT id FROM lcg_konto WHERE anvandarnamn = :anvandarnamn;";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("anvandarnamn", NpgsqlDbType.Varchar));
                command.Parameters["anvandarnamn"].Value = anvandarnamn;
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    anvandarid = (int)(dr["id"]);
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
            return anvandarid;
        }

        /// <summary>
        /// Returnerar personid för respektive användare
        /// </summary>
        /// <param name="anvandarid"></param>
        /// <returns></returns>
        public static int GePersonId(int anvandarid)
        {
            int personid = 0;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT fk_person_id FROM lcg_konto WHERE id = :anvandarid;";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);

                command.Parameters.Add(new NpgsqlParameter("anvandarid", NpgsqlDbType.Integer));
                command.Parameters["anvandarid"].Value = anvandarid;

                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    personid = Convert.ToInt32(dr["fk_person_id"]);
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
            return personid;
        }

        /// <summary>
        /// Returnerar true om användare är licencierad annars false 
        /// </summary>
        /// <param name="personid"></param>
        /// <returns></returns>
        public static bool Licencierad(int personid)
        {
            bool licencierad = false;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT har_licens AS licencierad FROM lcg_person WHERE id = :personid";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("personid", NpgsqlDbType.Integer));
                command.Parameters["personid"].Value = personid;
                NpgsqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {   
                    licencierad = dr["licencierad"] != DBNull.Value ? (bool)(dr["licencierad"]) : false;
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
            return licencierad;
        }

        /// <summary>
        /// Metoden ta reda på en person är behörig för en prov. Används för synliggöra knappen  
        /// </summary>
        /// <param name="personid"></param>
        /// <returns></returns>
        public static bool BehorigForProv(int personid, out DateTime nasta_prov_tidigast)
        {
            bool behorig = true;
            nasta_prov_tidigast = DateTime.Today;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "";
                sql = sql + "SELECT lcg_person.id AS person_id, ";
                sql = sql + "       lcg_person.namn AS namn, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN 'Kunskapstest' ";
                sql = sql + "            ELSE 'Licenseringstest' END AS provtyp, ";
                sql = sql + "       CASE WHEN lcg_person.har_licens = TRUE THEN (lcg_provtillfalle.datum + interval '365 day') ::timestamp::date ";
                sql = sql + "            WHEN lcg_provtillfalle.godkand = FALSE THEN (lcg_provtillfalle.datum + '7 DAYS') ::timestamp::date " ;
                sql = sql + "            ELSE NULL END AS nasta_prov_tidigast " ;
                sql = sql + "FROM lcg_person ";
                sql = sql + "     LEFT JOIN lcg_roll AS lcg_roll ON lcg_roll.id = lcg_person.fk_roll_id ";
                sql = sql + "     LEFT JOIN lcg_provtillfalle AS lcg_provtillfalle ON lcg_provtillfalle.fk_person_id = lcg_person.id ";
                sql = sql + "WHERE lcg_person.id = :personid ";
                sql = sql + "AND ( lcg_provtillfalle.DATUM = (SELECT MAX(c.DATUM) ";
                sql = sql + "                                    FROM lcg_provtillfalle c ";
                sql = sql + "                                     WHERE c.fk_person_id = lcg_provtillfalle.fk_person_id) ";
                sql = sql + "      OR lcg_provtillfalle.DATUM IS NULL) ";
                sql = sql + "ORDER BY lcg_provtillfalle.datum ASC; ";
        
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);

                command.Parameters.Add(new NpgsqlParameter("personid", NpgsqlDbType.Integer));
                command.Parameters["personid"].Value = personid;

                DateTime idag = DateTime.Today;
                //DateTime nasta_prov_tidigast = idag;
                
                
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    // personid = (int)(dr["id"]);
                    // namn = (string)(dr["namn"]);
                    // provtyp = (string)(dr["provtyp"]);
                    nasta_prov_tidigast = dr["nasta_prov_tidigast"] != DBNull.Value ?  Convert.ToDateTime(dr["nasta_prov_tidigast"]) : DateTime.MinValue;
                }

                if (nasta_prov_tidigast > idag)
                {
                    behorig = false;
                }
                else
                {
                    behorig = true;
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
            return behorig;
        }

        public static DateTime GeSistaProvDatum(int personid)
        {
            // för att inet returnera inget värde alls 
            DateTime sistaprovdatum = Convert.ToDateTime("1000-01-01 19:11:11.80779");
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT datum FROM lcg_provtillfalle WHERE fk_person_id = :personid";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);

                command.Parameters.Add(new NpgsqlParameter("personid", NpgsqlDbType.Integer));
                command.Parameters["personid"].Value = personid;

                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    sistaprovdatum = dr["datum"] != DBNull.Value ? Convert.ToDateTime(dr["datum"]) : DateTime.MinValue;
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
            return sistaprovdatum;
        }

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

        protected void CheckBoxSvarText_CheckedChanged(object sender, EventArgs e)
        {
            LabelIndResKategori1.Text = "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi";
            LabelIndResKategori2.Text = "Produkter och hantering av kundens affärer";
            LabelIndResKategori3.Text = "Etik och regelverk";
            fyllGridViewIndividResultat(GridViewIndividResultat1, "Ekonomi – nationalekonomi, finansiell ekonomi och privatekonomi");
            fyllGridViewIndividResultat(GridViewIndividResultat2, "Produkter och hantering av kundens affärer ");
            fyllGridViewIndividResultat(GridViewIndividResultat3, "Etik och regelverk");
        }

        protected void laddaResultat()
        {
            int personId = GePersonId(GlobalValues.anvandarid);

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

            LabelIndResNamn.Text = Context.User.Identity.Name;
            DateTime datum = new DateTime();
            //LabelIndResDatum.Text = Convert.ToString(datum.Date.ToLocalTime());
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
                    nyProvtillfalle.Provresultat = dr["provresultat"] != DBNull.Value ? (int)(dr["provresultat"]) : 0;
                    nyProvtillfalle.Godkand = dr["godkand"] != DBNull.Value ? (bool)(dr["godkand"]) : false;
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
            if (Convert.ToInt32(prov.Antal_ratt) >= 70)
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
        /// Hämtar prov datat för respektive person och provtillfälle fördelat på de olika fråge kategorier. 
        /// </summary>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public static List<Provstatistik> GeStatistikPerKategori(int person_id)
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
                    nyProv.Namn = dr["namn"] != DBNull.Value ? (string)(dr["namn"]) : "";
                    nyProv.Provtillfalle_id = dr["provtillfalle_id"] != DBNull.Value ? (int)(dr["provtillfalle_id"]) : 0;
                    nyProv.Datum = dr["datum"] != DBNull.Value ? (DateTime)(dr["datum"]) : DateTime.MinValue;
                    nyProv.Typ_av_test = dr["typ_av_test"] != DBNull.Value ? (string)(dr["typ_av_test"]) : "";
                    nyProv.Kategori = dr["kategori"] != DBNull.Value ? (string)(dr["kategori"]) : "";
                    nyProv.Antal_fragor = dr["antal_fragor"] != DBNull.Value ? Convert.ToInt32(dr["antal_fragor"]) : 0;
                    nyProv.Antal_poang = dr["antal_poang"] != DBNull.Value ? Convert.ToInt32(dr["antal_poang"]) : 0;
                    nyProv.Antal_ratt = dr["antal_ratt"] != DBNull.Value ? Convert.ToDouble(dr["antal_ratt"]) : 0.00;
                    listaPerKategori.Add(nyProv);
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
            return listaPerKategori;
        }
        /// <summary>
        /// Hämtar prov datat för respektive person och provtillfälle. 
        /// </summary>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public static List<Provstatistik> GeStatistikPerProv(int person_id)
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
                    nyProv.Namn = dr["namn"] != DBNull.Value ? (string)(dr["namn"]) : "";
                    nyProv.Provtillfalle_id = dr["provtillfalle_id"] != DBNull.Value ? (int)(dr["provtillfalle_id"]) : 0;
                    nyProv.Datum = dr["datum"] != DBNull.Value ? (DateTime)(dr["datum"]) : DateTime.MinValue;
                    nyProv.Typ_av_test = dr["typ_av_test"] != DBNull.Value ? (string)(dr["typ_av_test"]) : "";
                    nyProv.Antal_fragor = dr["antal_fragor"] != DBNull.Value ? Convert.ToInt32(dr["antal_fragor"]) : 0;
                    nyProv.Antal_poang = dr["antal_poang"] != DBNull.Value ? Convert.ToInt32(dr["antal_poang"]) : 0;
                    nyProv.Antal_ratt = dr["antal_ratt"] != DBNull.Value ? Convert.ToDouble(dr["antal_ratt"]) : 0.00;
                    listaPerProv.Add(nyProv);
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
            return listaPerProv;
        }
    }
}

