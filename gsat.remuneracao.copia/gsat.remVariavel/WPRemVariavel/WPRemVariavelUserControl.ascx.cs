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
using System.Text;
using Microsoft.SharePoint.Utilities;
using Cit.Globosat.Common;

namespace RemVariavel.WPRemVariavel
{
    public partial class WPRemVariavelUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Gerente dadosProfile = null;
            DataTable dtCentrosCusto = null;
            try
            {
                if (!this.IsPostBack)
                {
                    this.ddlCentroCusto.Items.Add(new ListItem("Selecione...", "0"));

                    dadosProfile = new Gerente();
                    dadosProfile = ManipularDados.BuscaMatriculaColigada(SPContext.Current.Web.CurrentUser.LoginName); // Busca matrícula e coligada do usuário atual.

                    using (var site = new SPSite(SPContext.Current.Site.ID, SPContext.Current.Site.SystemAccount.UserToken))
                    {
                        using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                        {
                            if (PossuiAcessoTotal(SPContext.Current.Web.CurrentUser.LoginName))
                            {
                                dtCentrosCusto = ManipularDados.BuscaTodosCentrosCustoParaRV();
                            }
                            else
                            {
                                dtCentrosCusto = ManipularDados.BuscaCentroCusto(dadosProfile.Matricula, dadosProfile.Coligada);
                            }

                            foreach (DataRow centroCusto in dtCentrosCusto.Rows)
                            {
                                this.ddlCentroCusto.Items.Add(new ListItem(centroCusto["CODSECAO"].ToString() + " - " + centroCusto["DESCRICAO"].ToString(), centroCusto["CODSECAO"].ToString()));
                            }

                            if (dtCentrosCusto.Rows.Count == 0)
                            {
                                this.lblSelCentroCusto.Visible = false;
                                this.lblSelAno.Visible = false;
                                this.lblCentroCusto.Visible = false;
                                this.ddlCentroCusto.Visible = false;
                                this.ddlAno.Visible = false;
                                this.lblErroMsg.Visible = true;
                                this.lblErroMsg.Text = "Não existe nenhum Centro de Custo para visualização.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar msg de erro ao usuário.
                lblErroMsg.Visible = true;
                lblErroMsg.Text = "Erro ao tentar abrir a página. Entre em contato com o Administrador.";

                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
            finally
            {
                dadosProfile = null;
                if (dtCentrosCusto != null)
                    dtCentrosCusto.Dispose();
            }
        }

        /// <summary>
        /// Consolida informações dos usuários. 
        /// Como a  view não traz dados consolidades (sem "distinct") foi necessário criar este método.
        /// </summary>
        /// <param name="oColaborador"></param>
        /// <param name="listaFunc"></param>
        private void ConsolidaSeExistir(FuncionarioRem oColaborador, List<FuncionarioRem> listaFunc)
        {
            //FuncionarioRem oColaboradorAux = oColaborador;

            //foreach (FuncionarioRem func in listaFunc)
            //{
            //    if (func.Matricula.Equals(oColaborador.Matricula))
            //    {
            //        oColaboradorAux = func;

            //        oColaboradorAux.Participe1 = (oColaborador.Participe1 > 0) ? oColaborador.Participe1 : func.Participe1;
            //        oColaboradorAux.ParticipeVariavel1 = (oColaborador.ParticipeVariavel1 > 0) ? oColaborador.ParticipeVariavel1 : func.ParticipeVariavel1;
            //        oColaboradorAux.Bonus1 = (oColaborador.Bonus1 > 0) ? oColaborador.Bonus1 : func.Bonus1;

            //        //oColaboradorAux.ParticipeVariavel1 += oColaborador.ParticipeVariavelExtra1;
            //        //oColaboradorAux.Bonus1 += oColaborador.BonusExtra1;


            //        oColaboradorAux.Participe7 = (oColaborador.Participe7 > 0) ? oColaborador.Participe7 : func.Participe7;
            //        oColaboradorAux.ParticipeVariavel7 = (oColaborador.ParticipeVariavel7 > 0) ? oColaborador.ParticipeVariavel7 : func.ParticipeVariavel7;
            //        oColaboradorAux.Bonus7 = (oColaborador.Bonus7 > 0) ? oColaborador.Bonus7 : func.Bonus7;

            //        //oColaboradorAux.ParticipeVariavel7 += oColaborador.ParticipeVariavelExtra7;
            //        //oColaboradorAux.Bonus7 += oColaborador.BonusExtra7;

            //        oColaboradorAux.TotalParticipeAno = oColaboradorAux.Participe1 + oColaboradorAux.Participe7;
            //        oColaboradorAux.TotalParticipeVariavelAno = oColaboradorAux.ParticipeVariavel1 + oColaboradorAux.ParticipeVariavel7;
            //        oColaboradorAux.TotalBonusAno = oColaboradorAux.Bonus1 + oColaboradorAux.Bonus7;

            //        oColaboradorAux.Total1 = oColaboradorAux.Participe1 + oColaboradorAux.ParticipeVariavel1;
            //        oColaboradorAux.Total7 = oColaboradorAux.Participe7 + oColaboradorAux.ParticipeVariavel7;

            //        oColaboradorAux.TotalS = oColaboradorAux.TotalParticipeAno + oColaboradorAux.TotalParticipeVariavelAno + oColaboradorAux.TotalBonusAno;
            //        oColaboradorAux.TotalParticipeAnoNSalarios = oColaboradorAux.TotalParticipeAno / oColaboradorAux.SalarioNumber;
            //        oColaboradorAux.TotalParticipeVariavelAnoNSalarios = oColaboradorAux.TotalParticipeVariavelAno / oColaboradorAux.SalarioNumber;
            //        oColaboradorAux.TotalBonusAnoNSalarios = oColaboradorAux.TotalBonusAno / oColaboradorAux.SalarioNumber;

            //        oColaboradorAux.TotalNSalarios = oColaboradorAux.TotalParticipeAnoNSalarios + oColaboradorAux.TotalParticipeVariavelAnoNSalarios + oColaboradorAux.TotalBonusAnoNSalarios;

            //        if (func.Matricula.Equals(oColaborador.Matricula) && func.Descricao.Equals("BONUS"))
            //        {
            //            if (oColaborador.Mes.Equals("1") && oColaborador.Descricao.Equals("BONUS EXTRA"))
            //                oColaboradorAux.Bonus1 += oColaborador.BonusExtra1;
            //            else if (oColaborador.Mes.Equals("7") && oColaborador.Descricao.Equals("BONUS EXTRA"))
            //                oColaboradorAux.Bonus7 += oColaborador.BonusExtra7;
            //        }
            //        else if (func.Matricula.Equals(oColaborador.Matricula) && func.Descricao.Equals("PARTICIPE VARIAVEL"))
            //        {
            //            if (oColaborador.Mes.Equals("1") && oColaborador.Descricao.Equals("PARTICIPE VARIAVEL EXTRA"))
            //                oColaboradorAux.ParticipeVariavel1 += oColaborador.ParticipeVariavelExtra1;
            //            else if (oColaborador.Mes.Equals("7") && oColaborador.Descricao.Equals("PARTICIPE VARIAVEL EXTRA"))
            //                oColaboradorAux.ParticipeVariavel7 += oColaborador.ParticipeVariavelExtra7;
            //        }
            //    }
            //}

            ////Se ainda não existe então adicionado na lista de Funcionários
            //if (!VerificaSeJahExiste(oColaboradorAux.Matricula, listaFunc))
            //    listaFunc.Add(oColaboradorAux);


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
        /// Devido à alteração na regra de negócio, quando no centro de custo as iniciais forem "02", a coligada será 1.
        /// </summary>
        /// <param name="centroCusto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Verifica se o usuário logado faz parte do grupo de Administradores
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
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
            string tabelaHtml = string.Empty;
            try
            {
                tabelaHtml += "<table width='100%'><tr><td font-family:Calibri; align='center'><font color='black' face='Calibri'><b>" + ddlCentroCusto.SelectedItem.Text + "</b></br></font></td></tr></table>";

                tabelaHtml += string.Format(@"<table width='100%'><tr><td  align='center' style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width:180px; min-width:140px;' >
                        <span><b>Imagem</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 180px; min-width:140px;' align='center'>
                        <span><b>Nome</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 150px; min-width:110px;' align='center'>
                        <span><b>Função</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 150px; min-width:110px;' align='center'>
                        <span><b>Salário (dez/{0})</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 110px; min-width:110px;' align='center'>
                        <span><b>Pagamento</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 110px; min-width:110px;' align='center'>
                        <span><b>Participe</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 110px; min-width:110px;' align='center'>
                        <span><b>Part. Variável</b></span>
                    </td>
                    <td style='font-family:Calibri; border-width: 1px; border-style: solid; border-color: black; width: 110px; min-width:110px;' align='center'>
                        <span><b>Totais</b></span>
                    </td>
                    </tr>
                </table><br/>", DateTime.Now.Year - 1);
                Repeater htmlRepeater = (Repeater)FindControlRecursive(this.Page, "rptColaboradores");
                tabelaHtml += RenderControlToHtml(htmlRepeater);


            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);

                SPUtility.TransferToErrorPage("Ocorreu um erro ao enviar o email.", null, null);
            }

            Email.EnvioEmail(SPContext.Current.Web.CurrentUser.Email, "Remuneração Variável", tabelaHtml);
            SPUtility.TransferToSuccessPage("Email enviado com sucesso.", "/Paginas/remuneracaoVariavelAno.aspx", null, null);
        }

        public string RenderControlToHtml(Control ControlToRender)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter stWriter = new System.IO.StringWriter(sb);
            System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(stWriter);
            ControlToRender.RenderControl(htmlWriter);
            return sb.ToString();
        }

        public static Control FindControlRecursive(Control container, string name)
        {
            if ((container.ID != null) && (container.ID.Equals(name)))
                return container;

            foreach (Control ctrl in container.Controls)
            {
                Control foundCtrl = FindControlRecursive(ctrl, name);
                if (foundCtrl != null)
                    return foundCtrl;
            }
            return null;
        }

        protected void ddlCentroCusto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlCentroCusto.SelectedValue != "0")
            {
                this.ddlAno.Enabled = true;
                this.ddlAno.SelectedValue = "0";
                this.ddlAno.Focus();

                this.trDados.Visible = false;
                this.rptColaboradores.Visible = false;
                this.txtLegenda.Visible = false;
                this.lblLegenda.Visible = false;

                // Caso o centro de custo for da coligada G2C (código coligada igual a 5) então, remover o ano de 2012.
                if (this.ddlCentroCusto.SelectedValue.StartsWith("5"))
                {
                    this.ddlAno.Items.Remove(new ListItem("2012", "2012"));
                    this.ddlAno.Items.Remove(new ListItem("2013", "2013"));
                }
                else
                {
                    bool flag = false;
                    bool flag2 = false;
                    foreach (ListItem item in this.ddlAno.Items)
                    {
                        if (item.Value == "2012")
                        {
                            flag = true;
                        }
                        if((item.Value == "2013"))                           
                        {
                            flag2 = true;
                        }
                    }

                    if (!flag)
                    {
                        this.ddlAno.Items.Insert(1, new ListItem("2012", "2012"));
                    }
                    if (!flag2)
                    {
                        this.ddlAno.Items.Insert(2, new ListItem("2013", "2013"));
                    }
                }
            }
            else
            {
                this.ddlAno.Enabled = false;
                this.trDados.Visible = false;
                this.rptColaboradores.Visible = false;
                this.txtLegenda.Visible = false;
                this.lblLegenda.Visible = false;
            }
        }

        protected void ddlAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlAno.SelectedValue != "0")
                {
                    this.trDados.Visible = true;
                    this.tableHeader.Visible = true;
                    this.lblCentroCusto.Text = ddlCentroCusto.SelectedItem.Text;
                    this.lblErroMsg.Visible = false;

                    string centroCusto = ddlCentroCusto.SelectedItem.Value;
                    string coligada = string.Empty;
                    coligada = ExtraiColigadaCentroCusto(centroCusto);

                    using (var spSite = new SPSite(SPContext.Current.Site.ID, SPContext.Current.Site.SystemAccount.UserToken))
                    {
                        using (var spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                        {
                            DataTable dtColaboradores = new DataTable();
                            if (PossuiAcessoTotal(SPContext.Current.Web.CurrentUser.LoginName))
                            {
                                dtColaboradores = ManipularDados.BuscaMatriculaColaboradoresRemuneracaoVariavel(centroCusto, Convert.ToInt32(this.ddlAno.SelectedValue));
                            }
                            else
                            {
                                Gerente dadosProfile = ManipularDados.BuscaMatriculaColigada(SPContext.Current.Web.CurrentUser.LoginName); // Busca matrícula e coligada do usuário atual.
                                dtColaboradores = ManipularDados.BuscaMatriculaColaboradoresRemuneracaoVariavel(centroCusto, dadosProfile.Coligada, dadosProfile.Matricula, Convert.ToInt32(this.ddlAno.SelectedValue));
                                dadosProfile = null;
                            }

                            if ((dtColaboradores != null) && (dtColaboradores.Rows.Count > 0))
                            {
                                int ano = Convert.ToInt32(this.ddlAno.SelectedValue);
                                if (ano == 2012)
                                {
                                    // Seta mês de acordo com o ano.
                                    this.lblMesSalario.Text = "Dez";

                                    // Mostrar a coluna bônus.
                                    this.tdColBonusTitulo.Visible = true;

                                    // Exibir legendas.
                                    this.lblLegenda.Visible = true;
                                    this.lblLegenda.Text = "Valores brutos a serem pagos em janeiro.";
                                    this.txtLegenda.Visible = true;
                                }
                                else
                                {
                                    // Ocultar a coluna bônus.
                                    this.tdColBonusTitulo.Visible = false;

                                    // Seta mês de acordo com o ano.
                                    this.lblMesSalario.Text = "Dez";

                                    // Ocultar legendas.
                                    this.lblLegenda.Visible = true;
                                    this.lblLegenda.Text = "Todos os valores desta tabela são brutos.";
                                    this.txtLegenda.Visible = false;
                                }

                                SPServiceContext serviceContext = SPServiceContext.GetContext(spSite); // Instância contexto para busca em profile.
                                UserProfileManager userProfileManager = new UserProfileManager(serviceContext); // Inicializa o usuário gerenciador de perfis.

                                List<FuncionarioRem> listColaboradores = new List<FuncionarioRem>();
                                foreach (DataRow linhaColaboradores in dtColaboradores.Rows)
                                {
                                    DadosProfile infoProfile = new DadosProfile();
                                    DataTable dtInfoColaborador = new DataTable();
                                    FuncionarioRem oColaborador = new FuncionarioRem();

                                    dtInfoColaborador = ManipularDados.BuscaColaboradoresRemuneracaoVariavel(centroCusto, linhaColaboradores["CHAPA"].ToString(), Convert.ToInt32(this.ddlAno.SelectedValue));
                                    if ((dtInfoColaborador != null) && (dtInfoColaborador.Rows.Count > 0))
                                    {
                                        string salarioColaborador = ManipularDados.BuscarSalarioColaborador(linhaColaboradores["CHAPA"].ToString(), coligada, Convert.ToInt32(this.ddlAno.SelectedValue));

                                        // Popular Dados.
                                        oColaborador.Nome = dtInfoColaborador.Rows[0]["NOME"] as string;
                                        oColaborador.Salario = Convert.ToDecimal(salarioColaborador == string.Empty ? dtInfoColaborador.Rows[0]["SALARIO"] : salarioColaborador).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                                        oColaborador.SalarioNumber = Convert.ToDecimal(salarioColaborador == string.Empty ? dtInfoColaborador.Rows[0]["SALARIO"] : salarioColaborador);
                                        oColaborador.Funcao = dtInfoColaborador.Rows[0]["CARGO"] as string;
                                        oColaborador.Matricula = linhaColaboradores["CHAPA"].ToString();
                                        oColaborador.Admissao = Convert.ToDateTime(dtInfoColaborador.Rows[0]["Admissao"]).ToString("dd/MM/yyyy");

                                        // Preenche nível salarial e classe.
                                        foreach (DataRow linhaInformacaoColaborador in dtInfoColaborador.Rows)
                                        {
                                            oColaborador.Descricao = linhaInformacaoColaborador["DESCRICAO"].ToString().ToUpper().Trim();
                                            oColaborador.Mes = linhaInformacaoColaborador["MESCOMP"].ToString().Trim();

                                            if (linhaInformacaoColaborador["DESCRICAO"].ToString().ToUpper().Trim().Equals("PARTICIPE"))
                                            {
                                                if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("1") || linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("2"))
                                                    oColaborador.Participe1 = Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                                else if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("7"))
                                                    oColaborador.Participe7 = Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);

                                            }
                                            else if (linhaInformacaoColaborador["DESCRICAO"].ToString().ToUpper().Trim().Equals("PARTICIPE VARIAVEL"))
                                            {
                                                if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("1") || linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("2"))
                                                    oColaborador.ParticipeVariavel1 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                                else if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("7"))
                                                    oColaborador.ParticipeVariavel7 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                            }
                                            else if (linhaInformacaoColaborador["DESCRICAO"].ToString().ToUpper().Trim().Equals("PARTICIPE VARIAVEL EXTRA"))
                                            {
                                                if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("1") || linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("2"))
                                                    oColaborador.ParticipeVariavel1 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                                else if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("7"))
                                                    oColaborador.ParticipeVariavel7 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                            }
                                            else if (linhaInformacaoColaborador["DESCRICAO"].ToString().ToUpper().Trim().Equals("BONUS"))
                                            {
                                                if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("1") || linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("2"))
                                                    oColaborador.Bonus1 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                                else if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("7"))
                                                    oColaborador.Bonus7 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                            }
                                            else if (linhaInformacaoColaborador["DESCRICAO"].ToString().ToUpper().Trim().Equals("BONUS EXTRA"))
                                            {
                                                if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("1") || linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("2"))
                                                    oColaborador.Bonus1 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                                else if (linhaInformacaoColaborador["MESCOMP"].ToString().Trim().Equals("7"))
                                                    oColaborador.Bonus7 += Convert.ToDecimal(linhaInformacaoColaborador["VALOR"]);
                                            }
                                        }

                                        oColaborador.TotalParticipeAno = oColaborador.Participe1 + oColaborador.Participe7;
                                        oColaborador.TotalParticipeVariavelAno = oColaborador.ParticipeVariavel1 + oColaborador.ParticipeVariavel7;
                                        oColaborador.TotalBonusAno = oColaborador.Bonus1 + oColaborador.Bonus7;

                                        oColaborador.Total1 = oColaborador.Participe1 + oColaborador.ParticipeVariavel1 + oColaborador.Bonus1;
                                        oColaborador.Total7 = oColaborador.Participe7 + oColaborador.ParticipeVariavel7 + oColaborador.Bonus7;

                                        oColaborador.TotalS = oColaborador.TotalParticipeAno + oColaborador.TotalParticipeVariavelAno + oColaborador.TotalBonusAno;
                                        oColaborador.TotalParticipeAnoNSalarios = oColaborador.TotalParticipeAno / oColaborador.SalarioNumber;
                                        oColaborador.TotalParticipeVariavelAnoNSalarios = oColaborador.TotalParticipeVariavelAno / oColaborador.SalarioNumber;
                                        oColaborador.TotalBonusAnoNSalarios = oColaborador.TotalBonusAno / oColaborador.SalarioNumber;

                                        oColaborador.TotalNSalarios = oColaborador.TotalParticipeAnoNSalarios + oColaborador.TotalParticipeVariavelAnoNSalarios + oColaborador.TotalBonusAnoNSalarios;

                                        infoProfile = ManipularDados.BuscaDadosColaborador(oColaborador.Matricula, coligada, userProfileManager, BaseDados.GetConnectionUP());
                                        if (infoProfile != null)
                                            oColaborador.Foto = infoProfile.Foto;

                                        listColaboradores.Add(oColaborador);
                                    }

                                    // Finaliza objetos.
                                    infoProfile = null;
                                    dtInfoColaborador.Dispose();
                                    oColaborador = null;
                                }

                                this.rptColaboradores.Visible = true;
                                this.rptColaboradores.DataSource = listColaboradores;
                                this.rptColaboradores.DataBind();

                                // Finaliza objetos.
                                dtColaboradores.Dispose();
                            }
                            else
                            {
                                this.trDados.Visible = false;
                                this.rptColaboradores.Visible = false;
                                this.txtLegenda.Visible = false;
                                this.lblLegenda.Visible = false;
                                this.lblErroMsg.Visible = true;
                                this.lblErroMsg.Text = "Não foram encontrados colaboradores!";
                            }
                        }
                    }
                }
                else
                {
                    this.trDados.Visible = false;
                    this.rptColaboradores.Visible = false;
                    this.txtLegenda.Visible = false;
                    this.lblLegenda.Visible = false;
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }

        protected void rptColaboradores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
                {
                    if (Convert.ToInt32(this.ddlAno.SelectedValue) == 2013 || Convert.ToInt32(this.ddlAno.SelectedValue) == 2014)
                    {
                        // Ocultar a linha referente à 2ª parcela de janeiro para o ano de 2013.
                        e.Item.FindControl("trParcelaJaneiro").Visible = true;
                        e.Item.FindControl("trTotalAno").Visible = true;

                        // Ocultar a coluna bônus.
                        e.Item.FindControl("tdColBonus7").Visible = false;
                        e.Item.FindControl("tdColBonus1").Visible = false;
                        e.Item.FindControl("tdColTotalBonusAno").Visible = false;
                        e.Item.FindControl("tdColTotalBonusAnoNSalarios").Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, CategoryID.Hard, EventID.Remuneracao);
            }
        }
    }
}
