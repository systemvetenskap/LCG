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
        
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                LabelPost.Text = "Välkommen tillbaka";
            else
                LabelPost.Text = "Du har kommit till JE Bankens Kunskapsportal";
            }
                
        }
    }
