using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Globosat.Library.Servicos;
using CIT.Sharepoint.Util;
using Microsoft.SharePoint.Utilities;
using System.Diagnostics;
using System.Data;
using Globosat.Library.Entidades;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.Threading;
using System.Globalization;

namespace Globosat.Remuneracao.CustomPages.Layouts.Globosat.Remuneracao.CustomPages
{
    public partial class AlteraSalarioProposto : LayoutsPageBase
    {
        protected string tipo = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            HttpContext context = HttpContext.Current;
            if (context.Request.QueryString["TIPO"] != null)
            {
                tipo = context.Request.QueryString["TIPO"];
            }
        }       

        private bool PossuiAcessoTotal(string login)
        {
            bool possuiAcesso = false;

            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {
                using (var web = site.AllWebs["remuneracoes"])
                {
                    SPUser userLogado = SPContext.Current.Web.CurrentUser;
                    SPGroup grupoAdministrador = web.Groups["Grupo_Remuneração_Administradores"];

                    foreach (SPUser administrador in grupoAdministrador.Users)
                    {
                        if (administrador.LoginName.Equals(login))
                            possuiAcesso = true;
                    }
                }
            }
            return possuiAcesso;
        }
        

        // Trims the user name string by removing the ;#.
        private string GetUserName(string spusername)
        {
            int index = spusername.LastIndexOf("#");
            if (index != -1)
            {
                // Remove the id and # from the username string.
                spusername = spusername.Remove(0, index + 1); //"9;#UserName" 
            }
            return spusername;
        }
        protected void Confirmar_Onclick(object sender, EventArgs e)
        {            
            string salario = "R$ " + SearchBox.Text;
            Dialogvalue.Text = string.Format("{1}{0}", ";", salario);

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ClosePopupScript", "SP.UI.ModalDialog.commonModalDialogClose(1, '" + Dialogvalue.Text + "');", true);
            
        }
    }
}
