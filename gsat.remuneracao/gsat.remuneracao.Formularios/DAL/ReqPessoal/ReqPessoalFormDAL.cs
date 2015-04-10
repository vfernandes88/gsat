using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using CIT.Sharepoint.Util;
using System.Configuration;
using Cit.Globosat.Common;
using System.Diagnostics;

namespace Cit.Globosat.Remuneracao.Formularios.DAL.ReqPessoal
{
    class ReqPessoalFormDAL
    {
        static SqlCommand command = null;
        static SqlDataAdapter adapter = null;
        static DataTable tableDados = null;

        private static bool ambiente_producao = Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]);

        public static DataTable GetDiretoria(string codSecao)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand(string.Format(@"SELECT * FROM OPENQUERY(LK_RM, 'SELECT DISTINCT ENDERECOPAGTO AS DIRETORIA 
                                FROM INT_SHAREP_RM.VW_PERFIL_INTRANET  
                                F JOIN INT_SHAREP_RM.VW_CCUSTO_INTRANET R ON R.CHAPA = F.CHAPA 
                                AND R.CODCOLIGADA = F.CODCOLIGADA 
                                WHERE R.CODSECAO=''{0}''')", codSecao), DAL.ReqPessoal.ReqPessoalBaseDAL.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }


                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                dt.Columns.Add("NOME", Type.GetType("System.String"));
                dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));
                dt.Columns.Add("DIRETORIA", Type.GetType("System.String"));
                dt.Columns.Add("DEPARTAMENTO", Type.GetType("System.String"));
                dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                dt.Columns.Add("CARGO", Type.GetType("System.String"));
                dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));
                dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                dt.Columns.Add("DTBASE", Type.GetType("System.String"));
                dt.Columns.Add("CODFILIAL", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["CODCOLIGADA"] = "1";
                dr["DIRETORIA"] = "EnderecoPagto";
                dr["DEPARTAMENTO"] = "Depto";
                dr["CODSECAO"] = "01.101.102";
                dr["CARGO"] = "aux";
                dr["GRUPOSALARIAL"] = "B";
                dr["CODNIVELSAL"] = "11";
                dr["DTBASE"] = "04/02/2012";
                dr["SALARIO"] = "300";
                dr["NOME"] = "FRANCISCO";
                dr["CODFILIAL"] = "2";
                dt.Rows.Add(dr);

                return dt;
                #endregion
            }
        }

        public static DataTable GetCargos(string codSecao)
        {
            try
            {

                //Query deve buscar cargos de acordo com centro de custo. E retornar salario/classe /nivel do cargo seleciondo
                // command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO  FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET ORDER BY DESCRICAO", BaseDAL.GetConnection());//("SELECT distinct CODSECAO, DESCRICAO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CHAPASUBST = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "'", BaseDAL.GetConnection());
                command = new SqlCommand("SELECT * FROM CARGOS WHERE GESTOR = '" + codSecao + "'", DAL.ReqPessoal.ReqPessoalBaseDAL.GetConnection());//("SELECT distinct CODSECAO, DESCRICAO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CHAPASUBST = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "'", BaseDAL.GetConnection());

                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        public static DataRow GetSalario(string classe, string nivel, string jornada, string filial, string coligada)
        {
            try
            {
                if (filial == "SP")
                {
                    command = new SqlCommand("SELECT CAST(SALARIO as decimal(10,2)) SALARIO FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODCOLIGADA = '" + coligada + "' AND NOMETABELA LIKE '%" + jornada + "%' AND NIVEL = '" + nivel + "' AND FAIXA = '" + classe + "' AND NOMETABELA LIKE '%SP%'", DAL.ReqPessoal.ReqPessoalBaseDAL.GetConnection());
                }
                else
                {
                    command = new SqlCommand("SELECT CAST(SALARIO as decimal(10,2)) SALARIO FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODCOLIGADA = '" + coligada + "' AND NOMETABELA LIKE '%" + jornada + "%' AND NIVEL = '" + nivel + "' AND FAIXA = '" + classe + "' AND NOMETABELA NOT LIKE '%SP%'", DAL.ReqPessoal.ReqPessoalBaseDAL.GetConnection());
                }

                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);
                if (tableDados.Rows.Count == 1)
                {
                    return tableDados.Rows[0];
                }
                return null;
            }
            catch (SqlException ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
            //  

        }

        public static DataTable GetDadosCargo(string cargo)
        {
            try
            {
                command = new SqlCommand("SELECT CARGO, NIVEL, CLASSE, CARGA_HORARIA, SALARIO FROM CARGOS WHERE CARGO = '" + cargo + "' ORDER BY CARGO ASC", DAL.ReqPessoal.ReqPessoalBaseDAL.GetConnection());
                //command = new SqlCommand("SELECT TOP 1 SALARIO, DTMUDANCA FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL WHERE CHAPA = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "' ORDER BY DTMUDANCA DESC", BaseDAL.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);
                //Alterei aqui
                return tableDados;//.Rows[0];
            }
            catch (SqlException ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Função que busca os dados do funcionário no banco de dados
        /// </summary>
        /// <param name="matricula">
        /// Matrícula do Funcionário
        /// </param>
        /// <returns>Tabela com todos os funcionarios</returns>
        public static DataTable GetCentroCusto(string matricula, string coligada)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    string sql = string.Format(@"
SELECT DISTINCT CCI.CODSECAO,CCI.DESCRICAO, CCI.CODCOLIGADA, (CCI.CODSECAO + ' - ' + CCI.DESCRICAO) AS COD_DESC, (CCI.CODSECAO + '_' + CCI.ESTADO) AS CODSECAO_ESTADO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
INNER JOIN LK_RM..RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE.CODSECAO
WHERE PCE.CODEXTERNO = '{0}' AND CODCOLSUBST = {1}
UNION
SELECT distinct C.CODSECAO, C.DESCRICAO, C.CODCOLIGADA, (C.CODSECAO + ' - ' + C.DESCRICAO) AS COD_DESC, (C.CODSECAO + '_' + C.ESTADO) AS CODSECAO_ESTADO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET C
LEFT JOIN LK_RM..RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C.CODSECAO
where C.CHAPASUBST = '{0}' AND C.CODCOLIGADA = {1}", matricula, coligada);

                    command = new SqlCommand(sql, ReqPessoalBaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                dt.Columns.Add("COD_DESC", Type.GetType("System.String"));
                dt.Columns.Add("CODSECAO_ESTADO", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CODSECAO"] = "1";
                dr["DESCRICAO"] = "Globosat";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "01.202.106";
                dr["DESCRICAO"] = "REMUNERAÇÃO";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "01.202.106_RJ";
                dt.Rows.Add(dr);
                return dt;
                #endregion
            }
        }

        /// <summary>
        /// Busca no banco todos os centros de custo disponíveis
        /// </summary>
        /// <returns>DataTable com as informações</returns>
        public static DataTable GetAllCentrosCusto()
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, (CODSECAO + ' - ' + DESCRICAO) AS COD_DESC, (CODSECAO + '_' + ESTADO) AS CODSECAO_ESTADO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET ORDER BY DESCRICAO ASC", ReqPessoalBaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                dt.Columns.Add("COD_DESC", Type.GetType("System.String"));
                dt.Columns.Add("CODSECAO_ESTADO", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CODSECAO"] = "01.202.106";
                dr["DESCRICAO"] = "REMUNERAÇÃO";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "01.202.106_RJ";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "6.401.102";
                dr["DESCRICAO"] = "SEXY HOT";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "6.401.102_RJ";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "01.505.101";
                dr["DESCRICAO"] = "SISTEMA DE TV";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "01.505.101_RJ";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "02.202.102";
                dr["DESCRICAO"] = "RECURSOS HUMANOS SP";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "02.202.102_SP";
                dt.Rows.Add(dr);
                return dt;
                #endregion
            }
        }

        /// <summary>
        /// Busca todos os colaboradores pertencentes ao centro de custo selecionado
        /// </summary>
        /// <param name="centroCusto">Centro de custo selecionado</param>
        /// <returns>Tabela com todos os colaboradores</returns>
        public static DataTable GetTodosColaboradores(string centroCusto)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand("SELECT distinct CHAPA, NOME FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CODSECAO = '" + centroCusto.Trim() + "' ORDER BY NOME DESC", ReqPessoalBaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                dt.Columns.Add("NOME", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["NOME"] = "FRANCISCO";
                dt.Rows.Add(dr);

                return dt;
                #endregion
            }
        }

        /// <summary>
        /// Busca dados de colaborador selecionado
        /// </summary>
        /// <param name="matricula">Matrícula do colaborador selecionado</param>
        /// <returns>Tabela com todos os dados necessários</returns>
        public static DataTable GetDadosColaborador(string matricula)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand("SELECT * FROM LK_RM..INT_SHAREP_RM.VW_PERFIL_INTRANET f join LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET r on r.CHAPA = f.CHAPA WHERE r.CHAPA = '" + matricula + "'", ReqPessoalBaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                dt.Columns.Add("NOME", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["SALARIO"] = "3";
                dr["NOME"] = "FRANCISCO";
                dt.Rows.Add(dr);

                return dt;
                #endregion
            }
        }

        public static DataTable GetDadosColaborador(string matricula, string codcoligada)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand("SELECT * FROM LK_RM..INT_SHAREP_RM.VW_PERFIL_INTRANET f join LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET r on r.CHAPA = f.CHAPA AND R.CODCOLIGADA = F.CODCOLIGADA WHERE r.CHAPA = '" + matricula + "'" +
                    " AND r.CODCOLIGADA = " + codcoligada, ReqPessoalBaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                dt.Columns.Add("NOME", Type.GetType("System.String"));
                dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));
                dt.Columns.Add("ENDERECOPAGTO", Type.GetType("System.String"));
                dt.Columns.Add("DEPARTAMENTO", Type.GetType("System.String"));
                dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                dt.Columns.Add("CARGO", Type.GetType("System.String"));
                dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));
                dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                dt.Columns.Add("DTBASE", Type.GetType("System.String"));
                dt.Columns.Add("CODFILIAL", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["CODCOLIGADA"] = "1";
                dr["ENDERECOPAGTO"] = "EnderecoPagto";
                dr["DEPARTAMENTO"] = "Depto";
                dr["CODSECAO"] = "01.101.102";
                dr["CARGO"] = "aux";
                dr["GRUPOSALARIAL"] = "B";
                dr["CODNIVELSAL"] = "11";
                dr["DTBASE"] = "04/02/2012";
                dr["SALARIO"] = "300";
                dr["NOME"] = "FRANCISCO";
                dr["CODFILIAL"] = "2";
                dt.Rows.Add(dr);

                return dt;
                #endregion
            }
        }

        /// <summary>
        /// Pega o salário atual do colaborador
        /// </summary>
        /// <param name="matricula">Matrícula do colaborador</param>
        /// <param name="coligada">Coligada do colaborador</param>
        /// <returns>Linha da tabela com a informação</returns>
        public static DataRow GetSalarioAtual(string matricula, string coligada)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand("SELECT TOP 1 SALARIO, DTMUDANCA FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL WHERE CHAPA = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "' ORDER BY DTMUDANCA DESC", ReqPessoalBaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados.Rows[0];
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                        Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
                #endregion
            }
            else
            {
                #region DESENVOLVIMENTO
                DataTable dt = new DataTable();
                dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                dt.Columns.Add("NOME", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["SALARIO"] = "300";
                dr["NOME"] = "FRANCISCO";
                dt.Rows.Add(dr);

                return dt.Rows[0];
                #endregion
            }
        }
    }
}
