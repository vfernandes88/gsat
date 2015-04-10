using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Cit.Globosat.Common;
using System.Diagnostics;
using CIT.Sharepoint.Util;
using Microsoft.SharePoint;
using Microsoft.Reporting.WebForms;
using Globosat.Library.Servicos;
using System.Linq;
using System.Data;
using Cit.Globosat.Remuneracao.Formularios.BLL.AltFuncCargo;
using Globosat.Library.Entidades;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Web.UI.HtmlControls;

namespace Cit.Globosat.Premios.WPFuncionariosPremios
{
    public partial class WPFuncionariosPremiosUserControl : UserControl
    {
        public string ReportPath { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarCentroCusto();
            }
        }

        private void CarregarCentroCusto()
        {
            DadosProfile dadosProfile = new DadosProfile();
            try
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                    {
                        bool isAdministrator = false;
                        isAdministrator = BLL.UserExistsInList(spSite, spWebRemuneracoes, spWebRemuneracoes.CurrentUser.LoginName,
                                            Constants.AdministradoresRemuneracaolistName);
                        if (isAdministrator)
                        {
                            dadosProfile.Coligada = "1";
                            dadosProfile.Matricula = "00000";
                            dadosProfile.Classe = "I";
                            dadosProfile.FaixaSalarial = 21;
                        }
                        else
                        {
                            dadosProfile = ManipularDados.BuscaDadosProfile(spSite, spWebRemuneracoes.CurrentUser.LoginName);
                        }
                    }
                }

                if (dadosProfile.Matricula.Equals("00000"))
                {
                    this.dropDownListCentroCusto.DataSource = ManipularDados.BuscaTodosCentroCustoToPremios();
                }
                else
                {
                    this.dropDownListCentroCusto.DataSource = ManipularDados.BuscaCentroCustoToPremios(dadosProfile.Matricula, dadosProfile.Coligada);
                }
                this.dropDownListCentroCusto.DataValueField = "CODSECAO";
                this.dropDownListCentroCusto.DataTextField = "DESCRICAO";
                this.dropDownListCentroCusto.DataBind();
                this.dropDownListCentroCusto.Items.Insert(0, new ListItem("Selecione...", "0"));
                this.dropDownListCentroCusto.Items.Insert(this.dropDownListCentroCusto.Items.Count, new ListItem("TODOS", "1"));
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
            finally
            {
                dadosProfile = null;
            }
        }

        private void CarregarFuncionarios(int pageIndex, string centroCusto)
        {
            try
            {
                if (centroCusto.Contains(","))
                {
                    this.gridViewFuncionarios.DataSource = ManipularDados.BuscaColaboradoresToPremiosIN(centroCusto.Replace("''", "'"));
                }
                else
                {
                    this.gridViewFuncionarios.DataSource = ManipularDados.BuscaColaboradoresToPremios(centroCusto.Replace("''", string.Empty).Trim());
                }

                this.gridViewFuncionarios.PageIndex = pageIndex;
                this.gridViewFuncionarios.DataBind();
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        private void CarregarEvento()
        {
            this.dropDownListEventos.DataSource = ManipularDados.GetEventos().Where(e => !e.Equals("SALARIO MENSAL")).ToList();
            this.dropDownListEventos.DataBind();
            this.dropDownListEventos.Items.Insert(0, new ListItem("Selecione...", "0"));
        }

        private void CarregarRelatorio(int setPage, string idFuncionarios)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ReportPath))
                {
                    // Habilita uso de functions expressions.
                    // Referência: http://insomniacgeek.com/how-to-fix-the-failed-to-load-expression-host-assembly-error-in-a-sharepoint-custom-solution/
                    this.reportViewerFuncPremios.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
                    this.reportViewerFuncPremios.LocalReport.ReportPath = this.ReportPath;
                    this.reportViewerFuncPremios.LocalReport.EnableExternalImages = true;

                    DataTable dt = ManipularDados.BuscarPremios(GetCentroCusto(), idFuncionarios);
                    ReportDataSource rdsPremios = new ReportDataSource("dsReportFuncionariosPremios", dt);
                    this.reportViewerFuncPremios.LocalReport.DataSources.Add(rdsPremios);

                    this.reportViewerFuncPremios.LocalReport.SetParameters(new ReportParameter("currentUser", SPContext.Current.Web.CurrentUser.Name));
                    this.reportViewerFuncPremios.LocalReport.SetParameters(new ReportParameter("graficoEvento", this.dropDownListEventos.SelectedValue));

                    this.reportViewerFuncPremios.Visible = true;
                    this.reportViewerFuncPremios.CurrentPage = setPage;
                    this.reportViewerFuncPremios.LocalReport.Refresh();

                    // TODO: remover após a fase de homologação.
                    //DataView dados = ManipularDados.BuscarPremios(GetCentroCusto(), idFuncionarios).DefaultView;
                    //dados.RowFilter = "EVENTO = '" + this.dropDownListEventos.SelectedValue + "' AND ANO > 2009";
                    //dados.Sort = "NOME ASC";

                    //this.gridviewPremios.DataSource = dados;
                    //this.gridviewPremios.DataBind();
                }
                else
                {
                    this.labelMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void dropDownListCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reportViewerFuncPremios.Visible = false;

            if (this.dropDownListCentroCusto.SelectedValue != "0")
            {
                CarregarFuncionarios(0, GetCentroCusto());

                this.labelFuncionarios.Visible = true;
                this.gridViewFuncionarios.Visible = true;

                if (this.gridViewFuncionarios.Rows.Count > 0)
                {
                    this.labelFuncionarios.Visible = true;

                    CarregarEvento();
                    this.labelGraficoEvento.Visible = true;
                    this.dropDownListEventos.Visible = true;
                    this.buttonSelecionar.Visible = true;
                    this.buttonDesmarcar.Visible = true;
                }
                else
                {
                    this.labelGraficoEvento.Visible = false;
                    this.dropDownListEventos.Visible = false;
                    this.buttonSelecionar.Visible = false;
                    this.buttonDesmarcar.Visible = false;
                    this.buttonGerarRelatorio.Enabled = false;
                }
            }
            else
            {
                this.labelFuncionarios.Visible = false;
                this.gridViewFuncionarios.Visible = false;
                this.labelGraficoEvento.Visible = false;
                this.dropDownListEventos.Visible = false;
                this.buttonSelecionar.Visible = false;
                this.buttonDesmarcar.Visible = false;
                this.buttonGerarRelatorio.Enabled = false;
            }
        }

        protected void dropDownListEventos_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reportViewerFuncPremios.Visible = false;
            if (this.dropDownListEventos.SelectedValue == "0")
            {
                this.buttonGerarRelatorio.Enabled = false;
            }
            else
            {
                this.buttonGerarRelatorio.Enabled = true;
            }
        }

        protected void buttonGerarRelatorio_Click(object sender, EventArgs e)
        {
            if ((this.dropDownListCentroCusto.SelectedValue != "0") && (this.dropDownListEventos.SelectedValue != "0"))
            {
                string idFuncionarios = GetIdFuncionario();
                if (!string.IsNullOrEmpty(idFuncionarios))
                    CarregarRelatorio(0, idFuncionarios);
                else
                    this.reportViewerFuncPremios.Visible = false;
            }
            else
            {
                this.reportViewerFuncPremios.Visible = false;
            }
        }

        protected void reportViewerPremios_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string idFuncionarios = GetIdFuncionario();
            if (!string.IsNullOrEmpty(idFuncionarios))
                CarregarRelatorio(0, idFuncionarios);
            else
                this.reportViewerFuncPremios.Visible = false;
        }

        private string GetIdFuncionario()
        {
            string idFuncionarios = string.Empty;
            foreach (GridViewRow item in this.gridViewFuncionarios.Rows)
            {
                CheckBox checkBoxID = (CheckBox)item.FindControl("checkBoxID");
                if (checkBoxID.Checked)
                {
                    idFuncionarios += string.Format("{0},", item.Cells[1].Text);
                }
            }

            if (!string.IsNullOrEmpty(idFuncionarios))
            {
                return idFuncionarios.Remove(idFuncionarios.Length - 1, 1);
            }

            return string.Empty;
        }

        private string GetCentroCusto()
        {
            if (this.dropDownListCentroCusto.SelectedValue == "1")
            {
                string todosCentroCustos = string.Empty;
                foreach (ListItem item in this.dropDownListCentroCusto.Items)
                {
                    if ((item.Value != "0") && (item.Value != "1"))
                    {
                        todosCentroCustos += string.Format("''{0}'',", item.Value);
                    }
                }
                return todosCentroCustos.Remove(todosCentroCustos.Length - 1, 1);
            }
            else
            {
                return string.Format("''{0}''", this.dropDownListCentroCusto.SelectedValue);
            }
        }

        protected void gridViewFuncionarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox checkBoxID = (CheckBox)e.Row.Cells[0].FindControl("checkBoxID");
                checkBoxID.Checked = true;
            }
        }

        protected void gridViewFuncionarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (this.dropDownListCentroCusto.SelectedValue != "0")
            {
                CarregarFuncionarios(e.NewPageIndex, GetCentroCusto());
            }
        }
    }
}
