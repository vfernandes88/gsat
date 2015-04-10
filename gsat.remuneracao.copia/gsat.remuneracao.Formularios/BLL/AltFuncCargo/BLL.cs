using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using CIT.Sharepoint.Util;
using Cit.Globosat.Remuneracao.Formularios.Entidades;
using Microsoft.Office.Server.UserProfiles;
using System.Diagnostics;
using Cit.Globosat.Remuneracao.Formularios.DAL;
using System.Globalization;
using System.Configuration;
using System.Data;
using Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo;
using Cit.Globosat.Common;

namespace Cit.Globosat.Remuneracao.Formularios.BLL.AltFuncCargo
{
    public class BLL
    {
        private static bool ambiente_producao = Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]);

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
                    salarioProposto = salarioProposto.Replace("R$", "").Trim();

                salarioPropostoConvertido = Convert.ToDecimal(salarioProposto, CultureInfo.CreateSpecificCulture("pt-BR"));
                salarioAtualConvertido = Convert.ToDecimal(salarioAtual, CultureInfo.CreateSpecificCulture("pt-BR"));

                //return Math.Round(salarioPropostoConvertido - salarioAtualConvertido, 2);
                return salarioPropostoConvertido - salarioAtualConvertido;
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
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

                total = (salarioPropostoConvertido / salarioAtualConvertido) - 1;

                return total;
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                return 0;
            }
        }

        /// <summary>
        /// Busca dados no profile do colaborador que está abrindo o Formulário
        /// </summary>
        /// <param name="site">Contexto em que está o profile</param>
        /// <param name="login">Login do usuário logado</param>
        /// <returns>Matrícula e coligada do usuário (Entidade DadosProfile)</returns>
        public static DadosProfile BuscaDadosUserProfile(SPSite site, string login)
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

                        try
                        {
                            dadosProfile.CentroCusto = profile["CentroCusto"].Value.ToString();
                        }
                        catch
                        {
                            dadosProfile.CentroCusto = "0";
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
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
        }

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
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                return false;
            }
        }

        public static List<DadosRemuneracao> BuscaHistoricoSalarial(string matricula, string coligada)
        {
            try
            {
                if (true)
                {
                    #region PRODUCAO
                    List<DadosRemuneracao> listaHistoricoSalarial = new List<DadosRemuneracao>();
                    DataTable tableHistoricoSalarial = new DataTable();

                    tableHistoricoSalarial = FormDAL.GetHistoricoSalarial(matricula, coligada);

                    for (int i = 0; i < tableHistoricoSalarial.Rows.Count; i++)
                    {
                        DadosRemuneracao itemHistoricoSalarial = new DadosRemuneracao();

                        if (tableHistoricoSalarial.Rows[i]["Data"] != null)
                            itemHistoricoSalarial.Data = Convert.ToDateTime(tableHistoricoSalarial.Rows[i]["Data"]).ToString("MM/yyyy");

                        if (tableHistoricoSalarial.Rows[i]["Salário"] != null)
                        {
                            itemHistoricoSalarial.SalarioNumber = Convert.ToDouble(tableHistoricoSalarial.Rows[i]["Salário"]);
                            itemHistoricoSalarial.Salario = "R$" + itemHistoricoSalarial.SalarioNumber.ToString().Replace(".", ",");
                            if (!itemHistoricoSalarial.Salario.Contains(","))
                            {
                                itemHistoricoSalarial.Salario += ",00";
                            }
                        }

                        if (i != 0)
                        {
                            //Calcula percentual de acordo com o salário
                            itemHistoricoSalarial.PercentualNumber = Math.Round(CalculaPercentual(itemHistoricoSalarial.SalarioNumber, Convert.ToDouble(tableHistoricoSalarial.Rows[i - 1]["Salário"])), 2);
                            itemHistoricoSalarial.Percentual = itemHistoricoSalarial.PercentualNumber.ToString().Replace('.', ',') + "%";
                        }
                        else
                        {
                            //Se for a primeira linha o valor é 0%
                            itemHistoricoSalarial.Percentual = "0%";
                        }

                        if (tableHistoricoSalarial.Rows[i]["Motivo"] != null)
                            itemHistoricoSalarial.Motivo = tableHistoricoSalarial.Rows[i]["Motivo"].ToString();

                        if (tableHistoricoSalarial.Rows[i]["Cargo"] != null)
                            itemHistoricoSalarial.Funcao = tableHistoricoSalarial.Rows[i]["Cargo"].ToString();

                        listaHistoricoSalarial.Add(itemHistoricoSalarial);
                    }
                    return listaHistoricoSalarial;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    List<DadosRemuneracao> listaHistoricoSalarial = new List<DadosRemuneracao>();

                    DadosRemuneracao itemHistoricoSalarial = new DadosRemuneracao();
                    itemHistoricoSalarial.Data = "abril/2013";
                    itemHistoricoSalarial.SalarioNumber = Convert.ToDouble("1000");
                    itemHistoricoSalarial.Salario = "R$" + "1000";
                    itemHistoricoSalarial.Salario += ",00";
                    itemHistoricoSalarial.PercentualNumber = 10;
                    itemHistoricoSalarial.Percentual = itemHistoricoSalarial.PercentualNumber.ToString().Replace('.', ',') + "%";
                    itemHistoricoSalarial.Percentual = "0%"; // Se for a primeira linha o valor é 0%
                    itemHistoricoSalarial.Motivo = "Motivo";
                    itemHistoricoSalarial.Funcao = "Cargo";
                    listaHistoricoSalarial.Add(itemHistoricoSalarial);

                    DadosRemuneracao itemHistoricoSalarial1 = new DadosRemuneracao();
                    itemHistoricoSalarial1.Data = "abril/2013";
                    itemHistoricoSalarial1.SalarioNumber = Convert.ToDouble("1000");
                    itemHistoricoSalarial1.Salario = "R$" + "1000";
                    itemHistoricoSalarial1.Salario += ",00";
                    itemHistoricoSalarial1.PercentualNumber = 10;
                    itemHistoricoSalarial1.Percentual = itemHistoricoSalarial.PercentualNumber.ToString().Replace('.', ',') + "%";
                    itemHistoricoSalarial1.Percentual = "0%"; // Se for a primeira linha o valor é 0%
                    itemHistoricoSalarial1.Motivo = "Motivo";
                    itemHistoricoSalarial1.Funcao = "Cargo";
                    listaHistoricoSalarial.Add(itemHistoricoSalarial1);

                    return listaHistoricoSalarial;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                return null;
            }
        }

        /// <summary>
        /// Calcula percentual de acordo com o salário passado
        /// </summary>
        /// <param name="salarioAtual">Salário no momento do colaborador</param>
        /// <param name="salarioAnterior">Salário anterior do colaborador</param>
        /// <returns>Percentual calculado</returns>
        public static double CalculaPercentual(double salarioAtual, double salarioAnterior)
        {
            try
            {
                //Calcula e retorna valor
                return ((salarioAtual * 100) / salarioAnterior) - 100;
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
        }

        public static string BuscaSalarioProposto(string classe, string nivel, string jornada, string filial, string coligada)
        {
            try
            {
                DataRow salarioProposto = FormDAL.GetSalarioProposto(classe, nivel, jornada, filial, coligada);

                if ((salarioProposto != null) && (!DBNull.Value.Equals(salarioProposto["SALARIO"])))
                {
                    return Convert.ToDecimal(salarioProposto["SALARIO"]).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                }
                
                return Convert.ToDecimal("0").ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")); ;
            }
            catch (Exception ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);

                return Convert.ToDecimal("0").ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")); ;
            }
        }

        public static bool UserExistsInList(SPSite spSite, SPWeb spWeb, string loginName, string listName)
        {
            using (SPSite spSiteAdmin = new SPSite(spSite.ID, spSite.SystemAccount.UserToken))
            {
                using (SPWeb spWebAdmin = spSiteAdmin.OpenWeb(spWeb.ID))
                {
                    // Verifica se usuário é administrador.
                    return VerificaLogin(spWebAdmin, loginName, Constants.AdministradoresRemuneracaolistName);
                }
            }
        }
    }
}
