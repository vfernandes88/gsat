using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Cit.Globosat.Remuneracao.Formularios.BLL.AltFuncCargo;
using Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo;
using Cit.Globosat.Remuneracao.Formularios.Entidades;
using CIT.Sharepoint.Util;
using System.Configuration;
using Microsoft.SharePoint;
using System.Diagnostics;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using Globosat.Library.Entidades;
using Globosat.Library.Servicos;
using System.Linq;
using System.Web.Services;
using Cit.Globosat.Common;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Web;
using Winnovative.WnvHtmlConvert;

namespace Cit.Globosat.Remuneracao.Formularios.WebParts.AltFuncCargoVWP
{
    public partial class AltFuncCargoVWPUserControl : UserControl
    {
        public bool PDFButtonVisible { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                    {
                        #region PRODUCAO

                        using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                            {
                                bool isAdministrator = false;
                                isAdministrator = BLL.AltFuncCargo.BLL.UserExistsInList(spSite, spWebRemuneracoes, spWebRemuneracoes.CurrentUser.LoginName,
                                                    Constants.AdministradoresRemuneracaolistName);

                                this.labelNomeRequisitante.Text = spWebRemuneracoes.CurrentUser.Name;

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

                                // Seta dados em campos invisiveis... Nivel e Classe.
                                this.hiddenField_tb_Nivel.Value = dados.Classe;
                                this.hiddenField_tb_Classe.Value = dados.FaixaSalarial.ToString();

                                // Popula dados no formulário.
                                PopularColigadaMatricula(dados);

                                // Popular ComboBox de Classe.
                                this.dropDownListSalNivelProposto.Items.Clear();
                                PopularClasses(dados.Classe, dados.FaixaSalarial.ToString("00"));

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
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region DESENVOLVIMENTO
                        using (SPSite spSite = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb spWebRemuneracoes = spSite.OpenWeb(Constants.UrlWebRemuneracoes))
                            {
                                Entidades.DadosProfile dados = new Entidades.DadosProfile();
                                dados.Coligada = "1";
                                dados.Matricula = "00000";
                                dados.Classe = "I";
                                dados.FaixaSalarial = 21;

                                // Seta dados em campos invisiveis... Nivel e Classe.
                                this.hiddenField_tb_Nivel.Value = dados.Classe;
                                this.hiddenField_tb_Classe.Value = dados.FaixaSalarial.ToString();

                                // Popula dados no formulário.
                                PopularColigadaMatricula(dados);

                                // Popular ComboBox de Classe.
                                this.dropDownListSalNivelProposto.Items.Clear();
                                PopularClasses(dados.Classe, dados.FaixaSalarial.ToString("00"));

                                if (dados != null)
                                {
                                    // Popula logo no Formulário.
                                    PopularImagemLogo("2");
                                }
                                else
                                {
                                    // Popula logo no Formulário.
                                    PopularImagemLogo("0");
                                }
                            }
                        }
                        #endregion
                    }

                    AddCentroCusto(this.textBoxDiretoriaArea.Text, this.hiddenField_tb_Coligada.Value);
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

        private void PopularImagemLogo(string coligada)
        {
            try
            {
                switch (coligada)
                {
                    case "2":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/telecine.jpg";
                        break;

                    case "3":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/universal.jpg";
                        break;

                    case "4":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/canalbrasil.jpg";
                        break;

                    case "5":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/g2c.jpg";
                        break;

                    case "6":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/playboy.jpg";
                        break;

                    case "7":
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/horizonte.jpg";
                        break;

                    default:
                        this.imageLogo.ImageUrl = SPContext.Current.Web.Url + "/LogosAvisanet/globosat.jpg";
                        break;
                }
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
        /// Popula os campos do Formulário com informações vindas do banco.
        /// </summary>
        /// <param name="dt">Table com os dados</param>
        public void ExibeDados(DataTable dt)
        {
            try
            {
                this.radioButtonListFilial.SelectedValue = dt.Rows[0]["ESTADO"].ToString().Trim();
                this.hiddenField_tb_Funcionario.Value = dt.Rows[0]["NOME"].ToString().Trim();
                this.textBoxDataRequisicao.Text = DateTime.Today.ToString("dd/MM/yyyy");
                this.textBoxDiretoria.Text = dt.Rows[0]["ENDERECOPAGTO"].ToString().Trim();
                this.hiddenField_tb_DepartamentoArea.Value = dt.Rows[0]["DEPARTAMENTO"].ToString().Trim();
                this.textBoxMatricula.Text = dt.Rows[0]["CHAPA"].ToString().Trim();
                this.labelCodigoCargoAtual.Text = dt.Rows[0]["CODIGO"].ToString().Trim();
                this.textBoxCargoAtual.Text = dt.Rows[0]["CARGO"].ToString().Trim();
                this.textBoxClasseSalNivel.Text = string.Format("{0} - {1}", dt.Rows[0]["CODNIVELSAL"].ToString().Trim(), dt.Rows[0]["GRUPOSALARIAL"].ToString().Trim());
                this.textBoxDataAdmissao.Text = Convert.ToDateTime(dt.Rows[0]["DTBASE"]).ToString("dd/MM/yyyy").Trim();
                this.dateTimeControlAlteracaoValida.SelectedDate = DateTime.Today;
                this.hiddenField_JornadaAtual.Value = dt.Rows[0]["JORNADA"].ToString().Trim();
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
        public void PopularClasses(string classe)
        {
            try
            {
                this.dropDownListSalNivelProposto.Items.Clear();

                // Lista classes existentes.
                char[] classes = "ABCDEFGHI".ToCharArray();

                foreach (char item in classes)
                {
                    this.dropDownListSalNivelProposto.Items.Add(new ListItem(item.ToString(), item.ToString()));
                }

                //this.dropDownListSalNivelProposto.Items.Insert(0, new ListItem("...", "0"));
                this.dropDownListSalNivelProposto.Items.Insert(0, new ListItem(string.Empty, string.Empty));
                this.dropDownListSalNivelProposto.Items.Insert(this.dropDownListSalNivelProposto.Items.Count, new ListItem("I*", "I*"));
                
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
                this.dropDownListSalNivelProposto.Items.Clear();

                // Lista classes existentes.
                char[] classes = "ABCDEFGHI".ToCharArray();

                foreach (char item in classes)
                {
                    this.dropDownListSalNivelProposto.Items.Add(new ListItem(item.ToString(), item.ToString()));
                }

                //this.dropDownListSalNivelProposto.Items.Insert(0, new ListItem("...", "0"));
                this.dropDownListSalNivelProposto.Items.Insert(this.dropDownListSalNivelProposto.Items.Count, new ListItem("I*", "I*"));
                this.dropDownListSalNivelProposto.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        /// <summary>
        /// Cria repeater com o Histórico Salarial do colaborador
        /// </summary>
        /// <param name="dt">Tabela com o Histórico Salarial</param>
        public void CriarRepeater(List<DadosRemuneracao> listaHistoricoSalarial)
        {
            try
            {
                foreach (DadosRemuneracao itemHistoricoSalarial in listaHistoricoSalarial)
                {
                    if (itemHistoricoSalarial.Motivo.ToUpper().Contains("ADMISSÃO"))
                    {
                        this.textBoxDataAdmissao.Text = itemHistoricoSalarial.Data;
                    }
                }

                this.gridViewHistorico.DataSource = listaHistoricoSalarial.Where(item => !item.Motivo.ToUpper().Contains("ACORDO COLETIVO")).ToList();
                this.gridViewHistorico.DataBind();

                if (this.gridViewHistorico.Rows.Count <= 0)
                {
                    // Deixar apenas o cabeçalho visível.
                    this.gridViewHistorico.DataSource = new List<DadosRemuneracao>() { new Entidades.DadosRemuneracao() };
                    this.gridViewHistorico.DataBind();
                    this.gridViewHistorico.Rows[0].Visible = false;
                }
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
        public void AddCentroCusto(string matricula, string coligada)
        {
            DataTable tableCentroCusto = null;
            try
            {
                this.dropDownListCentroCusto.Items.Clear();
                tableCentroCusto = new DataTable();

                // Verifica se é administrador.
                if (matricula == "00000")
                {
                    tableCentroCusto = FormDAL.GetAllCentrosCusto();
                }
                else
                {
                    // Busca todos os centros de custo do Gerente.
                    tableCentroCusto = FormDAL.GetCentroCusto(matricula, coligada);
                }

                this.dropDownListCentroCusto.DataValueField = "CODSECAO";
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

        /// <summary>
        /// Adiciona centro de custo transferência em combobox do Formulário
        /// </summary>
        /// <param name="matricula">Matrícula do gerente</param>
        public void AddCentroCustoTransferencia(string matricula, string coligada)
        {
            DataTable tableCentroCusto = null;
            try
            {
                this.dropDownListTransferenciaPara.Items.Clear();
                tableCentroCusto = new DataTable();
                tableCentroCusto = FormDAL.GetAllCentrosCustoVazios(); // É independente de coligada.

                this.dropDownListTransferenciaPara.DataValueField = "CODSECAO_ESTADO";
                this.dropDownListTransferenciaPara.DataTextField = "COD_DESC";
                this.dropDownListTransferenciaPara.DataSource = tableCentroCusto;
                this.dropDownListTransferenciaPara.DataBind();
                this.dropDownListTransferenciaPara.Items.Insert(0, new ListItem("Selecione...", "0"));
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

        /// <summary>
        /// Adiciona funcionário que pertence a um determinado centro de custo em dropdown do Formulário
        /// </summary>
        /// <param name="centroCusto">Centro de custo que do gerente</param>
        public void AddFuncionarioCentroCusto(string centroCusto)
        {
            try
            {
                this.dropDownListFuncionarios.Items.Clear();
                this.dropDownListFuncionarios.DataValueField = "CHAPA";
                this.dropDownListFuncionarios.DataTextField = "NOME";
                this.dropDownListFuncionarios.DataSource = FormDAL.GetTodosColaboradores(centroCusto);
                this.dropDownListFuncionarios.DataBind();
                this.dropDownListFuncionarios.Items.Insert(0, new ListItem("Selecione...", "0"));
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void dropDownListCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LimparCampos();

                if (this.dropDownListCentroCusto.SelectedValue != "0")
                {
                    // Apaga os itens no dropdown.
                    this.dropDownListFuncionarios.Items.Clear();

                    // Adiciona os funcionários do centro de custo.
                    AddFuncionarioCentroCusto(this.dropDownListCentroCusto.SelectedValue);
                    this.dropDownListFuncionarios.Enabled = true;
                    this.dropDownListFuncionarios.Focus();

                    this.hiddenField_coligadaCentroCusto.Value = ExtraiColigadaCentroCusto(this.dropDownListCentroCusto.SelectedValue);

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

                            DesabilitarCampos();
                            this.dropDownListFuncionarios.SelectedValue = "0";
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

        protected void dropDownListFuncionarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LimparCampos();

                if (this.dropDownListFuncionarios.SelectedValue != "0")
                {
                    // Busca dados em campos invisiveis... Nivel e Classe.
                    int classe = Convert.ToInt32(this.hiddenField_tb_Classe.Value);
                    string nivel = this.hiddenField_tb_Nivel.Value;

                    PopularClasses(nivel);

                    List<DadosRemuneracao> listaDadosHistoricoSalarial = new List<DadosRemuneracao>();

                    // Pega matricula e coligada que estão carregados no formulário.
                    string matricula = this.dropDownListFuncionarios.SelectedValue;
                    string coligada = this.hiddenField_coligadaCentroCusto.Value; // Coligada do centro de custo.

                    string salarioAtual = string.Empty;
                    string cc = coligada;

                    // Busca todos os dados do colaborador.
                    DataTable dadosColaboradorTable = new DataTable();
                    dadosColaboradorTable = FormDAL.GetDadosColaborador(matricula, coligada);

                    if (dadosColaboradorTable.Rows.Count > 0)
                    {
                        ExibeDados(dadosColaboradorTable);

                        // Pega o salário atual do colaborador.
                        DataRow linhaSalarioAtual = FormDAL.GetSalarioAtual(matricula, cc);

                        if (linhaSalarioAtual != null)
                        {
                            salarioAtual = Convert.ToDecimal(linhaSalarioAtual["SALARIO"]).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                            // Insere salário no Formulário.
                            this.textBoxSalarioAtual.Text = salarioAtual;
                        }
                    }

                    dadosColaboradorTable.Clear();

                    // Busca dados do Histórico Salarial do Colaborador selecionado.
                    listaDadosHistoricoSalarial = BLL.AltFuncCargo.BLL.BuscaHistoricoSalarial(this.dropDownListFuncionarios.SelectedValue, cc);

                    // Pega os dados coletados e popula no repeater do formulário.
                    CriarRepeater(listaDadosHistoricoSalarial);

                    // Verifica se salário proposto já foi especificado. Se sim, faz calculo da diferenças.
                    string salarioProposto = this.textBoxSalarioProposto.Text;
                    if (salarioProposto != string.Empty)
                    {
                        decimal diferencaSalario = BLL.AltFuncCargo.BLL.CalcularDiferencaSalario(salarioProposto, salarioAtual);
                        this.textBoxDiferenca.Text = diferencaSalario.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));

                        decimal percentualDiferencaSalario = BLL.AltFuncCargo.BLL.CalcularPercentualDiferencaSalario(salarioProposto, salarioAtual);
                        this.textBoxPercentualAumentoProposto.Text = percentualDiferencaSalario.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
                    }

                    HabilitarCampos();
                    AddCentroCustoTransferencia(this.textBoxDiretoriaArea.Text, this.hiddenField_tb_Coligada.Value);
                }
                else
                {
                    DesabilitarCampos();
                    this.dropDownListFuncionarios.Enabled = true;
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
                return FormDAL.GetCodigoColigada(centroCusto);
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }

            return string.Empty;
        }

        public void LimparCampos()
        {
            try
            {
                this.textBoxDataRequisicao.Text = string.Empty;
                this.textBoxDiretoria.Text = string.Empty;
                this.textBoxMatricula.Text = string.Empty;
                this.textBoxDataAdmissao.Text = string.Empty;
                this.checkBoxTranfCentroCusto.Checked = false;
                this.dropDownListTransferenciaPara.ClearSelection();
                this.radioButtonListFilial.ClearSelection();
                this.radioButtonListMotivoPromocao.Checked = false;
                this.radioButtonListMotivoMerito.Checked = false;
                this.radioButtonListMotivoReenquadramento.Checked = false;
                this.dateTimeControlAlteracaoValida.ClearSelection();
                this.labelCodigoCargoAtual.Text = string.Empty;
                this.textBoxCargoAtual.Text = string.Empty;
                this.textBoxSalarioAtual.Text = string.Empty;
                this.textBoxClasseSalNivel.Text = string.Empty;
                this.textBoxDiferenca.Text = string.Empty;
                this.labelCodigoCargoProposto.Text = string.Empty;
                this.textBoxCargoProposto.Text = string.Empty;
                this.textBoxSalarioProposto.Text = string.Empty;
                this.textBoxClasseProposto.Text = string.Empty;
                this.dropDownListSalNivelProposto.ClearSelection();
                this.textBoxPercentualAumentoProposto.Text = string.Empty;
                this.labelNovaJornadaDiferenteAtual.Text = string.Empty;
                this.radioButtonListNovaJornada.ClearSelection();
                this.textBoxJustificativa.Text = string.Empty;

                // Deixar apenas o cabeçalho visível.
                this.gridViewHistorico.DataSource = new List<DadosRemuneracao>() { new Entidades.DadosRemuneracao() };
                this.gridViewHistorico.DataBind();
                this.gridViewHistorico.Rows[0].Visible = false;

                this.hiddenField_strJornada.Value = string.Empty;
                this.hiddenField_tb_DepartamentoArea.Value = string.Empty;
                this.hiddenField_tb_Funcionario.Value = string.Empty;
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        private void PopularSalarioProposto(string classe, string nivel, string jornada, string filial, string coligadaGerente)
        {
            try
            {
                string salarioProposto = string.Empty;

                salarioProposto = BLL.AltFuncCargo.BLL.BuscaSalarioProposto(classe, nivel, jornada, filial, coligadaGerente);
                this.textBoxSalarioProposto.Text = salarioProposto;

                string salarioAtual = this.textBoxSalarioAtual.Text;
                if (salarioAtual != string.Empty)
                {
                    decimal diferencaSalario = BLL.AltFuncCargo.BLL.CalcularDiferencaSalario(salarioProposto, salarioAtual);
                    this.textBoxDiferenca.Text = diferencaSalario.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));

                    decimal percentualDiferencaSalario = BLL.AltFuncCargo.BLL.CalcularPercentualDiferencaSalario(salarioProposto, salarioAtual);
                    this.textBoxPercentualAumentoProposto.Text = percentualDiferencaSalario.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void radioButtonListNovaJornada_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {
                    string nivel = this.textBoxClasseProposto.Text;
                    string filial = this.radioButtonListFilial.SelectedValue;
                    string jornada = this.radioButtonListNovaJornada.SelectedValue;
                    string classe = this.dropDownListSalNivelProposto.SelectedValue;
                    string coligadaGerente = this.hiddenField_coligadaCentroCusto.Value;

                    if (classe != "..." && nivel != "..." && coligadaGerente != "")
                    {
                        PopularSalarioProposto(classe, nivel, jornada, filial, coligadaGerente);
                    }
                }

            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void radioButtonListFilial_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string nivel = this.textBoxClasseProposto.Text;
                    string filial = this.radioButtonListFilial.SelectedValue;
                    string jornada = this.radioButtonListNovaJornada.SelectedValue;
                    string classe = this.dropDownListSalNivelProposto.SelectedValue;
                    string coligadaGerente = this.hiddenField_coligadaCentroCusto.Value;

                    if (classe != "..." && nivel != "..." && coligadaGerente != "")
                    {
                        PopularSalarioProposto(classe, nivel, jornada, filial, coligadaGerente);
                    }
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void checkBoxTranfCentroCusto_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.checkBoxTranfCentroCusto.Checked)
                {
                    this.dropDownListTransferenciaPara.SelectedValue = "0";
                    this.dropDownListTransferenciaPara.Enabled = false;

                    #region Seta o campo filial
                    // Pega matricula e coligada que estão carregados no formulário.
                    string matricula = this.dropDownListFuncionarios.SelectedValue;
                    string coligada = this.hiddenField_coligadaCentroCusto.Value;

                    // Busca todos os dados do colaborador.
                    DataTable dadosColaboradorTable = new DataTable();
                    dadosColaboradorTable = FormDAL.GetDadosColaborador(matricula, coligada);

                    if (dadosColaboradorTable.Rows.Count > 0)
                    {
                        this.radioButtonListFilial.SelectedValue = dadosColaboradorTable.Rows[0]["ESTADO"].ToString().Trim();
                    }
                    #endregion

                    LimparCamposFilial();
                }
                else
                {
                    this.dropDownListTransferenciaPara.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void buttonMesmoCargo_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            try
            {
                string cargoAtual = this.textBoxCargoAtual.Text;
                string salarioAtual = this.textBoxSalarioAtual.Text;
                string nivelAtual = this.textBoxClasseSalNivel.Text.Split('-')[0].Trim();

                string faixaAtual = this.textBoxClasseSalNivel.Text.Split('-')[1].Trim();
                if (string.IsNullOrEmpty(faixaAtual))
                {
                    // Setar A para faixa vazia.
                    faixaAtual = string.Empty;
                }

                int codFilial = 1;                
                if (!Convert.ToInt32(this.hiddenField_coligadaCentroCusto.Value).Equals(5))
                {
                    if (this.radioButtonListFilial.SelectedValue.Contains("SP"))
                    {
                        codFilial = 2;
                    }
                }
                int jornada = 0;

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    jornada = ManipularDados.GetJornada(cargoAtual, ((faixaAtual == string.Empty) ? "A" : faixaAtual), nivelAtual, codFilial, Convert.ToInt32(this.hiddenField_coligadaCentroCusto.Value));
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    jornada = 150;
                    #endregion
                }

                this.labelCodigoCargoProposto.Text = this.labelCodigoCargoAtual.Text;
                this.textBoxCargoProposto.Text = cargoAtual;
                this.textBoxSalarioProposto.Text = salarioAtual;
                this.textBoxClasseProposto.Text = nivelAtual;
                this.dropDownListSalNivelProposto.SelectedValue = faixaAtual;
                this.hiddenField_tb_Nivel.Value = faixaAtual;
                this.radioButtonListNovaJornada.SelectedValue = jornada.ToString();
                this.labelNovaJornadaDiferenteAtual.Text = "NÃO";
                this.hiddenField_strJornada.Value = this.radioButtonListNovaJornada.SelectedValue;

                // Atualiza a diferença e o percentual.
                this.textBoxDiferenca.Text = 0.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                this.textBoxPercentualAumentoProposto.Text = "0 %";

                // Atualizar o campo Motivo.
                UpdateMotivo();
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
            finally
            {
                dataTable.Dispose();
            }
        }


        //VINICIUS 16/03/2015 REMOVER UPDATE DE REENQUADRAMENTO
        private void UpdateMotivo()
        {
            try
            {
                if (!this.radioButtonListMotivoReenquadramento.Checked)
                {
                    decimal salarioAtual = 0;
                    decimal salarioNovo = 0;

                    if (!string.IsNullOrEmpty(this.textBoxSalarioAtual.Text))
                    {
                        salarioAtual = Convert.ToDecimal(this.textBoxSalarioAtual.Text.Replace("R$", string.Empty).Trim());
                    }

                    if (!string.IsNullOrEmpty(this.textBoxSalarioProposto.Text))
                    {
                        salarioNovo = Convert.ToDecimal(this.textBoxSalarioProposto.Text.Replace("R$", string.Empty).Trim());
                    }

                    if (salarioNovo > salarioAtual)
                    {
                        int antigaClasse = int.Parse(this.textBoxClasseSalNivel.Text.Split('-')[0].Trim());
                        int novaClasse = int.Parse(this.textBoxClasseProposto.Text);           

                        if (antigaClasse == novaClasse)
                        {
                            // Campo Motivo igual a 'Merito'.
                            this.radioButtonListMotivoPromocao.Checked = false;
                            this.radioButtonListMotivoMerito.Checked = true;
                            this.radioButtonListMotivoReenquadramento.Checked = false;
                        }
                        else if (novaClasse >= antigaClasse)
                        {
                            // Campo Motivo igual a 'Promoção'.
                            this.radioButtonListMotivoPromocao.Checked = true;
                            this.radioButtonListMotivoMerito.Checked = false;
                            this.radioButtonListMotivoReenquadramento.Checked = false;
                        }
                        else 
                        {
                            // Nenhum valor no campo Motivo.
                            this.radioButtonListMotivoPromocao.Checked = false;
                            this.radioButtonListMotivoMerito.Checked = false;
                            this.radioButtonListMotivoReenquadramento.Checked = false;
                        }
                    }
                    else 
                    {
                        // Nenhum valor no campo Motivo.
                        this.radioButtonListMotivoPromocao.Checked = false;
                        this.radioButtonListMotivoMerito.Checked = false;
                        this.radioButtonListMotivoReenquadramento.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        private void DesabilitarCampos()
        {
            this.imageButtonImprimir.Enabled = false;
            this.imageButtonImprimir.ImageUrl = "~/_layouts/images/Cit.Globosat.Base/print_icon_disable.jpg";
            this.imageButtonImprimir.ToolTip = "É preciso preencher o formulário!";
            this.imageButtonGerarPDF.Enabled = false;
            this.dropDownListFuncionarios.Enabled = false;
            this.imageButtonGerarPDF.ImageUrl = "~/_layouts/images/Cit.Globosat.Base/pdf_icon_disable.jpg";
            this.imageButtonGerarPDF.ToolTip = "É preciso preencher o formulário!";

            // Não foi aplicado a propriedade "enabeld" p/ não atravar a impressão.
            // Pois campos disabled ficam com o layout escuro após imprimir.

            this.textBoxDataRequisicao.Attributes.Add("readonly", "readonly");
            this.textBoxDiretoria.Attributes.Add("readonly", "readonly");
            this.textBoxMatricula.Attributes.Add("readonly", "readonly");
            this.textBoxDataAdmissao.Attributes.Add("readonly", "readonly");

            this.checkBoxTranfCentroCusto.Enabled = false;
            this.dropDownListTransferenciaPara.Enabled = false;
            this.dateTimeControlAlteracaoValida.Enabled = false;
            ((TextBox)this.dateTimeControlAlteracaoValida.Controls[0]).Attributes.Add("readonly", "readonly");

            this.textBoxCargoAtual.Attributes.Add("readonly", "readonly");
            this.textBoxSalarioAtual.Attributes.Add("readonly", "readonly");
            this.textBoxClasseSalNivel.Attributes.Add("readonly", "readonly");
            this.textBoxDiferenca.Attributes.Add("readonly", "readonly");

            this.textBoxCargoProposto.Attributes.Add("readonly", "readonly");
            this.buttonBuscar.Enabled = false;
            this.buttonMesmoCargo.Enabled = false;
            this.textBoxSalarioProposto.Attributes.Add("readonly", "readonly");

            this.textBoxClasseProposto.Attributes.Add("readonly", "readonly");
            this.dropDownListSalNivelProposto.Enabled = false;
            this.textBoxPercentualAumentoProposto.Attributes.Add("readonly", "readonly");

            // Deixar apenas o cabeçalho visível.
            this.gridViewHistorico.DataSource = new List<DadosRemuneracao>() { new Entidades.DadosRemuneracao() };
            this.gridViewHistorico.DataBind();
            this.gridViewHistorico.Rows[0].Visible = false;

            this.textBoxJustificativa.Attributes.Add("readonly", "readonly");
            this.textBoxDiretoriaArea.Attributes.Add("readonly", "readonly");
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

            this.checkBoxTranfCentroCusto.Enabled = true;
            this.dateTimeControlAlteracaoValida.Enabled = true;

            this.radioButtonListMotivoReenquadramento.Enabled = false;

            this.textBoxClasseSalNivel.Enabled = true;

            this.buttonBuscar.Enabled = true;
            this.buttonMesmoCargo.Enabled = true;
            this.dropDownListSalNivelProposto.Enabled = true;

            this.textBoxJustificativa.Attributes.Remove("readonly");
        }

        protected void dropDownListTransferenciaPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.dropDownListTransferenciaPara.SelectedValue != "0")
            {
                if (this.dropDownListTransferenciaPara.SelectedValue.ToUpper().Contains("RJ"))
                {
                    this.radioButtonListFilial.SelectedValue = "RJ";
                }
                else if (this.dropDownListTransferenciaPara.SelectedValue.ToUpper().Contains("SP"))
                {
                    this.radioButtonListFilial.SelectedValue = "SP";
                }

                LimparCamposFilial();
            }
        }

        protected void imageButtonGerarPDF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string downloadName = string.Format("{0}_{1}.{2}", "FormSolAltFunc", DateTime.Now.ToShortDateString().Replace("/", "_") + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_"), "pdf");
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

        protected void gridViewHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[4].Text.ToUpper().Equals("NULL"))
                {
                    e.Row.Cells[4].Text = string.Empty;
                }
            }
        }
        

        /// <summary>
        /// Limpar alguns campos. Pois estes possuem regras ligadas à filial escolhida.
        /// </summary>
        private void LimparCamposFilial()
        {
            this.radioButtonListMotivoPromocao.Checked = false;
            this.radioButtonListMotivoMerito.Checked = false;
            this.radioButtonListMotivoReenquadramento.Checked = false;
            this.textBoxDiferenca.Text = string.Empty;
            this.textBoxCargoProposto.Text = string.Empty;
            this.radioButtonListNovaJornada.ClearSelection();
            this.textBoxSalarioProposto.Text = string.Empty;
            this.textBoxClasseProposto.Text = string.Empty;
            this.dropDownListSalNivelProposto.ClearSelection();
            this.textBoxPercentualAumentoProposto.Text = string.Empty;
        }

        protected void imageButtonVoltar_Click(object sender, ImageClickEventArgs e)
        {
            SPUtility.Redirect("/remuneracoes", SPRedirectFlags.Default, HttpContext.Current);
        }

        protected void dateTimeControlAlteracaoValida_DateChanged(object sender, EventArgs e)
        {
            if (textBoxCargoAtual.Text.Trim() == textBoxCargoProposto.Text.Trim())
            {
                this.labelCodigoCargoProposto.Text = this.labelCodigoCargoAtual.Text;
            }
            else if(this.hiddenField_CodigoCargoProposto != null)
            {
                this.labelCodigoCargoProposto.Text = this.hiddenField_CodigoCargoProposto.Value;
            }
            this.labelNovaJornadaDiferenteAtual.Text = hiddenField_NovaJornadaDiferenteAtual.Value;
        }

        protected void textBoxJustificativa_TextChanged(object sender, EventArgs e)
        {

        }

        protected void textBoxJustificativa_TextChanged1(object sender, EventArgs e)
        {

        }

        protected void textBoxJustificativa_TextChanged2(object sender, EventArgs e)
        {

        }

        protected void radioButtonListMotivoReenquadramento_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void dropDownListSalNivelProposto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void hiddenFieldPDF_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
