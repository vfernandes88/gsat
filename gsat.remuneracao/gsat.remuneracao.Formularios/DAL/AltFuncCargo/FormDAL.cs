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

namespace Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo
{
    public class FormDAL
    {
        static SqlCommand command = null;
        static SqlDataAdapter adapter = null;
        static DataTable tableDados = null;

        private static bool ambiente_producao = Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]);

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
SELECT DISTINCT CCI.CODSECAO,CCI.DESCRICAO, CCI.CODCOLIGADA, (CCI.CODSECAO + ' - ' + CCI.DESCRICAO) AS COD_DESC, (CCI.CODSECAO + '_' + CCI.ESTADO) AS CODSECAO_ESTADO, (CCI.CODSECAO + '_' + CCI.ESTADO + '_' + cast(CCI.CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
INNER JOIN LK_RM..RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE.CODSECAO
WHERE PCE.CODEXTERNO = '{0}' AND CODCOLSUBST = {1}
UNION
SELECT distinct C.CODSECAO, C.DESCRICAO, C.CODCOLIGADA, (C.CODSECAO + ' - ' + C.DESCRICAO) AS COD_DESC, (C.CODSECAO + '_' + C.ESTADO) AS CODSECAO_ESTADO, (C.CODSECAO + '_' + C.ESTADO + '_' + CAST(C.CODCOLIGADA as VARCHAR)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET C
LEFT JOIN LK_RM..RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C.CODSECAO
where C.CHAPASUBST = '{0}' AND C.CODCOLIGADA = {1}  ORDER BY DESCRICAO ASC", matricula, coligada);



                    command = new SqlCommand(sql, BaseDAL.GetConnection());
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
                dt.Columns.Add("CODSECAO_ESTADO_COLIGADA", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CODSECAO"] = "01.202.107";
                dr["DESCRICAO"] = "TESTE";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "01.202.107_RJ";
                dr["CODSECAO_ESTADO_COLIGADA"] = "01.202.107_RJ_1";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "01.202.106";
                dr["DESCRICAO"] = "REMUNERAÇÃO";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "01.202.106_RJ";
                dr["CODSECAO_ESTADO_COLIGADA"] = "01.202.106_RJ_1";
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
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, (CODSECAO + ' - ' + DESCRICAO) AS COD_DESC, (CODSECAO + '_' + ESTADO) AS CODSECAO_ESTADO, (CODSECAO + '_' + ESTADO + '_' + cast(CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET ORDER BY DESCRICAO ASC", BaseDAL.GetConnection());
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
                dt.Columns.Add("CODSECAO_ESTADO_COLIGADA", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CODSECAO"] = "01.202.106";
                dr["DESCRICAO"] = "REMUNERAÇÃO";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "01.202.106_RJ";
                dr["CODSECAO_ESTADO_COLIGADA"] = "01.202.106_RJ_1";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "6.401.102";
                dr["DESCRICAO"] = "SEXY HOT";
                dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                dr["CODSECAO_ESTADO"] = "6.401.102_RJ";
                dr["CODSECAO_ESTADO_COLIGADA"] = "6.401.102_RJ_1";
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
                    command = new SqlCommand("SELECT distinct CHAPA, NOME FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CODSECAO = '" + centroCusto.Trim() + "' ORDER BY NOME ASC", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Erro: {1}",
                        Utility.GetCurrentMethod(), ex.Message), EventLogEntryType.Error, 2, 3);
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
                dr["NOME"] = "FRANCISCO DA SILVA JOSE PEREIRA";
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
                    command = new SqlCommand("SELECT * FROM LK_RM..INT_SHAREP_RM.VW_PERFIL_INTRANET f join LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET r on r.CHAPA = f.CHAPA WHERE r.CHAPA = '" + matricula + "'", BaseDAL.GetConnection());
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
                dr["NOME"] = "FRANCISCO DA SILVA JOSE PEREIRA";
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
                    " AND r.CODCOLIGADA = " + codcoligada, BaseDAL.GetConnection());
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
                dt.Columns.Add("ESTADO", Type.GetType("System.String"));
                dt.Columns.Add("CODIGO", Type.GetType("System.String"));
                dt.Columns.Add("JORNADA", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["CODCOLIGADA"] = "1";
                dr["ENDERECOPAGTO"] = "EnderecoPagto";
                dr["DEPARTAMENTO"] = "Depto";
                dr["CODSECAO"] = "01.101.102";
                dr["CODIGO"] = "P000023";
                dr["CARGO"] = "aux";
                dr["GRUPOSALARIAL"] = "B";
                dr["CODNIVELSAL"] = "11";
                dr["DTBASE"] = "04/02/2012";
                dr["SALARIO"] = "300";
                dr["NOME"] = "FRANCISCO DA SILVA JOSE PEREIRA";
                dr["CODFILIAL"] = "2";
                dr["ESTADO"] = "RJ";
                dr["JORNADA"] = "180";
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
                    command = new SqlCommand("SELECT TOP 1 SALARIO, DTMUDANCA FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL WHERE CHAPA = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "' ORDER BY DTMUDANCA DESC", BaseDAL.GetConnection());
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
                dr["NOME"] = "FRANCISCO DA SILVA JOSE PEREIRA";
                dt.Rows.Add(dr);

                return dt.Rows[0];
                #endregion
            }
        }

        /// <summary>
        /// Busca Historico Salarial do Colaborador
        /// </summary>
        /// <param name="matricula">Matricula do Colaborador</param>
        /// <returns>Tabela com o histórico salarial do colaborador</returns>
        public static DataTable GetHistoricoSalarial(string matricula, string coligada)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand(@"SELECT * FROM OPENQUERY(LK_RM, 'SELECT H.CHAPA Matrícula, NOME_FUNC Nome, H.DTMUDANCA Data,
                                                SALARIO Salário, DESCRICAO Motivo, F.NOME Cargo, CODNIVEL Classe, CODFAIXA Nível
                                                FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                                LEFT JOIN INT_SHAREP_RM.VW_HISTORICO_FUNCIONAL F
                                                ON H.DTMUDANCA = F.DTMUDANCA
                                                AND H.CODCOLIGADA = F.CODCOLIGADA
                                                AND F.CHAPA = H.CHAPA WHERE H.CHAPA = ''" + matricula + @"'' AND H.CODCOLIGADA = ''" + coligada + @"''
                                                AND extract(year from sysdate) - extract(year from H.DTMUDANCA) <= ''8'' ORDER BY H.DTMUDANCA')"
                                                , BaseDAL.GetConnection());

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
                #region Desenvolvimento
                DataTable dt = new DataTable();
                dt.Columns.Add("Matrícula", Type.GetType("System.String"));
                dt.Columns.Add("Nome", Type.GetType("System.String"));
                dt.Columns.Add("Data", Type.GetType("System.String"));
                dt.Columns.Add("Salário", Type.GetType("System.String"));
                dt.Columns.Add("Motivo", Type.GetType("System.String"));
                dt.Columns.Add("Cargo", Type.GetType("System.String"));
                dt.Columns.Add("Classe", Type.GetType("System.String"));
                dt.Columns.Add("Nível", Type.GetType("System.String"));

                DataRow dr0 = dt.NewRow();
                dr0["Matrícula"] = "08052";
                dr0["Nome"] = "HAROLDO LEDANDECK";
                dr0["Data"] = "2003-01-21 00:00:34.0000000";
                dr0["Salário"] = "381.00";
                dr0["Motivo"] = "ADMISSÃO";
                dr0["Cargo"] = "NULL";
                dr0["Classe"] = "NULL";
                dr0["Nível"] = "NULL";
                dt.Rows.Add(dr0);

                DataRow dr1 = dt.NewRow();
                dr1["Matrícula"] = "20270";
                dr1["Nome"] = "DJALMA FUENTES LEAL";
                dr1["Data"] = "2003-05-01 00:00:01.0000000";
                dr1["Salário"] = "5215.00";
                dr1["Motivo"] = "ADMISSÃO";
                dr1["Cargo"] = "NULL";
                dr1["Classe"] = "NULL";
                dr1["Nível"] = "NULL";
                dt.Rows.Add(dr1);

                DataRow dr2 = dt.NewRow();
                dr2["Matrícula"] = "01720";
                dr2["Nome"] = "ESTER DE ALBERGARIA GOMES PACHECO";
                dr2["Data"] = "2003-05-01 00:00:01.0000000";
                dr2["Salário"] = "1114.00";
                dr2["Motivo"] = "ADMISSÃO";
                dr2["Cargo"] = "NULL";
                dr2["Classe"] = "NULL";
                dr2["Nível"] = "NULL";
                dt.Rows.Add(dr2);

                DataRow dr3 = dt.NewRow();
                dr3["Matrícula"] = "01326";
                dr3["Nome"] = "AMURA DA SILVA LIMA";
                dr3["Data"] = "2003-05-01 00:00:01.0000000";
                dr3["Salário"] = "2040.00";
                dr3["Motivo"] = "ADMISSÃO";
                dr3["Cargo"] = "NULL";
                dr3["Classe"] = "NULL";
                dr3["Nível"] = "NULL";
                dt.Rows.Add(dr3);

                return dt;
                #endregion
            }
        }

        public static DataRow GetSalarioProposto(string classe, string nivel, string jornada, string filial, string coligada)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                // Atenta-se às condições "nível" e "classe" pois os valores estão trocados.
                try
                {
                    string query = string.Empty;
                    if (filial == "SP")
                    {
                        query = " SELECT" +
                                "   CAST(SALARIO as decimal(10,2)) SALARIO" +
                                " FROM" +
                                "   LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL" +
                                " WHERE" +
                                "   CODCOLIGADA = '" + coligada + "'" +
                                " AND" +
                                "   NOMETABELA LIKE '%" + jornada + "%'" +
                                " AND" +
                                "   NIVEL = " + classe +
                                " AND" +
                                "   FAIXA = '" + nivel + "'";
                                
                        if (Convert.ToInt32(coligada) != 5) // G2C
                        {
                            query += " AND NOMETABELA LIKE '%SP%'";
                        }
                    }
                    else
                    {
                        query = " SELECT" + 
                                "   CAST(SALARIO as decimal(10,2)) SALARIO" + 
                                " FROM" + 
                                "   LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL" + 
                                " WHERE" + 
                                "   CODCOLIGADA = '" + coligada + "'" + 
                                " AND" + 
                                "   NOMETABELA LIKE '%" + jornada + "%'" + 
                                " AND" + 
                                "   NIVEL = '" + classe + "'" + 
                                " AND" + 
                                "   FAIXA = '" + nivel + "'" + 
                                " AND" + 
                                "   NOMETABELA NOT LIKE '%SP%'";
                    }

                    command = new SqlCommand(query, BaseDAL.GetConnection());
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
                #endregion
            }
            else
            {
                #region Desenvolvimento
                DataTable dt = new DataTable();
                dt.Columns.Add("SALARIO", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["SALARIO"] = "NULL";
                dt.Rows.Add(dr);

                return dt.Rows[0];
                #endregion
            }
        }

        public static string GetCodigoColigada(string codSecao)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    command = new SqlCommand("SELECT TOP 1 CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET WHERE CODSECAO = '" + codSecao + "'", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    if(tableDados.Rows.Count > 0)
                    {
                        return tableDados.Rows[0][0].ToString();
                    }

                    return string.Empty;
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
                return "1";
                #endregion
            }
        }
        public static string GetCodigoColigadaRP(string codSecao)
        {
            try
            {
                command = new SqlCommand("SELECT TOP 1 CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP WHERE CODSECAO = '" + codSecao + "'", BaseDAL.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                if (tableDados.Rows.Count > 0)
                {
                    return tableDados.Rows[0][0].ToString();
                }

                return string.Empty;
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

    }
}
