using System;
using Microsoft.SharePoint.WebControls;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections.Generic;
using Globosat.Library.Entidades;
using Globosat.Library.Servicos;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using Winnovative.WnvHtmlConvert;
using System.Web;
using System.Configuration;
using Globosat.Library.AcessoDados;

namespace Globosat.Remuneracao.EvolucaoSalarial.Layouts.EvolucaoSalarial
{
    public partial class MaisDetalhes : LayoutsPageBase
    {
        string matriculaColaborador = string.Empty;
        string coligada = string.Empty;
        string acordoColetivo = string.Empty;
        List<Funcionario> listaHistoricoSalarial = null;
        string nomeColaborador = string.Empty;
        string pdf = string.Empty;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Busca matrícula em queryString
                matriculaColaborador = Request.QueryString["Matricula"];

                if (this.ViewState["matricula"] != null)
                    matriculaColaborador = this.ViewState["matricula"].ToString();

                //Busca coligada em queryString
                coligada = Request.QueryString["Coligada"];

                //Busca acordo coletivo em queryString
                acordoColetivo = Request.QueryString["AC"];

                //Buscar PDF em queryString
                pdf = Request.QueryString["PDF"];

                if (!IsPostBack)
                {
                    #region PreencheDropDown

                    ddlFuncionarios.AutoPostBack = true;
                    DataTable dtFuncionarios = new DataTable();
                    dtFuncionarios = AcessoDados.GetFuncionariosEvolucaoSalarial(matriculaColaborador, coligada);
                    if (dtFuncionarios != null)
                    {
                        foreach (DataRow funcionario in dtFuncionarios.Rows)
                            ddlFuncionarios.Items.Add(new ListItem(funcionario["NOME"].ToString(), funcionario["CHAPA"].ToString()));
                    }

                    //Seta usuário
                    ddlFuncionarios.SelectedValue = matriculaColaborador;

                    #endregion
                }

                if (!string.IsNullOrEmpty(matriculaColaborador))
                {
                    //Popula label com matrícula do colaborador
                    lblMatricula.Text = matriculaColaborador;

                    //Popula label com nome do colaborador
                    lblNome.Text = nomeColaborador = ManipularDados.BuscaNomeColaborador(coligada, matriculaColaborador);

                    listaHistoricoSalarial = new List<Funcionario>();
                    if (acordoColetivo.Equals("1"))
                        cbAcordoColetivo.Checked = true;

                    if (cbAcordoColetivo.Checked || acordoColetivo.Equals("1"))
                    {
                        //Busca dados tratados para exibir no grid
                        listaHistoricoSalarial = ManipularDados.PopularGridView(matriculaColaborador, coligada, true);
                    }
                    else
                    {
                        //Busca dados tratados para exibir no grid
                        listaHistoricoSalarial = ManipularDados.PopularGridView(matriculaColaborador, coligada, false);
                    }

                    GVMaisInf.DataSource = listaHistoricoSalarial;
                    GVMaisInf.DataBind();

                    //Cria gráfico com dados importados.
                    CriarGrafico(listaHistoricoSalarial);
                    
                    if (pdf.Equals("1"))
                    {
                        lblFuncionarios.Visible = false;
                        ddlFuncionarios.Visible = false;
                    }
                }
                else
                {
                    lblError.Text = "Erro ao visualizar página. Entre em contato com o Administrador.";
                    lblError.Visible = true;
                }

                if (IsPostBack)
                {
                    CloseModalMessage();
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                     Cit.Globosat.Common.Utility.GetCurrentMethod(), Cit.Globosat.Common.Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        /// <summary>
        /// Cria gráfico para ser exibido
        /// </summary>
        /// <param name="todosDadosFunc">Lista com todos os dados do funcionário</param>
        public void CriarGrafico(List<Funcionario> todosDadosFunc)
        {
            decimal maximoPercentual = 0;
            try
            {
                ChartEvolucao.Series[0].ToolTip = "Data= #VALX\nPercentual= #VALY%";
                ChartEvolucao.DataSource = todosDadosFunc;
                ChartEvolucao.DataBind();
                ChartEvolucao.ChartAreas[0].AxisY.LabelStyle.Format = "{0.##}%";

                if (todosDadosFunc.Count > 1)
                    ChartEvolucao.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                else
                    ChartEvolucao.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Auto;

                ChartEvolucao.ChartAreas[0].AxisX.LabelStyle.Angle = 60;
                ChartEvolucao.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8);
                ChartEvolucao.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 9);
                ChartEvolucao.AntiAliasing = AntiAliasingStyles.All;

                for (int i = 0; i <= todosDadosFunc.Count - 1; i++)
                {
                    if (todosDadosFunc[i].PercentualNumber > maximoPercentual)
                        maximoPercentual = todosDadosFunc[i].PercentualNumber;

                    ChartEvolucao.Series[0].Points[i].Label = todosDadosFunc[i].Percentual;
                }

                ChartEvolucao.ChartAreas[0].AxisY.Minimum = 0;
                ChartEvolucao.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(maximoPercentual + 10);
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao gerar gráfico: " + ex.Message, EventLogEntryType.Error, 2, 1);
                throw;
            }
        }

        public void btnEmail_Click(object sender, ImageClickEventArgs e)
        {
            string email = string.Empty;
            try
            {
                email = ManipularDados.EnviarConteudoEmail(listaHistoricoSalarial, "630x300", matriculaColaborador, nomeColaborador);
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao enviar email em Mais Detalhes: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                SPUtility.TransferToErrorPage("Ocorreu um erro ao enviar o email.", null, null);
            }

            Email.EnvioEmail(SPContext.Current.Web.CurrentUser.Email, "Evolução Salarial - " + listaHistoricoSalarial[0].Nome, email);
            CloseModalMessage();
            this.ClientScript.RegisterStartupScript(this.GetType(), "EnviarEmail", "<script language='javascript'>window.alert('O email foi enviado com sucesso!'); SP.UI.ModalDialog.commonModalDialogClose();</script>");
        }

        public void btnPDF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string urlBase = string.Format("http://{0}:{1}", ConfigurationManager.AppSettings["Server:Name"], ConfigurationManager.AppSettings["Server:Port"]);
                string downloadName = matriculaColaborador+".pdf";
                string urlToConvert = string.Empty;                

                if (!cbAcordoColetivo.Checked)
                {
                    urlToConvert = string.Format("{0}{1}?Matricula={2}&Coligada={3}&AC=0&PDF=1&IsDlg=1", urlBase, Request.Url.AbsolutePath, matriculaColaborador, coligada);
                }
                else
                {
                    urlToConvert = string.Format("{0}{1}?Matricula={2}&Coligada={3}&AC=1&PDF=1&IsDlg=1", urlBase, Request.Url.AbsolutePath, matriculaColaborador, coligada);
                }
                
                byte[] downloadBytes = null;
                PdfConverter pdfConverter = GetPdfConverter();
                downloadBytes = pdfConverter.GetPdfBytesFromUrl(urlToConvert);

                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                SetEncoding(response);
                response.AddHeader("Content-Type", "binary/octet-stream");
                response.AddHeader("Content-Disposition", "attachment; filename=" + downloadName + "; size=" + downloadBytes.Length.ToString());
                response.Flush();
                response.BinaryWrite(downloadBytes);                
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao gerar o arquivo PDF em Mais Detalhes: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                SPUtility.TransferToErrorPage("Ocorreu um erro ao gerar o arquivo PDF. Tente novamente mais tarde.", null, null);
            }
        }

        #region Métodos auxiliares
        /// <summary>
        /// Seleciona os parâmetros do conversor
        /// </summary>
        /// <returns></returns>
        private PdfConverter GetPdfConverter()
        {
            try
            {
                PdfConverter pdfConverter = new PdfConverter();
                pdfConverter.LicenseKey = "GjEoOis6KCoqOiw0KjopKzQrKDQjIyMj";
                pdfConverter.PageWidth = 1300;

                // set if the generated PDF contains selectable text or an embedded image - default value is true
                pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;

                //set the PDF page size 
                pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;

                // set the PDF compression level
                pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;

                // set the PDF page orientation (portrait or landscape)
                pdfConverter.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Landscape;

                //set the PDF standard used to generate the PDF document
                pdfConverter.PdfStandardSubset = PdfStandardSubset.Full;

                // show or hide header and footer
                pdfConverter.PdfDocumentOptions.ShowHeader = false;
                pdfConverter.PdfDocumentOptions.ShowFooter = false;

                //set the PDF document margins
                pdfConverter.PdfDocumentOptions.LeftMargin = 0;
                pdfConverter.PdfDocumentOptions.RightMargin = 0;
                pdfConverter.PdfDocumentOptions.TopMargin = 0;
                pdfConverter.PdfDocumentOptions.BottomMargin = 0;

                // set if the HTTP links are enabled in the generated PDF
                pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;

                // set if the HTML content is resized if necessary to fit the PDF page width - default is true
                pdfConverter.PdfDocumentOptions.FitWidth = true;

                // set if the PDF page should be automatically resized to the size of the HTML content when FitWidth is false
                pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;

                // embed the true type fonts in the generated PDF document
                pdfConverter.PdfDocumentOptions.EmbedFonts = false;

                // compress the images in PDF with JPEG to reduce the PDF document size - default is true
                pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;

                // set if the JavaScript is enabled during conversion 
                pdfConverter.ScriptsEnabled = true;

                // set if the converter should try to avoid breaking the images between PDF pages
                pdfConverter.AvoidImageBreak = true;

                pdfConverter.PdfBookmarkOptions.TagNames = null;

                return pdfConverter;
            }
            catch (Exception ex)
            {
                Logger.Write("Houve um erro ao gerar o arquivo PDF. Detalhes = " + ex.Message, System.Diagnostics.EventLogEntryType.Error, 2, 1);
                return null;
            }
        }

        /// <summary>
        /// Seta a codificação
        /// </summary>
        /// <param name="response"></param>
        private void SetEncoding(HttpResponse response)
        {
            response.HeaderEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1"); ;
            response.ContentEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
        }

        protected void dllFuncionarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ViewState["matricula"] = ((DropDownList)sender).SelectedItem.Value.Trim();

            #region Teste
            matriculaColaborador = ((DropDownList)sender).SelectedItem.Value.Trim();

            if (!string.IsNullOrEmpty(matriculaColaborador))
            {
                //Popula label com matrícula do colaborador
                lblMatricula.Text = matriculaColaborador;

                //Popula label com nome do colaborador
                lblNome.Text = nomeColaborador = ManipularDados.BuscaNomeColaborador(coligada, matriculaColaborador);

                listaHistoricoSalarial = new List<Funcionario>();
                if (acordoColetivo.Equals("1"))
                    cbAcordoColetivo.Checked = true;

                if (cbAcordoColetivo.Checked || acordoColetivo.Equals("1"))
                {
                    //Busca dados tratados para exibir no grid
                    listaHistoricoSalarial = ManipularDados.PopularGridView(matriculaColaborador, coligada, true);
                }
                else
                {
                    //Busca dados tratados para exibir no grid
                    listaHistoricoSalarial = ManipularDados.PopularGridView(matriculaColaborador, coligada, false);
                }

                GVMaisInf.DataSource = listaHistoricoSalarial;
                GVMaisInf.DataBind();

                //Cria gráfico com dados importados.
                CriarGrafico(listaHistoricoSalarial);
            }
            #endregion

            CloseModalMessage();
        }
        #endregion

        private void CloseModalMessage()
        {
            string script = "<script language='javascript'>" +
                            "   if (window.frameElement != null) {" +
                            "       if (window.parent.waitDialog != null) {" +
                            "           window.parent.waitDialog.close();" +
                            "       }" +
                            "   }" +
                            "</script>";

            //Emit the script that will close the wait screen. We only need to do this when
            //the app page is in dialog mode, in which case window.frameElement will NOT be null
            this.ClientScript.RegisterStartupScript(this.GetType(), "CloseWaitDialog", script);
        }
    }
}
