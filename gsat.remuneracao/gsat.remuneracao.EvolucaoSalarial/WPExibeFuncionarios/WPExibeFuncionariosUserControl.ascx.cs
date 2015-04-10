using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Globosat.Library.Entidades;
using Globosat.Library.AcessoDados;
using Globosat.Library.Servicos;
using System.Data;
using Microsoft.SharePoint;
using System.Web.UI.HtmlControls;
using CIT.Sharepoint.Util;
using Microsoft.SharePoint.Utilities;
using System.Diagnostics;


namespace Globosat.Remuneracao.EvolucaoSalarial.WPExibeFuncionarios
{
    public partial class WPExibeFuncionariosUserControl : UserControl
    {
        Gerente dadosGerente = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string login = SPContext.Current.Web.CurrentUser.LoginName;

                treeview.Attributes.Add("onclick", "OnCheckBoxCheckChanged(event)");

                SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;

                using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        //if (ManipularDados.VerificaLogin(web, login, "Administradores Remuneração"))
                        if(PossuiAcessoTotal(login))
                        {
                            if (!this.IsPostBack)
                            {
                                DataTable todosCentrosCusto = new DataTable();
                                todosCentrosCusto = ManipularDados.BuscaTodosCentrosCusto();

                                PopulaCentroCusto(todosCentrosCusto, treeview.Nodes);

                                //Fecha treeView
                                treeview.CollapseAll();
                            }
                        }
                        else
                        {
                            dadosGerente = new Gerente();

                            //Busca matrícula e coligada do usuário atual.
                            dadosGerente = ManipularDados.BuscaMatriculaColigada(login);

                            //Busca dados apenas uma vez por ciclo de vida da página
                            if (!this.IsPostBack)
                            {
                                DataTable tableCentroCusto = new DataTable();

                                if (dadosGerente.Matricula != null)
                                {
                                    //Busca centro de custo
                                    tableCentroCusto = ManipularDados.BuscaCentroCusto(dadosGerente.Matricula, dadosGerente.Coligada);

                                    if (tableCentroCusto.Rows.Count == 0)
                                    {
                                        lblValidacao.Text = "Você não pode visualizar esta página.";
                                        lblValidacao.Visible = true;
                                        divTitulo.Visible = false;
                                        emailButton.Visible = false;
                                    }
                                    else
                                    {
                                        //Popula centro de custo com funcionários
                                        PopulaCentroCusto(tableCentroCusto, treeview.Nodes);

                                        //Fecha treeView
                                        treeview.CollapseAll();
                                    }
                                }
                                else
                                {
                                    //Mostra mensagem de não autorizado.
                                    lblValidacao.Text = "Erro ao buscar Matrícula.";
                                    lblValidacao.Visible = true;
                                    divTitulo.Visible = false;
                                    emailButton.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                     Cit.Globosat.Common.Utility.GetCurrentMethod(), Cit.Globosat.Common.Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        public void emailButton_Click(object sender, ImageClickEventArgs e)
        {
            string corpoEmail = string.Empty;
            int countChecked = 0;
            bool tituloInserido = false;
            try
            {
                foreach (TreeNode node in treeview.Nodes)
                {
                    foreach (TreeNode nodeFilho in node.ChildNodes)
                    {
                        if (!node.Checked)
                        {
                            if (nodeFilho.Checked)
                            {
                                if (!tituloInserido)
                                {
                                    corpoEmail += "<strong>" + node.Text + "</strong><br/>";
                                    tituloInserido = true;
                                }
                                countChecked++;
                                corpoEmail += MontarEmail(nodeFilho);
                            }
                        }
                        else
                        {
                            if (!tituloInserido)
                            {
                                corpoEmail += "<strong>" + node.Text + "</strong><br/>";
                                tituloInserido = true;
                            }
                            countChecked++;
                            corpoEmail += MontarEmail(nodeFilho);
                        }
                    }

                    tituloInserido = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao criar corpo do email na página principal: " + ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 1);
                SPUtility.TransferToErrorPage("Ocorreu um erro ao enviar o email.", null, null);
            }

            if (countChecked > 0)
            {   
                    Email.EnvioEmail(SPContext.Current.Web.CurrentUser.Email, "Evolução Salarial de Funcionários", corpoEmail);
                    SPUtility.TransferToSuccessPage("Email enviado com sucesso.", "/", null, null);
            }
        }

        private void PopulaCentroCusto(DataTable tableCentroCusto, TreeNodeCollection nodes)
        {
            try
            {
                foreach (DataRow dr in tableCentroCusto.Rows)
                {
                    TreeNode tn = new TreeNode();

                    //Insere dados em nó pai do Treeview
                    tn.PopulateOnDemand = true;
                    tn.SelectAction = TreeNodeSelectAction.None;
                    tn.Text = dr["DESCRICAO"].ToString() + " - " + dr["CODSECAO"].ToString();
                    tn.Value = dr["CODSECAO"].ToString();

                    //Adiciona nó à árvore.
                    nodes.Add(tn);
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao popular centro de custo: " + ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 1);
                throw;
            }

        }

        public void PopulaColaboradores(TreeNode treeNode)
        {
            try
            {
                dadosGerente = new Gerente();
                
                //Busca dados do gerente
                dadosGerente = ManipularDados.BuscaMatriculaColigada(SPContext.Current.Web.CurrentUser.LoginName);

                DataTable tableColaboradores = new DataTable();

                //Busca todos os colaboradores pertencentes ao centro de custo
                if (PossuiAcessoTotal(SPContext.Current.Web.CurrentUser.LoginName))
                    tableColaboradores = ManipularDados.BuscaColaboradores(treeNode.Value);
                else
                    tableColaboradores = ManipularDados.BuscaColaboradores(treeNode.Value, dadosGerente.Coligada, dadosGerente.Matricula);

                foreach (DataRow dr in tableColaboradores.Rows)
                {
                    TreeNode tn = new TreeNode();

                    //Adiciona dados ao nó filho na árvore
                    tn.SelectAction = TreeNodeSelectAction.Select;
                    tn.Text = dr["NOME"].ToString();
                    //tn.Value = "Matricula=" + dr["CHAPA"].ToString() + "&Coligada=" + dadosGerente.Coligada;

                    tn.Value = SPContext.Current.Web.Url + "/_layouts/EvolucaoSalarial/MaisDetalhes.aspx?" + "Matricula=" + dr["CHAPA"].ToString() + "&Coligada=" + dr["CODCOLIGADA"].ToString().Trim() 
                        +  "&AC=0&PDF=0";
                    tn.NavigateUrl = "javascript:open('" + tn.Value + "')";
                    tn.PopulateOnDemand = false;
                    treeNode.ChildNodes.Add(tn);
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao popular colaboradores: " + ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 1);
                throw;
            }

        }

        public void treeview_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            PopulaColaboradores(e.Node);
        }

        public string MontarEmail(TreeNode nodeFilho)
        {
            List<Funcionario> dadosFunc = new List<Funcionario>(); 
            string matricula = string.Empty;
            string coligada = string.Empty;

            matricula = nodeFilho.Value.Split('&')[0].Split('=')[1];
            coligada = nodeFilho.Value.Split('&')[1].Split('=')[1];

            dadosFunc = ManipularDados.PopularGridView(matricula, coligada, false);

            return ManipularDados.EnviarConteudoEmail(dadosFunc, "800x200",matricula, nodeFilho.Text) + "<br/><br/>";
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
    }
}
