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

        }

        private void XML(string path, string level, int index)
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
            XmlNodeList svar = doc.SelectNodes(level + "/Question[@id=" + index + "]");
            // Hämtar vissa info i element
            foreach (XmlNode nod in svar)
            {
                Label1.Text = nod["Svarsalternativ1"].InnerText;
                Label2.Text = nod["Svarsalternativ2"].InnerText;
                Label3.Text = nod["Svarsalternativ3"].InnerText;
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
        }

        protected void ButtonStart_Click(object sender, EventArgs e)
        {

            XML("APP_CODE/XML_Query.xml", "/Licenseringstest", 1);
        }

        protected void Move(int maxNr, int nr)
        {
            if (GlobalValues.FrageNr == 1)
            {
                ButtonNext.Enabled = true;
                ButtonPrevious.Enabled = false;
                XML("APP_CODE/XML_Query.xml", "/Licenseringstest", nr);
            }
            else if ((GlobalValues.FrageNr > 1) && (GlobalValues.FrageNr < maxNr))
            {
                ButtonNext.Enabled = true;
                ButtonPrevious.Enabled = true;
                XML("APP_CODE/XML_Query.xml", "/Licenseringstest", nr);
            }
            else if (GlobalValues.FrageNr == maxNr)
            {
                ButtonNext.Enabled = false;
                ButtonPrevious.Enabled = true;
                XML("APP_CODE/XML_Query.xml", "/Licenseringstest", nr);
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

