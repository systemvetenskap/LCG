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
        }

        void Signout_Click(object sender, EventArgs e)
         {
            FormsAuthentication.SignOut();
            Response.Redirect("Login.aspx");
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

        protected void TextBoxAnvId_TextChanged(object sender, EventArgs e)
        {

        }

        protected void ButtonLogga_Click(object sender, EventArgs e)
        {
            if (AuthenticateUser(TextBoxAnvId.Text, TextBoxLosen.Text))
            {
                int behorighet = Convert.ToInt32(Session["lcg_roll"]);
                
                if (behorighet == 1)
                {
                    FormsAuthentication.RedirectFromLoginPage(TextBoxAnvId.Text, true);
                    Response.Redirect("Licensiering.aspx");
                }
                else if (behorighet == 2)
                {
                    FormsAuthentication.RedirectFromLoginPage(TextBoxAnvId.Text, true);
                    Response.Redirect("Admin.aspx");
                }
                else
                {
                    Msg.Text = "Fel inloggningsuppgifter. Snälla, skriv rätt.";
                }
            }
           }
       }
}
            
    
    