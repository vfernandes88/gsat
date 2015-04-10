using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cit.Globosat.Remuneracao.Formularios.Entidades;
using System.Diagnostics;
using CIT.Sharepoint.Util;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.Office.Server.UserProfiles;
using System.Globalization;
using System.Configuration;
using Cit.Globosat.Remuneracao.Formularios.DAL.ReqPessoal;

namespace Cit.Globosat.Remuneracao.Formularios.BLL.ReqPessoal
{
    class ReqPessoalBLL
    {
        private static bool ambiente_producao = Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]);

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
                Logger.Write("Erro ao buscar dados do Profile: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 2, 3);
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
        public static bool VerificaLogin(SPWeb web, string login)
        {
            try
            {
                bool possuiAcesso = false;

                SPUser userLogado = SPContext.Current.Web.CurrentUser;
                SPGroup grupoAdministrador = web.Groups["Grupo_Remuneração_Administradores"];

                foreach (SPUser administrador in grupoAdministrador.Users)
                {
                    if (administrador.LoginName.Equals(login))
                        possuiAcesso = true;
                }
                return possuiAcesso;
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Erro validar usuário administrador : {0}.", ex.Message + ex.StackTrace), EventLogEntryType.Error, 1, 2);
                return false;
            }
        }
        
        public static string BuscaSalario(string classe, string nivel, string jornada, string filial, string coligada)
        {
            try
            {
                DataRow salario = DAL.AltFuncCargo.FormDAL.GetSalarioProposto(classe, nivel, jornada, filial, coligada);

                if (salario != null)
                {
                    return Convert.ToDecimal(salario["SALARIO"]).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                }
                return "";
            }
            catch (Exception e)
            {
                Logger.Write("Erro ao buscar salario: " + e.Message + e.StackTrace, EventLogEntryType.Error, 1, 2);
                throw;
            }
        }
    }
    
}
