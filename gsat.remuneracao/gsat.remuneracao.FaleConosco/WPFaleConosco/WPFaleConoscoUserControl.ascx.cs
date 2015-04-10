using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using CIT.Sharepoint.Util;
using Microsoft.SharePoint.Utilities;
namespace Globosat.Remuneracao.FaleConosco.WPFaleConosco
{
    public partial class WPFaleConoscoUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void btnEnviar_Click(object sender, EventArgs e)
        {
            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {

                using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPUser userLogado = SPContext.Current.Web.CurrentUser;
                    SPGroup grupoAdministrador = web.Groups["Grupo_Remuneração_Administradores"];

                    foreach (SPUser administrador in grupoAdministrador.Users)
                    {
                        Email.EnvioEmail(administrador.Email, "Fale Conosco Remuneração", String.Format("O usuário {0} ({1}) enviou o seguinte comentário: {2}", userLogado.LoginName, userLogado.Email, tbComentario.Text));
                    }
                }
            }
            SPUtility.TransferToSuccessPage("Email enviado com sucesso!", "/", "", "");
        }
    }
}
