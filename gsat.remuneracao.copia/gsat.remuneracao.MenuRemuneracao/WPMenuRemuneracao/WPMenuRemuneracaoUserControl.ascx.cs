using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CIT.Sharepoint.Util;
using Globosat.Library;
using Globosat.Library.Entidades;
using Globosat.Library.Servicos;
using Microsoft.SharePoint;
using Cit.Globosat.Common;
using System.Diagnostics;

namespace WPMenuRemuneracao.WPMenuRemuneracao
{
    public partial class WPMenuRemuneracaoUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string login = string.Empty;
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    login = web.CurrentUser.LoginName;

                    // Todos os gerentes e diretores deverão ter acesso total.
                    if (PossuiAcessoTotal(login) || VerificarDados.UsuarioEstaNaAudiencia(login, "Gestores_audience", site)) 
                    {
                        divDescCargos.Visible = true;
                        divEvolucao.Visible = true;
                        divFaleConosco.Visible = true;
                        divFolhaPagto.Visible = true;
                        divFormularios.Visible = true;
                        divHome.Visible = true;
                        divMetas.Visible = true;
                        divRegras.Visible = true;
                        divRemuneracaoVariavel.Visible = true;
                        divTabSalarial.Visible = true;
                    }
                    else // Todos os chefes, supervisores e coordenadores* (*) grupo a ser criado
                    {
                        divDescCargos.Visible = false;
                        divEvolucao.Visible = false;
                        divFaleConosco.Visible = true;
                        divFolhaPagto.Visible = false;
                        divFormularios.Visible = false;
                        divHome.Visible = true;
                        divMetas.Visible = false;
                        divRegras.Visible = true;
                        divRemuneracaoVariavel.Visible = false;
                        divTabSalarial.Visible = false;
                    }

                    // Item organograma
                    if ((Utility.DoesUserIsGroupMember(SPContext.Current.Web.CurrentUser, Constants.GrupoRemuneracaoAdministradores))
                        || (Utility.DoesUserIsGroupMember(SPContext.Current.Web.CurrentUser, Constants.GrupoRemuneracaoGerentes)))
                    {
                        divOrganograma.Visible = true;
                    }
                    else
                    {
                        divOrganograma.Visible = false;
                    }

                    // Item relatório
                    if (Utility.DoesUserIsGroupMember(SPContext.Current.Web.CurrentUser, Constants.GrupoVisualizadoresPremios))
                    {
                        divRelatorios.Visible = true;
                    }
                    else
                    {
                        divRelatorios.Visible = false;
                    }
                }
            }
        }

        private bool PossuiAcessoTotal(string login)
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
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);

                return false;
            }
        }
    }
}
