using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Reporting.WebForms;
using System.Data;
using Cit.Globosat.Common;
using System.Diagnostics;
using CIT.Sharepoint.Util;
using Microsoft.SharePoint;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;
using System.Data.SqlClient;
using Microsoft.Office.Server.UserProfiles;
using Globosat.Library.AcessoDados;
using Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Cit.Globosat.Remuneracao.Formularios.BLL.AltFuncCargo;


namespace Cit.Globosat.Premios.WPPremios
{
    public partial class WPPremiosUserControl : UserControl
    {
        public string ReportPath { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CarregarCentroCusto();
                    this.dropDownListFuncionarios.Enabled = false;
                    this.dropDownListEventos.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
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

        private void CarregarFuncionarios(string centroCusto)
        {
            try
            {
                this.dropDownListFuncionarios.DataValueField = "CHAPA";
                this.dropDownListFuncionarios.DataTextField = "NOME";
                this.dropDownListFuncionarios.DataSource = ManipularDados.BuscaColaboradoresToPremios(centroCusto);
                this.dropDownListFuncionarios.DataBind();
                this.dropDownListFuncionarios.Items.Insert(0, new ListItem("Selecione...", "0"));
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        private void CarregarEventos()
        {
            this.dropDownListEventos.DataSource = ManipularDados.GetEventos().Where(e => !e.Equals("SALARIO MENSAL")).ToList();
            this.dropDownListEventos.DataBind();
            this.dropDownListEventos.Items.Insert(0, new ListItem("Selecione...", "0"));
            this.dropDownListEventos.Items.Insert(this.dropDownListEventos.Items.Count, new ListItem("TOTAL", "TOTAL"));
        }

        private void CarregarRelatorio(int setPage)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ReportPath))
                {
                    // Habilita uso de functions expressions.
                    // Referência: http://insomniacgeek.com/how-to-fix-the-failed-to-load-expression-host-assembly-error-in-a-sharepoint-custom-solution/
                    this.reportViewerPremios.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
                    this.reportViewerPremios.LocalReport.ReportPath = setPage != 0 ? this.ReportPath : this.ReportPath.Replace("ReportPremios", "ReportPremiosTotal");
                    this.reportViewerPremios.LocalReport.EnableExternalImages = true;

                    string codigoColigada = ExtraiColigadaCentroCusto(this.dropDownListCentroCusto.SelectedValue);
                    ReportDataSource rdsSalario = new ReportDataSource("dsReportDadosFuncionario", ManipularDados.BuscarDadosFuncionarios(Convert.ToInt32(codigoColigada), this.dropDownListFuncionarios.SelectedValue));
                    ReportDataSource rdsPremios = new ReportDataSource("dsReportPremios", ManipularDados.BuscarPremios(Convert.ToInt32(codigoColigada), this.dropDownListFuncionarios.SelectedValue));

                    this.reportViewerPremios.LocalReport.DataSources.Add(rdsSalario);
                    this.reportViewerPremios.LocalReport.DataSources.Add(rdsPremios);

                    this.reportViewerPremios.LocalReport.SetParameters(new ReportParameter("currentUser", SPContext.Current.Web.CurrentUser.Name));
                    this.reportViewerPremios.LocalReport.SetParameters(new ReportParameter("fotoFuncionario", GetFotoFuncionario(this.dropDownListFuncionarios.SelectedValue, codigoColigada)));
                    this.reportViewerPremios.LocalReport.SetParameters(new ReportParameter("graficoEvento", this.dropDownListEventos.SelectedValue));

                    this.reportViewerPremios.Visible = true;
                    this.reportViewerPremios.CurrentPage = setPage;
                    this.reportViewerPremios.ShowPageNavigationControls = setPage == 0;
                    this.reportViewerPremios.LocalReport.Refresh();
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

        private string GetFotoFuncionario(string matriculaFucionario, string codigoColigada)
        {
            SPServiceContext spServiceContext = null;
            UserProfileManager userProfileManager = null;
            DadosProfile dadosProfile = null;
            try
            {
                spServiceContext = SPServiceContext.GetContext(SPContext.Current.Site);
                userProfileManager = new UserProfileManager(spServiceContext);
                dadosProfile = ManipularDados.BuscaDadosColaborador(matriculaFucionario, codigoColigada, userProfileManager, BaseDados.GetConnectionUP());

                if (dadosProfile != null)
                {
                    return dadosProfile.Foto;
                }

                return SPContext.Current.Site.Url + "/_layouts/images/O14_person_placeHolder_96.png"; // Imagem padrão.
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);

                return SPContext.Current.Site.Url + "/_layouts/images/O14_person_placeHolder_96.png"; // Imagem padrão.
            }
            finally
            {
                spServiceContext = null;
                userProfileManager = null;
                dadosProfile = null;
            }
        }

        private string ExtraiColigadaCentroCusto(string centroCusto)
        {
            try
            {
                return FormDAL.GetCodigoColigada(centroCusto);
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }

            return string.Empty;
        }

        protected void dropDownListCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reportViewerPremios.Visible = false;
            this.tdGraficoEvento.Visible = false;

            if (this.dropDownListCentroCusto.SelectedValue != "0")
            {
                CarregarFuncionarios(this.dropDownListCentroCusto.SelectedValue);
                this.dropDownListFuncionarios.Enabled = true;
            }
            else
            {
                this.dropDownListFuncionarios.SelectedValue = "0";
                this.dropDownListFuncionarios.Enabled = false;
            }
        }

        protected void dropDownListFuncionarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reportViewerPremios.Visible = false;

            if (this.dropDownListFuncionarios.SelectedValue != "0")
            {
                CarregarEventos();
                CarregarRelatorio(0);

                this.tdGraficoEvento.Visible = true;
                this.dropDownListEventos.Enabled = true;
            }
            else
            {
                this.reportViewerPremios.Visible = false;
                this.tdGraficoEvento.Visible = false;
                this.dropDownListEventos.Enabled = false;
            }
        }

        protected void dropDownListEventos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.dropDownListEventos.SelectedValue != "0")
            {
                this.dropDownListEventos.Items[0].Text = "Resumo";
                CarregarRelatorio(3);
            }
            else
            {
                CarregarRelatorio(0);
            }
        }

        protected void reportViewerPremios_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CarregarRelatorio(0);
        }
    }
}
