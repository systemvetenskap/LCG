using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Npgsql;
using System.Configuration;

namespace LCGBanking
{
    public partial class Login : System.Web.UI.Page
    {
        private const string conString = "cirkus";
        public int anvandare;

        protected void Page_Load(object sender, EventArgs e)
        {
            anvandare = Convert.ToInt32(Session["lcg_roll"]);

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
        }

        private bool AuthenticateUser(string anvNamn, string anvLosen)
        {
            Session["id"] = -1;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);

            try
            {
                conn.Open();
                string sql = "SELECT lcg_konto.id, fk_roll_id FROM lcg_konto, lcg_person WHERE  anvandarnamn = '" + anvNamn + "' AND losenord = '" + anvLosen + "' AND fk_person_id = lcg_person.id";

                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                NpgsqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    Session["id"] = (int)datareader["id"];
                    Session["lcg_roll"] = (int)datareader["fk_roll_id"];
                }
            }

            finally
            {
                conn.Close();
            }

            int id = Convert.ToInt32(Session["id"]);
            GlobalValues.anvandarid = id;
            if (id != -1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        protected void ButtonLogga_Click(object sender, EventArgs e)
        {
            if (AuthenticateUser(TextBoxAnvId.Text, TextBoxLosen.Text))
            {
                int anvandare = Convert.ToInt32(Session["lcg_roll"]);

                if (anvandare == 1)
                {
                    FormsAuthentication.RedirectFromLoginPage(anvandare.ToString(), false);
                    Response.Redirect("Licensiering.aspx");
                }

                else if (anvandare == 2)
                {
                    FormsAuthentication.RedirectFromLoginPage(anvandare.ToString(), false);
                    Response.Redirect("/Admin/Admin.aspx");
                }
            }
            else
            {
                Msg.Text = "Fel inloggningsuppgifter. Snälla, skriv rätt.";
            }
        }
    }
}