using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Data;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;
using System.Collections.Generic;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Office.Server.UserProfiles;
using System.Data.SqlClient;
using Globosat.Library.AcessoDados;
using System.Web;
using System.IO;
using System.Security.Principal;
using System.Reflection;
using Microsoft.Office.Server;
using System.Text;
using Microsoft.SharePoint.Utilities;

namespace Globosat.Remuneracao.FolhaPagamento.WPFolhaPagamento
{
    public partial class WPFolhaPagamentoUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Gerente dadosProfile = null;
            List<Funcionario> colaboradores = null;
            string login = string.Empty;
            Funcionario oColaborador = null;
            DataTable dtCentrosCusto = null;

            try
            {
                dadosProfile = new Gerente();

                login = SPContext.Current.Web.CurrentUser.LoginName;

                //Busca matrícula e coligada do usuário atual.
                dadosProfile = ManipularDados.BuscaMatriculaColigada(login);

                if (!this.IsPostBack)
                {
                    SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
                    using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                    {
                        using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                        {
                            ddlCentroCusto.Items.Add(new ListItem("Selecione...", "0"));

                            if (PossuiAcessoTotal(login))
                            {
                                dtCentrosCusto = ManipularDados.BuscaTodosCentrosCusto();
                            }
                            else
                            {
                                dtCentrosCusto = ManipularDados.BuscaCentroCusto(dadosProfile.Matricula, dadosProfile.Coligada);
                            }
                            foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                            {
                                ddlCentroCusto.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                            }
                            if (dtCentrosCusto.Rows.Count == 0)
                            {
                                ddlCentroCusto.Visible = false;
                                lblErroMsg.Visible = true;
                                lblErroMsg.Text = "Não existe nenhum Centro de Custo para visualização.";
                            }
                        }
                    }
                }
                else
                {
                    if (ddlCentroCusto.SelectedItem.Value != null && ddlCentroCusto.SelectedValue != "0")
                    {
                        tableHeader.Visible = true;
                        lblCentroCusto.Text = ddlCentroCusto.SelectedItem.Text;
                        string centroCusto = ddlCentroCusto.SelectedItem.Value;
                        string coligada = string.Empty;
                        coligada = ExtraiColigadaCentroCusto(centroCusto);
                                                
                        DataTable dtColaboradores = new DataTable();
                        dtColaboradores = ManipularDados.BuscaColaboradoresFolhaPagamento(centroCusto);
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                            {
                                // É preciso acessar dados de uma propriedade privada no user profile.
                                SPServiceContext serviceContext = SPServiceContext.GetContext(spSite);
                                HttpContext currentContext = HttpContext.Current;
                                HttpContext.Current = null;
                                UserProfileManager upm = new UserProfileManager(serviceContext);
                                SqlConnection conn = BaseDados.GetConnectionUP();

                                colaboradores = new List<Funcionario>();
                                foreach (DataRow linhaColaboradores in dtColaboradores.Rows)
                                {
                                    DadosProfile infoProfile = new DadosProfile();
                                    oColaborador = new Funcionario();

                                    //Popular Dados...
                                    oColaborador.Nome = linhaColaboradores["NOME"] as string;
                                    oColaborador.Salario = Convert.ToDecimal(linhaColaboradores["SALARIO"]).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                                    oColaborador.Funcao = linhaColaboradores["CARGO"] as string;
                                    oColaborador.Matricula = linhaColaboradores["CHAPA"].ToString();
                                    oColaborador.Admissao = Convert.ToDateTime(linhaColaboradores["Admissao"]).ToString("dd/MM/yyyy");

                                    //Preenche nível salarial e classe
                                    oColaborador.Classe = linhaColaboradores["CODNIVELSAL"] as string;
                                    oColaborador.Nivel = linhaColaboradores["GRUPOSALARIAL"] as string;
                                    
                                    infoProfile = ManipularDados.BuscaDadosColaborador(oColaborador.Matricula, coligada, upm, conn);

                                    if (infoProfile != null)
                                    {
                                        oColaborador.Foto = infoProfile.Foto;

                                        if (!string.IsNullOrEmpty(infoProfile.DtNascimento))
                                            oColaborador.DtNascimento = infoProfile.DtNascimento.Replace('.', '/');
                                    }

                                    colaboradores.Add(oColaborador);
                                }

                                rptColaboradores.DataSource = colaboradores;
                                rptColaboradores.DataBind();
                                
                                HttpContext.Current = currentContext;
                            }
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                //Mostrar msg de Erro.
                lblErroMsg.Visible = true;
                lblErroMsg.Text = "Erro ao tentar abrir a página. Entre em contato com o Administrador.";
                Logger.Write("Erro no page_load da Folha de Pagamento: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
            }
        }

        private string ExtraiColigadaCentroCusto(string centroCusto)
        {
            string coligada = string.Empty;
            coligada = centroCusto.Substring(0, centroCusto.IndexOf('.'));

            if (coligada.Equals("02"))
                coligada = "1";
            else
                coligada = Convert.ToInt32(coligada).ToString();

            return coligada;
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
                    foreach (SPUser administrador in grupoAdministrador.Users)
                    {
                        if (administrador.LoginName.Equals(login))
                            possuiAcesso = true;
                    }
                }
            }
            return possuiAcesso;
        }

        protected void Onclick_btnEnviar(object sender, ImageClickEventArgs e)
        {
            StringBuilder email = new StringBuilder();
            Label lblConteudo = new Label();
            string centroCusto = string.Empty;
            try
            {
                centroCusto = ddlCentroCusto.SelectedItem.Text;

                StringBuilder sb = new StringBuilder();
                StringWriter tw = new StringWriter(sb);
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                pnlDados.RenderControl(hw);
                var html = sb.ToString();

                email.Append("<br/>" + html);

            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao enviar email em Mais Detalhes: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                SPUtility.TransferToErrorPage("Ocorreu um erro ao enviar o email.", null, null);
            }

            Email.EnvioEmail(SPContext.Current.Web.CurrentUser.Email, "Metas Funcionarios", email.ToString());
            SPUtility.TransferToSuccessPage("Email enviado com sucesso.", "/Paginas/metasFuncionarios.aspx", null, null);
        }
    }
}
