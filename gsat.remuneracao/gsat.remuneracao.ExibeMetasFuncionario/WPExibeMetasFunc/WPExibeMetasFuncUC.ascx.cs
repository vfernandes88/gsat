using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.IO;
using Microsoft.Office.Server.UserProfiles;
using Globosat.Library.Servicos;
using Globosat.Library.Entidades;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Data.SqlClient;
using Globosat.Library.AcessoDados;
using System.Configuration;

namespace Globosat.Remuneracao.ExibeMetasFuncionario.WPExibeMetasFuncionario
{
    public partial class WPExibeMetasFuncUC : UserControl
    {
        public SiteLists Ano { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string strColigada = string.Empty;
            if (!IsPostBack)
            {
                try
                {
                    // Carrega os centros de custo utilizados em ambiente de desenvolvimento, 
                    // pois em produção este carregamento é realizado através de uma outra web part.
                    DataTable dtCentrosCusto = new DataTable();

                    if (!Convert.ToBoolean(ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                    {
                        #region TESTE
                        ddlCentroCusto.Items.Add(new ListItem("Selecione...", "0"));
                        ddlCentroCusto.Items.Add(new ListItem("3.201.100 - ADMINIST/FINANCEIRA", "3.201.100"));
                        ddlCentroCusto.AutoPostBack = true;
                        ddlAno.AutoPostBack = true;
                        ddlAno.Items.Add(new ListItem("Selecione...", "0"));

                        if (this.Ano == SiteLists.Ano_2012)
                        {
                            ddlAno.Items.Add(new ListItem("2012", "2012"));
                        }
                        else if (this.Ano == SiteLists.Ano_2013)
                        {
                            ddlAno.Items.Add(new ListItem("2013", "2013"));
                        }
                        else if (this.Ano == SiteLists.Ano_2014)
                        {
                            ddlAno.Items.Add(new ListItem("2014", "2014"));
                        }
                        else if (this.Ano == SiteLists.Ano_2013_2014)
                        {
                            ddlAno.Items.Add(new ListItem("2013", "2013"));
                            ddlAno.Items.Add(new ListItem("2014", "2014"));

                        }
                        else
                        {
                            ddlAno.Items.Add(new ListItem("2013", "2013"));
                            ddlAno.Items.Add(new ListItem("2014", "2014"));

                        }
                        #endregion
                    }
                    else
                    {
                        //Aqui a web part citada acima faz o carregamento dos centros de custo a partir do RM
                        #region Produção
                        //Carrega combobox de Ano com os anos os quais existe algum dado na lista de Metas
                        #region Preenche DropDownList de Ano
                        DataTable dtAnos = new DataTable();
                        dtAnos = PreencheAno();
                        ddlAno.Items.Add(new ListItem("Selecione...", "0"));
                        ddlAno.AutoPostBack = true;
                        if (dtAnos != null)
                        {
                            foreach (DataRow ano in dtAnos.Rows)
                            {
                                if (ano["Ano"].ToString().Trim() != string.Empty)
                                {
                                    // P.O. solicitou a remoção do ano de "2012"
                                    ddlAno.Items.Add(new ListItem(ano["Ano"].ToString().Trim(), ano["Ano"].ToString().Trim()));
                                }
                            }
                        }
                        //ddlAno.SelectedValue = DateTime.Now.Year.ToString();
                        #endregion

                        //Carrega Centros de custo
                        #region Preenche DropDownList de Centros de Custo
                        ddlCentroCusto.AutoPostBack = true;
                        //Get Centros de Custo do Usuário Logado
                        dtCentrosCusto = PreencheCentroCustosUsuario();

                        ddlCentroCusto.Items.Add(new ListItem("Selecione...", "0"));

                        if (dtCentrosCusto != null)
                        {
                            strColigada = dtCentrosCusto.Rows[0]["CODCOLIGADA"].ToString().Trim();
                            foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                                ddlCentroCusto.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                        }
                        #endregion
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Erro ao carregar os centros de custo: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);

                }
            }
            else
            {
                try
                {
                    //Inicio do Html
                    if ((this.ddlCentroCusto.SelectedItem.Value != null) && (this.ddlCentroCusto.SelectedValue != "0")
                        && (this.ddlAno.SelectedItem.Value != null) && (this.ddlAno.SelectedValue != "0"))
                    {
                        strColigada = ExtraiColigadaCentroCusto(ddlCentroCusto.SelectedValue.Trim());
                        #region Monta Layout de Meta
                        #region Limpa Página
                        //Limpa página
                        lbltabela.Text = string.Empty;
                        lblErro.Text = string.Empty;
                        lblLink.Text = string.Empty;
                        lblArquivoMeta.Text = string.Empty;
                        lblFrase.Text = string.Empty;
                        lblMensagem.Text = string.Empty;
                        #endregion

                        //Monta HTML Template
                        string strTemplate = SelecionaTemplate(ddlCentroCusto.SelectedItem.Text);
                        string listItemIDArquivo = string.Empty;
                        if (!string.IsNullOrEmpty(strTemplate))
                        {
                            string[] s = strTemplate.Split(';');
                            listItemIDArquivo = s[1].ToString();
                            strTemplate = s[0].ToString();
                            strTemplate = strTemplate.Substring(0);
                        }

                        string login = SPContext.Current.Web.CurrentUser.LoginName;

                        //Valida strTemplate
                        if (string.IsNullOrEmpty(strTemplate))
                        {
                            lblErro.Text = "Não há um <b>template</b> associado ao <b>centro de custo</b> selecionado.<br>";

                            if (PossuiAcessoTotal(login))
                                lblErro.Text += "Para fazer esta associação, <a href='/Remuneracoes/Lists/Templates%20e%20Centros%20de%20Custo/AllItems.aspx' target='_blank'>clique aqui</a>.";

                            lbltabela.Text = string.Empty;
                        }
                        else
                            lblErro.Text = string.Empty;

                        using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb web = site.OpenWeb("Remuneracoes"))
                            {
                                #region Monta HTML Template

                                SPListItemCollection lista = BuscaStringTemplate(strTemplate);

                                if (lista.Count.Equals(0))
                                    return;

                                //Criação de Listas, strings e StringBuilders e doubles que receberão os dados para a montagem do template.
                                StringBuilder sbParticipe = new StringBuilder();
                                StringBuilder sbBonus = new StringBuilder();
                                StringBuilder sbValorB = new StringBuilder();
                                StringBuilder sbValorP = new StringBuilder();
                                StringBuilder sbPercentualRealizadoParticipe = new StringBuilder();
                                StringBuilder sbPercentualRealizadoInternet = new StringBuilder();
                                StringBuilder sbInternet = new StringBuilder();
                                StringBuilder sbValorResultado = new StringBuilder();
                                StringBuilder sbPercentualRealizadoBonus = new StringBuilder();
                                StringBuilder sbOrcadoParticipe = new StringBuilder();
                                StringBuilder sbOrcadoInternet = new StringBuilder();
                                StringBuilder sbOrcadoBonus = new StringBuilder();
                                StringBuilder sbEspacoParticipe = new StringBuilder();
                                StringBuilder sbEspacoInternet = new StringBuilder();
                                StringBuilder sbEspacoBonus = new StringBuilder();
                                StringBuilder sbRealizadoParticipe = new StringBuilder();
                                StringBuilder sbRealizadoInternet = new StringBuilder();
                                StringBuilder sbRealizadoBonus = new StringBuilder();
                                StringBuilder sbTotalObtidoParticipe = new StringBuilder();
                                StringBuilder sbTotalObtidoInternet = new StringBuilder();
                                StringBuilder sbTotalObtidoBonus = new StringBuilder();
                                StringBuilder sbResultadoParticipe = new StringBuilder();
                                StringBuilder sbResultadoInternet = new StringBuilder();
                                StringBuilder sbResultadoBonus = new StringBuilder();
                                StringBuilder sbCompletaQuadroParticipe = new StringBuilder();
                                StringBuilder sbCompletaQuadroInternet = new StringBuilder();
                                StringBuilder sbCompletaQuadroBonus = new StringBuilder();
                                StringBuilder sbNotaAvaliacaoDiretoria = new StringBuilder();
                                StringBuilder sbNotaProjetosNegociados = new StringBuilder();
                                List<string> baseCalculoParticipe = new List<string>();
                                List<string> baseCalculoInternet = new List<string>();
                                List<string> baseCalculoBonus = new List<string>();

                                double somaSalarioParticipe = 0;
                                double somaSalarioInternet = 0;
                                double somaSalarioBonus = 0;
                                double metaAvalicaoDiretoria = 0;
                                double metaProjetosNegociados = 0;

                                lbltabela.Text += GeraCabecalhoHtml(lista);

                                //Esta variável faz a vericação se informações de resultado de internet serão exibidos ou não.
                                bool resultado = false;
                                //Variáveis que participam da construção do HTML
                                string strValor = string.Empty;
                                string strValorMonetario = string.Empty;
                                string strOrcado = string.Empty;
                                string strRealizado = string.Empty;
                                string strResultado = string.Empty;
                                string strCalculo = string.Empty;
                                string strPorcentagemMeta = string.Empty;
                                string strMetaEbitda = string.Empty;


                                foreach (SPListItem item in lista)
                                {
                                    #region Validação de campos vindos da lista
                                    strValor = (item["Valor"] == null) ? string.Empty : item["Valor"].ToString();
                                    strValorMonetario = item["valorMonetario"] == null ? string.Empty : item["valorMonetario"].ToString();
                                    strOrcado = item["Orcado"] == null ? string.Empty : item["Orcado"].ToString();
                                    strRealizado = item["Realizado"] == null ? string.Empty : item["Realizado"].ToString();
                                    strResultado = item["Resultado"] == null ? string.Empty : item["Resultado"].ToString();

                                    if (item["Realizado"] != null && item["Orcado"] != null)
                                        strPorcentagemMeta = ((Convert.ToDecimal(item["Realizado"]) / Convert.ToDecimal(item["Orcado"])) * 100).ToString("N2") + " %";
                                    else
                                        strPorcentagemMeta = "N/A";  // TODO: verificar se realmente o valor deve ser zero.

                                    if (item["Valor"] != null && item["Resultado"] != null)
                                        strCalculo = ((Convert.ToDouble(item["Valor"]) * Convert.ToDouble(item["Resultado"])) / 100).ToString();
                                    else
                                        strCalculo = string.Empty;


                                    #endregion

                                    if (item["Categoria"].Equals("PV"))
                                    {
                                        //Pega a Porcentagem da MetaEBITIDA
                                        if (item["Descricao"].ToString().Contains("EBITDA") || item["Descricao"].ToString().Contains("EBTIDA"))
                                            strMetaEbitda = strPorcentagemMeta.Replace("%", " ").Trim();

                                        //Preenchimento das StringBuilders relacionadas a "Participe Varivel"
                                        {
                                            if (ddlAno.SelectedValue.Trim().Equals("2013"))
                                            {
                                                sbParticipe.Append("<td style='min-width:100px;background-color:#FFFF9F; font-family:Calibri; font-size:8pt; height:85px;' align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                sbValorP.Append("<td style='width:160px;height:30px;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                sbEspacoParticipe.Append("<td style='min-width:100px;height:30px;'></td>" + Environment.NewLine);
                                                //sbResultadoBonus.Append("<td align='center' style='min-width:100px;height:30px;width:160px; background-color:#CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                sbPercentualRealizadoParticipe.Append("<td style='width:160px;height:30px; background-color: #FFFF9F; color: #008000;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                                sbOrcadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                                sbRealizadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>");
                                                sbResultadoParticipe.Append("<td align='center' style='width:160px;height:30px;; background-color: #CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                //Esta lista armazena os dados para auxilio no calculo dos resultados individuais referentes a Participe.
                                                baseCalculoParticipe.Add(strCalculo);
                                            }
                                            else if (ddlAno.SelectedValue.Trim().Equals("2014"))
                                            {
                                                sbParticipe.Append("<td style='min-width:100px;background-color:#FFFF9F; font-family:Calibri; font-size:8pt; height:85px;' align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                sbValorP.Append("<td style='width:160px;height:30px;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                sbEspacoParticipe.Append("<td style='min-width:100px;height:30px;'></td>" + Environment.NewLine);
                                                //sbResultadoBonus.Append("<td align='center' style='min-width:100px;height:30px;width:160px; background-color:#CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                sbPercentualRealizadoParticipe.Append("<td style='width:160px;height:30px; background-color: #FFFF9F; color: #008000;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                                sbOrcadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                                sbRealizadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>");
                                                sbResultadoParticipe.Append("<td align='center' style='width:160px;height:30px;; background-color: #CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                //Esta lista armazena os dados para auxilio no calculo dos resultados individuais referentes a Participe.
                                                baseCalculoParticipe.Add(strCalculo);
                                            }
                                            else
                                            {
                                                sbParticipe.Append("<td style='min-width:100px;background-color:#FFFF9F; font-family:Calibri; font-size:8pt; height:85px;' align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                sbValorP.Append("<td style='width:160px;height:30px;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                sbEspacoParticipe.Append("<td style='min-width:100px;height:30px;'></td>" + Environment.NewLine);
                                                sbResultadoBonus.Append("<td align='center' style='min-width:100px;height:30px;width:160px; background-color:#CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                //sbPercentualRealizadoParticipe.Append("<td style='width:160px;height:30px; background-color: #FFFF9F; color: #008000;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                                //sbOrcadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                                //sbRealizadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>");
                                                //sbResultadoParticipe.Append("<td align='center' style='width:160px;height:30px;; background-color: #CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                //Esta lista armazena os dados para auxilio no calculo dos resultados individuais referentes a Participe.
                                                baseCalculoParticipe.Add(strCalculo);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (item["Descricao"].Equals("Visitantes Únicos no Subdomínio") || item["Descricao"].Equals("Visitantes Únicos nas prop. canal") || item["Descricao"].Equals("Receitas Líquidas de Internet") || item["Descricao"].Equals("VideoViews de íntegras em prod. p/ ass."))
                                        {
                                            //Preenchimento das StringBuilders e da lista relacionadas a "Resultados de Internet"
                                            resultado = true;
                                            sbInternet.Append("<td style='min-width:100px;font-family:Calibri; font-size:8pt; padding-bottom:0.75pt; padding-left:0.75pt; padding-right:0.75pt; padding-top:0.75pt; background-color: #FFFF9F; height:40px' align='center' ><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                            sbValorResultado.Append("<td style='min-width:100px;height:30px;width:160px' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                            sbPercentualRealizadoInternet.Append("<td  style='min-width:100px;height:30px;width:160px; background-color: #FFFF9F;' align='center' style='color: #008000;'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                            sbOrcadoInternet.Append("<td align='center' style='min-width:100px;height:30px;width:160px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                            sbEspacoInternet.Append("<td style='min-width:100px;height:30px;width:160px;'></td>" + Environment.NewLine);
                                            sbRealizadoInternet.Append("<td align='center' style='min-width:100px;background-color: #FFFF9F; height:30px;width:160px' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>" + Environment.NewLine);
                                            sbResultadoInternet.Append("<td align='center' style='min-width:100px;background-color: #CCCCCC; color:Black; height:30px;width:160px' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                            //Esta lista armazena os dados para auxilio no calculo dos resultados individuais referentes a Internet.
                                            baseCalculoInternet.Add(strCalculo);
                                        }
                                        else
                                        {
                                            if (item["Descricao"].ToString().Trim().Equals("AVALIAÇÃO DIRETORIA") || item["Descricao"].ToString().Trim().Equals("AVALIAÇÃO DA DIRETORIA"))
                                            {
                                                if (ddlAno.SelectedValue.Trim().Equals("2014"))
                                                {
                                                    //NI
                                                    sbParticipe.Append("<td style='min-width:100px;font-family:Calibri; font-size:8pt; background-color:#FFFF9F;height:85px' align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                    sbValorP.Append("<td align='center' style='min-width:100px;height:30px;width:160px'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                    sbEspacoParticipe.Append("<td style='min-width:100px;height:30px;'></td>" + Environment.NewLine);
                                                    //sbResultadoBonus.Append("<td align='center' style='min-width:100px;height:30px;width:160px; background-color:#CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);
                                                    //sbPercentualRealizadoParticipe.Append("<td style='width:160px;height:30px; background-color: #FFFF9F; color: #008000;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                                    sbOrcadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                                    sbRealizadoParticipe.Append("<td align='center' style='width:160px;height:30px; background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>");
                                                    //sbResultadoParticipe.Append("<td align='center' style='width:160px;height:30px;; background-color: #CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);

                                                    metaAvalicaoDiretoria = item["Valor"] == null ? 0 : Convert.ToDouble(item["Valor"]) / 100;
                                                }
                                                else
                                                {
                                                    //Preenchimento das StringBuilders  e a lista relacionadas a "Bonus" onde a descrição é Avaliação da ditetoria que possui uma nota "resultado" à parte.
                                                    sbBonus.Append("<td style='min-width:100px;font-family:Calibri; font-size:8pt; background-color:#FFFF9F;height:85px' align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                    sbValorB.Append("<td align='center' style='min-width:100px;height:30px;width:160px'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                    sbPercentualRealizadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center'></td>" + Environment.NewLine);
                                                    sbOrcadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center'></td>" + Environment.NewLine);
                                                    sbEspacoBonus.Append("<td style='min-width:100px;height:30px;width:160px'></td>" + Environment.NewLine);
                                                    sbRealizadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center'></td>" + Environment.NewLine);
                                                    sbResultadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center' ></td>" + Environment.NewLine);
                                                    metaAvalicaoDiretoria = item["Valor"] == null ? 0 : Convert.ToDouble(item["Valor"]) / 100;
                                                    //avaliacaoDiretoria = true;//setando avaliação para indicar que no template há nota avaliação da diretoria
                                                }
                                            }
                                            else if (item["Descricao"].ToString().Trim().Equals("PROJETOS NEGOCIADOS"))
                                            {
                                                sbBonus.Append("<td style='min-width:100px;font-family:Calibri; font-size:8pt; background-color: #FFFF9F;height:85px' align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                sbValorB.Append("<td style='min-width:100px;height:30px;width:160px' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                sbPercentualRealizadoBonus.Append("<td style='min-width:100px;height:30px;width:160px; color: #008000;' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                                sbOrcadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                                sbEspacoBonus.Append("<td style='min-width:100px;height:30px;width:160px'></td>" + Environment.NewLine);
                                                sbRealizadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>" + Environment.NewLine);
                                                sbResultadoBonus.Append("<td style='min-width:100px;height:30px;width:160px' align='center' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);

                                                metaProjetosNegociados = item["Valor"] == null ? 0 : Convert.ToDouble(item["Valor"]) / 100;
                                            }
                                            else
                                            {
                                                //Preenchimento das StringBuilders e da lista relacionadas a "Bonus"
                                                sbBonus.Append("<td style='min-width:100px;font-family:Calibri; font-size:8pt; background-color: #FFFF9F; height:85px;width:160px'  align='center'><div>" + item["Descricao"] + "</div></td>" + Environment.NewLine);
                                                sbValorB.Append("<td style='min-width:100px;height:30px;width:160px ' align='center'><p style='font-family:Calibri;font-size:8pt'>" + strValor + "%" + "</p></td>" + Environment.NewLine);
                                                sbPercentualRealizadoBonus.Append("<td align='center' style='min-width:100px;background-color: #FFFF9F; color: #008000; height:30px;width:160px'><p style='font-family:Calibri;font-size:8pt'>" + strPorcentagemMeta + "</p></td>" + Environment.NewLine);
                                                sbOrcadoBonus.Append("<td align='center' style='min-width:100px;background-color: #FFFF9F; height:20px;width:160px' ><p style='font-family:Calibri;font-size:8pt'>" + strOrcado + "</p></td>" + Environment.NewLine);
                                                sbEspacoBonus.Append("<td style='min-width:100px;height:30px;width:160px' ></td>" + Environment.NewLine);
                                                sbRealizadoBonus.Append("<td align='center' style='min-width:100px;height:30px;width:160px;background-color: #FFFF9F;' ><p style='font-family:Calibri;font-size:8pt'>" + strRealizado + "</p></td>" + Environment.NewLine);
                                                sbResultadoBonus.Append("<td align='center' style='min-width:100px;height:30px;width:160px; background-color:#CCCCCC; color:Black;' ><p style='font-family:Calibri;font-size:8pt'>" + strResultado + "</p></td>" + Environment.NewLine);

                                            }
                                            //Esta lista armazena os dados para auxilio no calculo dos resultados individuais referentes a Bonus.
                                            if (strCalculo != string.Empty)
                                                baseCalculoBonus.Add(item["Descricao"].ToString().Trim() + "#" + strCalculo);
                                            else
                                                baseCalculoBonus.Add(item["Descricao"].ToString().Trim() + "#0");

                                        }
                                    }
                                }

                                //Gera a primeira parte do html com as informaçoes da area selecionada no dropdownlist
                                lbltabela.Text += GeraParteHtmlTemplate(resultado, sbParticipe, sbValorP, sbEspacoParticipe, sbOrcadoParticipe, sbRealizadoParticipe,
                                    sbPercentualRealizadoParticipe, sbResultadoParticipe, sbInternet, sbValorResultado, sbEspacoInternet, sbOrcadoInternet, sbRealizadoInternet,
                                    sbPercentualRealizadoInternet, sbResultadoInternet, sbBonus, sbValorB, sbEspacoBonus, sbOrcadoBonus, sbRealizadoBonus, sbPercentualRealizadoBonus, sbResultadoBonus);

                                //if (ddlAno.SelectedValue.Trim().Equals("2012"))
                                //{
                                //Gera a segunda parte do html com as informaçoes dos funcionarios com classe >= 13 referentes a area selecionada no dropdownlist
                                lbltabela.Text += GeraParteHtmlFuncionario(web, strTemplate, sbTotalObtidoParticipe, sbTotalObtidoInternet, sbTotalObtidoBonus,
                                    sbCompletaQuadroParticipe, sbCompletaQuadroInternet, sbCompletaQuadroBonus, sbNotaAvaliacaoDiretoria, sbNotaProjetosNegociados,
                                    somaSalarioParticipe, somaSalarioBonus, somaSalarioInternet, baseCalculoParticipe, baseCalculoInternet, baseCalculoBonus, metaAvalicaoDiretoria,
                                    metaProjetosNegociados, ddlCentroCusto, strMetaEbitda);


                                //Fecha a tabela html.
                                lbltabela.Text += @"</tr>
                                                  </table>";

                                lblMessagem.Text = string.Format("No pagamento será acrescentado o Participe e descontado o adiantamento de julho/{0}", ddlAno.SelectedValue.Trim());
                                lblMessagem.Visible = true;
                                //}
                                //else
                                //{
                                //    lblMessagem.Visible = false;
                                //}

                                lblFrase.Text = "<b><u>ATENÇÃO</u> : A Página de Remuneração está disponível para Diretores e Gerentes. Repasse essa informação aos elegíveis à Remuneração Variável (classe 13) da sua área que não possuem esse acesso ainda.</b><br><br>";
                                bool haAnexos = false;

                                GeraHTMLArquivosMeta(web, RetornaNomeColigada(Convert.ToInt32(strColigada)), ref haAnexos);

                                //Busca Mensagem por Coligada ou por Centro de Custo
                                BuscaMensagem(web, Convert.ToInt32(strColigada), ddlCentroCusto.SelectedItem.Text.Trim(), ddlAno.SelectedValue.Trim());

                                if (lista.Count > 0)
                                {
                                    SPList list = web.Lists["book_metas"];
                                    SPQuery query = new SPQuery();
                                    query.Query = "<Where>" +
                                                        "<And>" +
                                                            "<Eq>" +
                                                                "<FieldRef Name='Ano' />" +
                                                                "<Value Type='Choice'>" + this.ddlAno.SelectedValue.Trim() + "</Value>" +
                                                            "</Eq>" +
                                                            "<Eq>" +
                                                                "<FieldRef Name='ProjetosNegociados' />" +
                                                                "<Value Type='Boolean'>1</Value>" +
                                                            "</Eq>" +
                                                        "</And>" +
                                                    "</Where>" +
                                                    "<OrderBy>" +
                                                        "<FieldRef Name='FileLeafRef' Ascending='True' />" +
                                                    "</OrderBy>";

                                    SPListItemCollection itens = list.GetItems(query);
                                    if (itens.Count > 0)
                                    {
                                        haAnexos = true;
                                        lblLink.Text = string.Empty;

                                        foreach (SPListItem item in itens)
                                        {
                                            // Verifica se existe itens no field "Centro de Custos Específico".
                                            int centroCustos = 0;
                                            if (item.Fields.ContainsField("CentroCustoEspecifico"))
                                            {
                                                SPFieldLookupValueCollection fieldCentroCustoEspecifico = new SPFieldLookupValueCollection(item["CentroCustoEspecifico"].ToString());
                                                if (fieldCentroCustoEspecifico.Count > 0)
                                                {
                                                    foreach (SPFieldLookupValue fieldValue in fieldCentroCustoEspecifico)
                                                    {
                                                        if (fieldValue.LookupValue.Split('-')[0].Trim().Equals(this.ddlCentroCusto.SelectedValue.Split('-')[0].Trim()))
                                                        {
                                                            centroCustos++;
                                                        }
                                                    }

                                                    if (centroCustos == 0)
                                                        continue;
                                                }
                                            }

                                            if (centroCustos == 0)
                                            {
                                                // Caso não possuir itens no field "Centro de Custo Específico", verificar se possui o mesmo template.
                                                if (item.Fields.ContainsField("Template"))
                                                {
                                                    SPFieldLookupValue fieldTemplate = new SPFieldLookupValue(item["Template"].ToString());
                                                    if ((fieldTemplate.LookupValue != null) && (!fieldTemplate.LookupValue.Trim().Equals(strTemplate.Trim())))
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }

                                            SPFile file = item.File;
                                            string urlFile = (string)file.Item[SPBuiltInFieldId.EncodedAbsUrl];
                                            string iconFile = web.Url + "/_layouts/Images/" + item.File.IconUrl;

                                            lblLink.Visible = true;
                                            lblLink.Text += @"  <table>
	                                                            <tr>
		                                                            <td>
			                                                            <img src='" + iconFile + @"' />
		                                                            </td>
		                                                            <td>";
                                            lblLink.Text += "<a href=\"";
                                            lblLink.Text += urlFile + "\"";
                                            lblLink.Text += " target='_blank'>" + file.Name + "</a>";
                                            lblLink.Text += @"</td>
	                                                            </tr>
                                                            </table>";
                                        }
                                    }
                                }

                                if (haAnexos)
                                    lblAnexos.Text = "<b>ANEXOS:</b><br><br>";
                                else
                                    lblAnexos.Text = string.Empty;


                                //Fecha a tabela html.
                                lbltabela.Text += @"</tr>
                                                  </table>";
                                #endregion

                                #region Popup Li e aceito
                                if (!VerificaStatus(SPContext.Current.Web.CurrentUser, ddlCentroCusto.SelectedValue.Trim()) && ddlAno.SelectedValue.Trim().Equals("2012"))
                                {
                                    string strUrl = SPContext.Current.Site.RootWeb.Url + "/_layouts/Globosat.Remuneracao.LieAceito/TermoAceite.aspx?CENTROCUSTO=" + ddlCentroCusto.SelectedValue.Trim();
                                    string strScript = @"
                                                            function openModalDialog() {
                                                                var options = SP.UI.$create_DialogOptions();
                                                                options.width = 350;
                                                                options.allowMaximize = false;
                                                                options.height = 210;
                                                                options.x = 20;
                                                                options.y = 20;
                                                                options.url = '" + strUrl + @"'
                                                                options.dialogReturnValueCallback =
                                                                    Function.createDelegate(null, whenMyModalDialogCloses);
                                                                SP.UI.ModalDialog.showModalDialog(options);

                                                            } 

                                                            function whenMyModalDialogCloses() {
    
                                                            }";

                                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), strScript, true);

                                    OpenMySweetModalDialog();
                                }

                                #endregion
                            }
                        #endregion
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Erro ao montar a tabela Html: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                }
            }
        }

        private void BuscaMensagem(SPWeb web, int coligada, string centroCusto, string ano)
        {
            lblMensagem.Text = string.Empty;

            SPList list = web.Lists["Metas_Mensagem"];
            SPQuery query = new SPQuery();
            query.Query = string.Format(@"<Where>
                                              <And>
                                                 <Eq>
                                                    <FieldRef Name='Por_x0020_empresa' />
                                                    <Value Type='Boolean'>1</Value>
                                                 </Eq>
                                                 <And>
                                                    <Eq>
                                                       <FieldRef Name='Coligada' />
                                                       <Value Type='LookupMulti'>{0}</Value>
                                                    </Eq>
                                                    <Eq>
                                                       <FieldRef Name='Ano' />
                                                       <Value Type='Choice'>{1}</Value>
                                                    </Eq>
                                                 </And>
                                              </And>
                                           </Where>", RetornaNomeColigada(coligada), ano);

            SPListItemCollection itens = list.GetItems(query);

            if (itens.Count > 0)
            {
                lblMensagem.Text = itens[0]["Mensagem"].ToString();
            }
            else
            {
                SPList listCentroCusto = web.Lists["Metas_Mensagem"];
                SPQuery queryCentroCusto = new SPQuery();
                queryCentroCusto.Query = string.Format(@"<Where>
                                                              <And>
                                                                 <Eq>
                                                                    <FieldRef Name='Por_x0020_empresa' />
                                                                    <Value Type='Boolean'>0</Value>
                                                                 </Eq>
                                                                 <And>
                                                                    <Eq>
                                                                       <FieldRef Name='CentroCusto' />
                                                                       <Value Type='LookupMulti'>{0}</Value>
                                                                    </Eq>
                                                                    <Eq>
                                                                       <FieldRef Name='Ano' />
                                                                       <Value Type='Choice'>{1}</Value>
                                                                    </Eq>
                                                                 </And>
                                                              </And>
                                                           </Where>", centroCusto, ano);

                SPListItemCollection itemCentroCusto = listCentroCusto.GetItems(queryCentroCusto);

                if (itemCentroCusto.Count > 0)
                {
                    lblMensagem.Text = itemCentroCusto[0]["Mensagem"].ToString();
                }
            }

        }

        private string RetornaNomeColigada(int coligada)
        {
            switch (coligada)
            {
                case 1:
                    return "GLOBOSAT";
                case 2:
                    return "TELECINE";
                case 3:
                    return "UNIVERSAL";
                case 4:
                    return "CANAL BRASIL";
                case 5:
                    return "G2C";
                case 6:
                    return "PLAYBOY";
                case 7:
                    return "HORIZONTE";
                default:
                    return "";
            }
        }

        private void GeraHTMLArquivosMeta(SPWeb web, string coligada, ref bool haAnexos)
        {
            SPList list = web.Lists["book_metas"];
            SPQuery query = new SPQuery();
            query.Query = string.Format(@"<Where>
                                              <And>
                                                 <Eq>
                                                    <FieldRef Name='Ano' />
                                                    <Value Type='Choice'>{0}</Value>
                                                 </Eq>
                                                 <And>
                                                    <Eq>
                                                       <FieldRef Name='Coligada' />
                                                       <Value Type='Lookup'>{1}</Value>
                                                    </Eq>
                                                    <Eq>
                                                       <FieldRef Name='ProjetosNegociados' />
                                                       <Value Type='Boolean'>0</Value>
                                                    </Eq>
                                                 </And>
                                              </And>
                                           </Where>
                                           <OrderBy>
                                              <FieldRef Name='FileLeafRef' Ascending='True' />
                                           </OrderBy>", ddlAno.SelectedValue.Trim(), coligada);

            SPListItemCollection itens = list.GetItems(query);

            if (itens.Count > 0)
            {
                haAnexos = true;
                lblArquivoMeta.Text = string.Empty;

                foreach (SPListItem item in itens)
                {
                    SPFile file = item.File;
                    string urlFile = (string)file.Item[SPBuiltInFieldId.EncodedAbsUrl];
                    string iconFile = web.Url + "/_layouts/Images/" + item.File.IconUrl;

                    lblArquivoMeta.Visible = true;
                    lblArquivoMeta.Text += @"  <table>
	                                                            <tr>
		                                                            <td>
			                                                            <img src='" + iconFile + @"' />
		                                                            </td>
		                                                            <td>";
                    lblArquivoMeta.Text += "<a href=\"";
                    lblArquivoMeta.Text += urlFile + "\"";
                    lblArquivoMeta.Text += " target='_blank'>" + file.Name + "</a>";
                    lblArquivoMeta.Text += @"</td>
	                                                            </tr>
                                                            </table>";
                }
            }
        }

        /// <summary>
        /// Verifica se usuário logado já leu o termo de aceite.
        /// Para isso, consulta-se a lista "LieConcordoStatus"
        /// </summary>
        /// <param name="sPUser"></param>
        private bool VerificaStatus(SPUser sPUser, string centroCusto)
        {
            bool lido = false;
            SPSite site = new SPSite(SPContext.Current.Site.ID);
            SPWeb web = site.OpenWeb(SPContext.Current.Web.ID);

            try
            {
                #region Elevando permissões
                SPUserToken sysToken = site.SystemAccount.UserToken;
                using (SPSite siteAdmin = new SPSite(site.ID, sysToken))
                {
                    using (SPWeb webAdmin = siteAdmin.OpenWeb(web.ID))
                    {
                        SPList oList = webAdmin.Lists["LiEConcordoStatus"];
                        SPQuery query = new SPQuery();
                        query.Query = string.Format(@"<Where>
                            <And>
                            <Eq>
                                <FieldRef Name='Usuario' LookupId='TRUE' />
                                    <Value Type='Int'>{0}</Value>
                            </Eq>
                            <Eq>
                                <FieldRef Name='CentroCusto' />
                                    <Value Type='Text'>{1}</Value>
                            </Eq>
                            </And>
                        </Where>", sPUser.ID, centroCusto);

                        SPListItemCollection itemCollection = oList.GetItems(query);
                        if (itemCollection.Count > 0)
                            lido = true;
                        else
                            lido = false;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro ao buscar status do usuário '{0}': {1}.", sPUser.LoginName, ex.Message + ex.StackTrace), EventLogEntryType.Error, 2, 2);
            }
            finally
            {

                web.Dispose();
                site.Dispose();
            }
            return lido;
        }

        protected void OpenMySweetModalDialog()
        {
            var script = string.Format(
                @"function reallyOpenDialogForRealYouGuys() {{ 
            openModalDialog(); 
        }}; 
        SP.SOD.executeOrDelayUntilScriptLoaded(reallyOpenDialogForRealYouGuys, ""sp.ui.dialog.js""); ");

            Page.ClientScript.RegisterStartupScript(
                this.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        public SPListItemCollection BuscaStringTemplate(string strTemplate)
        {
            SPListItemCollection ItemsLista;
            using (SPSite site = new SPSite(SPContext.Current.Site.Url))
            {
                using (SPWeb web = site.OpenWeb("Remuneracoes"))
                {
                    SPList List = web.Lists["Metas"];
                    SPQuery oQuery = new SPQuery();

                    #region Nova Consulta considerando campo "Exibir" - Solicitação do Daniel em 02/01/2012
                    oQuery.Query = string.Format(@"<Where>
                                                        <And>
                                                           <Eq>
                                                                <FieldRef Name='Template' /><Value Type='Lookup'>{0}</Value>
                                                           </Eq>
                                                                <And>
                                                                    <Eq>
                                                                        <FieldRef Name='Exibir' /><Value Type='Boolean'>1</Value>
                                                                    </Eq>
                                                                    <Eq>
                                                                        <FieldRef Name='Ano' /><Value Type='Text'>{1}</Value>
                                                                    </Eq>
                                                                </And>
                                                        </And>
                                                     </Where>
                                                        <OrderBy><FieldRef Name='Ordem' Ascending='True' /></OrderBy></Query>", strTemplate, ddlAno.SelectedValue.Trim());

                    #endregion

                    ItemsLista = List.GetItems(oQuery);
                }
            }
            return ItemsLista;
        }

        public StringBuilder GeraCabecalhoHtml(SPListItemCollection lista)
        {
            StringBuilder sbCabecalho = new StringBuilder();
            sbCabecalho.Append(@"<table style='width: 100%; color:black; border-color:Black'; border='2'>
                                    <tr>
                                        <td align='center' style='height: 30px; text-align: center;'><strong style='font-family:Calibri;font-size:12pt'><span>METAS " + lista[0]["Ano"] +
                  " - " + ddlCentroCusto.SelectedItem.Text + @"</span></strong></td>
                                    </tr>
                                        </table>
                                        <p>&nbsp;</p>
                                        <table border='3' style='table-layout:fixed; border-style:solid; border-color:Black;'>
                                    <tr>
                                        <td align='center'></td>
                                        <td align='center'><strong style='font-family:Calibri;font-size:12pt'>Participe Variável</strong></td>");
            if (ddlAno.SelectedValue.Trim().Equals("2013"))
            {
                sbCabecalho.Append("<td colspan='2' align='center'><strong style='font-family:Calibri;font-size:12pt'>Nota Individual</strong></td>");
            }
            sbCabecalho.Append("</tr>");
            return sbCabecalho;
        }

        public StringBuilder GeraParteHtmlTemplate(bool resultado, StringBuilder sbParticipe, StringBuilder sbValorP, StringBuilder sbEspacoParticipe, StringBuilder sbOrcadoParticipe, StringBuilder sbRealizadoParticipe, StringBuilder sbPercentualRealizadoParticipe, StringBuilder sbResultadoParticipe, StringBuilder sbInternet, StringBuilder sbValorResultado, StringBuilder sbEspacoInternet, StringBuilder sbOrcadoInternet, StringBuilder sbRealizadoInternet, StringBuilder sbPercentualRealizadoInternet, StringBuilder sbResultadoInternet, StringBuilder sbBonus, StringBuilder sbValorB, StringBuilder sbEspacoBonus, StringBuilder sbOrcadoBonus, StringBuilder sbRealizadoBonus, StringBuilder sbPercentualRealizadoBonus, StringBuilder sbResultadoBonus)
        {
            StringBuilder sbParteSuperiorTemplate = new StringBuilder();
            if (resultado)
            {
                // Montagem da tabela com as informações relacionadas ao centro de custo selecionado, com resultado de internet a serem exibidos.
                sbParteSuperiorTemplate.Append("\n<tr valign='top'>" +
                                "<td border='3' style='border-style:solid; border-color:Black;width:250' align='center'>" +
                                  "<table cellpadding='1px' cellspacing='5px' style='height:240px;table-layout:fixed'>" +
                                        "<tr><td style='height:80px; width:110px' align='left'><b style='font-family:Calibri;font-size:12pt'>Meta</b></td>" + " <td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                                        "<!--<tr><td style='height:30px; width:110px' align='left'></td></tr>-->" +
                                        "<tr><td style='height:30px; width:110px' rowspan='1' align='left'><b style='font-family:Calibri;font-size:12pt'>Peso</b></td>" + " <td rowspan='1'><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                                        "<!--<tr><td style='height:30px; width:110px' align='left'></td></tr>-->" +
                                        "<!--<tr><td style='height:30px; width:110px' align='left'><b style='font-family:Calibri;font-size:12pt'>Orçado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>-->");

                sbParteSuperiorTemplate.Append("<!--<tr><td style='height:30px; width:110px' align='left'><b style='font-family:Calibri;font-size:12pt'>Realizado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>-->" +
                                    "<tr><td style='height:30px; width:110px' align='left'><b style='font-family:Calibri;font-size:12pt'>% Atingimento</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                                    "<tr><td style='height:30px; width:110px' align='left'><b style='font-family:Calibri;font-size:12pt'>Resultado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>");

                sbParteSuperiorTemplate.Append("</table>" +
        "</td>" +
        "<td border='3' style='border-style:solid; border-color:Black;'>" +
            "<table cellpadding='1px' cellspacing='5px' style='height:240px; table-layout:fixed'>" +
                            "<tr name='sbParticipe'>" + sbParticipe.ToString() + "<td style='width:160px'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>" +
                            "<tr name='sbValorP'>" + sbValorP.ToString() + "</tr>" +
                            "<!--<tr name='sbEspacoParticipe'>" + sbEspacoParticipe.ToString() + "</tr>-->" +
                            "<!--<tr name='sbOrcadoParticipe'>" + sbOrcadoParticipe.ToString() + "</tr>-->");

                sbParteSuperiorTemplate.Append("<!--<tr name='sbRealizadoParticipe'>" + sbRealizadoParticipe.ToString() + "</tr>-->" +
                "<tr name='sbPercentualRealizadoParticipe'>" + sbPercentualRealizadoParticipe.ToString() + "</tr>" +
                "<tr name='sbResultadoParticipe'>" + sbResultadoParticipe.ToString() + "</tr>");

                sbParteSuperiorTemplate.Append("</table>" +
                                "</td>" +
                                "<td border='3' style='border-style:solid; border-color:Black;'>" +
                                    "\n<table cellpadding='1px' cellspacing='5px' style='height:240px;'>" +
                                        "<tr>" +
                                            "<td align='center' colspan='4' style='background-color: #FFFF9F; height:30px'>Resultado de Internet</td>" +
                                        "</tr>" +
                                        "<tr name='sbInternet'>" + sbInternet.ToString() + "<td style='width:160px'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>" +
                                        "<tr name='sbValorResultado'>" + sbValorResultado.ToString() + "</tr>" +
                                        "<!--<tr name='sbEspacoInternet'>" + sbEspacoInternet.ToString() + "</tr>-->" +
                                        "<!--<tr name='sbOrcadoInternet'>" + sbOrcadoInternet.ToString() + "</tr>-->");

                sbParteSuperiorTemplate.Append("<!--<tr name='sbRealizadoInternet'>" + sbRealizadoInternet.ToString() + "</tr>-->" +
                 "<tr name='sbPercentualRealizadoInternet'>" + sbPercentualRealizadoInternet.ToString() + "</tr>" +
                 "<tr name='sbResultadoInternet'>" + sbResultadoInternet.ToString() + "</tr>");
                sbParteSuperiorTemplate.Append("</table>" + "</td>");

                if (ddlAno.SelectedValue.Trim().Equals("2013"))
                {
                    sbParteSuperiorTemplate.Append(
                                "<td border='3' style='border-style:solid; border-color:Black;'>" +
                                    "\n<table style='height:240px' cellpadding='1px' cellspacing='5px'>" +
                                        "<tr name='sbBonus'>" + sbBonus.ToString() + "<td style='min-width:100px; width:160px'></td></tr>" +
                                        "<tr name='sbValorB'>" + sbValorB.ToString() + "</tr>" +
                                        "<!--<tr name='sbEspacoBonus'>" + sbEspacoBonus.ToString() + "</tr>-->" +
                                        "<!--<tr name='sbOrcadoBonus'>" + sbOrcadoBonus.ToString() + "</tr>-->" +
                                        "<!--<tr name='sbRealizadoBonus'>" + sbRealizadoBonus.ToString() + "</tr>-->" +
                                        "<tr name='sbPercentualRealizadoBonus'>" + sbPercentualRealizadoBonus.ToString() + "</tr>" +
                                        "<tr name='sbResultadoBonus'>" + sbResultadoBonus.ToString() + "</tr>" +
                                    "</table>" +
                                "</td>");
                }
                sbParteSuperiorTemplate.Append("</tr>");

            }
            else
            {
                // Montagem da tabela com as informações relacionadas ao centro de custo selecionado.
                sbParteSuperiorTemplate.Append("<tr valign='top'>" +
                                     "<td border='3' style='border-style:solid; border-color:Black;' align='center'>");

                sbParteSuperiorTemplate.Append("\n<table style='width:250px;height:217px' cellpadding='1px' cellspacing='5px'>");

                sbParteSuperiorTemplate.Append("<tr><td style='height:85px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>Meta</b></td>" + " <td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                "<!--<tr><td style='height:30px; width:110px' align='left'></td></tr>-->" +
                "<tr><td style='height:30px; width:110px' rowspan='1' align='left'><b style='font-family:Calibri;font-size:12pt'>Peso</b></td>" + " <td rowspan='1'><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                "<!--<tr><td style='height:30px; width:110px' align='left'></td></tr>-->" +
                "<!--<tr><td style='height:30px; width:110px' align='left'><b style='font-family:Calibri;font-size:12pt'>Orçado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>-->");

                if (ddlAno.SelectedValue.Trim().Equals("2014"))
                {
                    sbParteSuperiorTemplate.Append("<!--<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>Realizado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>-->" +
                        "<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>% Atingimento</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                        "<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>Resultado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>"); 
                                            //"<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'></b></td>" + "<td>" + "</td></tr>" +
                                            //"<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'></b></td>" + "<td>" + "</td></tr>");
                }
                else
                {
                    sbParteSuperiorTemplate.Append("<!--<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>Realizado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>-->" +
                        "<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>% Atingimento</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>" +
                        "<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'>Resultado</b></td>" + "<td><img src='/_layouts/Images/Globosat.Remuneracao.ExibeMetasFuncionario/seta.png' style='width:100px'>" + "</td></tr>");
                    //"<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'></b></td>" + "<td>" + "</td></tr>" +
                    //"<tr><td style='height:30px; width:100px' align='left'><b style='font-family:Calibri;font-size:12pt'></b></td>" + "<td>" + "</td></tr>");
                }
                sbParteSuperiorTemplate.Append("</table>" +
                "</td>" +
                 "<td border='3' style='border-style:solid; border-color:Black;'>" +
                     "\n<table style='height:217px' cellpadding='1px' cellspacing='5px'>" +
                                "<tr name='sbParticipe'>" + sbParticipe.ToString() + "<td style='width:160px'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>" +
                                "<tr name='sbValorP'>" + sbValorP.ToString() + "<td></td></tr>" +
                                "<!--<tr name='sbEspacoParticipe'>" + sbEspacoParticipe.ToString() + "<td></td></tr>-->" +
                                "<!--<tr name='sbOrcadoParticipe'>" + sbOrcadoParticipe.ToString() + "<td></td></tr>-->");

                sbParteSuperiorTemplate.Append("<!--<tr name='sbRealizadoParticipe'>" + sbRealizadoParticipe.ToString() + "<td></td></tr>-->" +
                                "<tr name='sbPercentualRealizadoParticipe'>" + sbPercentualRealizadoParticipe.ToString() + "<td></td></tr>" +
                                "<tr name='sbResultadoParticipe'>" + sbResultadoParticipe.ToString() + "</tr>");
                sbParteSuperiorTemplate.Append("</table>" + "</td>");

                if (ddlAno.SelectedValue.Trim().Equals("2013"))
                {
                    sbParteSuperiorTemplate.Append("<td border='3' style='border-style:solid; border-color:Black;'>" +
                                  "\n<table style='height:217px' cellpadding='1px' cellspacing='5px'>" +
                                             "<tr name='sbBonus'>" + sbBonus.ToString() + "<td style='min-width:100px; width:160px'></td></tr>" +
                                             "<tr name='sbValorB'>" + sbValorB.ToString() + "<td></td></tr>" +
                                             "<!--<tr name='sbEspacoBonus'>" + sbEspacoBonus.ToString() + "<td></td></tr>-->" +
                                             "<!--<tr name='sbOrcadoBonus'>" + sbOrcadoBonus.ToString() + "<td></td></tr>-->" +
                                             "<!--<tr name='sbRealizadoBonus'>" + sbRealizadoBonus.ToString() + "<td></td></tr>-->" +
                                             "<tr name='sbPercentualRealizadoBonus'>" + sbPercentualRealizadoBonus.ToString() + "<td></td></tr>" +
                                             "<tr name='sbResultadoBonus'>" + sbResultadoBonus.ToString() + "</tr>" +
                                   "</table>" +
                              "</td>");
                }
                if (ddlAno.SelectedValue.Trim().Equals("2014"))
                {
                    //sbParteSuperiorTemplate.Append("<td border='10' style='border-style:solid; border-color:Black;'>" +
                    //              "\n<table style='height:217px' cellpadding='1px' cellspacing='5px'>" +
                    //                         "<tr name='sbBonus'>" + sbBonus.ToString() + "<td style='min-width:100px; width:160px'></td></tr>" +
                    //                         "<tr name='sbValorB'>" + sbValorB.ToString() + "<td></td></tr>" +
                    //                         "<tr name='sbPercentualRealizadoBonus'>" + sbPercentualRealizadoBonus.ToString() + "<td></td></tr>" +
                    //                         "<tr name='sbResultadoBonus'>" + sbResultadoBonus.ToString() + "</tr>" +
                    //               "</table>" +
                    //          "</td>");
                }
                sbParteSuperiorTemplate.Append("</tr>");
            }
            return sbParteSuperiorTemplate;
        }

        /// <summary>
        /// Verifica se o funciário já existe na lista de funcionários.
        /// Dessa forma, evita-se ter funcionário duplicado.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="listaFunc"></param>
        /// <returns></returns>
        private bool VerificaSeJahExiste(string p, List<FuncionarioRem> listaFunc)
        {
            bool existe = false;
            foreach (FuncionarioRem f in listaFunc)
                if (f.Matricula.Equals(p))
                    existe = true;

            return existe;
        }

        /// <summary>
        /// Gera área do Funcionário no template de Metas
        /// </summary>
        /// <param name="web"></param>
        /// <param name="strTemplate"></param>
        /// <param name="sbTotalObtidoParticipe"></param>
        /// <param name="sbTotalObtidoInternet"></param>
        /// <param name="sbTotalObtidoBonus"></param>
        /// <param name="sbCompletaQuadroParticipe"></param>
        /// <param name="sbCompletaQuadroInternet"></param>
        /// <param name="sbCompletaQuadroBonus"></param>
        /// <param name="sbNotaAvaliacaoDiretoria"></param>
        /// <param name="sbNotaProjetosNegociados"></param>
        /// <param name="somaSalarioParticipe"></param>
        /// <param name="somaSalarioBonus"></param>
        /// <param name="somaSalarioInternet"></param>
        /// <param name="baseCalculoParticipe"></param>
        /// <param name="baseCalculoInternet"></param>
        /// <param name="baseCalculoBonus"></param>
        /// <param name="metaAvalicaoDiretoria"></param>
        /// <param name="metaProjetosNegociados"></param>
        /// <param name="ddlCentroCusto"></param>
        /// <param name="avaliacao"></param>
        /// <param name="projetosNegociados"></param>
        /// <returns></returns>
        public StringBuilder GeraParteHtmlFuncionario(SPWeb web, string strTemplate, StringBuilder sbTotalObtidoParticipe, StringBuilder sbTotalObtidoInternet, StringBuilder sbTotalObtidoBonus,
            StringBuilder sbCompletaQuadroParticipe, StringBuilder sbCompletaQuadroInternet, StringBuilder sbCompletaQuadroBonus, StringBuilder sbNotaAvaliacaoDiretoria,
            StringBuilder sbNotaProjetosNegociados, double somaSalarioParticipe, double somaSalarioBonus, double somaSalarioInternet, List<string> baseCalculoParticipe,
            List<string> baseCalculoInternet, List<string> baseCalculoBonus, double metaAvalicaoDiretoria, double metaProjetosNegociados, DropDownList ddlCentroCusto, string strMetaEBITDA)
        {
            try
            {
                #region Regra para cálculos considerando PROJETOS NEGOCIADOS e AVALIAÇÃO DA DIRETORIA
                /*
                    Só buscar da lista na lista "notas_modelo_metas" caso haja no template de metas (na parte de Bônus) registros com o nome "AVALIAÇÃO DA DIRETORIA" OU "PROJETOS NEGOCIADOS".

                    Caso não haja nenhum dos dois ("AVALIAÇÃO DA DIRETORIA" e "PROJETOS NEGOCIADOS")
                    {
	                    O cálculo de bônus é o mesmo para todos.
	                    Não será necessário  buscar na lista "notas_modelo_metas". O valor utilizado no cálculo do funcionário será o valor geral do template. O campo "Resultado", para ser mais específico.
	                    O quadradinho cinza do funcionário não deverá aparecer, pois não há nota pessoal.
                    } 

                    Caso haja "AVALIAÇÃO DA DIRETORIA"
                    {
	                    O cálculo continua o mesmo.
	                    Buscar na lista "notas_modelo_metas" o valor do funcionário para Avaliação da Diretoria.
	                    O cinza da meta fica em branco (na parte acima).
	                    o campo cinza da parte do funcionário deverá aparecer.
	                    Não é necessário pegar o resultado da meta e sim trabalhar com o resultado individual do funcionário.
                    }

                    Caso haja "PROJETOS NEGOCIADOS"
                    {
	                    idem ao "AVALIAÇÃO DA DIRETORIA".
                    }
                */
                #endregion

                string login = SPContext.Current.Web.CurrentUser.LoginName;

                #region producao
                FuncionarioRem funcionario = null;
                string centroCusto = ddlCentroCusto.SelectedItem.Value;
                string coligada = string.Empty;
                coligada = ExtraiColigadaCentroCusto(centroCusto);

                //Busca matrícula e coligada do usuário atual.
                Gerente dadosProfile = null;
                dadosProfile = ManipularDados.BuscaMatriculaColigada(login);

                DataTable dtColaboradores = new DataTable();
                if (PossuiAcessoTotal(login))
                {
                    dtColaboradores = ManipularDados.BuscaColaboradoresFolhaPagamento(centroCusto, ddlAno.SelectedValue.ToString());
                }
                else
                {
                    if (dadosProfile != null)
                    {
                        dtColaboradores = ManipularDados.BuscaColaboradoresFolhaPagamento(centroCusto, coligada, dadosProfile.Matricula);
                    }
                }

                StringBuilder sbParteInferiorTemplate = new StringBuilder();
                //Label lblAux = new Label();
                //StringBuilder aux = new StringBuilder();
                #endregion

                foreach (DataRow linhaColaboradores in dtColaboradores.Rows)
                {
                    funcionario = new FuncionarioRem();
                    //Popular Dados...
                    funcionario.Nome = linhaColaboradores["NOME"] as string;
                    funcionario.Salario = Convert.ToDecimal(linhaColaboradores["SALARIO"]).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                    funcionario.SalarioNumber = Convert.ToDecimal(linhaColaboradores["SALARIO"]);
                    funcionario.Funcao = linhaColaboradores["CARGO"] as string;
                    funcionario.Matricula = linhaColaboradores["CHAPA"].ToString();
                    funcionario.Admissao = Convert.ToDateTime(linhaColaboradores["Admissao"]).ToString("dd/MM/yyyy");
                    funcionario.Nivel = linhaColaboradores["CODNIVELSAL"].ToString().Trim();

                    if (string.IsNullOrEmpty(funcionario.Nivel))
                        funcionario.Nivel = "0";

                    //Dentro do nível funcionario temos as carcterísticas necessárias para manipular as informações
                    if (sbTotalObtidoParticipe.Length > 0)
                        sbTotalObtidoParticipe.Remove(0, sbTotalObtidoParticipe.Length);
                    if (sbTotalObtidoInternet.Length > 0)
                        sbTotalObtidoInternet.Remove(0, sbTotalObtidoInternet.Length);
                    if (sbTotalObtidoBonus.Length > 0)
                        sbTotalObtidoBonus.Remove(0, sbTotalObtidoBonus.Length);
                    if (sbCompletaQuadroParticipe.Length > 0)
                        sbCompletaQuadroParticipe.Remove(0, sbCompletaQuadroParticipe.Length);
                    if (sbCompletaQuadroInternet.Length > 0)
                        sbCompletaQuadroInternet.Remove(0, sbCompletaQuadroInternet.Length);
                    if (sbCompletaQuadroBonus.Length > 0)
                        sbCompletaQuadroBonus.Remove(0, sbCompletaQuadroBonus.Length);
                    if (sbNotaAvaliacaoDiretoria.Length > 0)
                        sbNotaAvaliacaoDiretoria.Remove(0, sbNotaAvaliacaoDiretoria.Length);
                    if (sbNotaProjetosNegociados.Length > 0)
                        sbNotaProjetosNegociados.Remove(0, sbNotaProjetosNegociados.Length);

                    //Verifica se o nível do funcionario é maior ou igual a 13, que é um dos requisitos para exibição.
                    if (Convert.ToInt32(funcionario.Nivel) >= 13)
                    {
                        #region Get ParticipeVariavel e Bonus da lista "Remuneracao Variavel"
                        SPList listaRemuneracaoVariavel = web.Lists["Remuneracao Variavel"];
                        SPQuery query = new SPQuery();
                        query.Query = string.Format("<Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq><Eq><FieldRef Name='Ano' /><Value Type='Text'>{1}</Value></Eq></And></Where>", funcionario.Nivel, ddlAno.SelectedValue.Trim());
                        SPListItemCollection ItensRemuneracaoVariavel = listaRemuneracaoVariavel.GetItems(query);

                        /*Realiza Cálculo de Participe vs EBITDA
                         * Se usuário possui Classe acima de 16 é necessário calcular seu Participe Variável e Bônus de acordo com a porcentagem do EBITDA da seção.
                         */
                        //Para Nível < 17 considerar apenas EBITDA 100%
                        if (Convert.ToInt32(funcionario.Nivel) < 17)
                        {
                            if (ItensRemuneracaoVariavel.Count > 0)
                            {
                                funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_100"]);
                                funcionario.TotalBonusAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_100"]);

                            }
                        }
                        else //Para Nível > 16 considerar EBITDA 100% + proporção de 130%
                        {
                            if (Convert.ToDecimal(strMetaEBITDA) < 100) //&& Convert.ToInt32(funcionario.Nivel) > 16)
                            {
                                SPList spList = web.Lists["EbtidaMenorCem"];
                                SPQuery spQuery = new SPQuery();
                                spQuery.ViewFieldsOnly = true;
                                spQuery.ViewFields = "<FieldRef Name='Valor' />";
                                spQuery.Query = "<Where>" +
                                                    "<And>" +
                                                        "<Eq>" +
                                                            "<FieldRef Name='Title' />" +
                                                                "<Value Type='Text'>" + coligada + "</Value>" +
                                                        "</Eq>" +
                                                        "<And>" +
                                                            "<Eq>" +
                                                                "<FieldRef Name='Classe' />" +
                                                                    "<Value Type='Text'>" + funcionario.Nivel + "</Value>" +
                                                            "</Eq>" +
                                                            "<Eq>" +
                                                                "<FieldRef Name='Ano' />" +
                                                                    "<Value Type='Text'>" + this.ddlAno.SelectedValue + "</Value>" +
                                                            "</Eq>" +
                                                        "</And>" +
                                                    "</And>" +
                                                "</Where>";

                                SPListItemCollection items = spList.GetItems(spQuery);
                                if (items.Count > 0)
                                {
                                    funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(items[0]["Valor"]);
                                }
                                else
                                {
                                    funcionario.TotalParticipeVariavelAnoNSalarios = 0;
                                }
                            }
                            else if ((Convert.ToDecimal(strMetaEBITDA) >= 100) && (Convert.ToDecimal(strMetaEBITDA) <= 130) && (Convert.ToInt32(funcionario.Nivel) > 16))
                            {
                                //considerar EBITDA 100% + proporção de 130%
                                #region Participe Variavel Real
                                decimal porcentagemMetaP = Convert.ToDecimal(strMetaEBITDA) - 100;

                                decimal valorPPVariavel = (Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_100"]) * porcentagemMetaP) / 100;
                                funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_100"]) + valorPPVariavel;
                                #endregion

                                decimal valorBonus = (Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_100"]) * porcentagemMetaP) / 100;
                                funcionario.TotalBonusAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_100"]) + valorBonus;
                            }
                            else if (Convert.ToDecimal(strMetaEBITDA) > 130) //&& Convert.ToInt32(funcionario.Nivel) > 16)
                            {
                                //considerar EBITDA 130%
                                funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_130"]);
                                funcionario.TotalBonusAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_130"]);
                            }
                        }

                        #endregion

                        //Verifica na lista notas_modelo_metas a nota da diretoria.
                        SPList listaNotaMetas = web.Lists["notas_modelo_metas"];
                        SPQuery oNotaquery = new SPQuery();
                        oNotaquery.Query = "<Where><And><Eq><FieldRef Name=\"matricula\" /><Value Type=\"Text\">" + funcionario.Matricula + "</Value></Eq><And><Eq><FieldRef Name=\"coligada\" /><Value Type=\"Text\">" + coligada + "</Value></Eq><Eq><FieldRef Name=\"ano\" /><Value Type=\"Text\">" + this.ddlAno.SelectedValue + "</Value></Eq></And></And></Where>";

                        SPListItemCollection AvaliacaoFuncionario = listaNotaMetas.GetItems(oNotaquery);

                        double percentualPadrao = 1.5;
                        FuncionarioMeta notaFuncionario = new FuncionarioMeta();
                        string formataHtml = string.Empty;
                        if (AvaliacaoFuncionario.Count > 0)
                        {
                            notaFuncionario.NotaDiretoria = AvaliacaoFuncionario[0]["avaliacao_diretoria"].ToString();
                            notaFuncionario.ProjetosNegociados = AvaliacaoFuncionario[0]["projetos_negociados"].ToString();
                            formataHtml = "<p>&nbsp;</p>";
                        }

                        // Os próximos três laços montam a visualização com as notas referente a cada funcionarios buscado.
                        somaSalarioParticipe = 0;
                        for (int iParticipe = 0; iParticipe < baseCalculoParticipe.Count; iParticipe++)
                        {
                            sbTotalObtidoParticipe.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(baseCalculoParticipe[iParticipe] != "" ? baseCalculoParticipe[iParticipe] : "0") * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios)).ToString("N2") + "</p></td>");
                            sbCompletaQuadroParticipe.Append("<td style='min-width:100px;height:20px;width:160px' align='center'>" + formataHtml + "</td>");

                            somaSalarioParticipe += Convert.ToDouble(baseCalculoParticipe[iParticipe] != "" ? baseCalculoParticipe[iParticipe] : "0") * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios);
                        }
                        if (!ddlAno.SelectedValue.Trim().Equals("2014"))
                            sbCompletaQuadroParticipe.Append("<td style='min-width:100px;height:20px;width:160px' align='center'></td>");
                        somaSalarioInternet = 0;
                        for (int iInternet = 0; iInternet < baseCalculoInternet.Count; iInternet++)
                        {
                            sbTotalObtidoInternet.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * Convert.ToDouble(baseCalculoInternet[iInternet])).ToString("N2") + "</p></td>");
                            sbCompletaQuadroInternet.Append("<td style='min-width:100px;height:20px;width:160px' align='center'></td>");
                            somaSalarioInternet += Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * Convert.ToDouble(baseCalculoInternet[iInternet]);
                        }
                        sbCompletaQuadroInternet.Append("<td style='min-width:100px;height:20px;width:160px' align='center'></td>");

                        somaSalarioBonus = 0;
                        string descricaoCalculoBonus = string.Empty;
                        string valorCalculoBonus = string.Empty;
                        for (int iBonus = 0; iBonus < baseCalculoBonus.Count; iBonus++)
                        {
                            if (baseCalculoBonus[iBonus].ToString() != string.Empty)
                            {
                                int index = baseCalculoBonus[iBonus].ToString().IndexOf("#");
                                index++;
                                valorCalculoBonus = baseCalculoBonus[iBonus].ToString().Substring(index); //Retorna a string após o caractere #
                                descricaoCalculoBonus = baseCalculoBonus[iBonus].ToString().Substring(0, index - 1); //Retorna a string da posição o até o caractere #
                            }

                            // Pela ordem de exibição já sabemos que o ultimo item do template ser exibido é o avaliação da diretoria e o penultimo projetos negociados, por isso verificamos se é o ultimos item a ser exibido para exibir a nota individual da diretoria na lista notas_modelo_metas.
                            // if (iBonus == baseCalculoBonus.Count - 2 && projetosNegociados)
                            if (descricaoCalculoBonus.Equals("PROJETOS NEGOCIADOS"))
                            {
                                // Pegas as informações da parte com os resultados de projetos negociados
                                sbTotalObtidoBonus.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * metaProjetosNegociados * Convert.ToDouble(notaFuncionario.ProjetosNegociados)).ToString("N2") + "</p></td>");
                                sbCompletaQuadroBonus.Append("");
                                sbNotaProjetosNegociados.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color:#CCCCCC; color:Black;'><p style='font-size:8pt;font-family:Calibri'>" + notaFuncionario.ProjetosNegociados + "</p></td>");
                                somaSalarioBonus += Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * metaProjetosNegociados * Convert.ToDouble(notaFuncionario.ProjetosNegociados);// Convert.ToDouble(baseCalculoBonus[iBonus]);// * Convert.ToDecimal(baseCalculoBonus[iBonus]);
                            }
                            //else if (iBonus == baseCalculoBonus.Count - 1 && avaliacaoDiretoria)
                            else if (descricaoCalculoBonus.Equals("AVALIAÇÃO DA DIRETORIA") || descricaoCalculoBonus.Equals("AVALIAÇÃO DIRETORIA"))
                            {
                                //Pegas as informações da parte com os resultados de avaliação da diretoria
                                //sbTotalObtidoBonus.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * metaAvalicaoDiretoria * Convert.ToDouble(notaFuncionario.NotaDiretoria)).ToString("N2") + "</p></td>");
                                //25-01


                                if (Convert.ToDecimal(strMetaEBITDA) < 100)
                                {
                                    SPList spList = web.Lists["EbtidaMenorCem"];
                                    SPQuery spQuery = new SPQuery();
                                    spQuery.ViewFieldsOnly = true;
                                    spQuery.ViewFields = "<FieldRef Name='Valor' />";
                                    spQuery.Query = "<Where>" +
                                                        "<And>" +
                                                            "<Eq>" +
                                                                "<FieldRef Name='Title' />" +
                                                                    "<Value Type='Text'>" + coligada + "</Value>" +
                                                            "</Eq>" +
                                                            "<And>" +
                                                                "<Eq>" +
                                                                    "<FieldRef Name='Classe' />" +
                                                                        "<Value Type='Text'>" + funcionario.Nivel + "</Value>" +
                                                                "</Eq>" +
                                                                "<Eq>" +
                                                                    "<FieldRef Name='Ano' />" +
                                                                        "<Value Type='Text'>" + this.ddlAno.SelectedValue + "</Value>" +
                                                                "</Eq>" +
                                                            "</And>" +
                                                        "</And>" +
                                                    "</Where>";

                                    SPListItemCollection items = spList.GetItems(spQuery);
                                    if (items.Count > 0)
                                    {
                                        funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(items[0]["Valor"]);
                                    }
                                    else
                                    {
                                        //27/01 funcionario.TotalParticipeVariavelAnoNSalarios = 0;
                                    }
                                    sbTotalObtidoBonus.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(notaFuncionario.NotaDiretoria) * metaAvalicaoDiretoria * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios)).ToString("N2") + "</p></td>");
                                }
                                else if (Convert.ToDecimal(strMetaEBITDA) >= 100 && Convert.ToDecimal(strMetaEBITDA) <= 130)
                                {
                                    //considerar EBITDA 100% + proporção de 130%
                                    #region Participe Variavel Real
                                    decimal porcentagemMetaP = Convert.ToDecimal(strMetaEBITDA) - 100;
                                    //27/01 INSERACAO IF
                                    if (Convert.ToInt32(funcionario.Nivel) > 16)
                                    {
                                        decimal valorPPVariavel = (Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_100"]) * porcentagemMetaP) / 100;
                                        funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_100"]) + valorPPVariavel;
                                    }
                                    sbTotalObtidoBonus.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(notaFuncionario.NotaDiretoria) * metaAvalicaoDiretoria * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios)).ToString("N2") + "</p></td>");

                                    #endregion

                                    //27/01decimal valorBonus = (Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_100"]) * porcentagemMetaP) / 100;
                                    //27/01funcionario.TotalBonusAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_100"]) + valorBonus;
                                }
                                else if (Convert.ToDecimal(strMetaEBITDA) > 130)
                                {
                                    //considerar EBITDA 130%
                                    funcionario.TotalParticipeVariavelAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Participe Variavel_130"]);
                                    //funcionario.TotalBonusAnoNSalarios = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Bonus_130"]);
                                    sbTotalObtidoBonus.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + Convert.ToDouble(notaFuncionario.NotaDiretoria) * metaAvalicaoDiretoria * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios) + "</p></td>");
                                }

                                //sbTotalObtidoBonus.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + (Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * metaAvalicaoDiretoria * Convert.ToDouble(notaFuncionario.NotaDiretoria)).ToString("N2") + "</p></td>");

                                sbCompletaQuadroBonus.Append("");
                                if (ddlAno.SelectedValue.Trim().Equals("2014"))
                                {
                                    sbCompletaQuadroParticipe.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color:#CCCCCC; color:Black;'><p style='font-size:8pt;font-family:Calibri'>" + notaFuncionario.NotaDiretoria + "</p></td>");
                                }
                                else
                                {
                                    sbNotaAvaliacaoDiretoria.Append("<td align='center' style='min-width:100px;height:20px; width:160px; background-color:#CCCCCC; color:Black;'><p style='font-size:8pt;font-family:Calibri'>" + notaFuncionario.NotaDiretoria + "</p></td>");
                                }
                                //somaSalarioBonus += Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * metaAvalicaoDiretoria * Convert.ToDouble(notaFuncionario.NotaDiretoria);//funcionario.Bonus * Convert.ToDouble(baseCalculoBonus[iBonus]);// * Convert.ToDecimal(baseCalculoBonus[iBonus]);
                                //COMPLETA QUADRO AVALIACAO
                                //somaSalarioBonus += Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao * metaAvalicaoDiretoria * Convert.ToDouble(notaFuncionario.NotaDiretoria);//funcionario.Bonus * Convert.ToDouble(baseCalculoBonus[iBonus]);// * Convert.ToDecimal(baseCalculoBonus[iBonus]);
                                somaSalarioBonus += Convert.ToDouble(notaFuncionario.NotaDiretoria) * metaAvalicaoDiretoria * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios);
                            }

                            //else if (iBonus != baseCalculoBonus.Count - 2 && iBonus != baseCalculoBonus.Count - 1)
                            else
                            {
                                //Pegas as informações todas as outras situações de Bonus
                                sbCompletaQuadroBonus.Append("<td style='min-width:100px;height:20px; width:160px;' align='center'></td>");
                                //sbTotalObtidoBonus.Append("<td height='20px' width='160px' align='center' class='style2'>" + ((Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao) * Convert.ToDouble(baseCalculoBonus[iBonus] == string.Empty ? "0" : baseCalculoBonus[iBonus])).ToString("N2") + "</td>");
                                //somaSalarioBonus += Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) * Convert.ToDouble(baseCalculoBonus[iBonus] == string.Empty ? "0" : baseCalculoBonus[iBonus]);
                                sbTotalObtidoBonus.Append("<td align='center'  style='min-width:100px;height:20px; width:160px;background-color: #FFFF9F;'><p style='font-size:8pt;font-family:Calibri'>" + ((Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao) * Convert.ToDouble(valorCalculoBonus == string.Empty ? "0" : valorCalculoBonus)).ToString("N2") + "</p></td>");
                                //sbTotalObtidoBonus.Append("<td height='20px' width='160px' align='center' class='style2'>" + ((Convert.ToDouble(funcionario.TotalBonusAnoNSalarios)) * Convert.ToDouble(valorCalculoBonus == string.Empty ? "0" : valorCalculoBonus)).ToString("N2") + "</td>");
                                somaSalarioBonus += (Convert.ToDouble(funcionario.TotalBonusAnoNSalarios) / percentualPadrao) * Convert.ToDouble(valorCalculoBonus == string.Empty ? "0" : valorCalculoBonus);
                                //Logger.Write(string.Format("valorCalculoBonus: {0}, somaSalarioBonus: {1}", valorCalculoBonus, somaSalarioBonus), EventLogEntryType.Error, 2, 1);
                            }
                        }



                        //Monta a segunda parte do template com os resultados dos funcionarios,  se houver informações relacionadas a resultado de internet.
                        if (baseCalculoInternet.Count != 0)
                        {
                            sbParteInferiorTemplate.Append("<tr>" +
                                                 "<td border='3' style='border-style:solid; border-color:Black;'>" +
                                                  "\n<table cellspacing='1px'>" +
                                                   "<tr><td style='width:250px' colspan='2' ><strong style='font-size:12pt;font-family:Calibri;'>" + funcionario.Nome + "</strong></td></tr>" +
                                                             "<tr><td><div style='font-size:10pt;font-family:Calibri;'>" + funcionario.Funcao + "</div></td></tr>" +
                                                             "<tr><td><div style='font-size:10pt;font-family:Calibri;'>Classe " + funcionario.Nivel + "</div></td>" +
                                                             "<tr><td><div style='font-size:10pt;font-family:Calibri;'>Participe Variável</div></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style='font-size:10pt;font-family:Calibri;'>" + funcionario.TotalParticipeVariavelAnoNSalarios.ToString("N2") + "</b></td></tr>");

                            if (ddlAno.SelectedValue.Trim().Equals("2013") || ddlAno.SelectedValue.Trim().Equals("2014"))
                            {
                                sbParteInferiorTemplate.Append("<tr><td><div style='font-size:10pt;font-family:Calibri;'>Bônus (Supera)</div></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style='font-size:10pt;font-family:Calibri;'>" + funcionario.TotalBonusAnoNSalarios.ToString("N2") + "</b></td></tr></table>");
                            }
                            else
                            {
                                sbParteInferiorTemplate.Append("<tr><td><div style='font-size:10pt;font-family:Calibri;'></div></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style='font-size:10pt;font-family:Calibri;'></b></td></tr></table>");
                            }

                            sbParteInferiorTemplate.Append("</table>" +
                      "</td>" +
                        "<td border='3' style='border-style:solid; border-color:Black;'>\n" +
                         "<table cellpadding='1px' cellspacing='5px'>" +
                            "<tr>" + sbCompletaQuadroParticipe.ToString() + "</tr><tr>" + sbTotalObtidoParticipe.ToString() + "<td style='height:20px; width:160px; background-color: #FFFF9F;' align='center'>\n<strong style='font-size:10pt;font-family:Calibri;'>Total nº de Salários Participe</strong><br><strong style='font-size:10pt;font-family:Calibri;'>" + somaSalarioParticipe.ToString("N2") + "\n</strong></td></tr>" +
                         "</table>" +
                      "</td>" +
                      "<td border='3' style='border-style:solid; border-color:Black;'>" +
                         "<table cellpadding='1px' cellspacing='5px'>" +
                          "<tr>" + sbCompletaQuadroInternet.ToString() + "</tr><tr>" + sbTotalObtidoInternet.ToString() + "<td style='height:20px; width:160px; background-color: #FFFF9F align='center'>\n<strong style='font-size:10pt;font-family:Calibri;'>Total nº de Salários</strong><br><strong style='font-size:10pt;font-family:Calibri;'>" + somaSalarioInternet.ToString("N2") + "\n</strong></td></tr>" +
                         "</table>" +
                      "</td>");


                            sbParteInferiorTemplate.Append("<td border='3' style='border-style:solid; border-color:Black;'>" +
                                              "<table cellpadding='1px' cellspacing='5px'>" +
                                                 "<tr>" + sbCompletaQuadroBonus.ToString() + sbNotaProjetosNegociados.ToString() + sbNotaAvaliacaoDiretoria.ToString() + "</tr><tr>" + sbTotalObtidoBonus.ToString() + "<td style='height:20px; width:160px; background-color: #FFFF9F;' align='center'>\n<strong style='font-size:10pt;font-family:Calibri;'>Total nº de Salários</strong><br><strong style='font-size:10pt;font-family:Calibri;'>" + somaSalarioBonus.ToString("N2") + "\n</strong></td></tr>" +
                                              "</table>" +
                                           "</td>");
                        }
                        else
                        {

                            //Monta a segunda parte do template com os resultados dos funcionarios.Montando quatro colunas, sendo a primeira com os dados do funcionario, a segunda com os resultados do de participe, a terceira com resultados de internet, a quarta com resultado de bonus e resultados individuais.
                            sbParteInferiorTemplate.Append("<tr>" +
                                                 "<td border='3' style='border-style:solid; border-color:Black;'>\n" +
                                                  "\n<table cellspacing='1px'>" +
                                                     "<tr>" +
                                                         "<td>" +
                                                             "<table><tr><td style='font-size:12pt;font-family:Calibri' colspan='2' style='background-color: #FFFF9F; width:250px' ><strong style='font-size:12pt;font-family:Calibri;'>" + funcionario.Nome + "</strong></td></tr>" +
                                                             "<tr><td><div style='font-size:10pt;font-family:Calibri;'>" + funcionario.Funcao + "</div></td></tr>" +
                                                             "<tr><td><div style='font-size:10pt;font-family:Calibri;'>Classe " + funcionario.Nivel + "</div></td>" +
                                                             "<tr><td><div style='font-size:10pt;font-family:Calibri;'>Participe Variável</div></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style='font-size:10pt;font-family:Calibri;'>" + funcionario.TotalParticipeVariavelAnoNSalarios.ToString("N2") + "</b></td></tr>");
                            if (ddlAno.SelectedValue.Trim().Equals("2013") || ddlAno.SelectedValue.Trim().Equals("2014"))
                            {
                                sbParteInferiorTemplate.Append("<tr><td><div style='font-size:10pt;font-family:Calibri;'>Avaliação da Diretoria Individual</div></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style='font-size:10pt;font-family:Calibri;'>" + notaFuncionario.NotaDiretoria + "</b></td></tr></table>");
                            }
                            else
                            {
                                sbParteInferiorTemplate.Append("<tr><td><div style='font-size:10pt;font-family:Calibri;'></div></td><td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b style='font-size:10pt;font-family:Calibri;'></b></td></tr></table>");
                            }

                            if (ddlAno.SelectedValue.Trim().Equals("2014"))
                            {
                                sbParteInferiorTemplate.Append("</td>" +
                                                        "</tr>" +
                                                "</table>" +
                                            "</td>" +
                                                "<td border='3' style='border-style:solid; border-color:Black;'>" +
                                                "\n<table cellpadding='1px' cellspacing='5px'>" +
                                                    "<tr>" + sbCompletaQuadroParticipe.ToString() + "</tr><tr>" + sbTotalObtidoParticipe.ToString() + sbTotalObtidoBonus.ToString() + "<td style='min-width:100px;height:20px; width:100px; background-color: #FFFF9F;' align='center'><strong style='font-size:10pt;font-family:Calibri;'>Total nº de<br/> Salários</strong><br/><strong style='font-size:10pt;font-family:Calibri;'>" + (somaSalarioParticipe + Convert.ToDouble(notaFuncionario.NotaDiretoria) * metaAvalicaoDiretoria * Convert.ToDouble(funcionario.TotalParticipeVariavelAnoNSalarios)).ToString("N2") + "\n</strong></td></tr>" +
                                                "</table>" +
                                            "</td>");
                            }
                            else
                            {
                                sbParteInferiorTemplate.Append("</td>" +
                                                         "</tr>" +
                                                      "</table>" +
                                                   "</td>" +
                                                     "<td border='3' style='border-style:solid; border-color:Black;'>" +
                                                      "\n<table cellpadding='1px' cellspacing='5px'>" +
                                                                 "<tr>" + sbCompletaQuadroParticipe.ToString() + "</tr><tr>" + sbTotalObtidoParticipe.ToString() + "<td style='min-width:100px;height:20px; width:100px; background-color: #FFFF9F;' align='center'><strong style='font-size:10pt;font-family:Calibri;'>Total nº de<br/> Salários</strong><br/><strong style='font-size:10pt;font-family:Calibri;'>" + somaSalarioParticipe.ToString("N2") + "\n</strong></td></tr>" +
                                                      "</table>" +
                                                   "</td>");
                            }
                            if (ddlAno.SelectedValue.Trim().Equals("2013"))
                            {
                                somaSalarioBonus += Convert.ToDouble(somaSalarioParticipe);
                                //VINICIUS - COMPLETA QUADRO COR
                                sbParteInferiorTemplate.Append(@"<td border='3' style='border-style:solid; border-color:Black;'>" +
                                                      "<table cellpadding='1px' cellspacing='5px'>" +
                                                           "<tr>" + sbCompletaQuadroBonus.ToString() + sbNotaProjetosNegociados.ToString() + sbNotaAvaliacaoDiretoria.ToString() + "</tr><tr>" + sbTotalObtidoBonus.ToString() + "<td style='min-width:100px;height:20px; width:100px;background-color: #FFFF9F;' align='center'><strong style='font-size:10pt;font-family:Calibri;'>Total nº de<br/> Salárioss</strong><br/><strong style='font-size:10pt;font-family:Calibri;'>" + somaSalarioBonus.ToString("N2") + "\n</strong></td></tr>" +
                                                      "</table>" +
                                                   "</td>");
                            }
                            else if (ddlAno.SelectedValue.Trim().Equals("2014"))
                            {
                                //somaSalarioBonus += Convert.ToDouble(somaSalarioParticipe);
                                ////VINICIUS - COMPLETA QUADRO COR
                                //sbParteInferiorTemplate.Append(@"<td border='3' style='border-style:solid; border-color:Black;'>" +
                                //                      "<table cellpadding='1px' cellspacing='5px'>" +
                                //                           "<tr>" + sbCompletaQuadroBonus.ToString() + sbNotaProjetosNegociados.ToString() + sbNotaAvaliacaoDiretoria.ToString() + "</tr><tr>" + sbTotalObtidoBonus.ToString() + "<td style='min-width:100px;height:20px; width:100px;background-color: #FFFF9F;' align='center'><strong style='font-size:10pt;font-family:Calibri;'>Total nº de<br/> Salários</strong><br/><strong style='font-size:10pt;font-family:Calibri;'>" + somaSalarioBonus.ToString("N2") + "\n</strong></td></tr>" +
                                //                      "</table>" +
                                //                   "</td>");
                            }
                        }

                    } // Fim Funcionario.Nivel >= 13
                }
                return sbParteInferiorTemplate;
            }
            catch (Exception ex)
            {
                Logger.WriteError("Erro ao gerar informações do funcionário: " + ex.Message + ex.StackTrace, 1, 1);
                lblErro.Text = "Erro ao gerar informações do funcionário: " + ex.Message;
                return null;
            }
        }

        private static string ExtraiColigadaCentroCusto(string centroCusto)
        {
            string coligada = string.Empty;
            coligada = centroCusto.Substring(0, centroCusto.IndexOf('.'));

            if (coligada.Equals("02"))
                coligada = "1";
            else
                coligada = Convert.ToInt32(coligada).ToString();


            return coligada;
        }

        private DataTable PreencheAno()
        {
            try
            {
                DataTable dtAnos = null;
                SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
                using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPQuery query = new SPQuery();
                        if (this.Ano == SiteLists.Ano_2013_2014)
                        {
                            query.Query = "<Where>" +
                                                "<Or>" +
                                                    "<Eq>" +
                                                        "<FieldRef Name=\"Ano\" />" +
                                                            "<Value Type=\"Text\">2013</Value>" +
                                                    "</Eq>" +
                                                    "<Eq>" +
                                                        "<FieldRef Name=\"Ano\" />" +
                                                            "<Value Type=\"Text\">2014</Value>" +
                                                    "</Eq>" +
                                                "</Or>" +
                                           "</Where>";
                        }
                        else if (this.Ano == SiteLists.Ano_2014)
                        {
                            query.Query = "<Where>" +
                                                "<Eq>" +
                                                    "<FieldRef Name=\"Ano\" />" +
                                                        "<Value Type=\"Text\">2014</Value>" +
                                                "</Eq>" +
                                            "</Where>" +
                                            "<OrderBy>" +
                                                "<FieldRef Name=\"Ano\" />" +
                                            "</OrderBy>";
                        }
                        else if (this.Ano == SiteLists.Ano_2013)
                        {
                            query.Query = "<Where>" +
                                                "<Eq>" +
                                                    "<FieldRef Name=\"Ano\" />" +
                                                        "<Value Type=\"Text\">2013</Value>" +
                                                "</Eq>" +
                                            "</Where>" +
                                            "<OrderBy>" +
                                                "<FieldRef Name=\"Ano\" />" +
                                            "</OrderBy>";
                        }
                        else if (this.Ano == SiteLists.Ano_2012)
                        {
                            query.Query = "<Where>" +
                                                "<Eq>" +
                                                    "<FieldRef Name=\"Ano\" />" +
                                                        "<Value Type=\"Text\">2012</Value>" +
                                                "</Eq>" +
                                            "</Where>" +
                                            "<OrderBy>" +
                                                "<FieldRef Name=\"Ano\" />" +
                                            "</OrderBy>";
                        }
                        else
                        {
                            query.Query = "<Where>" +
                                                "<Or>" +
                                                    "<Eq>" +
                                                        "<FieldRef Name=\"Ano\" />" +
                                                            "<Value Type=\"Text\">2012</Value>" +
                                                    "</Eq>" +
                                                    "<Eq>" +
                                                        "<FieldRef Name=\"Ano\" />" +
                                                            "<Value Type=\"Text\">2013</Value>" +
                                                    "</Eq>" +
                                                "</Or>" +
                                           "</Where>";
                        }

                        dtAnos = web.Lists["Metas"].GetItems(query).GetDataTable();
                        DataView dtv = new DataView(dtAnos);
                        dtAnos = dtv.ToTable(true, "Ano");
                        return dtAnos;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao preencher campo Ano: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                return null;
            }


        }

        // Faz o preenchimento do centros de custo.
        private DataTable PreencheCentroCustosUsuario()
        {
            #region Produção
            string login = string.Empty;
            Gerente dadosProfile = null;
            DataTable dtCentrosCusto = null;

            dadosProfile = new Gerente();

            login = SPContext.Current.Web.CurrentUser.LoginName;

            try
            {
                //Busca matrícula e coligada do usuário atual.
                dadosProfile = ManipularDados.BuscaMatriculaColigada(login);

                SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
                using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                {
                    using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        if (PossuiAcessoTotal(login))
                            dtCentrosCusto = ManipularDados.BuscaTodosCentrosCustoParaRV();
                        //28/01else if (dadosProfile.Coligada.Trim().Equals("5") || dadosProfile.Coligada.Trim().Equals("05"))
                        //28/01dtCentrosCusto = ManipularDados.BuscaCentroCusto(dadosProfile.Matricula, "99999");
                        else
                            dtCentrosCusto = ManipularDados.BuscaCentroCusto(dadosProfile.Matricula, dadosProfile.Coligada);

                        return dtCentrosCusto;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao preencher campo Centro de Custo: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                return null;
            }
            #endregion
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

        private string SelecionaTemplate(string strCentroCusto)
        {
            string strTemplate = string.Empty;

            using (SPSite site = new SPSite(SPContext.Current.Site.Url))
            {
                using (SPWeb web = site.OpenWeb("Remuneracoes"))
                {
                    SPList listTemplate = web.Lists["Templates e Centros de Custo"];
                    SPQuery query = new SPQuery();
                    query.Query = @"<Where><Eq><FieldRef Name='Centros_x0020_de_x0020_custo' /><Value Type='Lookup'>"
                                    + strCentroCusto + @"</Value></Eq></Where>";

                    SPListItemCollection lista = listTemplate.GetItems(query);

                    foreach (SPListItem listITem in lista)
                    {
                        strTemplate = listITem["Title"].ToString() + ";" + listITem.ID.ToString();
                    }
                }
            }
            return strTemplate;
        }

        private bool ExistePasta(string p, SPWeb web)
        {
            bool existe = false;
            foreach (SPFolder folder in web.Folders["Lists"].SubFolders["Templates e Centros de Custo"].SubFolders["Attachments"].SubFolders)
            {
                if (folder.Name.Equals(p))
                    existe = true;
            }
            return existe;

        }

        // Metodo que verifica a extensão dos arquivos
        public static string tipoArquivo(string strFileExtension)
        {
            string arquivo = null;
            //verifica se  é uma imagem
            if (strFileExtension == ".xlsx" || strFileExtension == ".xlsm" || strFileExtension == ".xls" || strFileExtension == ".xlt" || strFileExtension == ".xla" || strFileExtension == ".ods" || strFileExtension == ".doc" || strFileExtension == ".docx" || strFileExtension == ".odt" || strFileExtension == ".xltx" || strFileExtension == ".pdf")
            {
                arquivo = "arquivo";
            }
            else
            {
                arquivo = "outro";
            }

            //Retorna de acordo com a verificação
            return arquivo;
        }

        // Método que faz o tratamento do retorno do item de lista de campo calculado, que faz o preenchimento da descrição e outros campos do referido centro de custo.
        public string CorrecaoString(string item)
        {
            int indexTralha = item.LastIndexOf('#');
            int indexPonto = item.IndexOf('.');
            string aux = item.Substring(indexTralha + 1, indexPonto - 4);
            return aux;
        }

        protected void Onclick_btnEnviar(object sender, ImageClickEventArgs e)
        {
            StringBuilder email = new StringBuilder();
            Label lblConteudo = new Label();
            string centroCusto = string.Empty;
            try
            {
                centroCusto = ddlCentroCusto.SelectedItem.Text;

                //lblConteudo.Text += "<table><tr><td><font color='blue'>" + centroCusto + "</font></td></tr></table></br>" + lbltabela.Text + "</tr></table>";
                //email.Append(lblConteudo.Text);     
                email.Append("<br/>" + lbltabela.Text);

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
