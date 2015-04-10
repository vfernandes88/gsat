using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Server.Audience;
using Microsoft.SharePoint;
using CIT.Sharepoint.Util;
using System.Diagnostics;

namespace Globosat.Library.Servicos
{
    public class VerificarDados
    {
        /// <summary>
        /// Função que verifica se o usuário está na audiencia
        /// </summary>
        /// <param name="usuario">Usuário que está logado</param>
        /// <returns></returns>
        public static bool UsuarioEstaNaAudiencia(string usuario, string audiencia, SPSite site)
        {
            try
            {
                bool estaNaAudiencia = false;
                Audience aud = null;
                SPServiceContext ctx = SPServiceContext.GetContext(site);

                //Executa o código com privilégio de administrador
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    AudienceManager audManager = new AudienceManager(ctx);
                    //aud = audManager.GetAudience(audiencia);

                    #region Teste
                    foreach (Microsoft.Office.Server.Audience.Audience objAudience in audManager.Audiences)
                    {
                        if (objAudience != null && objAudience.AudienceName == audiencia)
                        {
                            if (objAudience.IsMember(usuario))
                                estaNaAudiencia = true;
                        }
                    }

                    #endregion


                });

                //Verificar se o usuário logado é um membro da audiência.
                //return aud.IsMember(usuario);
                return estaNaAudiencia;
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao verificar se o usuário ja está na audiencia:" + ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 1, 1);
                return false;
            }
        }

        public static bool PossuiAcessoTotal(string login)
        {
            try
            {
                bool possuiAcesso = false;
                SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

                using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
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
            catch(Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Cit.Globosat.Common.Utility.GetCurrentMethod(), Cit.Globosat.Common.Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
                
                return false;
            }
        }
    }
}
