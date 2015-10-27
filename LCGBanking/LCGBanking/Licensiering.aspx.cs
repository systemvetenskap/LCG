using System;
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
           

        }

        private void XML(int index)
        {
            
                string xmlfil = Server.MapPath("APP_CODE/XML_Query.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfil);

                XmlNodeList fraga = doc.SelectNodes("/Licenseringstest/Question[@id="+ index +"]");

                // Hämtar vissa info i element
                foreach (XmlNode nod in fraga)
                {
                    LabelQuestion.Text = nod["Fraga"].InnerText + "<br /> ";
                }

                // Hämta noder utifrån namn
                XmlNodeList svar = doc.SelectNodes("/Licenseringstest/Question[@id=" + index + "]");
                // Hämtar vissa info i element
	            foreach(XmlNode nod in svar)
	            {
                    Label1.Text = nod["Svarsalternativ1"].InnerText + "<br /> ";
                    Label2.Text = nod["Svarsalternativ2"].InnerText + "<br /> ";
                    Label3.Text = nod["Svarsalternativ3"].InnerText + "<br /> ";
                }
        }

   

        /*
// Hämta noder utifrån attribut
xmlNodeList musikintrument = doc.SelectNodes(”/musikinstrument/instrumen”);
// Hämtar vissa info i element
foreach(XmlNode nod in musikinstrument)
{
Label1.Text += nod[”namn”].InnerText + ” ";
 }
       */ 
        
        
        
    

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
        XML(2);
    }
    
    }
}
