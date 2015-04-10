using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using CIT.Sharepoint.Util;

namespace Cit.Globosat.Remuneracao.DAL
{
    public class FormDAL
    {
        static SqlCommand command = null;
        static SqlDataAdapter adapter = null;
        static DataTable tableDados = null;

        private static bool ambiente_producao = true;

        /// <summary>
        /// Função que busca os dados do funcionário no banco de dados.
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
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CHAPASUBST = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "'", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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

                DataRow dr = dt.NewRow();
                dr["CODSECAO"] = "1";
                dr["DESCRICAO"] = "Globosat";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "2";
                dr["DESCRICAO"] = "Globosat 2";
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
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, (CODSECAO + '_' + ESTADO) AS CODSECAO_ESTADO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET ORDER BY DESCRICAO", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar todos os Centros de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                dt.Columns.Add("CODSECAO_ESTADO", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CODSECAO"] = "01.202.106";
                dr["DESCRICAO"] = "REMUNERAÇÃO";
                dr["CODSECAO_ESTADO"] = "01.202.106_RJ";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "6.401.102";
                dr["DESCRICAO"] = "SEXY HOT";
                dr["CODSECAO_ESTADO"] = "6.401.102_RJ";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "01.505.101";
                dr["DESCRICAO"] = "SISTEMA DE TV";
                dr["CODSECAO_ESTADO"] = "01.505.101_RJ";
                dt.Rows.Add(dr);

                dr = null;
                dr = dt.NewRow();
                dr["CODSECAO"] = "02.202.102";
                dr["DESCRICAO"] = "RECURSOS HUMANOS SP";
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
                    command = new SqlCommand("SELECT distinct CHAPA, NOME FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CODSECAO = '" + centroCusto.Trim() + "' ORDER BY NOME DESC", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                    command = new SqlCommand("SELECT * FROM LK_RM..INT_SHAREP_RM.VW_PERFIL_INTRANET f join LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET r on r.CHAPA = f.CHAPA WHERE r.CHAPA = '" + matricula + "'", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                    " AND r.CODCOLIGADA = " + codcoligada, BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                dr["CODSECAO"] = "1.";
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
                    command = new SqlCommand("SELECT TOP 1 SALARIO, DTMUDANCA FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL WHERE CHAPA = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "' ORDER BY DTMUDANCA DESC", BaseDAL.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados.Rows[0];
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar Salario Atual: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                    //command = new SqlCommand("SELECT CHAPA Matrícula, NOME_FUNC Nome, DTMUDANCA Data, SALARIO Salário, DESCRICAO Motivo, FUNCAO Cargo, CODNIVEL Nível, CODFAIXA Classe FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL WHERE CHAPA = '" + matricula + "' AND CODCOLIGADA = '" + coligada + "' AND YEAR(GETDATE()) - year(DTMUDANCA) <= '10'", BaseDAL.GetConnection());
                    //command = new SqlCommand("SELECT H.CHAPA Matrícula, NOME_FUNC Nome, H.DTMUDANCA Data, SALARIO Salário, DESCRICAO Motivo, F.NOME Cargo, CODNIVEL Classe, CODFAIXA Nível FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL H LEFT JOIN LK_RM..INT_SHAREP_RM.VW_HISTORICO_FUNCIONAL F ON CONVERT(VARCHAR,H.DTMUDANCA,103) = CONVERT(VARCHAR,F.DTMUDANCA,103) AND F.CHAPA = H.CHAPA WHERE H.CHAPA = '" + matricula + "' AND H.CODCOLIGADA = '" + coligada + "' AND YEAR(GETDATE()) - year(H.DTMUDANCA) <= '10' ORDER BY H.DTMUDANCA", BaseDAL.GetConnection());
                    command = new SqlCommand(@"SELECT * FROM OPENQUERY(LK_RM, 'SELECT H.CHAPA Matrícula, NOME_FUNC Nome, H.DTMUDANCA Data,
                SALARIO Salário, DESCRICAO Motivo, F.NOME Cargo, CODNIVEL Classe, CODFAIXA Nível
                FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                LEFT JOIN INT_SHAREP_RM.VW_HISTORICO_FUNCIONAL F
                ON H.DTMUDANCA = F.DTMUDANCA
                AND H.CODCOLIGADA = F.CODCOLIGADA
                AND F.CHAPA = H.CHAPA WHERE H.CHAPA = ''" + matricula + @"'' AND H.CODCOLIGADA = ''" + coligada + @"''
                AND extract(year from sysdate) - extract(year from H.DTMUDANCA) <= ''10'' ORDER BY H.DTMUDANCA')", BaseDAL.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                catch (SqlException ex)
                {
                    Logger.Write("(InfoPaht Error) - Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                dt.Columns.Add("data", Type.GetType("System.String"));

                DataRow dr = dt.NewRow();
                dr["CHAPA"] = "1.";
                dr["CODCOLIGADA"] = "50.";
                dr["ENDERECOPAGTO"] = "EnderecoPagto";
                dr["DEPARTAMENTO"] = "Depto";
                dr["CODSECAO"] = "1.";
                dr["CHAPA"] = "chapa";
                dr["CARGO"] = "aux";
                dr["GRUPOSALARIAL"] = "B";
                dr["CODNIVELSAL"] = "3";
                dr["DTBASE"] = "04/02/2012";
                dr["data"] = "04/02/2012";
                dr["SALARIO"] = "000";
                dr["NOME"] = "FRANCISCO";
                dt.Rows.Add(dr);

                return dt;
                #endregion
            }
        }

        public static DataRow GetSalarioProposto(string classe, string nivel, string jornada, string filial, string coligada)
        {
            if (ambiente_producao)
            {
                #region PRODUCAO
                try
                {
                    if (filial == "SP")
                    {
                        command = new SqlCommand("SELECT CAST(SALARIO as decimal(10,2)) SALARIO FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODCOLIGADA = '" + coligada + "' AND NOMETABELA LIKE '%" + jornada + "%' AND NIVEL = '" + nivel + "' AND FAIXA = '" + classe + "' AND NOMETABELA LIKE '%SP%'", BaseDAL.GetConnection());
                    }
                    else
                    {
                        command = new SqlCommand("SELECT CAST(SALARIO as decimal(10,2)) SALARIO FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODCOLIGADA = '" + coligada + "' AND NOMETABELA LIKE '%" + jornada + "%' AND NIVEL = " + nivel + " AND FAIXA = '" + classe + "' AND NOMETABELA NOT LIKE '%SP%'", BaseDAL.GetConnection());
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
                    Logger.Write("(InfoPaht Error) - Erro ao buscar salário proposto: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 3, 1);
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
                dr["SALARIO"] = "100";
                dt.Rows.Add(dr);

                return dt.Rows[0];
                #endregion
            }
        }
    }
    }