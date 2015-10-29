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

        // string connectionString = "Server=webblabb.miun.se;Port=5432;Database=pgmvaru_g1;User Id=pgmvaru_g1;Password=enhjuling;SSL=true";
        private const string connectionString = "Server=webblabb.miun.se;Port=5432;Database=pgmvaru_g1;User Id=pgmvaru_g1;Password=enhjuling;SSL=true";
        private const string conString = "cirkus";
        // Suad start

        protected void Page_Load(object sender, EventArgs e)
        {
            ButtonPrevious.Enabled = false;
            loadXML("APP_CODE/XML_Query.xml", "/Licenseringstest");
        }

        /// <summary>
        /// laddar in alla frågor och deras svar och lägger dem i en global lista
        /// svaren innehåller info om svaret är icheckat eller inte
        /// </summary>
        /// <param name="path"></param>
        /// <param name="level"></param>
        private void loadXML(string path, string level)
        {
            //!postback etc., så metoden bara kallas en gång
            
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
        /// laddar in en fråga och dess svar från den globala frågelistan
        /// </summary>
        private void loadQuestion()
        {
            int index = GlobalValues.FrageNr - 1;
            Fraga question = GlobalValues.Fragor[index];

            LabelKategori.Text = "Kategori: " + question.kategori;
            LabelQuestion.Text = question.fraga;

            ////kontrollera om frågan är en flervalsfråga
            //int rattSvar = 0;
            //foreach (Svar sv in question.svarLista)
            //{
            //    if (sv.facit == "true")
            //    {
            //        rattSvar += 1;
            //    }
            //}

            //if (rattSvar > 1)
            //{
            //    question.flerVal = true;
            //}

            //generera radioknappar/checkboxar
            if (question.flerVal)
            {
                foreach (Svar sv in question.svarLista)
                {
                    CheckBox cb = new CheckBox();
                    cb.Text = sv.svar;
                    cb.ID = sv.alt;
                    PanelSvar.Controls.Add(cb);
                    PanelSvar.Controls.Add(new LiteralControl("<br />"));
                }
            }
            else
            {
                foreach (Svar sv in question.svarLista)
                {
                    RadioButton rb = new RadioButton();
                    rb.Text = sv.svar;
                    rb.ID = sv.alt;
                    rb.GroupName = "gr" + index;
                    PanelSvar.Controls.Add(rb);
                    PanelSvar.Controls.Add(new LiteralControl("<br />"));
                }
            }
        }

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
                        if (rb.ID == sv.alt)
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
                        if (cb.ID == sv.alt)
                        {
                            sv.icheckad = cb.Checked;
                        }
                    }
                }
            }
        }

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
                        if (rb.ID == sv.alt)
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
                        if (cb.ID == sv.alt)
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
            //XML("APP_CODE/XML_Query.xml", "/Licenseringstest", 1);
            GlobalValues.FrageNr = 1;
            loadQuestion();
        }

        protected void Move(int maxNr, int nr)
        {
            if (GlobalValues.FrageNr == 1)
            {
                ButtonNext.Enabled = true;
                ButtonPrevious.Enabled = false;
                //XML("APP_CODE/XML_Query.xml", "/Licenseringstest", nr);
                //registreraVal();
                loadQuestion();
                //laddaVal();
            }
            else if ((GlobalValues.FrageNr > 1) && (GlobalValues.FrageNr < maxNr))
            {
                ButtonNext.Enabled = true;
                ButtonPrevious.Enabled = true;
                //XML("APP_CODE/XML_Query.xml", "/Licenseringstest", nr);
                //registreraVal();
                loadQuestion();
                //laddaVal();
            }
            else if (GlobalValues.FrageNr == maxNr)
            {
                ButtonNext.Enabled = false;
                ButtonPrevious.Enabled = true;
                //XML("APP_CODE/XML_Query.xml", "/Licenseringstest", nr);
                //registreraVal();
                loadQuestion();
                //laddaVal();
            }        
        }

        protected void ButtonNext_Click(object sender, EventArgs e)
        {
            int maxNr = GetNodeCount();
            int nr = GlobalValues.FrageNr += 1;
            Move(maxNr, nr);
        }

        protected void ButtonPrevious_Click(object sender, EventArgs e)
        {
            int maxNr = GetNodeCount();
            int nr = GlobalValues.FrageNr -= 1;
            Move(maxNr, nr);
        }

        private int GetNodeCount(string node = "/Licenseringstest/Question")
        {
            string xmlfil = Server.MapPath("APP_CODE/XML_Query.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfil);

            XmlNodeList nodeList = doc.SelectNodes(node);
            return nodeList.Count;
        }

        public void SparaProvtillfalle()
        {
            Lcg_provtillfalle lcg_provtillfalle = new Lcg_provtillfalle();
            lcg_provtillfalle.Datum = DateTime.Now;
            lcg_provtillfalle.Typ_av_test = "Licenseringstest"; // OBS! Hårdkodat värde än så länge. Måste korrigeras.
            lcg_provtillfalle.Anvandarnamn = "johan1400"; // OBS! Hårdkodat värde än så länge. Måste korrigeras.  
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
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlTransaction tran = null;
            int id = 0;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                
                string plsql = "";
                plsql = plsql + "INSERT INTO lcg_provtillfalle (datum, typ_av_test, fk_person_id)";
                plsql = plsql + "VALUES (:newDatum, :newTypAvTest, (SELECT fk_person_id FROM lcg_konto WHERE anvandarnamn = :newAnvandarnamn))";
                plsql = plsql + " RETURNING id";

                NpgsqlCommand command = new NpgsqlCommand(@plsql, conn);

                command.Parameters.Add(new NpgsqlParameter("newDatum", NpgsqlDbType.Timestamp));
                command.Parameters["newDatum"].Value = lcg_provtillfalle.Datum;
                command.Parameters.Add(new NpgsqlParameter("newTypAvTest", NpgsqlDbType.Varchar));
                command.Parameters["newTypAvTest"].Value = lcg_provtillfalle.Typ_av_test;
                command.Parameters.Add(new NpgsqlParameter("newAnvandarnamn", NpgsqlDbType.Varchar));
                command.Parameters["newAnvandarnamn"].Value = lcg_provtillfalle.Anvandarnamn;
                
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

    }
}

