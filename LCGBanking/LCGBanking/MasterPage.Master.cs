using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LCGBanking
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        private const string conString = "cirkus";
        public int anvandare;

        protected void Page_Load(object sender, EventArgs e)
        {
            anvandare = Convert.ToInt32(Session["lcg_roll"]);

            if (anvandare == 1)
                {
                    headertext.Visible = true;
                    HyperLinkLicens.Visible = true;
                }
                else if (anvandare == 2)
                {
                    headertext.Visible = true;
                    HyperLinkLicens.Visible = true;
                    HyperLinkAdmin.Visible = true;
                }


                /*
                ((Label)Master.FindControl("headertext")).Visible = false;
                ((Label)Master.FindControl("HyperLinkLicens")).Visible = false;
                ((Label)Master.FindControl("HyperLinkAdmin")).Visible = false;
                 */
            }
                
        }
    }
