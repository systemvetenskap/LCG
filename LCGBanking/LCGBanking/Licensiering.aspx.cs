﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Web.UI.WebControls;
using System.Text;

namespace LCGBanking
{
    public partial class Licensiering : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ButtonPrevious.Enabled = false;
            loadAnswers("APP_CODE/XML_Query.xml", "/Licenseringstest");
        }

        /// <summary>
        /// laddar in alla frågor och deras svar och lägger dem i en global lista
        /// svaren innehåller info om svaret är icheckat eller inte
        /// </summary>
        /// <param name="path"></param>
        /// <param name="level"></param>
        private void loadAnswers(string path, string level)
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
                    fraga = nod["Fraga"].InnerText
                };

                XmlNodeList subNoder = nod.ChildNodes;

                foreach (XmlNode subNod in subNoder)
                {
                    if (subNod.Name == "Svarsalternativ")
                    {
                        Svar sv = new Svar
                        {
                            svar = subNod.InnerText,
                            icheckad = false
                        };
                        fr.svarLista.Add(sv);
                    }
                }
                GlobalValues.Fragor.Add(fr);
            }

            //LabelQuestion.Text = GlobalValues.Fragor[1].svarLista.Count.ToString();
        }

        /// <summary>
        /// läser in fråga och svarsalternativ från XML-fil
        /// </summary>
        /// <param name="path"></param>
        /// <param name="level"></param>
        /// <param name="index"></param>
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
            XmlNodeList svar = doc.SelectNodes(level + "/Question[@id=" + index + "]/Svarsalternativ");

            //kontrollera om frågan är en flervalsfråga
            int rattSvar = 0;
            foreach (XmlNode nod in svar)
            {
                if(nod.Attributes["facit"].Value == "true")
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
    }
}

