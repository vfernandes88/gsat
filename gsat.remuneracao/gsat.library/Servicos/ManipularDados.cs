using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.SharePoint;
using System.IO;
using System.Collections;
using Globosat.Library.Entidades;
using CIT.Sharepoint.Util;
using System.Diagnostics;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.WebControls;
using System.Globalization;
using Globosat.Library.AcessoDados;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using Cit.Globosat.Common.Extensions;

namespace Globosat.Library.Servicos
{
    public class ManipularDados
    {
        /// <summary>
        /// Verifica se usuário pertence a grupo de administradores
        /// </summary>
        /// <param name="web">Instância da web</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="lista">Lista de acesso para verificação</param>
        /// <returns>True ou False</returns>
        public static bool VerificaLogin(SPWeb web, string login, string lista)
        {
            try
            {
                SPQuery oQuery = new SPQuery();
                oQuery.Query = string.Format("<Where><Eq><FieldRef Name=\"Usuario\" /><Value Type=\"User\">{0}</Value></Eq></Where>", login);

                SPListItemCollection listaUsuarios = web.Lists[lista].GetItems(oQuery);

                if (listaUsuarios.Count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro validar usuário administrador : {0}.", ex.Message + ex.StackTrace), EventLogEntryType.Error, 2, 2);
                return false;
            }
        }

        /// <summary>
        /// Busca dados de Tabela Salarial selecionada
        /// </summary>
        /// <param name="coligada">Coligada</param>
        /// <param name="nivel">Faixa Salarial</param>
        /// <param name="codigoTabela">Código da Tabela Salarial</param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable BuscaTabelaSalarial(string coligada, string codigoTabela)
        {
            return AcessoDados.AcessoDados.GetTabelaSalarial(codigoTabela, coligada);
        }

        /// <summary>
        /// Manipula e trata os dados vindo do Banco de Dados
        /// </summary>
        /// <param name="matriculaFuncionario">Matrícula do colaborador consultado</param>
        /// <param name="coligada">Coligada do colaborador consultado</param>
        /// <returns>Array com os dados tratados</returns>
        public static List<Funcionario> 
            PopularGridView(string matriculaFuncionario, string coligada, bool isChecked)
        {
            List<Funcionario> listaHistoricoSalarial = new List<Funcionario>();
            DataTable tableHistoricoSalarial = new DataTable();
            List<Funcionario> listaSemAcordoColetivo = new List<Funcionario>();
            decimal percentual = 0;

            try
            {
                //Busca dados no banco
                tableHistoricoSalarial = AcessoDados.AcessoDados.GetHistoricoSalarialAcordoColetivo(matriculaFuncionario, coligada);

                //Cria iteração com os dados
                for (int i = 0; i < tableHistoricoSalarial.Rows.Count; i++)
                {
                    Funcionario funcHistoricoSalarial = new Funcionario();

                    if (i == 0)
                    {
                        //Percentual (Começa em 0 porque nada foi calculado ainda.
                        funcHistoricoSalarial.Percentual = "0%";
                        funcHistoricoSalarial.PercentualNumber = 0;
                    }

                    //Data
                    if (tableHistoricoSalarial.Rows[i]["Data"] != null)
                        funcHistoricoSalarial.Data = Convert.ToDateTime(tableHistoricoSalarial.Rows[i]["Data"]).ToString("MM/yyyy");

                    //Salário
                    if (tableHistoricoSalarial.Rows[i]["Salário"] != null)
                    {
                        funcHistoricoSalarial.SalarioNumber = Convert.ToDecimal(tableHistoricoSalarial.Rows[i]["Salário"]);
                        funcHistoricoSalarial.Salario = funcHistoricoSalarial.SalarioNumber.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                    }

                    //Percentual
                    if (i != 0)
                    {
                        percentual = Math.Round(CalculaPercentual(funcHistoricoSalarial.SalarioNumber, listaHistoricoSalarial[i - 1].SalarioNumber), 5);
                        funcHistoricoSalarial.Percentual = percentual.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
                        funcHistoricoSalarial.PercentualNumber = percentual * 100;
                    }

                    //Motivo
                    if (tableHistoricoSalarial.Rows[i]["Motivo"] != null)
                        funcHistoricoSalarial.Motivo = tableHistoricoSalarial.Rows[i]["Motivo"].ToString();

                    //Cargo
                    if (tableHistoricoSalarial.Rows[i]["Cargo"] != null)
                    {
                        if (i == 0)
                            funcHistoricoSalarial.Funcao = tableHistoricoSalarial.Rows[i]["Cargo"].ToString();
                        else if (i > 0 && tableHistoricoSalarial.Rows[i]["Cargo"].ToString() != tableHistoricoSalarial.Rows[i - 1]["Cargo"].ToString())
                            funcHistoricoSalarial.Funcao = tableHistoricoSalarial.Rows[i]["Cargo"].ToString();
                    }

                    //Nível
                    if (tableHistoricoSalarial.Rows[i]["Nível"] != null)
                        funcHistoricoSalarial.Nivel = tableHistoricoSalarial.Rows[i]["Nível"].ToString();

                    //Classe
                    if (tableHistoricoSalarial.Rows[i]["Classe"] != null)
                        funcHistoricoSalarial.Classe = tableHistoricoSalarial.Rows[i]["Classe"].ToString();

                    listaHistoricoSalarial.Add(funcHistoricoSalarial);
                }

                if (!isChecked)
                {
                    for (int i = 0; i < listaHistoricoSalarial.Count; i++)
                    {
                        if (listaHistoricoSalarial[i].Motivo != "ACORDO COLETIVO")
                        {
                            listaSemAcordoColetivo.Add(listaHistoricoSalarial[i]);
                        }
                    }

                    return listaSemAcordoColetivo;
                }

                return listaHistoricoSalarial;

            }
            catch (Exception e)
            {
                Logger.Write("Erro ao popular dados: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                throw;
            }
        }

        /// <summary>
        /// Calcula percentual entre salários
        /// </summary>
        /// <param name="SalarioAtual">Salário Atual</param>
        /// <param name="SalarioAnterior">Salário Anterior</param>
        /// <returns>Diferença Calculada</returns>
        private static decimal CalculaPercentual(decimal SalarioAtual, decimal SalarioAnterior)
        {
            try
            {
                //Calcula e retorna o percentual
                return (SalarioAtual / SalarioAnterior) - 1;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao calcular porcentagem: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                throw;
            }
        }

        /// <summary>
        /// Busca dados do Gestor em Profile
        /// </summary>
        /// <param name="login">Login do usuário atual</param>
        /// <returns>Entidade com a matrícula e coligada</returns>
        public static Gerente BuscaMatriculaColigada(string login)
        {
            try
            {
                Gerente gerente = new Gerente();
                UserProfileManager upm = new UserProfileManager(SPServiceContext.Current);
                UserProfile user = null;

                //Verifica se usuário existe
                if (upm.UserExists(login))
                {
                    //pega dados do profile
                    user = upm.GetUserProfile(login);

                    try
                    {
                        //Busca a coligada
                        gerente.Coligada = user["Coligada"].ToString();

                    }
                    catch
                    {
                        gerente.Coligada = "0";
                    }

                    try
                    {
                        //Busca a matrícula
                        gerente.Matricula = user["Matricula"].ToString();
                    }
                    catch
                    {
                        gerente.Matricula = "99999";
                    }

                    return gerente;
                }
                else
                {
                    gerente.Coligada = "0";
                    gerente.Matricula = "99999";
                }
                return gerente;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao buscar Matrícula e Coligada de Gerente: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                throw;
            }
        }

        /// <summary>
        /// Busca centro de custo em banco de dados
        /// </summary>
        /// <param name="matricula">Matrícula de usuário logado</param>
        /// <returns>Data Table com centros de custo</returns>
        public static DataTable BuscaCentroCusto(string matricula, string coligada)
        {
            return AcessoDados.AcessoDados.GetCentroCusto(matricula, coligada);
        }

        public static DataTable BuscaCentroCustoAtivos(string matricula, string coligada)
        {
            return AcessoDados.AcessoDados.GetCentroCustoAtivos(matricula, coligada);
        }

        public static DataTable BuscaCentroCustoAtivosD(string matricula, string coligada)
        {
            return AcessoDados.AcessoDados.GetCentroCustoAtivosD(matricula, coligada);
        }

        public static DataTable BuscaCentroCustoToPremios(string matricula, string codigoColigada)
        {
            return AcessoDados.AcessoDados.GetCentroCustoToPremios(matricula, codigoColigada);
        }

        public static DataTable BuscaTodosCentroCustoToPremios()
        {
            return AcessoDados.AcessoDados.GetAllCentroCustoToPremios();
        }

        /// <summary>
        /// Busca colaboradores em banco de dados
        /// </summary>
        /// <param name="centroCusto">Centro de custo do colaborador</param>
        /// <returns>Data Table com todos os colaboradores</returns>
        public static DataTable BuscaColaboradores(string centroCusto, string coligada, string matricula)
        {
            return AcessoDados.AcessoDados.GetTodosColaboradores(centroCusto, coligada, matricula);
        }

        public static DataTable BuscaColaboradores(string centroCusto)
        {
            return AcessoDados.AcessoDados.GetTodosColaboradores(centroCusto);
        }

        public static DataTable BuscaColaboradoresToPremios(string centroCusto)
        {
            return AcessoDados.AcessoDados.GetTodosColaboradoresToPremios(centroCusto);
        }

        public static DataTable BuscaColaboradoresToPremiosIN(string centroCusto)
        {
            return AcessoDados.AcessoDados.GetTodosColaboradoresToPremiosIN(centroCusto);
        }

        /// <summary>
        /// Busca lista do sharepoint
        /// </summary>
        /// <param name="web">Site onde está a lista</param>
        /// <param name="nomeLista">Nome da lista solicitada</param>
        /// <returns>Coleção de itens</returns>
        public static SPListItemCollection BuscaLista(SPWeb web, string nomeLista)
        {
            SPList list = web.Lists[nomeLista];

            return list.Items;
        }

        /// <summary>
        /// Busca e trata opções de Tabela Salarial vindas do Banco Quando o usuário for Admnistrador
        /// </summary>
        /// <param name="coligada"></param>
        /// <returns>Opções de Tabela Salarial</returns>
        public static ListItemCollection BuscaOpcoesTabelaSalarial()
        {
            try
            {
                DataTable opcaoTabelaSalarial = new DataTable();
                ListItemCollection collecaoOpcoes = new ListItemCollection();
                opcaoTabelaSalarial = AcessoDados.AcessoDados.GetOpcaoTabelaSalarial();

                for (int i = 0; i < opcaoTabelaSalarial.Rows.Count; i++)
                {
                    collecaoOpcoes.Add(new ListItem(opcaoTabelaSalarial.Rows[i]["NOMETABELA"].ToString(), opcaoTabelaSalarial.Rows[i]["VALUE"].ToString()));
                }
                return collecaoOpcoes;
            }
            catch (Exception e)
            {
                Logger.Write(string.Format("Erro ao criar coleção de opções de Tabela Salarial : {0}.", e.Message + e.StackTrace), EventLogEntryType.Error, 2, 2);
                throw;
            }
        }

        /// <summary>
        /// Busca e trata opções de Tabela Salarial vindas do Banco
        /// </summary>
        /// <param name="coligada">Coligada</param>
        /// <returns>Opções de Tabela Salarial</returns>
        public static ListItemCollection BuscaOpcoesTabelaSalarial(string coligada)
        {
            try
            {
                DataTable opcaoTabelaSalarial = new DataTable();
                ListItemCollection collecaoOpcoes = new ListItemCollection();
                opcaoTabelaSalarial = AcessoDados.AcessoDados.GetOpcaoTabelaSalarial(coligada);

                for (int i = 0; i < opcaoTabelaSalarial.Rows.Count; i++)
                {
                    collecaoOpcoes.Add(new ListItem(opcaoTabelaSalarial.Rows[i]["NOMETABELA"].ToString(), opcaoTabelaSalarial.Rows[i]["CODTABELA"].ToString()));
                }
                return collecaoOpcoes;
            }
            catch (Exception e)
            {
                Logger.Write(string.Format("Erro ao criar coleção de opções de Tabela Salarial : {0}.", e.Message + e.StackTrace), EventLogEntryType.Error, 2, 2);
                throw;
            }
        }

        /// <summary>
        /// Busca profile do Gestor
        /// </summary>
        /// <param name="usuario">Usuário logado no sistema</param>
        /// <returns>Profile com informações</returns>
        public static UserProfile BuscaProfile(string usuario)
        {
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.AllWebs["remuneracoes"])
                    {
                        //SPSite site = new SPSite(strUrl);
                        SPServiceContext serviceContext = SPServiceContext.GetContext(site);

                        // Inicializa o usuário gerenciador de perfis
                        UserProfileManager upm = new UserProfileManager(serviceContext);

                        //Busca um perfil de usuário
                        string sAccount = usuario;

                        if (upm.UserExists(sAccount))
                            return upm.GetUserProfile(sAccount);
                        else return null;
                    }
                }
            }
            catch (UserProfileException e)
            {
                Logger.Write("Erro ao buscar User Profile de Colaborador: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return null;
            }
        }

        public static DadosProfile BuscaDadosProfile(SPSite site, string login)
        {
            try
            {
                DadosProfile dadosProfile = new DadosProfile();

                //Instancia um contexto passando o site
                SPServiceContext ctx = SPServiceContext.GetContext(site);

                //Carrega o userprofile do site
                UserProfileManager upm = new UserProfileManager(ctx);

                //Verifica se usuário existe no UserProfile
                if (upm.UserExists(login))
                {
                    //Pega dados do profile
                    UserProfile profile = upm.GetUserProfile(login);

                    if (profile != null)
                    {
                        //Pega a coligada do colaborador
                        try
                        {
                            dadosProfile.Coligada = profile["Coligada"].Value.ToString();
                        }
                        catch
                        {
                            dadosProfile.Coligada = "99";
                        }

                        //Pega a matrícula do colaborador
                        try
                        {
                            dadosProfile.Matricula = profile["Matricula"].Value.ToString();
                        }
                        catch
                        {
                            dadosProfile.Matricula = "";
                        }

                        //Pega a Faixa Salarial do colaborador
                        try
                        {
                            dadosProfile.FaixaSalarial = Convert.ToInt32(profile["FaixaSalarial"].Value);
                        }
                        catch
                        {
                            dadosProfile.FaixaSalarial = 1;
                        }

                        //Pega a Classe do colaborador
                        try
                        {
                            dadosProfile.Classe = profile["Classe"].Value.ToString();
                        }
                        catch
                        {
                            dadosProfile.Classe = "A";
                        }
                    }
                    return dadosProfile;
                }
                else
                {
                    return null;
                }

            }
            catch (UserProfileException ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Cit.Globosat.Common.Utility.GetCurrentMethod(), Cit.Globosat.Common.Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
        }

        public static string BuscaProfilePicture(string usuario, UserProfileManager upm)
        {
            try
            {
                string urlPicture = string.Empty;
                UserProfile profile = null;

                //Busca um perfil de usuário
                if (upm.UserExists(usuario))
                {
                    profile = upm.GetUserProfile(usuario);
                    return profile[PropertyConstants.PictureUrl].Value as string;
                }
                else
                    return "";
            }
            catch (UserProfileException e)
            {
                Logger.Write("Erro ao buscar User Profile de Colaborador: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return "";
            }
        }

        /// <summary>
        /// Busca níveis da tabela para verificação e validação
        /// </summary>
        /// <param name="coligada">Coligada do Gestor</param>
        /// <param name="codigoTabela">Código da Tabela</param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable BuscaNiveisTabelaSalarial(string coligada, string codigoTabela)
        {
            return AcessoDados.AcessoDados.GetNivelTabelaSalarial(codigoTabela, coligada);
        }

        /// <summary>
        /// Busca classes da tabela para verificação e validação
        /// </summary>
        /// <param name="coligada">Coligada do Gestor</param>
        /// <param name="codigoTabela">Código da Tabela</param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable BuscaClassesTabelaSalarial(string coligada, string codigoTabela)
        {
            return AcessoDados.AcessoDados.GetClasseTabelaSalarial(codigoTabela, coligada);
        }

        /// <summary>
        /// Cria corpo de email com informações de evolução salarial de colaborador
        /// </summary>
        /// <param name="listaHistoricoSalarial">Informações que estarão no email.</param>
        /// <param name="tamanhoGrafico">Tamanho do gráfico a ser enviado no email.</param>
        /// <returns>string com o corpo do email.</returns>
        public static string EnviarConteudoEmail(List<Funcionario> listaHistoricoSalarial, string tamanhoGrafico, string matricula, string nome)
        {
            //Coluna de tabela em HTML já estilizada
            const string styleTable = "<td style=\" border:1px solid black; padding-left:4px\">";

            //inicio do email
            string corpoEmail = "<table style=\"width: 100%;\"><tr><td align=\"center\">";
            string graficoLegendaEixoY = string.Empty;
            string graficoCompleto = string.Empty;
            string graficoValoresEixoY = string.Empty;
            int count = 0;
            decimal maiorValorPercentual = 0;
            try
            {
                //HEADER GRÁFICO
                graficoCompleto += "http://chart.apis.google.com/chart?";
                //Inicio das informações
                graficoCompleto += "chxl=1:|";

                //Tabela com nome e matrícula do colaborador
                corpoEmail += "<table style=\"border:1px solid #8A9095\" width=\"70%\">" +
                          "<tr>" +
                              "<td align=\"center\" style=\"border-right:1px solid #8A9095;\">" +
                              "<h2 style=\"margin:0\">" + matricula + "</h2>" +
                              "</td>" +
                              "<td align=\"center\">" +
                               "<h2 style=\"margin:0\">" + nome + "</h2>" +
                              "</td>" +
                          "</tr>" +
                      "</table>" +
                  "</td>" +
              "</tr>";

                //Criação de Tabela
                corpoEmail += "<tr><td><table style=\"width: 100%; font-family: Verdana, Arial; font-size:12px; border: 1px solid; border-collapse: collapse; border-spacing: 0px;\">" +
                    "<tr style=\"color: #fff; background-color: #666; text-align:center;\">" +
                    //styleTable + "Matrícula</td>" +
                    //styleTable + "Nome</td>" +
                        styleTable + "Data</td>" +
                        styleTable + "Salário</td>" +
                        styleTable + "%</td>" +
                        styleTable + "Motivo</td>" +
                        styleTable + "Função</td>" +
                        styleTable + "Classe</td>" +
                        styleTable + "Nível</td>" +
                    "</tr>\n";


                foreach (Funcionario func in listaHistoricoSalarial)
                {
                    corpoEmail += "<tr style=\"color: #000; \">" +
                        //styleTable + func.Matricula + "</td>" +
                        //styleTable + func.Nome + "</td>" +
                        styleTable + func.Data + "</td>" +
                        styleTable + func.Salario + "</td>" +
                        styleTable + func.Percentual + "</td>" +
                        styleTable + func.Motivo + "</td>" +
                        styleTable + func.Funcao + "</td>" +
                        styleTable + func.Classe + "</td>" +
                        styleTable + func.Nivel + "</td>" +
                      "</tr>\n";

                    //Valores do eixo X
                    graficoCompleto += func.Data + "|";
                    //Valores do eixo Y
                    graficoValoresEixoY += func.PercentualNumber.ToString().Replace(',', '.') + ",";
                    //Valores da legenda do eixo Y
                    graficoLegendaEixoY += "t" + func.Percentual.Replace(',', '.') + ",000000,0," + count + ",10|";
                    count++;

                    if (func.PercentualNumber > maiorValorPercentual)
                    {
                        maiorValorPercentual = func.PercentualNumber;
                    }
                }
                corpoEmail += "</table><br/><br/></td></tr><tr><td align=\"center\">";

                //Remove último caracter das strings
                graficoValoresEixoY = graficoValoresEixoY.Remove(graficoValoresEixoY.Length - 1);
                graficoCompleto = graficoCompleto.Remove(graficoCompleto.Length - 1);
                graficoLegendaEixoY = graficoLegendaEixoY.Remove(graficoLegendaEixoY.Length - 1);

                //Config do eixo X
                graficoCompleto += "&chxs=1,676767,8,0,l,676767";

                //Legenda do eixo Y
                graficoCompleto += "&chxr=0,0," + (maiorValorPercentual + 10).ToString() + "";

                //Valor maximo da coluna
                graficoCompleto += "&chds=0," + (maiorValorPercentual + 10).ToString() + "";

                //Eixos que existem
                graficoCompleto += "&chxt=y,x";

                //Nao sei o que é isso ainda
                graficoCompleto += "&chbh=a";

                //Tamanho do Gráfico
                graficoCompleto += "&chs=" + tamanhoGrafico;

                //Tipo do Gráfico
                graficoCompleto += "&cht=bvs";

                //Cor das colunas
                graficoCompleto += "&chco=3072F3";

                //Valores do eixo Y
                graficoCompleto += "&chd=t:" + graficoValoresEixoY;

                //Legenda das colunas
                graficoCompleto += "&chm=" + graficoLegendaEixoY;

                //Titulo do Gráfico
                graficoCompleto += "&chtt=Evolução+Salarial";

                //Config do eixo Y
                graficoCompleto += "&chts=676767,10";

                graficoCompleto = "<img src=\"" + graficoCompleto + "\"/>";

                corpoEmail = corpoEmail + graficoCompleto + "</td></tr></table>";

                return corpoEmail;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao criar email com informações de evolução salarial: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return string.Empty;
            }
        }

        /// <summary>
        /// Busca itens na lista
        /// </summary>
        /// <param name="nomeLista"> Nome da Lista a ser acessada</param>
        /// <returns>Itens da lista</returns>
        public static SPListItemCollection BuscaListItens(string nomeLista)
        {
            return SPContext.Current.Web.Lists[nomeLista].Items;
        }

        /// <summary>
        /// Busca todos os centros de custo encontrados no banco de dados
        /// </summary>
        /// <returns>DataTable com os dados</returns>
        public static DataTable BuscaTodosCentrosCusto()
        {
            return AcessoDados.AcessoDados.GetAllCentrosCusto();
        }

        public static DataTable BuscaTodosCentrosCustoAtivos()
        {
            return AcessoDados.AcessoDados.GetAllCentrosCustoAtivos();
        }

        public static DataTable BuscaTodosCentrosCustoAtivosD()
        {
            return AcessoDados.AcessoDados.GetAllCentrosCustoAtivosD();
        }

        /// <summary>
        /// Busca todos os Centros de Custo exceto os de coligada 5 (G2C).
        /// </summary>
        /// <returns></returns>
        public static DataTable BuscaTodosCentrosCustoParaRV()
        {
            return AcessoDados.AcessoDados.GetAllCentrosCustoParaRV();
        }

        /// <summary>
        /// Busca dados de estagiário na Lista do Sharepoint
        /// </summary>
        /// <param name="filial">Filial da empresa</param>
        /// <param name="web">Contexto da web</param>
        /// <returns>Coleção de itens</returns>
        public static SPListItemCollection BuscaDadosEstag(string filial, SPWeb web)
        {
            try
            {
                SPQuery query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name=\"Filial\" /><Value Type=\"Text\">" + filial + "</Value></Eq></Where>";

                SPListItemCollection listaEstag = web.Lists["TSEstag"].GetItems(query);

                return listaEstag;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao buscar dados em lista TSEstag: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return null;
            }

        }

        /// <summary>
        /// Busca títula da tabela salarial em questão (Estagiário)
        /// </summary>
        /// <param name="filial">Filial da empresa</param>
        /// <param name="web">Contexto da web</param>
        /// <returns></returns>
        public static string BuscarTituloTabelaEstag(string filial, SPWeb web)
        {
            SPQuery query = new SPQuery();
            query.Query = "<Where><And><Eq><FieldRef Name=\"Filial\" /><Value Type=\"Text\">" + filial + "</Value></Eq><IsNotNull><FieldRef Name=\"Title\" /></IsNotNull></And></Where>";
            SPListItemCollection subTitulo = web.Lists["TSEstag"].GetItems(query);
            if (subTitulo.Count == 1)
                return subTitulo[0][SPBuiltInFieldId.Title].ToString();

            return string.Empty;
        }

        /// <summary>
        /// Busca nome de colaborador
        /// </summary>
        /// <param name="coligada">Coligada do colaborador</param>
        /// <param name="matricula">Matrícula do colaborador</param>
        /// <returns></returns>
        public static string BuscaNomeColaborador(string coligada, string matricula)
        {
            try
            {
                DataRow nomeColaborador = AcessoDados.AcessoDados.GetNomeColaborador(coligada, matricula);

                if (nomeColaborador != null)
                {
                    return nomeColaborador["NOME_FUNC"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao extrair nome do colaborador da linha da tabela: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return "";
            }
        }

        public static DataTable BuscaColaboradoresFolhaPagamento(string centroCusto, string coligada, string matricula)
        {
            return AcessoDados.AcessoDados.GetFolhaPagamento(centroCusto, coligada, matricula);
        }

        public static DataTable BuscaColaboradoresFolhaPagamento(string centroCusto, string ano)
        {
            return AcessoDados.AcessoDados.GetFolhaPagamento(centroCusto, ano);
        }

        public static DataTable BuscaColaboradoresFolhaPagamento(string centroCusto)
        {
            return AcessoDados.AcessoDados.GetFolhaPagamento(centroCusto);
        }

        #region Dados de Remuneração Variável no Ano
        public static DataTable BuscaColaboradoresRemuneracaoVariavel(string centroCusto, string coligada, int ano)
        {
            return AcessoDados.AcessoDados.GetRemuneracaoVariavelAno(centroCusto, coligada, ano);
        }

        public static DataTable BuscaColaboradoresRemuneracaoVariavel(string centroCusto, string coligada)
        {
            return AcessoDados.AcessoDados.GetRemuneracaoVariavelAno(centroCusto, coligada);
        }

        public static DataTable BuscaColaboradoresRemuneracaoVariavel(string centroCusto)
        {
            return AcessoDados.AcessoDados.GetRemuneracaoVariavelAno(centroCusto);
        }
        #endregion

        public static DadosProfile BuscaDadosColaborador(string matricula, string coligada, UserProfileManager upm, SqlConnection conn)
        {
            string login = string.Empty;
            UserProfile profile = null;
            DadosProfile infoProfile = null;
            try
            {
                infoProfile = new DadosProfile();
                DataTable loginColaborador = AcessoDados.AcessoDados.GetLoginColaborador(matricula, coligada, conn);

                if (loginColaborador != null)
                {
                    if (loginColaborador.Rows.Count == 1)
                    {
                        login = loginColaborador.Rows[0]["LOGIN"].ToString();

                        //Busca a foto do colaborador
                        if (upm.UserExists(login))
                        {
                            profile = upm.GetUserProfile(login, false);
                            infoProfile.Foto = profile[PropertyConstants.PictureUrl].Value.ToString();

                            if (profile["Classe"].Value != null)
                            {
                                infoProfile.Classe = profile["Classe"].Value.ToString();
                            }

                            if (profile["FaixaSalarial"].Value != null)
                            {
                                infoProfile.Nivel = profile["FaixaSalarial"].Value.ToString();
                            }

                            if (profile["tempBirthday"].Value != null)
                            {
                                infoProfile.DtNascimento = profile["tempBirthday"].Value.ToString();
                            }
                            else
                            {
                                infoProfile.DtNascimento = string.Empty;
                            }

                            return infoProfile;
                        }
                    }
                }

                infoProfile.Foto = SPContext.Current.Site.Url + "/_layouts/images/O14_person_placeHolder_96.png";
                infoProfile.Classe = "";
                infoProfile.Nivel = "";

                return infoProfile;
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao buscar foto de colaborador: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 2);
                return null;
            }
        }

        public int GetFaixaSalarial(string login)
        {
            int faixaSalarial = 0;
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb("Remuneracoes"))
                    {
                        SPUserToken sysToken = site.SystemAccount.UserToken;
                        using (SPSite siteAdmin = new SPSite(site.ID, sysToken))
                        {
                            using (SPWeb webAdmin = siteAdmin.OpenWeb(web.ID))
                            {
                                //Instancia contexto para busca em profile
                                SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                                // Inicializa o usuário gerenciador de perfis
                                UserProfileManager upm = new UserProfileManager(serviceContext);
                                UserProfile profile = null;
                                profile = upm.GetUserProfile(login);

                                faixaSalarial = profile["FaixaSalarial"] != null ? Convert.ToInt32(profile["FaixaSalarial"]) : 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao gerar objeto simulador de remunerações: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);
            }
            return faixaSalarial;
        }

        public Simulador BuscaSimulador(string matricula, string coligada, string strEbitda)
        {
            Simulador simulador = new Simulador();
            try
            {
                DataTable dados = AcessoDados.AcessoDados.GetDadosSimulador(matricula, coligada);
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb("Remuneracoes"))
                    {
                        #region Elevando permissões
                        SPUserToken sysToken = site.SystemAccount.UserToken;
                        using (SPSite siteAdmin = new SPSite(site.ID, sysToken))
                        {
                            using (SPWeb webAdmin = siteAdmin.OpenWeb(web.ID))
                            {
                                SPList oList = webAdmin.Lists["Beneficios"];
                                SPQuery query = new SPQuery();
                                query.Query = string.Format(@"<Where>
                                                          <And>
                                                             <Eq>
                                                                <FieldRef Name='Matricula' />
                                                                <Value Type='Text'>{0}</Value>
                                                             </Eq>
                                                             <Eq>
                                                                <FieldRef Name='Coligada' />
                                                                <Value Type='Text'>{1}</Value>
                                                             </Eq>
                                                          </And>
                                                       </Where>", matricula, coligada);
                                SPListItemCollection itemCollection = oList.GetItems(query);

                                if (itemCollection.Count > 0)
                                {
                                    simulador.PlanoOdontologico = itemCollection[0]["PlanoOdontologico"] != null ? Convert.ToDecimal(itemCollection[0]["PlanoOdontologico"]) : 0;
                                    simulador.PlanoSaude = itemCollection[0]["PlanoSaude"] != null ? Convert.ToDecimal(itemCollection[0]["PlanoSaude"]) : 0;
                                }
                                else
                                {
                                    simulador.PlanoOdontologico = 0;
                                    simulador.PlanoSaude = 0;
                                }

                                //Monta objeto Simulador
                                foreach (DataRow row in dados.Rows)
                                {

                                    if (row["TIPO"].ToString().Trim().ToUpper().Equals("SALARIOBASE"))//Salário Base
                                        simulador.SalarioBase = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("FERIAS13MENSAL"))//Férias + 13° Mensal
                                        simulador.Ferias31Mensal = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("FERIAS13ANUAL"))//Férias + 13° Anual
                                        simulador.Ferias31Anual = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("PENSEPREV"))//Penseprev
                                        simulador.PensePrev = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("DEPENDENTES"))//Quantidade de Dependentes
                                        simulador.Dependentes = row["VALOR"] != null ? Convert.ToInt32(row["VALOR"]) : 1;
                                }

                                #region Busca dados user profile
                                //Instancia contexto para busca em profile
                                SPServiceContext serviceContext = SPServiceContext.GetContext(SPContext.Current.Site);
                                // Inicializa o usuário gerenciador de perfis
                                UserProfileManager upm = new UserProfileManager(serviceContext);
                                UserProfile profile = null;
                                profile = upm.GetUserProfile(SPContext.Current.Web.CurrentUser.LoginName);
                                #region Get ParticipeVariavel e Bonus da lista "Remuneracao Variavel"
                                SPList listaRemuneracaoVariavel = webAdmin.Lists["Remuneracao Variavel"];
                                SPQuery queryR = new SPQuery();
                                queryR.Query = "<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + profile["FaixaSalarial"].ToString().Trim() + "</Value></Eq></Where>";
                                SPListItemCollection ItensRemuneracaoVariavel = listaRemuneracaoVariavel.GetItems(queryR);

                                if (ItensRemuneracaoVariavel.Count > 0)
                                {
                                    decimal total = 0;
                                    if (strEbitda.Equals("100"))
                                    {
                                        total = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Total_100"]);
                                        simulador.RemuneracaoVariavel = (simulador.SalarioBase * total) / 12;
                                    }
                                    else
                                    {
                                        total = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Total_130"]);
                                        simulador.RemuneracaoVariavel = (simulador.SalarioBase * total) / 12;
                                    }
                                }
                                else
                                    simulador.RemuneracaoVariavel = simulador.SalarioBase / 12;

                                #endregion
                                //Busca a foto do colaborador
                                if (upm.UserExists(SPContext.Current.Web.CurrentUser.LoginName))
                                {

                                    if (profile["FaixaSalarial"] != null && Convert.ToInt32(profile["FaixaSalarial"].ToString().Trim()) > 16)
                                    {
                                        if (ItensRemuneracaoVariavel.Count > 0)
                                        {
                                            decimal participeBonus = 0;
                                            if (strEbitda.Equals("100"))
                                            {
                                                participeBonus = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Total_100"]);
                                                simulador.RemuneracaoVariavel = (simulador.SalarioBase * participeBonus) / 12;
                                            }
                                            else
                                            {
                                                participeBonus = Convert.ToDecimal(ItensRemuneracaoVariavel[0]["Total_130"]);
                                                simulador.RemuneracaoVariavel = (simulador.SalarioBase * participeBonus) / 12;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        //Multiplica benefícios pelo número de dependentes
                        if (simulador.Dependentes > 0)
                        {
                            simulador.PlanoSaude = simulador.PlanoSaude * simulador.Dependentes;
                            simulador.PlanoOdontologico = simulador.PlanoOdontologico * simulador.Dependentes;
                        }
                        simulador.RemuneracaoFixa = simulador.SalarioBase + simulador.Ferias31Mensal;
                        simulador.RemuneracaoDireta = simulador.RemuneracaoFixa + simulador.RemuneracaoVariavel;
                        simulador.RemuneracaoTotal = simulador.RemuneracaoDireta + simulador.PlanoSaude + simulador.PlanoOdontologico + simulador.PensePrev;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao gerar objeto simulador de remunerações: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);

            }
            return simulador;
        }

        /// <summary>
        /// Método que monta tabela do Simulador
        /// </summary>
        /// <param name="matricula"></param>
        /// <param name="coligada"></param>
        /// <returns></returns>
        public StringBuilder CriaTabelaSimulador(string matricula, string coligada, decimal remuneracaoVariavel, decimal planoSaude, decimal planoOdonto)
        {
            StringBuilder sbTabelaSimulador = new StringBuilder();
            Simulador simulador = new Simulador();
            DataTable dados = AcessoDados.AcessoDados.GetDadosSimulador(matricula, coligada);

            try
            {
                //Monta objeto Simulador
                foreach (DataRow row in dados.Rows)
                {
                    if (row["TIPO"].ToString().Trim().ToUpper().Equals("SALARIOBASE"))//Salário Base
                        simulador.SalarioBase = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("FERIAS13MENSAL"))//Férias + 13° Mensal
                        simulador.Ferias31Mensal = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("FERIAS13ANUAL"))//Férias + 13° Anual
                        simulador.Ferias31Anual = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("PENSEPREV"))//Penseprev
                        simulador.PensePrev = row["VALOR"] != null ? Decimal.Parse(row["VALOR"].ToString().Replace(".", ",")) : 0;
                    else if (row["TIPO"].ToString().Trim().ToUpper().Equals("DEPENDENTES"))//Quantidade de Dependentes
                        simulador.Dependentes = row["VALOR"] != null ? Convert.ToInt32(row["VALOR"].ToString()) : 0;

                }

                simulador.RemuneracaoVariavel = remuneracaoVariavel;
                simulador.PlanoSaude = planoSaude;
                simulador.PlanoOdontologico = planoOdonto;
                simulador.RemuneracaoFixa = simulador.SalarioBase + simulador.Ferias31Mensal;
                simulador.RemuneracaoDireta = simulador.RemuneracaoFixa + simulador.RemuneracaoVariavel;
                simulador.RemuneracaoTotal = simulador.RemuneracaoDireta + simulador.PlanoSaude + simulador.PlanoOdontologico + simulador.PensePrev;
                simulador.NomeFuncionario = SPContext.Current.Web.CurrentUser.Name;
                //Monta tabela HTML

                sbTabelaSimulador.Append(@"<style type='text/css'>
                        .style1 {
	                        background-color: #66CCFF;
	                        width:10px;
                        }
                        .style2 {
	                        background-color: #FFCC00;
	                        width:10px;
                        }
                        .style3 {
	                        background-color: #339966;
	                        width:10px;
                        }
                        .style4 {
	                        background-color: #000099;
	                        color:#FFCC00;
	                        height:60px;
	                        width:100px;
	                        text-align:center;  
                            font-weight:bolder;
	                        font-size:larger;
                            font-weight:900;

		                }
                        .style5 {
	                        background:#E1E1E1;
	                        color:black;
	                        height:60px;
	                        width:100px; 
                            text-align:center;
	                        font-weight:bolder;
	                        font-size:larger;

		                }
                        .style6{
                        width:100px;
                        text-align:center;
                        font-size:xx-large;
	                        
                        }
                         .style7 {
	                        background-color: #000099;
	                        color:#FFCC00;
	                        height:40px;
	                        width:100px;
	                        text-align:center;  
                            font-weight:bolder;
	                        font-size:larger;
                            font-weight:900;

		                }
                        </style>                        
                        <table cellpadding='1px' cellspacing='5px' width='100%'>
	                        <tr align='center' style='height:30px'>
		                        <td >&nbsp;</td>
		                        <td >&nbsp;</td>
		                        <td ></td>
		                        <td class='style4'><strong>Nome</strong></td>
		                        <td class='style5' colspan='2'><strong>" + simulador.NomeFuncionario + @"</strong></td>
	                        </tr>
	                        <tr style='height:30px'>
		                        <td >&nbsp;</td>
		                        <td >&nbsp;</td>
		                        <td >&nbsp;</td>
		                        <td>&nbsp;</td>
		                        <td class='style5'>Valores Mensais</strong></td>
		                        <td class='style5'>Valores Anuais</strong></td>
	                        </tr>
	                        <tr>
		                        <td rowspan='2' class='style1'>&nbsp;</td>
		                        <td rowspan='3' class='style2'>&nbsp;</td>
		                        <td rowspan='7' class='style3'>&nbsp;</td>
		                        <td class='style4'><strong>Salário Base</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.SalarioBase.ToString("N2") + @"</strong></td>
		                        <td class='style6'><strong>R$ " + (simulador.SalarioBase * 12).ToString("N2") + @"</strong></td>
	                        </tr>
	                        <tr>
		                        <td class='style4'><strong>Férias e 13º</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.Ferias31Mensal.ToString("N2") + @"</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.Ferias31Anual.ToString("N2") + @"</strong></td>
	                        </tr>
	                        <tr>
		                        <td rowspan='5'>&nbsp;</td>
		                        <td class='style4'><strong>Remuneração Variável</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.RemuneracaoVariavel.ToString("N2") + @"</strong></td>
		                        <td class='style6'><strong>R$ " + (simulador.RemuneracaoVariavel * 12).ToString("N2") + @"</strong></td>
	                        </tr>
	                        <tr>
		                        <td rowspan='4'>&nbsp;</td>
		                        <td class='style4'><strong>Plano de Saúde</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.PlanoSaude.ToString("N2") + @"</strong></td>
		                        <td class='style6'><strong>R$ " + (simulador.PlanoSaude * 12).ToString("N2") + @"</strong></td>
	                        </tr>
	                        <tr>
		                        <td class='style4'><strong>Plano Odontológico</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.PlanoOdontologico.ToString("N2") + @"</strong></td>
		                        <td class='style6'><strong>R$ " + (simulador.PlanoOdontologico * 12).ToString("N2") + @"</strong></td>
	                        </tr>
	                        <tr>
		                        <td class='style4'><strong>PensePrev</strong></td>
		                        <td class='style6'><strong>R$ " + simulador.PensePrev.ToString("N2") + @"</strong></td>
		                        <td class='style6'><strong>R$ " + (simulador.PensePrev * 12).ToString("N2") + @"</strong></td>
	                        </tr>
	                        <tr>
		                        <td class='style4'><strong>Total</strong></td>
		                        <td class='style5'><strong>R$ " + simulador.RemuneracaoTotal.ToString("N2") + @"</strong></td>
		                        <td class='style5'><strong>R$ " + (simulador.RemuneracaoTotal * 12).ToString("N2") + @"</strong></td>
	                        </tr>
                        </table>");



            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao gerar a tabela do simulador de remunerações: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 1);

            }
            return sbTabelaSimulador;
        }

        public static DataTable BuscaMatriculaColaboradoresRemuneracaoVariavel(string centroCusto, string coligada, string matricula)
        {
            return AcessoDados.AcessoDados.GetMatriculaRemuneracaoVariavelAno(centroCusto, coligada, matricula);
        }

        public static DataTable BuscaMatriculaColaboradoresRemuneracaoVariavel(string centroCusto, string coligada, string matricula, int ano)
        {
            return AcessoDados.AcessoDados.GetMatriculaRemuneracaoVariavelAno(centroCusto, coligada, matricula, ano);
        }

        public static DataTable BuscaMatriculaColaboradoresRemuneracaoVariavel(string centroCusto)
        {
            return AcessoDados.AcessoDados.GetMatriculaRemuneracaoVariavelAno(centroCusto);
        }

        public static DataTable BuscaMatriculaColaboradoresRemuneracaoVariavel(string centroCusto, int ano)
        {
            return AcessoDados.AcessoDados.GetMatriculaRemuneracaoVariavelAno(centroCusto, ano);
        }

        public static DataTable BuscaCargo(string codCentroCusto)
        {
            return AcessoDados.AcessoDados.GetCargo(codCentroCusto);
        }

        public static DataTable BuscaCargo(string codCentroCusto, string codCargo)
        {
            return AcessoDados.AcessoDados.GetCargo(codCentroCusto, codCargo);
        }

        public static DataTable BuscaCargo(string cargo, int codColigada, string nivel)
        {
            return AcessoDados.AcessoDados.GetCargo(cargo.ToUpper(), codColigada, nivel);
        }

        public static DataTable BuscaCargoRequisicaoPessoal(string cargo, int codColigada, string nivel, string tipo, int codFilial)
        {
            return AcessoDados.AcessoDados.GetCargoRequisicaoPessoal(cargo.ToUpper(), codColigada, nivel, tipo, codFilial);
        }

        public static int GetJornada(string cargo, string faixa, string nivel, int codFilial, int codColigada)
        {
            return AcessoDados.AcessoDados.GetJornada(cargo, faixa, nivel, codFilial, codColigada);
        }

        public static string BuscarSalarioColaborador(string matricula, string coligada, int? ano = null)
        {
            DataTable dtSalario = new DataTable();
            dtSalario = AcessoDados.AcessoDados.GetSalarioColaborador(matricula, coligada, ano);

            if ((dtSalario != null) && (dtSalario.Rows.Count > 0))
                return dtSalario.Rows[0]["SALARIO"].ToString();
            else
                return string.Empty;
        }

        public static string BuscarSalarioColaborador(string matricula, string coligada)
        {
            DataTable dtSalario = new DataTable();
            dtSalario = AcessoDados.AcessoDados.GetSalarioColaborador(matricula, coligada);

            if ((dtSalario != null) && (dtSalario.Rows.Count > 0))
                return dtSalario.Rows[0]["SALARIO"].ToString();
            else
                return string.Empty;
        }

        public static string BuscaNivelColaborador(string matricula, string coligada)
        {
            try
            {
                DataRow nivelColaborador = AcessoDados.AcessoDados.GetNivelColaborador(matricula, coligada);

                if (nivelColaborador != null)
                {
                    return nivelColaborador["NIVEL"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao extrair o nível do colaborador da linha da tabela1: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return "";
            }
        }

        /// <summary>
        /// Calcula a diferença entre o salário atual e o salário proposto.
        /// </summary>
        /// <param name="salarioProposto">Salário atual do colaborador</param>
        /// <param name="salarioAtual">Salário proposto para o colaborador</param>
        /// <returns>Cálculo da diferença em decimal</returns>
        public static decimal CalcularDiferencaSalario(string salarioProposto, string salarioAtual)
        {
            decimal salarioPropostoConvertido = 0;
            decimal salarioAtualConvertido = 0;
            try
            {
                if (salarioAtual.Contains("R$"))
                    salarioAtual = salarioAtual.Replace("R$", "");

                if (salarioProposto.Contains("R$"))
                    salarioProposto = salarioProposto.Replace("R$", "");

                salarioPropostoConvertido = Convert.ToDecimal(salarioProposto, CultureInfo.CreateSpecificCulture("pt-BR"));
                salarioAtualConvertido = Convert.ToDecimal(salarioAtual, CultureInfo.CreateSpecificCulture("pt-BR"));

                //return Math.Round(salarioPropostoConvertido - salarioAtualConvertido, 2);
                return salarioPropostoConvertido - salarioAtualConvertido;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao calcular diferença de salário: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 3);
                return 0;
            }
        }

        /// <summary>
        /// Calcula o percentual da diferença entre o salário atual e o salário proposto.
        /// </summary>
        /// <param name="salarioProposto">Salário atual do colaborador</param>
        /// <param name="salarioAtual">Salário proposto para o colaborador</param>
        /// <returns>Cálculo do percentual em decimal</returns>
        public static decimal CalcularPercentualDiferencaSalario(string salarioProposto, string salarioAtual)
        {
            decimal salarioPropostoConvertido = 0;
            decimal salarioAtualConvertido = 0;
            decimal total = 0;
            try
            {
                if (salarioAtual.Contains("R$"))
                    salarioAtual = salarioAtual.Replace("R$", "");

                if (salarioProposto.Contains("R$"))
                    salarioProposto = salarioProposto.Replace("R$", "");

                salarioPropostoConvertido = Convert.ToDecimal(salarioProposto, CultureInfo.CreateSpecificCulture("pt-BR"));
                salarioAtualConvertido = Convert.ToDecimal(salarioAtual, CultureInfo.CreateSpecificCulture("pt-BR"));

                total = (((salarioPropostoConvertido) / salarioAtualConvertido) - 1) * 100;

                return total;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao calcular percentual de diferença de salário " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 3);
                return 0;
            }
        }

        public static DataTable BuscarDadosFuncionarios(int codigoColigada, string matricula)
        {
            DataTable dtDadosFuncionario = null;
            DataTable dtFuncionario = null;
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    dtDadosFuncionario = AcessoDados.AcessoDados.GetDadosFuncionario(codigoColigada, matricula);
                }
                else
                {
                    #region DESENVOLVIMENTO
                    dtDadosFuncionario = new DataTable();
                    dtDadosFuncionario.Columns.Add("COLIGADA", Type.GetType("System.String"));
                    dtDadosFuncionario.Columns.Add("MATRICULA", Type.GetType("System.String"));
                    dtDadosFuncionario.Columns.Add("NOME", Type.GetType("System.String"));
                    dtDadosFuncionario.Columns.Add("FUNCAO", Type.GetType("System.String"));
                    dtDadosFuncionario.Columns.Add("SALARIO", Type.GetType("System.Decimal"));

                    DataRow drDadosFuncionario = dtDadosFuncionario.NewRow();
                    drDadosFuncionario["COLIGADA"] = "1";
                    drDadosFuncionario["MATRICULA"] = "01842";
                    drDadosFuncionario["NOME"] = "ALBERTO CARLOS PECEGUEIRO DO AMARAL";
                    drDadosFuncionario["FUNCAO"] = "DIRETORIA GERAL - STAFF";
                    drDadosFuncionario["SALARIO"] = 15252.00;
                    dtDadosFuncionario.Rows.Add(drDadosFuncionario);
                    #endregion
                }

                // Tratamento dados básicos do funcionário.
                if (dtDadosFuncionario.Rows.Count > 0)
                {
                    dtFuncionario = new DataTable();
                    dtFuncionario.Columns.Add("MATRICULA", Type.GetType("System.String"));
                    dtFuncionario.Columns.Add("NOME", Type.GetType("System.String"));
                    dtFuncionario.Columns.Add("FUNCAO", Type.GetType("System.String"));
                    dtFuncionario.Columns.Add("SALARIO", Type.GetType("System.Decimal"));

                    DataRow drFuncionario = dtFuncionario.NewRow();
                    drFuncionario["MATRICULA"] = dtDadosFuncionario.Rows[0]["MATRICULA"].ToString();
                    drFuncionario["NOME"] = dtDadosFuncionario.Rows[0]["NOME"].ToString();
                    drFuncionario["FUNCAO"] = dtDadosFuncionario.Rows[0]["FUNCAO"].ToString();
                    drFuncionario["SALARIO"] = decimal.Parse(dtDadosFuncionario.Rows[0]["SALARIO"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    dtFuncionario.Rows.Add(drFuncionario);

                    return dtFuncionario;
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao extrair o nível do colaborador da linha da tabela2: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return null;
            }
            finally
            {
                if (dtDadosFuncionario != null)
                    dtDadosFuncionario.Dispose();

                if (dtFuncionario != null)
                    dtFuncionario.Dispose();
            }
        }

        public static DataTable BuscarPremios(int codigoColigada, string matricula)
        {
            DataTable dtPremios = null;
            DataTable dtTodasPremiacoes = null;
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    dtPremios = AcessoDados.AcessoDados.GetPremios(codigoColigada, matricula);
                }
                else
                {
                    #region DESENVOLVIMENTO
                    dtPremios = new DataTable();
                    dtPremios.Columns.Add("ANO", Type.GetType("System.String"));
                    dtPremios.Columns.Add("MES", Type.GetType("System.String"));
                    dtPremios.Columns.Add("EVENTO", Type.GetType("System.String"));
                    dtPremios.Columns.Add("VALOR", Type.GetType("System.Decimal"));

                    DataRow drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2010";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 3389.10;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2010";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 11297.00;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2010";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 12188.00;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 12956.00;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 8798.90;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 3886.80;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 9069.20;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 3081.20;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 13604.00;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 14321.00;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 15782.00;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 10239.80;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 4575.60;
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 4500.60;
                    dtPremios.Rows.Add(drFuncPremios);
                    #endregion
                }

                if (dtPremios.Rows.Count > 0)
                {
                    #region FORMATA DADOS
                    // Tratamento p/ as premiações.
                    List<string> todosEventos = GetEventos();
                    List<Premio> premiosFromDataBase = new List<Premio>();
                    List<Premio> premios = new List<Premio>();

                    bool possuiEvento = false;
                    foreach (DataRow row in dtPremios.Rows)
                    {
                        // Caso entre mais algum evento na view do banco de dados.
                        foreach (string evento in todosEventos)
                        {
                            if (row["EVENTO"].ToString().Equals(evento))
                                possuiEvento = true;
                        }

                        if (possuiEvento)
                        {
                            possuiEvento = false;
                            Premio premio = new Premio()
                            {
                                Mes = Convert.ToInt32(row["MES"].ToString()),
                                Ano = Convert.ToInt32(row["ANO"].ToString()),
                                Evento = row["EVENTO"].ToString(),
                                Valor = decimal.Parse(row["VALOR"].ToString(), System.Globalization.CultureInfo.InvariantCulture)
                            };

                            premiosFromDataBase.Add(premio);
                        }
                    }

                    // Anos que possuem premiação.
                    var anos = (from p in premiosFromDataBase
                                orderby p.Ano ascending
                                select p.Ano).Distinct().ToList();

                    // Totais por mês.
                    decimal totalJan = 0;
                    decimal totalFev = 0;
                    decimal totalMar = 0;
                    decimal totalAbr = 0;
                    decimal totalMai = 0;
                    decimal totalJun = 0;
                    decimal totalJul = 0;
                    decimal totalAgo = 0;
                    decimal totalSet = 0;
                    decimal totalOut = 0;
                    decimal totalNov = 0;
                    decimal totalDez = 0;

                    // Totais por evento.
                    decimal totalSalarioMensal = 0;
                    decimal totalParticipe = 0;
                    decimal totalPremio = 0;
                    decimal totalPremioSemestral = 0;
                    decimal totalPremioTrimestral = 0;
                    decimal totalPremioAnual = 0;
                    decimal totalPremioExtra = 0;
                    decimal totalPremioDSR = 0;

                    int anoAnterior = anos.First();
                    Premio newPremio = null;
                    decimal? salarioValoresVazios = 0;
                    decimal valor = 0;

                    for (int i = 0; i <= anos.Count; i++)
                    {
                        if ((i == anos.Count) || (anoAnterior != anos[i]))
                        {
                            #region PreecherTotal

                            // Preencher totais p/ os itens do ano anterior.
                            var premiosAnoAnterior = (from p in premios
                                                      where p.Ano == anoAnterior
                                                      select p).ToList();

                            foreach (var premioAno in premiosAnoAnterior)
                            {
                                switch (premioAno.Mes)
                                {
                                    case 1:
                                        premioAno.TotalMes = totalJan;
                                        break;
                                    case 2:
                                        premioAno.TotalMes = totalFev;
                                        break;
                                    case 3:
                                        premioAno.TotalMes = totalMar;
                                        break;
                                    case 4:
                                        premioAno.TotalMes = totalAbr;
                                        break;
                                    case 5:
                                        premioAno.TotalMes = totalMai;
                                        break;
                                    case 6:
                                        premioAno.TotalMes = totalJun;
                                        break;
                                    case 7:
                                        premioAno.TotalMes = totalJul;
                                        break;
                                    case 8:
                                        premioAno.TotalMes = totalAgo;
                                        break;
                                    case 9:
                                        premioAno.TotalMes = totalSet;
                                        break;
                                    case 10:
                                        premioAno.TotalMes = totalOut;
                                        break;
                                    case 11:
                                        premioAno.TotalMes = totalNov;
                                        break;
                                    case 12:
                                        premioAno.TotalMes = totalDez;
                                        break;
                                    default:
                                        break;
                                }

                                switch (premioAno.Evento)
                                {
                                    case "SALARIO MENSAL":
                                        premioAno.TotalEvento = totalSalarioMensal;
                                        break;
                                    case "PARTICIPE":
                                        premioAno.TotalEvento = totalParticipe;
                                        break;
                                    case "PREMIO":
                                        premioAno.TotalEvento = totalPremio;
                                        break;
                                    case "PREMIO SEMESTRAL":
                                        premioAno.TotalEvento = totalPremioSemestral;
                                        break;
                                    case "PREMIO TRIMESTRAL":
                                        premioAno.TotalEvento = totalPremioTrimestral;
                                        break;
                                    case "PREMIO ANUAL":
                                        premioAno.TotalEvento = totalPremioAnual;
                                        break;
                                    case "PREMIO EXTRA":
                                        premioAno.TotalEvento = totalPremioExtra;
                                        break;
                                    case "DSR S/ PREMIOS":
                                        premioAno.TotalEvento = totalPremioDSR;
                                        break;
                                    default:
                                        break;
                                }

                                premioAno.Total = totalSalarioMensal + totalParticipe + totalPremio + totalPremioSemestral + totalPremioTrimestral + totalPremioAnual + totalPremioExtra + totalPremioDSR;
                            }

                            // Zerar os totais.
                            totalJan = 0;
                            totalFev = 0;
                            totalMar = 0;
                            totalAbr = 0;
                            totalMai = 0;
                            totalJun = 0;
                            totalJul = 0;
                            totalAgo = 0;
                            totalSet = 0;
                            totalOut = 0;
                            totalNov = 0;
                            totalDez = 0;
                            totalSalarioMensal = 0;
                            totalParticipe = 0;
                            totalPremio = 0;
                            totalPremioSemestral = 0;
                            totalPremioTrimestral = 0;
                            totalPremioAnual = 0;
                            totalPremioExtra = 0;
                            totalPremioDSR = 0;

                            // Setar próximo ano.
                            if (i < anos.Count)
                                anoAnterior = anos[i];

                            #endregion
                        }

                        if (i < anos.Count)
                        {
                            foreach (var evento in todosEventos)
                            {
                                // Tratamento p/ os meses que não existem valores.
                                for (int mes = 1; mes <= 12; mes++)
                                {
                                    Premio premioComMes = premiosFromDataBase.Where(p => p.Evento.Equals(evento) && p.Ano == anos[i] && p.Mes == mes).FirstOrDefault();
                                    if (premioComMes != null)
                                    {
                                        if (evento.Equals(todosEventos[0])) // Coluna salário mensal.
                                            salarioValoresVazios = premioComMes.Valor; // Armazena o último salário.
                                    }

                                    newPremio = new Premio();
                                    newPremio.Ano = anos[i];
                                    newPremio.Evento = evento;
                                    newPremio.Mes = mes;

                                    if (premioComMes == null)
                                    {
                                        // Este mês não existe para o último evento.
                                        if (evento.Equals(todosEventos[0]))
                                        {
                                            if (anos[i] == anos[anos.Count - 1])
                                            {
                                                if (mes < DateTime.Now.Month)
                                                {
                                                    newPremio.Valor = salarioValoresVazios.Value;
                                                }
                                                else
                                                {
                                                    newPremio.Valor = 0;
                                                }
                                            }
                                            else
                                            {
                                                newPremio.Valor = salarioValoresVazios.Value;
                                            }
                                        }
                                        else
                                        {
                                            newPremio.Valor = 0;
                                        }
                                    }
                                    else
                                    {
                                        newPremio.Valor = premioComMes.Valor;
                                    }

                                    valor = newPremio.Valor;
                                    premios.Add(newPremio);

                                    #region Total

                                    // Soma valor total por mês.
                                    switch (mes)
                                    {
                                        case 1:
                                            totalJan += valor;
                                            break;
                                        case 2:
                                            totalFev += valor;
                                            break;
                                        case 3:
                                            totalMar += valor;
                                            break;
                                        case 4:
                                            totalAbr += valor;
                                            break;
                                        case 5:
                                            totalMai += valor;
                                            break;
                                        case 6:
                                            totalJun += valor;
                                            break;
                                        case 7:
                                            totalJul += valor;
                                            break;
                                        case 8:
                                            totalAgo += valor;
                                            break;
                                        case 9:
                                            totalSet += valor;
                                            break;
                                        case 10:
                                            totalOut += valor;
                                            break;
                                        case 11:
                                            totalNov += valor;
                                            break;
                                        case 12:
                                            totalDez += valor;
                                            break;
                                        default:
                                            break;
                                    }

                                    // Soma valor total por evento.
                                    switch (evento)
                                    {
                                        case "SALARIO MENSAL":
                                            totalSalarioMensal += valor;
                                            break;
                                        case "PARTICIPE":
                                            totalParticipe += valor;
                                            break;
                                        case "PREMIO":
                                            totalPremio += valor;
                                            break;
                                        case "PREMIO SEMESTRAL":
                                            totalPremioSemestral += valor;
                                            break;
                                        case "PREMIO TRIMESTRAL":
                                            totalPremioTrimestral += valor;
                                            break;
                                        case "PREMIO ANUAL":
                                            totalPremioAnual += valor;
                                            break;
                                        case "PREMIO EXTRA":
                                            totalPremioExtra += valor;
                                            break;
                                        case "DSR S/ PREMIOS":
                                            totalPremioDSR += valor;
                                            break;
                                        default:
                                            break;
                                    }

                                    #endregion
                                }

                                // Tratamento para eventos que não existem valores.
                                Premio premioComEvento = premiosFromDataBase.Where(p => p.Ano == anos[i] && p.Evento.Contains(evento)).FirstOrDefault();
                                if (premioComEvento == null)
                                {
                                    // Este evento não existe p/ este ano. Preencher todos os meses p/ este evento.
                                    for (int j = 1; j <= 12; j++)
                                    {
                                        premios.Add(newPremio = new Premio()
                                        {
                                            Ano = anos[i],
                                            Mes = j,
                                            Evento = evento,
                                            Valor = 0
                                        });
                                    }
                                }
                            }
                        }
                    }

                    dtTodasPremiacoes = new DataTable();
                    dtTodasPremiacoes.Columns.Add("ANO", Type.GetType("System.Int32"));
                    dtTodasPremiacoes.Columns.Add("MES", Type.GetType("System.Int32"));
                    dtTodasPremiacoes.Columns.Add("EVENTO", Type.GetType("System.String"));
                    dtTodasPremiacoes.Columns.Add("VALOR", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("TOTAL_MES", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("TOTAL_EVENTO", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("TOTAL", Type.GetType("System.Decimal"));

                    DataRow drNewPremiacao = null;
                    foreach (var item in premios)
                    {
                        drNewPremiacao = null;
                        drNewPremiacao = dtTodasPremiacoes.NewRow();
                        drNewPremiacao["ANO"] = item.Ano;
                        drNewPremiacao["MES"] = item.Mes;
                        drNewPremiacao["EVENTO"] = item.Evento;

                        if ((item.Ano == DateTime.Now.Year) && (item.Mes >= DateTime.Now.Month))
                        {
                            if (item.Valor == 0)
                            {
                                drNewPremiacao["VALOR"] = DBNull.Value;
                            }
                            else
                            {
                                drNewPremiacao["VALOR"] = item.Valor;
                            }

                            if (item.TotalMes == 0)
                            {
                                drNewPremiacao["TOTAL_MES"] = DBNull.Value;
                            }
                            else
                            {
                                drNewPremiacao["TOTAL_MES"] = item.TotalMes;
                            }
                        }
                        else
                        {
                            drNewPremiacao["VALOR"] = item.Valor;
                            drNewPremiacao["TOTAL_MES"] = item.TotalMes;
                        }

                        drNewPremiacao["TOTAL_EVENTO"] = item.TotalEvento;
                        drNewPremiacao["TOTAL"] = item.Total;
                        dtTodasPremiacoes.Rows.Add(drNewPremiacao);
                    }

                    #endregion
                }

                return dtTodasPremiacoes;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao extrair o nível do colaborador da linha da tabela3: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return null;
            }
            finally
            {
                if (dtPremios != null)
                    dtPremios.Dispose();

                if (dtTodasPremiacoes != null)
                    dtTodasPremiacoes.Dispose();
            }
        }

        public static DataTable BuscarPremios(string centroCusto, string idFuncionarios)
        {
            DataTable dtPremios = null;
            DataTable dtTodasPremiacoes = null;
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    dtPremios = AcessoDados.AcessoDados.GetPremios(centroCusto, idFuncionarios);
                }
                else
                {
                    #region DESENVOLVIMENTO
                    dtPremios = new DataTable();
                    dtPremios.Columns.Add("ANO", Type.GetType("System.String"));
                    dtPremios.Columns.Add("MES", Type.GetType("System.String"));
                    dtPremios.Columns.Add("EVENTO", Type.GetType("System.String"));
                    dtPremios.Columns.Add("VALOR", Type.GetType("System.Decimal"));
                    dtPremios.Columns.Add("NOME", Type.GetType("System.String"));
                    dtPremios.Columns.Add("MATRICULA", Type.GetType("System.String"));

                    DataRow drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2010";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 3389.10;
                    drFuncPremios["NOME"] = "GUSTAVO LERNER";
                    drFuncPremios["MATRICULA"] = "90263";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2010";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 11297.00;
                    drFuncPremios["NOME"] = "GUSTAVO LERNER";
                    drFuncPremios["MATRICULA"] = "90263";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2010";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 12188.00;
                    drFuncPremios["NOME"] = "GUSTAVO LERNER";
                    drFuncPremios["MATRICULA"] = "90263";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 12956.00;
                    drFuncPremios["NOME"] = "GUSTAVO LERNER";
                    drFuncPremios["MATRICULA"] = "90263";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 8798.90;
                    drFuncPremios["NOME"] = "PAULO CESAR SOARES ITABAIANA";
                    drFuncPremios["MATRICULA"] = "90264";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "SALARIO MENSAL";
                    drFuncPremios["VALOR"] = 3886.80;
                    drFuncPremios["NOME"] = "PAULO CESAR SOARES ITABAIANA";
                    drFuncPremios["MATRICULA"] = "90264";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 9069.20;
                    drFuncPremios["NOME"] = "MURILO CESAR ROSA JUNIOR";
                    drFuncPremios["MATRICULA"] = "90265";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 3081.20;
                    drFuncPremios["NOME"] = "MURILO CESAR ROSA JUNIOR";
                    drFuncPremios["MATRICULA"] = "90265";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "PARTICIPE";
                    drFuncPremios["VALOR"] = 13604.00;
                    drFuncPremios["NOME"] = "MURILO CESAR ROSA JUNIOR";
                    drFuncPremios["MATRICULA"] = "90265";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 14321.00;
                    drFuncPremios["NOME"] = "MARCOS BRANDAO PEREZ MENDES";
                    drFuncPremios["MATRICULA"] = "90266";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "5";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 15782.00;
                    drFuncPremios["NOME"] = "MARCOS BRANDAO PEREZ MENDES";
                    drFuncPremios["MATRICULA"] = "90266";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 10239.80;
                    drFuncPremios["NOME"] = "MARCOS BRANDAO PEREZ MENDES";
                    drFuncPremios["MATRICULA"] = "90266";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2011";
                    drFuncPremios["MES"] = "7";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 4575.60;
                    drFuncPremios["NOME"] = "GABRIEL DE ARAUJO KRUSCHEWSKY DORIA";
                    drFuncPremios["MATRICULA"] = "90267";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2012";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 4500.60;
                    drFuncPremios["NOME"] = "GABRIEL DE ARAUJO KRUSCHEWSKY DORIA";
                    drFuncPremios["MATRICULA"] = "90267";
                    dtPremios.Rows.Add(drFuncPremios);

                    drFuncPremios = null;
                    drFuncPremios = dtPremios.NewRow();
                    drFuncPremios["ANO"] = "2013";
                    drFuncPremios["MES"] = "1";
                    drFuncPremios["EVENTO"] = "PREMIO";
                    drFuncPremios["VALOR"] = 4500.60;
                    drFuncPremios["NOME"] = "GABRIEL DE ARAUJO KRUSCHEWSKY DORIA";
                    drFuncPremios["MATRICULA"] = "90267";
                    dtPremios.Rows.Add(drFuncPremios);

                    #endregion
                }

                if (dtPremios.Rows.Count > 0)
                {
                    #region FORMATA DADOS
                    // Tratamento p/ as premiações.
                    List<string> todosEventos = GetEventos();
                    List<Premio> premiosFromDataBase = new List<Premio>();
                    List<Premio> premios = new List<Premio>();

                    bool possuiEvento = false;

                    DataRow[] result = dtPremios.Select("NOME is not null", "NOME ASC");

                    foreach (DataRow row in result)
                    {
                        // Caso entre mais algum evento na view do banco de dados.
                        foreach (string evento in todosEventos)
                        {
                            if (row["EVENTO"].ToString().Equals(evento))
                                possuiEvento = true;
                        }

                        if (possuiEvento)
                        {
                            possuiEvento = false;
                            Premio premio = new Premio()
                            {
                                Mes = Convert.ToInt32(row["MES"].ToString()),
                                Ano = Convert.ToInt32(row["ANO"].ToString()),
                                Evento = row["EVENTO"].ToString(),
                                Valor = decimal.Parse(row["VALOR"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                                Matricula = row["MATRICULA"].ToString(),
                                Nome = row["NOME"].ToString()
                            };

                            premiosFromDataBase.Add(premio);
                        }
                    }

                    // Anos que possuem premiação.
                    var anos = (from p in premiosFromDataBase
                                orderby p.Ano ascending
                                select p.Ano).Distinct().ToList();

                    // Totais por mês.
                    decimal totalJan;
                    decimal totalFev;
                    decimal totalMar;
                    decimal totalAbr;
                    decimal totalMai;
                    decimal totalJun;
                    decimal totalJul;
                    decimal totalAgo;
                    decimal totalSet;
                    decimal totalOut;
                    decimal totalNov;
                    decimal totalDez;

                    // Totais por evento.
                    decimal totalSalarioMensal;
                    decimal totalParticipe;
                    decimal totalPremio;
                    decimal totalPremioSemestral;
                    decimal totalPremioTrimestral;
                    decimal totalPremioAnual;
                    decimal totalPremioExtra;
                    decimal totalPremioDSR;

                    int anoAnterior;
                    decimal? salarioValoresVazios;
                    decimal valor;

                    // Distinct de todos os funcionários.
                    List<Cit.Globosat.Common.Tuple<string, string, string>> funcionarios = (from p in premiosFromDataBase
                                                                                            select new Cit.Globosat.Common.Tuple<string, string, string>(p.Matricula, p.Nome, string.Empty)).ToList()
                                        .DistinctBy(func => func.Item1).ToList();

                    foreach (var funcionario in funcionarios)
                    {
                        anoAnterior = anos.First();
                        salarioValoresVazios = 0;
                        valor = 0;

                        // Zerar os totais.
                        totalJan = 0;
                        totalFev = 0;
                        totalMar = 0;
                        totalAbr = 0;
                        totalMai = 0;
                        totalJun = 0;
                        totalJul = 0;
                        totalAgo = 0;
                        totalSet = 0;
                        totalOut = 0;
                        totalNov = 0;
                        totalDez = 0;
                        totalSalarioMensal = 0;
                        totalParticipe = 0;
                        totalPremio = 0;
                        totalPremioSemestral = 0;
                        totalPremioTrimestral = 0;
                        totalPremioAnual = 0;
                        totalPremioExtra = 0;
                        totalPremioDSR = 0;

                        for (int i = 0; i <= anos.Count; i++)
                        {
                            if ((i == anos.Count) || (anoAnterior != anos[i]))
                            {
                                #region PreecherTotal

                                // Preencher totais p/ os itens do ano anterior.
                                var premiosAnoAnterior = (from p in premios
                                                          where p.Ano == anoAnterior
                                                            && p.Matricula.Equals(funcionario.Item1)
                                                          select p).ToList();

                                foreach (var premioAno in premiosAnoAnterior)
                                {
                                    switch (premioAno.Mes)
                                    {
                                        case 1:
                                            premioAno.TotalMes = totalJan;
                                            break;
                                        case 2:
                                            premioAno.TotalMes = totalFev;
                                            break;
                                        case 3:
                                            premioAno.TotalMes = totalMar;
                                            break;
                                        case 4:
                                            premioAno.TotalMes = totalAbr;
                                            break;
                                        case 5:
                                            premioAno.TotalMes = totalMai;
                                            break;
                                        case 6:
                                            premioAno.TotalMes = totalJun;
                                            break;
                                        case 7:
                                            premioAno.TotalMes = totalJul;
                                            break;
                                        case 8:
                                            premioAno.TotalMes = totalAgo;
                                            break;
                                        case 9:
                                            premioAno.TotalMes = totalSet;
                                            break;
                                        case 10:
                                            premioAno.TotalMes = totalOut;
                                            break;
                                        case 11:
                                            premioAno.TotalMes = totalNov;
                                            break;
                                        case 12:
                                            premioAno.TotalMes = totalDez;
                                            break;
                                        default:
                                            break;
                                    }

                                    switch (premioAno.Evento)
                                    {
                                        case "SALARIO MENSAL":
                                            premioAno.TotalEvento = totalSalarioMensal;
                                            break;
                                        case "PARTICIPE":
                                            premioAno.TotalEvento = totalParticipe;
                                            break;
                                        case "PREMIO":
                                            premioAno.TotalEvento = totalPremio;
                                            break;
                                        case "PREMIO SEMESTRAL":
                                            premioAno.TotalEvento = totalPremioSemestral;
                                            break;
                                        case "PREMIO TRIMESTRAL":
                                            premioAno.TotalEvento = totalPremioTrimestral;
                                            break;
                                        case "PREMIO ANUAL":
                                            premioAno.TotalEvento = totalPremioAnual;
                                            break;
                                        case "PREMIO EXTRA":
                                            premioAno.TotalEvento = totalPremioExtra;
                                            break;
                                        case "DSR S/ PREMIOS":
                                            premioAno.TotalEvento = totalPremioDSR;
                                            break;
                                        default:
                                            break;
                                    }

                                    premioAno.Total = totalSalarioMensal + totalParticipe + totalPremio + totalPremioSemestral + totalPremioTrimestral + totalPremioAnual + totalPremioExtra + totalPremioDSR;
                                }

                                // Zerar os totais.
                                totalJan = 0;
                                totalFev = 0;
                                totalMar = 0;
                                totalAbr = 0;
                                totalMai = 0;
                                totalJun = 0;
                                totalJul = 0;
                                totalAgo = 0;
                                totalSet = 0;
                                totalOut = 0;
                                totalNov = 0;
                                totalDez = 0;
                                totalSalarioMensal = 0;
                                totalParticipe = 0;
                                totalPremio = 0;
                                totalPremioSemestral = 0;
                                totalPremioTrimestral = 0;
                                totalPremioAnual = 0;
                                totalPremioExtra = 0;
                                totalPremioDSR = 0;

                                // Setar próximo ano.
                                if (i < anos.Count)
                                    anoAnterior = anos[i];

                                #endregion
                            }

                            if (i < anos.Count)
                            {
                                foreach (var evento in todosEventos)
                                {
                                    // Tratamento p/ os meses que não existem valores.
                                    for (int mes = 1; mes <= 12; mes++)
                                    {
                                        Premio premioComMes = premiosFromDataBase.Where(p => p.Evento.Equals(evento) && p.Ano == anos[i] && p.Mes == mes && p.Matricula.Equals(funcionario.Item1)).FirstOrDefault();
                                        if (premioComMes != null)
                                        {
                                            if (evento.Equals(todosEventos[0])) // Coluna salário mensal.
                                                salarioValoresVazios = premioComMes.Valor; // Armazena o último salário.
                                        }

                                        Premio newPremio = new Premio();
                                        newPremio.Ano = anos[i];
                                        newPremio.Evento = evento;
                                        newPremio.Mes = mes;
                                        newPremio.Matricula = funcionario.Item1;
                                        newPremio.Nome = funcionario.Item2;

                                        if (premioComMes == null)
                                        {
                                            // Este mês não existe para o último evento.
                                            if (evento.Equals(todosEventos[0]))
                                            {
                                                if (anos[i] == anos[anos.Count - 1])
                                                {
                                                    if (mes < DateTime.Now.Month)
                                                    {
                                                        newPremio.Valor = salarioValoresVazios.Value;
                                                    }
                                                    else
                                                    {
                                                        newPremio.Valor = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    newPremio.Valor = salarioValoresVazios.Value;
                                                }
                                            }
                                            else
                                            {
                                                newPremio.Valor = 0;
                                            }
                                        }
                                        else
                                        {
                                            newPremio.Valor = premioComMes.Valor;
                                        }

                                        valor = newPremio.Valor;
                                        premios.Add(newPremio);

                                        #region Total

                                        // Soma valor total por mês.
                                        switch (mes)
                                        {
                                            case 1:
                                                totalJan += valor;
                                                break;
                                            case 2:
                                                totalFev += valor;
                                                break;
                                            case 3:
                                                totalMar += valor;
                                                break;
                                            case 4:
                                                totalAbr += valor;
                                                break;
                                            case 5:
                                                totalMai += valor;
                                                break;
                                            case 6:
                                                totalJun += valor;
                                                break;
                                            case 7:
                                                totalJul += valor;
                                                break;
                                            case 8:
                                                totalAgo += valor;
                                                break;
                                            case 9:
                                                totalSet += valor;
                                                break;
                                            case 10:
                                                totalOut += valor;
                                                break;
                                            case 11:
                                                totalNov += valor;
                                                break;
                                            case 12:
                                                totalDez += valor;
                                                break;
                                            default:
                                                break;
                                        }

                                        // Soma valor total por evento.
                                        switch (evento)
                                        {
                                            case "SALARIO MENSAL":
                                                totalSalarioMensal += valor;
                                                break;
                                            case "PARTICIPE":
                                                totalParticipe += valor;
                                                break;
                                            case "PREMIO":
                                                totalPremio += valor;
                                                break;
                                            case "PREMIO SEMESTRAL":
                                                totalPremioSemestral += valor;
                                                break;
                                            case "PREMIO TRIMESTRAL":
                                                totalPremioTrimestral += valor;
                                                break;
                                            case "PREMIO ANUAL":
                                                totalPremioAnual += valor;
                                                break;
                                            case "PREMIO EXTRA":
                                                totalPremioExtra += valor;
                                                break;
                                            case "DSR S/ PREMIOS":
                                                totalPremioDSR += valor;
                                                break;
                                            default:
                                                break;
                                        }

                                        #endregion
                                    }
                                }
                            }
                        }
                    }

                    dtTodasPremiacoes = new DataTable();
                    dtTodasPremiacoes.Columns.Add("ANO", Type.GetType("System.Int32"));
                    dtTodasPremiacoes.Columns.Add("MES", Type.GetType("System.Int32"));
                    dtTodasPremiacoes.Columns.Add("EVENTO", Type.GetType("System.String"));
                    dtTodasPremiacoes.Columns.Add("VALOR", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("TOTAL_MES", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("TOTAL_EVENTO", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("TOTAL", Type.GetType("System.Decimal"));
                    dtTodasPremiacoes.Columns.Add("MATRICULA", Type.GetType("System.String"));
                    dtTodasPremiacoes.Columns.Add("NOME", Type.GetType("System.String"));

                    DataRow drNewPremiacao = null;
                    foreach (var item in premios)
                    {
                        drNewPremiacao = null;
                        drNewPremiacao = dtTodasPremiacoes.NewRow();
                        drNewPremiacao["ANO"] = item.Ano;
                        drNewPremiacao["MES"] = item.Mes;
                        drNewPremiacao["EVENTO"] = item.Evento;

                        if ((item.Ano == DateTime.Now.Year) && (item.Mes >= DateTime.Now.Month))
                        {
                            if (item.Valor == 0)
                            {
                                drNewPremiacao["VALOR"] = DBNull.Value;
                            }
                            else
                            {
                                drNewPremiacao["VALOR"] = item.Valor;
                            }

                            if (item.TotalMes == 0)
                            {
                                drNewPremiacao["TOTAL_MES"] = DBNull.Value;
                            }
                            else
                            {
                                drNewPremiacao["TOTAL_MES"] = item.TotalMes;
                            }
                        }
                        else
                        {
                            drNewPremiacao["VALOR"] = item.Valor;
                            drNewPremiacao["TOTAL_MES"] = item.TotalMes;
                        }

                        drNewPremiacao["TOTAL_EVENTO"] = item.TotalEvento;
                        drNewPremiacao["TOTAL"] = item.Total;
                        drNewPremiacao["MATRICULA"] = item.Matricula;
                        drNewPremiacao["NOME"] = item.Nome;
                        dtTodasPremiacoes.Rows.Add(drNewPremiacao);
                    }

                    #endregion
                }

                return dtTodasPremiacoes;
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao extrair o nível do colaborador da linha da tabela4: " + e.Message + e.StackTrace, EventLogEntryType.Error, 2, 2);
                return null;
            }
            finally
            {
                if (dtPremios != null)
                    dtPremios.Dispose();

                if (dtTodasPremiacoes != null)
                    dtTodasPremiacoes.Dispose();
            }
        }

        public static List<string> GetEventos()
        {
            return new List<string>() { "SALARIO MENSAL", "PARTICIPE", "PREMIO", "PREMIO SEMESTRAL", "PREMIO TRIMESTRAL", "PREMIO ANUAL", "PREMIO EXTRA", "DSR S/ PREMIOS" };
        }
    }
}
