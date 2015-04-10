using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Globosat.Remuneracao.CustomPages.Layouts.Globosat.Remuneracao.CustomPages
{
    public partial class ItemException : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["msg"] != null)
            {
                lblErro.Text = this.Request.QueryString["msg"];
            }
        }
    }
}
