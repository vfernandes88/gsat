using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Configuration;
using Microsoft.SharePoint;
using Cit.Globosat.Common;
using System.Diagnostics;
using System.Data;
using Cit.Globosat.Remuneracao.Formularios.DAL.ReqPessoal;
using Cit.Globosat.Remuneracao.Formularios.Entidades;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CIT.Sharepoint.Util;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo;
using Winnovative.WnvHtmlConvert;
using Globosat.Library.Servicos;

namespace Cit.Globosat.Remuneracao.Formularios.WebParts.ReqPessoalVWP
{
    public partial class ReqPessoalVWPUserControl : UserControl
    {
        public bool PDFButtonVisible { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Seta valor no campo classe definido por scrip jquery.
                if (!string.IsNullOrEmpty(this.tbCargoPreencher.Text))
                {
                    lblClasse.Text = this.hiddenField_tb_Classe.Value;
                }

                if (!IsPostBack)
                {
                    Entidades.DadosProfile dados = new Entidades.DadosProfile();

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                    {
                        #region Producao

                        using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                            {
                                bool isAdministrator = false;
                                isAdministrator = BLL.AltFuncCargo.BLL.UserExistsInList(spSite, spWebRemuneracoes, spWebRemuneracoes.CurrentUser.LoginName,
                                                    Constants.AdministradoresRemuneracaolistName);

                                this.lblRequisitante.Text = spWebRemuneracoes.CurrentUser.Name;

                                
                                if (isAdministrator)
                                {
                                    dados.Coligada = "1";
                                    dados.Matricula = "00000";
                                    dados.Classe = "I";
                                    dados.FaixaSalarial = 21;
                                }
                                else
                                {
                                    // Busca dados do colaborador logado.
                                    dados = BLL.AltFuncCargo.BLL.BuscaDadosUserProfile(spSite, spWebRemuneracoes.CurrentUser.LoginName);

                                }
                                // Seta dados nos campos invisiveis: nivel e classe 
                                this.hiddenField_tb_Nivel.Value = dados.FaixaSalarial.ToString();
                                this.hiddenField_tb_Classe.Value = dados.Classe;

                                // Popula dados no Forumulário.
                                PopularColigadaMatricula(dados);


                                // Popula combobox classe
                                this.ddlSalNivel.Items.Clear();
                                PopularClasses(dados.Classe, dados.FaixaSalarial.ToString("00"));

                                if (dados != null)
                                {
                                    // Insere logo no Forumulario
                                    PopularImagemLogo(dados.Coligada);
                                }
                                else
                                {
                                    PopularImagemLogo("0");
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Desenvolvimento

                        using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                            {
                                dados = new Entidades.DadosProfile();
                                dados.Coligada = "1";
                                dados.Matricula = "00000";
                                dados.Classe = "I";
                                dados.FaixaSalarial = 21;

                                // Seta dados em campos invisiveis... Nivel e Classe.
                                this.hiddenField_tb_Nivel.Value = dados.FaixaSalarial.ToString();
                                this.hiddenField_tb_Classe.Value = dados.Classe;

                                // Popula dados no formulário.
                                PopularColigadaMatricula(dados);

                                // Popular ComboBox de Classe.
                                this.ddlSalNivel.Items.Clear();
                                PopularClasses(dados.Classe, dados.FaixaSalarial.ToString("00"));

                                if (dados != null)
                                {
                                    // Popula logo no Formulário.
                                    PopularImagemLogo("3");
                                }
                                else
                                {
                                    // Popula logo no Formulário.
                                    PopularImagemLogo("3");
                                }

                                lblRequisitante.Text = spWebRemuneracoes.CurrentUser.Name;
                            }
                        }
                        #endregion
                    }
                    PreencherCentroCusto(this.textBoxDiretoriaArea.Text, this.hiddenField_tb_Coligada.Value);

                    if (ddlCentroCusto.Items.Count == 1)
                    {
                        ddlCentroCusto.Items.Add(dados.CentroCusto);
                    }
                    DesabilitarCampos();
                    this.imageButtonGerarPDF.Visible = this.PDFButtonVisible;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void imageButtonGerarPDF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string downloadName = string.Format("{0 }_{1}.{2}", "FormSolReqPessoal", DateTime.Now.ToShortDateString().Replace("/", "_") + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_"), "pdf");
                string urlBase = string.Format("http://{0}:{1}", ConfigurationManager.AppSettings["Server:Name"], ConfigurationManager.AppSettings["Server:Port"]);

                PdfConverter pdfConverter = Utility.GetPdfConverter();
                byte[] downloadBytes = pdfConverter.GetPdfBytesFromHtmlString(this.hiddenFieldPDF.Value.Trim().Replace(SPContext.Current.Site.Url, urlBase), SPContext.Current.Site.Url);

                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();

                Utility.SetEncoding(response);
                response.AddHeader("Content-Type", "binary/octet-stream");
                response.AddHeader("Content-Disposition",
                    "attachment; filename=" + downloadName + "; size=" + downloadBytes.Length.ToString());
                response.Flush();
                response.BinaryWrite(downloadBytes);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        /// <summary>
        /// Insere os dados coletados no formulário para visualização
        /// </summary>
        /// <param name="dados">Matricula e coligada do Gerente</param>
        private void PopularColigadaMatricula(Entidades.DadosProfile dados)
        {
            try
            {
                if (dados != null)
                {
                    // Coloca valor de coligada em campo no form (Campo Invisível).
                    this.hiddenField_tb_Coligada.Value = dados.Coligada;
                    this.textBoxDiretoriaArea.Text = dados.Matricula;
                    this.textBoxDiretoriaArea.Visible = false;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        /// <summary>
        /// Popula ComboBox com Classes disponíveis
        /// </summary>
        /// <param name="classe"></param>
        /// <param name="nivel"></param>
        /// Andre
        public void PopularClasses(string classe, string nivel)
        {
            try
            {
                this.ddlSalNivel.Items.Clear();

                // Lista classes existentes.
                char[] classes = "ABCDEFGHI".ToCharArray();

                foreach (char item in classes)
                {
                    this.ddlSalNivel.Items.Add(new ListItem(item.ToString(), item.ToString()));
                }

                //this.ddlSalNivel.Items.Insert(0, new ListItem("...", "0"));
                this.ddlSalNivel.Items.Insert(this.ddlSalNivel.Items.Count, new ListItem("I*", "I*"));
                this.ddlSalNivel.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        /// <summary>
        /// Adiciona centro de custo em dropdown do Formulário
        /// </summary>
        /// <param name="matricula">Matrícula do gerente</param>
        public void PreencherCentroCusto(string matricula, string coligada)
        {
            DataTable tableCentroCusto = null;
            try
            {
                this.ddlCentroCusto.Items.Clear();
                tableCentroCusto = new DataTable();

                // Verifica se é administrador.
                if (matricula == "00000")
                {
                    tableCentroCusto = ManipularDados.BuscaTodosCentrosCustoAtivosD();
                }
                else
                {
                    // Busca todos os centros de custo do Gerente.
                    tableCentroCusto = ManipularDados.BuscaCentroCustoAtivosD(matricula, coligada);
                }

                this.ddlCentroCusto.DataValueField = "CODSECAO_ESTADO";
                this.ddlCentroCusto.DataTextField = "COD_DESC";
                this.ddlCentroCusto.DataSource = tableCentroCusto;
                this.ddlCentroCusto.DataBind();
                this.ddlCentroCusto.Items.Insert(0, new ListItem("Selecione...", "0"));
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
            finally
            {
                tableCentroCusto.Dispose();
            }
        }

        private void PopularImagemLogo(string coligada)
        {
            try
            {
                switch (coligada)
                {
                    case "2":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/CanaisLogo/Medio/telecine.jpg";
                        break;

                    case "3":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/CanaisLogo/Medio/Universal.jpg";
                        break;

                    case "4":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/CanaisLogo/Medio/CBrasil.jpg";
                        break;

                    case "5":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/CanaisLogo/Medio/g2c.jpg";
                        break;

                    case "6":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/CanaisLogo/Medio/Playboy_PB.jpg";
                        break;

                    case "7":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/CanaisLogo/Medio/Horizonte.jpg";
                        break;

                    default:
                        this.imageLogo.ImageUrl = "/CanaisLogo/Medio/Globosat.jpg";
                        break;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }
            private string ExtraiColigadaCentroCusto(string centroCusto)
        {
            try
            {
                return FormDAL.GetCodigoColigadaRP(centroCusto);
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }

            return string.Empty;
        }

        protected void ddlCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LimparCampos();

                if (this.ddlCentroCusto.SelectedValue != "0")
                {
                    PreencheDiretoria(this.ddlCentroCusto.SelectedValue.Split('_')[0]);
                    this.lblDataRequisicao.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    this.btnBuscar.Enabled = true;
                    this.hiddenField_coligadaCentroCusto.Value = ExtraiColigadaCentroCusto(this.ddlCentroCusto.SelectedValue.Split('_')[0]);

                    if (this.ddlCentroCusto.SelectedValue.ToUpper().Contains("RJ"))
                    {
                        this.rbFilial.SelectedValue = "RJ";
                    }
                    else if (this.ddlCentroCusto.SelectedValue.ToUpper().Contains("SP"))
                    {
                        this.rbFilial.SelectedValue = "SP";
                    }

                    HabilitarCampos();

                    // Alterar a imagem de acordo com a coligada do centro de custo escolhido.
                    PopularImagemLogo(this.hiddenField_coligadaCentroCusto.Value);
                }
                else
                {
                    using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                    {
                        using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                        {
                            bool isAdministrator = false;
                            isAdministrator = BLL.AltFuncCargo.BLL.UserExistsInList(SPContext.Current.Site, spWebRemuneracoes, spWebRemuneracoes.CurrentUser.LoginName,
                                                Constants.AdministradoresRemuneracaolistName);

                            if (isAdministrator)
                            {
                                // Alterar para imagem default.
                                PopularImagemLogo("1");
                            }
                            else
                            {
                                // Busca dados do colaborador logado.
                                Entidades.DadosProfile dadosProfile = BLL.AltFuncCargo.BLL.BuscaDadosUserProfile(SPContext.Current.Site, spWebRemuneracoes.CurrentUser.LoginName);

                                // Alterar a imagem de acordo com a coligada do usuário corrente.
                                PopularImagemLogo(dadosProfile.Coligada);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        private void PreencheDiretoria(string centroCusto)
        {
            DataTable dtDiretoria = DAL.ReqPessoal.ReqPessoalFormDAL.GetDiretoria(centroCusto);
            if ((dtDiretoria != null) && (dtDiretoria.Rows.Count > 0))
                lblDiretoria.Text = dtDiretoria.Rows[0]["DIRETORIA"].ToString().Trim();
        }

        private void DesabilitarCampos()
        {
            this.imageButtonImprimir.Enabled = false;
            this.imageButtonGerarPDF.Enabled = false;
            this.tbCargoPreencher.Attributes.Add("readonly", "readonly");

            this.btnBuscar.Enabled = false;
            this.dtDataInicio.Enabled = false;
            ((TextBox)this.dtDataInicio.Controls[0]).Attributes.Add("readonly", "readonly");
            this.textBoxFuncSubstituido.Attributes.Add("readonly", "readonly");
            this.rbMotivo.Attributes.Add("readonly", "readonly");
            this.rbTipoContrato.Attributes.Add("readonly", "readonly");
            this.dtPrazoDeterminado.Enabled = false;
            ((TextBox)this.dtPrazoDeterminado.Controls[0]).Attributes.Add("readonly", "readonly");
            this.dtTemporario.Enabled = false;
            ((TextBox)this.dtTemporario.Controls[0]).Attributes.Add("readonly", "readonly");
            this.lblDiretoria.Attributes.Add("readonly", "readonly");
            this.tbCandidatoSelecionado.Attributes.Add("readonly", "readonly");
            this.rbOrcado.Attributes.Add("readonly", "readonly");
            this.tbSalario.Attributes.Add("readonly", "readonly");
            this.ddlSalNivel.Enabled = false;
            this.rbJornada.Attributes.Add("readonly", "readonly");
            this.rbFilial.Attributes.Add("readonly", "readonly");
            this.tbObservacao.Attributes.Add("readonly", "readonly");
            this.tbResumoResponsabilidades.Attributes.Add("readonly", "readonly");
            this.tbObservacao.Attributes.Add("readonly", "readonly");
            this.tbJustificativa.Attributes.Add("readonly", "readonly");
            this.tbParecerRH.Attributes.Add("readonly", "readonly");
            this.tbParecerRemuneracao.Attributes.Add("readonly", "readonly");
            this.dtAssRequisitante.Enabled = false;
            ((TextBox)this.dtAssRequisitante.Controls[0]).Attributes.Add("readonly", "readonly");
            this.dtAssDiretoriaArea.Enabled = false;
            ((TextBox)this.dtAssDiretoriaArea.Controls[0]).Attributes.Add("readonly", "readonly");
            this.dtAssRH.Enabled = false;
            ((TextBox)this.dtAssRH.Controls[0]).Attributes.Add("readonly", "readonly");
            this.dtAssDiretoriaGestao.Enabled = false;
            ((TextBox)this.dtAssDiretoriaGestao.Controls[0]).Attributes.Add("readonly", "readonly");
        }

        public void LimparCampos()
        {
            try
            {
                this.lblDataRequisicao.Text = DateTime.Now.ToString();
                this.lblDiretoria.Text = string.Empty;
                this.tbCargoPreencher.Text = string.Empty;
                this.dtDataInicio.ClearSelection();
                this.textBoxFuncSubstituido.Text = string.Empty;
                this.rbJornada.ClearSelection();
                this.rbFilial.ClearSelection();
                this.rbMotivo.ClearSelection();
                this.rbOrcado.ClearSelection();
                this.rbTipoContrato.ClearSelection();
                this.rbTipoVaga.ClearSelection();
                this.tbSalario.Text = string.Empty;
                this.lblClasse.Text = string.Empty;
                this.ddlSalNivel.ClearSelection();
                this.tbObservacao.Text = string.Empty;
                this.tbJustificativa.Text = string.Empty;
                this.tbParecerRemuneracao.Text = string.Empty;
                this.tbParecerRH.Text = string.Empty;
                this.tbResumoResponsabilidades.Text = string.Empty;
                this.hiddenField_strJornada.Value = string.Empty;
                this.hiddenField_tb_DepartamentoArea.Value = string.Empty;
                this.hiddenField_tb_CentroCusto.Value = string.Empty;
                this.hiddenField_tb_Funcionario.Value = string.Empty;
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        private void HabilitarCampos()
        {
            this.imageButtonImprimir.Enabled = true;
            this.imageButtonImprimir.ImageUrl = "~/_layouts/images/Cit.Globosat.Base/print_icon.jpg";
            this.imageButtonImprimir.ToolTip = "Clique aqui para imprimir o formulário.";
            this.imageButtonGerarPDF.Enabled = true;
            this.imageButtonGerarPDF.ImageUrl = "~/_layouts/images/Cit.Globosat.Base/pdf_icon.jpg";
            this.imageButtonGerarPDF.ToolTip = "Clique aqui para gerar o arquivo PDF do formulário.";
            this.imageButtonGerarPDF.Visible = this.PDFButtonVisible;

            this.btnBuscar.Enabled = true;
            this.dtDataInicio.Enabled = true;
            this.textBoxFuncSubstituido.Attributes.Remove("readonly");
            this.dtPrazoDeterminado.Enabled = true;
            this.dtTemporario.Enabled = true;
            this.tbCandidatoSelecionado.Attributes.Remove("readonly");
            this.ddlSalNivel.Enabled = true;
            this.rbJornada.Attributes.Remove("readonly");
            this.rbFilial.Attributes.Remove("readonly");
            this.tbObservacao.Attributes.Remove("readonly");
            this.tbResumoResponsabilidades.Attributes.Remove("readonly");
            this.tbObservacao.Attributes.Remove("readonly");
            this.tbJustificativa.Attributes.Remove("readonly");
            this.dtAssRequisitante.Enabled = true;
            this.dtAssDiretoriaArea.Enabled = true;
            this.dtAssRH.Enabled = true;
            this.dtAssDiretoriaGestao.Enabled = true;
        }

        protected void rbTipoContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbTipoContrato.SelectedValue.Equals("indeterminado"))
            {
                dtPrazoDeterminado.ClearSelection();
                dtPrazoDeterminado.Enabled = false;

                dtTemporario.ClearSelection();
                dtTemporario.Enabled = false;
            }
            else if (rbTipoContrato.SelectedValue.Equals("determinado"))
            {
                dtPrazoDeterminado.Enabled = true;
                dtPrazoDeterminado.Focus();

                dtTemporario.ClearSelection();
                dtTemporario.Enabled = false;
            }
            else
            {
                dtPrazoDeterminado.Enabled = false;
                dtPrazoDeterminado.ClearSelection();

                dtTemporario.Enabled = true;
                dtTemporario.Focus();
            }
        }

        private void PopularSalarioProposto(string classe, string nivel, string jornada, string filial, string coligadaGerente)
        {
            try
            {
                string salarioProposto = string.Empty;
                salarioProposto = BLL.ReqPessoal.ReqPessoalBLL.BuscaSalario(classe, nivel, jornada, filial, coligadaGerente);
                this.tbSalario.Text = salarioProposto;

            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void imageButtonVoltar_Click(object sender, ImageClickEventArgs e)
        {
            SPUtility.Redirect("/remuneracoes", SPRedirectFlags.Default, HttpContext.Current);
        }

        protected void imageButtonImprimir_Click(object sender, ImageClickEventArgs e)
        {
            // Código transferido do ascx p/ o code behind pelo fato de não está levando os
            // valores escolhidos nos campos de raddionbuttonlist.
            StringBuilder script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" language=\"javascript\">");
            script.Append(" $(document).ready(function () {");
            script.Append("     var windowUrl = '';");
            script.Append("     var uniqueName = new Date();");
            script.Append("     var windowName = 'Print';");
            script.Append("     var printWindow = window.open(windowUrl, windowName, 'resizable=yes,location=0,top=0,scrollbars=auto,width=0,height=0');");
            script.Append("     printWindow.document.write('<HTML><Head><Title></Title>');");
            script.Append("     printWindow.document.write('<link rel=\"stylesheet\" type=\"text/css\" href=\"/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoal/ReqPessoal.css\" />');");
            script.Append("     printWindow.document.write('<link rel=\"stylesheet\" type=\"text/css\" href=\"/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoal/PrintOnlyForm.css\" media=\"print\" />');");
            script.Append("     printWindow.document.write('</Head><Body>');");
            script.Append("     printWindow.document.write($('#divForm').html());");
            script.Append("     printWindow.document.write('</Body></HTML>');");
            script.Append("     printWindow.document.close();");
            script.Append("     printWindow.focus();");
            script.Append("     printWindow.print();");
            script.Append("     printWindow.close();");
            script.Append(" });");
            script.Append("</script>");

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "doOpenPdf", script.ToString());
        }
    }
}
