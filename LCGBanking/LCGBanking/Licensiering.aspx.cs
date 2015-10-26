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
            visaXML();

        }
    

    private void visaXML()
    {
        string xmlfil = Server.MapPath("APP_CODE/XML_Query.xml");
        XmlTextReader reader = new XmlTextReader(xmlfil);
        StringBuilder str = new StringBuilder();

        reader.ReadStartElement("Questions");

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
        Label1.Text = str.ToString();
    }
    
    }
}
