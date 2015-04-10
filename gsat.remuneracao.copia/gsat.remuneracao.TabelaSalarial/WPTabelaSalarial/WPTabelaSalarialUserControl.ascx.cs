using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.Office.Server.UserProfiles;
using Globosat.Library.Servicos;
using Globosat.Library.AcessoDados;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SharePoint.Utilities;

namespace Globosat.Remuneracao.TabelaSalarial.WPTabelaSalarial
{
    public partial class WPTabelaSalarialUserControl : UserControl
    {
        int countLinhasTabela = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            string login = string.Empty;
            UserProfile profile = null;
            bool isAdministrator = false;
            int nivelProfile = 0;
            string classeProfile = string.Empty;
            string coligadaProfile = string.Empty;
            string filialProfile = string.Empty;
            bool profileOK = false;
            countLinhasTabela = 0;
            try
            {
                ListItemCollection opcoesTabelaSalarial = new ListItemCollection();
                ddlTabelas.AutoPostBack = true;
                login = SPContext.Current.Web.CurrentUser.LoginName;
                profile = ManipularDados.BuscaProfile(login);

                try
                {
                    //recebe o nivel salarial do funcionario e converte para int
                    nivelProfile = Convert.ToInt32(profile["FaixaSalarial"].Value.ToString());

                    //recebe classe salarial do funcionario e converte para int
                    classeProfile = profile["Classe"].Value.ToString();

                    //Busca coligada do colaborador
                    coligadaProfile = profile["Coligada"].Value.ToString();

                    //Busca filial do colaborador
                    filialProfile = profile["Filial"].Value.ToString();

                    profileOK = true;
                }
                catch
                {
                    profileOK = false;
                }

                //Verifica se o usuário é administrador
                isAdministrator = PossuiAcessoTotal(login);
                string codtabela = null;
                if (isAdministrator)
                {
                    nivelProfile = 20;
                    classeProfile = "I";
                    coligadaProfile = "1";
                    profileOK = true;
                }

                //if (!this.IsPostBack && isAdministrator)
                if (!this.IsPostBack)
                {
                    if (isAdministrator)
                    {
                        opcoesTabelaSalarial = ManipularDados.BuscaOpcoesTabelaSalarial();
                        //ddlTabelas.Items.Add(new ListItem("G2C 220H/210H/180H", "G2CX"));
                    }
                    else
                    {
                        //Busca opçoes de Tabela Salarial com a coligada da Globosat
                        opcoesTabelaSalarial = ManipularDados.BuscaOpcoesTabelaSalarial(coligadaProfile);
                    }

                    //Popula dropdownlist
                    foreach (ListItem item in opcoesTabelaSalarial)
                    {
                        ddlTabelas.Items.Add(item);
                    }

                    //Adiciona dois itens na Combo para estagiários
                    //ddlTabelas.Items.Add(new ListItem("ESTAG. GLOBOSAT RJ e CANAIS", "88"));
                    //ddlTabelas.Items.Add(new ListItem("ESTAG. GLOBOSAT SP", "89"));

                }
                else
                {
                    ddlTabelas.Items.Remove(new ListItem("Selecione...", "Selecione..."));
                }

                if (this.ddlTabelas.SelectedValue.Contains(";"))
                {
                    codtabela = this.ddlTabelas.SelectedValue.Split(';')[0];
                }
                else
                {
                    codtabela = this.ddlTabelas.SelectedValue;
                }

                if (profileOK)
                {
                    if (ddlTabelas.SelectedItem.Text.ToString().Contains("ESTAG"))
                    {
                        SPUserToken sysToken = SPContext.Current.Site.SystemAccount.UserToken;
                        //Acessa a lista com privilégios de System\\Account
                        using (var site = new SPSite(SPContext.Current.Site.ID, sysToken))
                        {
                            using (var web = site.OpenWeb(SPContext.Current.Web.ID))
                            {
                                SPListItemCollection dadosEstag = null;
                                string subTituloTabela = string.Empty;
                                //Verifica se a tabela é RJ ou SP
                                if (ddlTabelas.SelectedItem.Text.ToString().Contains("RJ"))
                                {
                                    //Acessa lista
                                    subTituloTabela = ManipularDados.BuscarTituloTabelaEstag("RJ", web);
                                    dadosEstag = ManipularDados.BuscaDadosEstag("RJ", web);
                                }
                                else
                                {
                                    subTituloTabela = ManipularDados.BuscarTituloTabelaEstag("SP", web);
                                    dadosEstag = ManipularDados.BuscaDadosEstag("SP", web);
                                }

                                if (dadosEstag != null)
                                {
                                    string htmlEstag = GerarTabelaEstag(dadosEstag, ddlTabelas.SelectedItem.Text, subTituloTabela);
                                    table_salarial.Text = htmlEstag;
                                }
                                else
                                {
                                    ddlTabelas.Visible = false;
                                    table_salarial.Text += "<br />Não foi possível carregar a Tabela Salarial. Entre em contato com o administrador.";
                                }
                            }
                        }
                    }
                    else
                    {
                        //string onde será montada a tabela
                        string tableHtml = string.Empty;

                        //Condição especial para trazer os dados da G2C, pois o codtabela é o mesmo que a Globosat.
                        if (codtabela.Equals("G2CX"))
                        {
                            coligadaProfile = "5";
                            codtabela = "01";
                        }

                        //Verifica quantos níveis tem e quantas classe tem o item selecionado.
                        DataTable niveisTabela = ManipularDados.BuscaNiveisTabelaSalarial(coligadaProfile, codtabela);
                        DataTable classesTabela = ManipularDados.BuscaClassesTabelaSalarial(coligadaProfile, codtabela);

                        //monta a parte estática da tabela
                        if (classesTabela.Rows.Count == 9)
                        {
                            //Header com porcentagem
                            tableHtml = "<table class='stats'>" +
                                                            "<tr>" +
                                                                  "<td rowspan='3' style=\"background-color: #BBB;\" class='style1'>Classe <br /> Salarial</td>";
                        }
                        else
                        {
                            //Header sem porcentagem
                            tableHtml = "<table class='stats'>" +
                                                                "<tr>" +
                                                                      "<td rowspan='2' style=\"background-color: #BBB;\" class='style1'>Classe <br /> Salarial</td>";
                        }


                        for (int i = 0; i < classesTabela.Rows.Count; i++)
                        {
                            //Insere classe salarial dinamicamente
                            tableHtml += "<td class='style1' style=\"background-color: #BBB;\">(" + classesTabela.Rows[i]["FAIXA"].ToString() + ")</td>";
                        }

                        //Insere linha de percentual
                        tableHtml += "</tr>" +
                            "<tr class='style3'>";

                        int percentualTabela = 80;

                        for (int i = 0; i < classesTabela.Rows.Count; i++)
                        {
                            tableHtml += "<td style=\"background-color: #BBB;\">" + percentualTabela.ToString() + "%</td>";
                            percentualTabela += 5;
                        }

                        tableHtml += "</tr>";

                        if (classesTabela.Rows.Count == 9)
                        {
                            tableHtml += "<tr>" +
                                              "<td style=\"background-color: #BBB;\" align='center'>Min.</td>" +
                                                "<td colspan='2' style=\"background-color: #BBB;\"></td>" +
                                                "<td style=\"background-color: #BBB;\"></td>" +
                                                "<td style=\"background-color: #BBB;\" align='center'>Mediana</td>" +
                                                "<td style=\"background-color: #BBB;\"></td>" +
                                                "<td colspan='2' style=\"background-color: #BBB;\"></td>" +
                                              "<td align='right' style=\"background-color: #BBB;\" align='center'>Max.</td>" +
                                        "</tr>";
                        }

                        int classeToInt = 0;
                        switch (classeProfile)
                        {
                            case "A":
                                classeToInt = 1;
                                break;
                            case "B":
                                classeToInt = 2;
                                break;
                            case "C":
                                classeToInt = 3;
                                break;
                            case "D":
                                classeToInt = 4;
                                break;
                            case "E":
                                classeToInt = 5;
                                break;
                            case "F":
                                classeToInt = 6;
                                break;
                            case "G":
                                classeToInt = 7;
                                break;
                            case "H":
                                classeToInt = 8;
                                break;
                            case "I":
                                classeToInt = 9;
                                break;
                            default:
                                classeToInt = 0;
                                break;
                        }

                        DataTable dtTabelaSalarial = new DataTable();

                        dtTabelaSalarial = ManipularDados.BuscaTabelaSalarial(coligadaProfile, codtabela);

                        if (niveisTabela.Rows.Count < nivelProfile)
                        {
                            nivelProfile = niveisTabela.Rows.Count;
                        }

                        if (dtTabelaSalarial.Rows.Count > 0)
                        {
                            //foreach para montar a parte dinaminca da tabela
                            for (int countNivel = 0; countNivel < nivelProfile; countNivel++)
                            {
                                if (countNivel != 21)
                                {
                                    if (countNivel % 2 == 0)
                                    {
                                        tableHtml += "<tr>";
                                    }
                                    else
                                    {
                                        tableHtml += "<tr style=\"background-color: #D9D9D9;\">";
                                    }

                                    //adiciona na tabela a classe 
                                    tableHtml += "<td align='center'>" + niveisTabela.Rows[countNivel]["NIVEL"].ToString() + "</td>";
                                    tableHtml += GerarLinha(countNivel, classeToInt, nivelProfile, dtTabelaSalarial, classesTabela.Rows.Count);
                                }
                            }

                            //finaliza a tabela
                            tableHtml += "</tr></table> ";

                            // passa a tabela para a label
                            table_salarial.Text = tableHtml;
                        }
                        else
                        {
                            if (!codtabela.Contains("Selecione"))
                            {
                                ddlTabelas.Visible = false;
                                table_salarial.Text += "<br />Não foi possível carregar a Tabela Salarial. Entre em contato com o administrador.";
                            }
                        }
                    }
                }
                else
                {
                    ddlTabelas.Visible = false;
                    table_salarial.Text += "<br />Não foi possível carregar os dados no UserProfile. Entre em contato com o administrador.";
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao carregar Tabela Salarial: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
                table_salarial.Text = "<br />Erro ao carregar a Tabela Salarial. Entre em contato com o administrador.";
            }
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
                        {
                            possuiAcesso = true;
                        }
                    }
                }
            }
            return possuiAcesso;
        }

        /// <summary>
        /// Gera html com tabela de estagiários.
        /// </summary>
        /// <param name="dadosEstag"></param>
        /// <param name="tituloTabela"></param>
        /// <returns></returns>
        private string GerarTabelaEstag(SPListItemCollection dadosEstag, string tituloTabela, string subTituloTabela)
        {
            try
            {
                string htmlEstag = string.Empty;
                //TODO: popular html

                htmlEstag += "<table width=\"100%\">" +
                                "<tr>" +
                                    "<td align=\"center\">" +
                                        "<table width=\"800px\">" +
                                            "<tr>" +
                                                "<td style=\"background-color: #C0C0C0\" align=\"center\">" +
                                                    "<div style=\"font-weight: bold; font-size: 18px\">" + tituloTabela + "</div>" +
                                                "</td>" +
                                            "</tr>" +
                                            "<tr>" +
                                                "<td style=\"height: 20px\">" +
                                                "</td>" +
                                            "</tr>" +
                                            "<tr>" +
                                                "<td style=\"background-color: #C0C0C0\" align=\"center\">" +
                                                    "<div style=\"font-weight: bold; font-size: 15px\">" + subTituloTabela + "</div>" +
                                                "</td>" +
                                            "</tr>" +
                                            "<tr>" +
                                                "<td style=\"height: 5px\">" +
                                                "</td>" +
                                            "</tr>" +
                                            "<tr>" +
                                                "<td>" +
                                                    "<table width=\"800px\" cellpadding=\"0\" cellspacing=\"0\" id=\"valores\" style=\"border-collapse: collapse;\">" +
                                                        "<tr>" +
                                                            "<td style=\"border-top-color: white; border-left-color: white\">" +
                                                            "</td>" +
                                                            "<td>" +
                                                                "<b>Carga Horária</b>" +
                                                            "</td>" +
                                                            "<td>" +
                                                                "<b>VALOR</b><br />" +
                                                                "<b>(em R$)</b>" +
                                                            "</td>" +
                                                            "<td>" +
                                                                "<b>Auxílio Transporte</b>" +
                                                            "</td>" +
                                                            "<td>" +
                                                                "<b>Total</b>" +
                                                            "</td>" +
                                                        "</tr>";

                for (int i = 0; i < dadosEstag.Count; i++)
                {
                    if (dadosEstag[i][SPBuiltInFieldId.Title] != null)
                    {
                        subTituloTabela = dadosEstag[i][SPBuiltInFieldId.Title].ToString();
                    }
                    else
                    {
                        htmlEstag += "<tr>" +
                            "<td><b>" + dadosEstag[i]["Nivel"].ToString() + "</b></td>" +
                            "<td>" + dadosEstag[i]["CargaHoraria"].ToString() + "</td>" +
                              "<td>" + VerificaValorDecimal(dadosEstag[i]["Valor"].ToString()) + "</td>" +
                             "<td>" + VerificaValorDecimal(dadosEstag[i]["AuxilioTransp"].ToString()) + "</td>" +
                        "<td>" + VerificaValorDecimal((Convert.ToDecimal(dadosEstag[i]["Valor"]) + Convert.ToDecimal(dadosEstag[i]["AuxilioTransp"])).ToString().Replace('.', ',')) + "</td>" +
                    "</tr>";
                    }
                }

                htmlEstag += "</table>" +
                         "</td>" +
                     "</tr>" +
                 "</table>" +
                "</td>" +
                "</tr>" +
                "</table>";

                return htmlEstag;
            }
            catch (Exception e)
            {
                Logger.Write("Erro gerar HTML de Tabela Salarial: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 1);
                return string.Empty;
            }
        }

        private string VerificaValorDecimal(string campo)
        {
            return Convert.ToDecimal(campo).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Função para gerar as linhas de acordo com o nivel e classe do funcionario
        /// </summary>
        /// <param name="nivelAtual">Nivel salarial máximo da tabela</param>
        /// <param name="classeLimite">Classe Salarial em que o colaborador está</param>
        /// <param name="nivelLimite">Nivel em que o colaborador está</param>
        /// <param name="dtTabelaSalarial">Tabela com os dados</param>
        /// <param name="classesTabela">Classe Salarial máxima da tabela</param>
        /// <returns>Linha da tabela html preenchida</returns>
        public string GerarLinha(int nivelAtual, int classeLimite, int nivelLimite, DataTable dtTabelaSalarial, int classesTabela)
        {
            string valores = "";
            //for para correr as as colunas
            for (int linhaClasse = 0; linhaClasse < classesTabela; linhaClasse++)
            {
                //verifica se a (classe onde está sendo preenchida é igual a classe do funcionario e o nivel do funcionario é maior que o nivel que está sendo preenchido) ou classe prenchida é maior que a do funcionario.
                if (((nivelAtual + 1 == nivelLimite) && (linhaClasse + 1 > classeLimite)) || nivelAtual > nivelLimite)
                {
                    //retorna a tabela
                    return valores;
                }
                else
                {
                    //acrescenta o item na tabela.
                    valores += "<td>" + VerificaValorDecimal(dtTabelaSalarial.Rows[countLinhasTabela]["SALARIO"].ToString().Replace('.', ',')) + "</td>";
                    countLinhasTabela++;
                }
            }

            //retorna a string com valores da tabela
            return valores;
        }

        protected void Onclick_btnEnviar(object sender, ImageClickEventArgs e)
        {
            StringBuilder email = new StringBuilder();
            Label lblConteudo = new Label();
            string centroCusto = string.Empty;
            try
            {
                centroCusto = ddlTabelas.SelectedItem.Text;

                email.Append("<br/>" + table_salarial.Text);

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
