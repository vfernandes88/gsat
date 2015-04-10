using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Data;
using Globosat.Library.Servicos;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Globosat.Library.Entidades;
using System.Web;
using System.Configuration;

namespace Globosat.Remuneracao.CustomPages.Layouts.Globosat.Remuneracao.CustomPages
{
    public partial class BuscaCargo : LayoutsPageBase
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

            // Register the SPGridView SelectedIndexChanged event.
            ResultGrid.SelectedIndexChanged += new EventHandler(ResultGrid_SelectedIndexChanged);
            ResultGrid.RowDataBound += new GridViewRowEventHandler(ResultGrid_RowDataBound);

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["filial"]))
                {
                    this.rbLocaltrabalho.SelectedValue = Request.QueryString["filial"];
                }
            }
        }

        void ResultGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                ResultGrid.Columns[0].Visible = true;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[5].Text = Convert.ToDecimal(e.Row.Cells[5].Text).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")); // Salário
                    ((System.Web.UI.WebControls.LinkButton)(e.Row.Cells[0].Controls[0])).Text = e.Row.Cells[1].Text;
                }
                else if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].Text = "Cargo";
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao alterar o idioma do campo Salário: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
            }
        }

        // Fires when the search button is clicked.
        protected void Dosearch_Click(object sender, ImageClickEventArgs e)
        {
            // Declare a variable to store the website's URL.
            string url = string.Empty;

            // Reset the hidden text box to empty.
            Dialogvalue.Text = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(SearchBox.Text.Trim()))
                {
                    DataTable results = new DataTable();

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                    {
                        #region PRODUCAO
                        Gerente dadosGerente = new Gerente();

                        // Busca matrícula e coligada do usuário atual.
                        dadosGerente = ManipularDados.BuscaMatriculaColigada(SPContext.Current.Web.CurrentUser.LoginName);

                        int codColigada = (string.IsNullOrEmpty(dadosGerente.Coligada)) ? 1 : Convert.ToInt32(dadosGerente.Coligada);
                        string strNivel = ManipularDados.BuscaNivelColaborador(dadosGerente.Matricula, codColigada.ToString());

                        if (PossuiAcessoTotal(SPContext.Current.Web.CurrentUser.LoginName))
                        {
                            strNivel = "22"; // Como é administrador pode visualizar todos os níveis
                        }
                        else
                        {
                            strNivel = (Convert.ToInt32(strNivel)-1).ToString();
                        }

                        int codFilial = 1;
                        if (!string.IsNullOrEmpty(Request.QueryString["codColigada"]))
                        {
                            if (!Request.QueryString["codColigada"].Equals("5"))
                            {
                                if (rbLocaltrabalho.SelectedValue.Trim().Equals("SP"))
                                {
                                    codFilial = 2;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(Request.QueryString["codColigada"]))
                        {
                            codColigada = Convert.ToInt32(Request.QueryString["codColigada"]);
                        }

                        results = ManipularDados.BuscaCargoRequisicaoPessoal(SearchBox.Text.Trim(), codColigada, strNivel, tipo, codFilial);
                        #endregion
                    }
                    else
                    {
                        #region DESEVOLVIMENTO
                        results.Columns.Add("CODNOME", Type.GetType("System.String"));
                        results.Columns.Add("JORNADA", Type.GetType("System.String"));
                        results.Columns.Add("FAIXA", Type.GetType("System.String"));
                        results.Columns.Add("NIVEL", Type.GetType("System.String"));
                        results.Columns.Add("SALARIO", Type.GetType("System.String"));

                        DataRow dr = results.NewRow();
                        dr["CODNOME"] = "G000764 - ANALISTA DE DESENVOLVIMENTO COMERCIAL JR";
                        dr["JORNADA"] = "180";
                        dr["FAIXA"] = "C";
                        dr["NIVEL"] = "22";
                        dr["SALARIO"] = "500";

                        results.Rows.Add(dr);
                        #endregion
                    }

                    // Display the result count.
                    if (results.Rows.Count > 1)
                        ResultCount.Text = string.Concat(results.Rows.Count.ToString(), " itens encontrados.");
                    else
                        ResultCount.Text = "Nenhum cargo localizado ou você não possui permissão para visualizar este cargo.";

                    ResultCount.Visible = true;

                    // Bind the gridview with the result object collection.

                    // Populate the Grid.
                    ResultGrid.DataSource = results;
                    ResultGrid.DataBind();

                    // Reset the selected gridview row from previous search.
                    ResultGrid.SelectedIndex = -1;

                    // Display the gridview is there are results to display.
                    if (results.Rows.Count > 0)
                    {
                        ResultGrid.Visible = true;
                        ResultGrid.Columns[0].HeaderText = "Cargo";
                        ResultGrid.Columns[0].Visible = false;
                    }
                    else
                    {
                        ResultGrid.Visible = false;
                    }
                }
                else
                {
                    // Ask the user to enter a search term. 
                    ResultCount.Text = "Por favor, digite uma ou mais palavras.";
                    ResultCount.Visible = true;

                    // Reset the gridview datasource if any of the required parameters are absent.
                    ResultGrid.DataSource = null;
                    ResultGrid.DataBind();
                    ResultGrid.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao executar a busca por cargo: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                throw;
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
        // Fires on a row that is selected in the gridview.
        void ResultGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Determine whether an items has been selected.
                if (ResultGrid.SelectedIndex != -1)
                {
                    // Get the selected row from the gridview.
                    SPGridViewRow row = ResultGrid.SelectedRow as SPGridViewRow;
                    if (row != null)
                    {
                        // Create a hyperlink object to retrieve the selected document details.
                        string codigo = SPHttpUtility.HtmlDecode(row.Cells[1].Text.Split('-')[0].Trim()); // Código
                        string valor = SPHttpUtility.HtmlDecode(row.Cells[1].Text.Split('-')[1].Trim()); // Cargo
                        string jornada = (row.Cells[2].Text == "&nbsp;") ? "N/A" : row.Cells[2].Text.Trim(); // Jornada

                        string nivel = row.Cells[4].Text.Trim();// Nível
                        string faixa = row.Cells[3].Text.Trim();// Faixa
                        string salario = row.Cells[5].Text.Trim();

                        // Create a semicolon (;) separated string of docname and docurl and assign to 
                        // a hidden text box.
                        Dialogvalue.Text = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}", ";", codigo, valor, jornada, nivel, faixa, salario);
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ClosePopupScript", "SP.UI.ModalDialog.commonModalDialogClose(1, '" + Dialogvalue.Text + "');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao executar a seleção de cargo: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                throw;
            }
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
    }
}
