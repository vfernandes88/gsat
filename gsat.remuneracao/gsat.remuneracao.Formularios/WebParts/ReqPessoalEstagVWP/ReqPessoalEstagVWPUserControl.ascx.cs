using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Microsoft.SharePoint;
using Cit.Globosat.Common;
using System.Diagnostics;
using CIT.Sharepoint.Util;
using System.Configuration;
using Cit.Globosat.Remuneracao.Formularios.Entidades;
using System.Text;
using System.Data;
using Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo;
using Winnovative.WnvHtmlConvert;
using Globosat.Library.Servicos;

namespace Cit.Globosat.Remuneracao.Formularios.WebParts.ReqPessoalEstagVWP
{
    public partial class ReqPessoalEstagVWPUserControl : UserControl
    {
        public bool PDFButtonVisible { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarDados();
                DesabilitarCampos();
                this.imageButtonGerarPDF.Visible = this.PDFButtonVisible;
            }
        }

        private void CarregarDados()
        {
            try
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                    {
                        bool isAdministrator = false;
                        isAdministrator = BLL.AltFuncCargo.BLL.UserExistsInList(spSite, spWebRemuneracoes, spWebRemuneracoes.CurrentUser.LoginName,
                                            Constants.AdministradoresRemuneracaolistName);

                        //this.labelNomeRequisitante.Text = spWebRemuneracoes.CurrentUser.Name;

                        Entidades.DadosProfile dados = new Entidades.DadosProfile();
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

                        if (dados != null)
                        {
                            // Popula logo no Formulário.
                            PopularImagemLogo(dados.Coligada);
                        }
                        else
                        {
                            // Popula logo no Formulário.
                            PopularImagemLogo("0");
                        }

                        this.dateTimeControlRequisicao.SelectedDate = DateTime.Now;
                        CarregarCentroCusto(dados.Matricula, dados.Coligada);

                        if (dropDownListCentroCusto.Items.Count < 2)
                            dropDownListCentroCusto.Items.Add(dados.CentroCusto);
                        this.textBoxRequisitante.Text = spWebRemuneracoes.CurrentUser.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
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

        public void CarregarCentroCusto(string matricula, string coligada)
        {
            DataTable tableCentroCusto = null;
            try
            {
                this.dropDownListCentroCusto.Items.Clear();
                tableCentroCusto = new DataTable();

                // Verifica se é administrador.
                if (matricula == "00000")
                {
                    tableCentroCusto = ManipularDados.BuscaTodosCentrosCustoAtivos();
                }
                else
                {
                    // Busca todos os centros de custo do Gerente.
                    tableCentroCusto = ManipularDados.BuscaCentroCustoAtivos(matricula, coligada);
                }

                this.dropDownListCentroCusto.DataValueField = "CODSECAO_ESTADO_COLIGADA";
                this.dropDownListCentroCusto.DataTextField = "COD_DESC";
                this.dropDownListCentroCusto.DataSource = tableCentroCusto;
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
                tableCentroCusto.Dispose();
            }
        }

        private void LimparCampos()
        {
            this.textBoxDepartamentoArea.Text = string.Empty;
            this.textBoxDiretoria.Text = string.Empty;
            this.textBoxValorAuxilioBolsa.Text = string.Empty;
            this.radioButtonListNivel.ClearSelection();
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

            this.radioButtonListNivel.Enabled = true;
            this.radioButtonListNivel.ClearSelection();
            this.textBoxValorAuxilioBolsa.Text = string.Empty;
        }

        private void DesabilitarCampos()
        {
            this.imageButtonImprimir.Enabled = false;
            this.imageButtonImprimir.ImageUrl = "~/_layouts/images/Cit.Globosat.Base/print_icon_disable.jpg";
            this.imageButtonImprimir.ToolTip = "É preciso preencher o formulário!";
            this.imageButtonGerarPDF.Enabled = false;
            this.imageButtonGerarPDF.ImageUrl = "~/_layouts/images/Cit.Globosat.Base/pdf_icon_disable.jpg";
            this.imageButtonGerarPDF.ToolTip = "É preciso preencher o formulário!";

            this.radioButtonListNivel.Enabled = false;
        }

        protected void imageButtonGerarPDF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string downloadName = string.Format("{0}_{1}.{2}", "FormReqPessoalEstag", DateTime.Now.ToShortDateString().Replace("/", "_") + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_"), "pdf");
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

        protected void imageButtonVoltar_Click(object sender, ImageClickEventArgs e)
        {
            SPUtility.Redirect("/remuneracoes", SPRedirectFlags.Default, HttpContext.Current);
        }

        protected void dropDownListCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dropDownListCentroCusto.SelectedValue != "0")
                {
                    string codSecao = this.dropDownListCentroCusto.SelectedValue.Split('_')[0];
                    int codigoColigada = Convert.ToInt32(this.dropDownListCentroCusto.SelectedValue.Split('_')[2]);
                    
                    DataTable dataTable = DAL.ReqPessoalEstag.ReqPessoalEstag.GetDados(codSecao, codigoColigada);
                    this.textBoxDepartamentoArea.Text = dataTable.Rows[0]["DEPARTAMENTO"].ToString();
                    this.textBoxDiretoria.Text = dataTable.Rows[0]["ENDERECOPAGTO"].ToString();
                    PopularImagemLogo(dataTable.Rows[0]["CODCOLIGADA"].ToString());
                    
                    HabilitarCampos();
                }
                else
                {
                    PopularImagemLogo("1"); // Alterar para imagem default.
                    LimparCampos();
                    DesabilitarCampos();
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void radioButtonListNivel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.radioButtonListNivel.SelectedValue != string.Empty)
            {
                string filial = this.dropDownListCentroCusto.SelectedValue.Split('_')[1];
                int codigoColigada = Convert.ToInt32(this.dropDownListCentroCusto.SelectedValue.Split('_')[2]);

                // G2C só tem em SP mas codigo filial é 1.
                int codFilial = 1;
                if (!filial.ToUpper().Equals("RJ"))
                    codFilial = 2;

                bool nivelTecnico = false;
                if (this.radioButtonListNivel.SelectedValue.ToUpper().Equals("MEDIO"))
                    nivelTecnico = true;

                this.textBoxValorAuxilioBolsa.Text = DAL.ReqPessoalEstag.ReqPessoalEstag.GetValorAuxilioBolsa(codigoColigada, codFilial, nivelTecnico);
            }
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
            script.Append("     printWindow.document.write('<link rel=\"stylesheet\" type=\"text/css\" href=\"/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoalEstag/ReqPessoalEstag.css\" />');");
            script.Append("     printWindow.document.write('<link rel=\"stylesheet\" type=\"text/css\" href=\"/_layouts/Cit.Globosat.Remuneracao.Formularios/CSS/ReqPessoalEstag/PrintOnlyForm.css\" media=\"print\" />');");
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
