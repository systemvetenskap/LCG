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

namespace LCGBanking
{
    public partial class Licensiering : System.Web.UI.Page
    {
        private const string conString = "cirkus";

        protected void Page_Load(object sender, EventArgs e)
        {
            Welcome.Text = "Tjenare, din gamle indianbanker, " + Context.User.Identity.Name;
            int behorighet = Convert.ToInt32(Session["lcg_roll"]);

            ButtonPrevious.Enabled = false;
            if (!Page.IsPostBack)
            {
                if (behorighet == 3)
                {
                    GlobalValues.testtyp = "Licenseringstest";
                    GlobalValues.xmlfilename = "APP_CODE/XML_Query.xml";
                    loadXML(GlobalValues.xmlfilename, "/Licenseringstest");
                    
                }
                else if (behorighet == 1)
                {
                    GlobalValues.testtyp = "Kunskapstest";
                    GlobalValues.xmlfilename = "APP_CODE/XML_QueryKunskap.xml";
                    loadXML(GlobalValues.xmlfilename, "/Kunskapstest");
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
            Lcg_provtillfalle lcg_provtillfalle = new Lcg_provtillfalle();
            lcg_provtillfalle.Datum = DateTime.Now;
            lcg_provtillfalle.Typ_av_test = GlobalValues.testtyp; 
            lcg_provtillfalle.AnvandarId = GlobalValues.anvandarid; 
            nyProvtillfalle(lcg_provtillfalle);
        }

        /// <summary>
        /// Metod som används för att skapa ett ny Provtillfalle
        /// </summary>
        /// <param name="lcg_provtillfalle"></param>
        /// <returns></returns>
        public static int nyProvtillfalle(Lcg_provtillfalle lcg_provtillfalle)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            NpgsqlTransaction tran = null;
            int id = 0;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                string plsql = "";
                plsql = plsql + "INSERT INTO lcg_provtillfalle (datum, typ_av_test, fk_person_id)";
                plsql = plsql + "VALUES (:newDatum, :newTypAvTest, (SELECT fk_person_id FROM lcg_konto WHERE id = :newAnvandarId))";
                plsql = plsql + " RETURNING id";

                NpgsqlCommand command = new NpgsqlCommand(@plsql, conn);

                command.Parameters.Add(new NpgsqlParameter("newDatum", NpgsqlDbType.Timestamp));
                command.Parameters["newDatum"].Value = lcg_provtillfalle.Datum;
                command.Parameters.Add(new NpgsqlParameter("newTypAvTest", NpgsqlDbType.Varchar));
                command.Parameters["newTypAvTest"].Value = lcg_provtillfalle.Typ_av_test;
                command.Parameters.Add(new NpgsqlParameter("newAnvandarId", NpgsqlDbType.Integer));
                command.Parameters["newAnvandarId"].Value = lcg_provtillfalle.AnvandarId;

                Convert.ToInt32(command.ExecuteScalar());

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
            return id;
        }

        protected void ButtonSparaProv_Click(object sender, EventArgs e)
        {
            SparaProvtillfalle();
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
                    anvandarid = Convert.ToInt32(dr["id"]);
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
                //MessageBox.Show("Ett fel uppstod:\n" + ex.Message); OBS! Lämplig medellande?
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
                    licencierad = (bool)(dr["licencierad"]);
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
                    sistaprovdatum = (DateTime)dr["datum"];
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
    }
}

