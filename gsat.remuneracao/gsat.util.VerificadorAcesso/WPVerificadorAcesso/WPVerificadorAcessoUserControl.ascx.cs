using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Data;
using CIT.Sharepoint.Util;
using Microsoft.Office.Server.Audience;
using System.Collections.Generic;

namespace Globosat.Util.VerificadorAcesso.WPVerificadorAcesso
{
    public partial class WPVerificadorAcessoUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string login = string.Empty;
                using (var site = new SPSite(SPContext.Current.Site.Url))
                {
                    using (var web = site.OpenWeb())
                    {
                        login = web.CurrentUser.LoginName;

                        bool acesso = false;
                        if (PossuiAcessoTotal(login))
                            acesso = true;
                        else if (VerificarDados.UsuarioEstaNaAudiencia(login, "Gestores_audience", site))
                            acesso = true;
                        else if(pageAudience())
                            acesso = true;

                        if(acesso)
                        {
                            if(Request.Url.AbsoluteUri.Contains("AccessDenied.aspx"))
                                HttpContext.Current.Response.Redirect("/remuneracoes/default.aspx", true);
                        }
                        else
                            HttpContext.Current.Response.Redirect("/remuneracoes/Custom%20Pages/AccessDenied.aspx", true);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.Write("Erro ao verificar acesso na página: " + SPContext.Current.Web.Url + " /n" + exc.Message + exc.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 1);
                throw;
            }
        }

        protected bool pageAudience()
        {
            SPFile pagina = SPContext.Current.File;
            SPListItem item = pagina.Item;
            String[] guids;

            try
            {
                guids = ((string)item.Properties["Públicos-alvos"]).Split(',');
            }
            catch
            {
                guids = new String[0];
            }
            List<Guid> audienciasGuids = new List<Guid>();

            foreach (String guid in guids)
                try
                {
                    String add = guid.Split(';')[0];
                    if (add != string.Empty)
                        audienciasGuids.Add(new Guid(add));
                }
                catch { }

            bool estaNaAudiencia = false;

            if (audienciasGuids.Count > 0)
            {
                string login = SPContext.Current.Web.CurrentUser.LoginName;
                SPServiceContext ctx = SPServiceContext.GetContext(SPContext.Current.Site);
                
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    AudienceManager audManager = new AudienceManager(ctx);

                    for (int i = 0; i < audManager.Audiences.Count && !estaNaAudiencia; i++)
                    {
                        Microsoft.Office.Server.Audience.Audience objAudience = audManager.Audiences[i];
                        if (objAudience != null)
                            for (int j = 0; j < audienciasGuids.Count && !estaNaAudiencia; j++)
                            {
                                Guid aGuid = audienciasGuids[j];
                                if (objAudience.AudienceID == aGuid)
                                    estaNaAudiencia = objAudience.IsMember(login);
                            }
                    }
                });
            }
            return estaNaAudiencia;
        }
        
        private bool PossuiAcessoTotal(string login)
        {
            bool possuiAcesso = false;

            SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

            using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
            {

                using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPUser userLogado = SPContext.Current.Web.CurrentUser;
                    SPGroup grupoAdministrador = web.Groups["Grupo_Remuneração_Administradores"];
                    SPGroup grupoGerentes = web.Groups["Grupo_Remuneração_Gerentes"];

                    foreach (SPUser administrador in grupoAdministrador.Users)
                    {
                        if (administrador.LoginName.Equals(login))
                            possuiAcesso = true;

                    }
                    if (!possuiAcesso)
                    {
                        foreach (SPUser gerente in grupoGerentes.Users)
                        {
                            if (gerente.LoginName.Equals(login))
                                possuiAcesso = true;
                        }
                    }
                }
            }
            return possuiAcesso;
        }
    }
}
