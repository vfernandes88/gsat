using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CIT.Sharepoint.Util;
using Globosat.Library.Entidades;
using System.Configuration;

namespace Globosat.Library.AcessoDados
{
    public class AcessoDados
    {
        static SqlCommand command = null;
        static SqlDataAdapter adapter = null;
        static DataTable tableDados = null;
        private static bool ambiente_producao = Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]);



        /// <summary>
        /// Busca opções de Tabela Salarial no Banco Quando o Usuário for Administrador
        /// </summary>
        /// <param name="coligada"></param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable GetOpcaoTabelaSalarial()
        {
            try
            {
                if (true)
                {
                    command = new SqlCommand("SELECT DISTINCT CODTABELA, NOMETABELA, (CODTABELA + ';' + RTRIM(LTRIM(NOMETABELA))) AS VALUE FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE NOMETABELA NOT LIKE '%ESTAG%' ORDER BY NOMETABELA", BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CODTABELA", Type.GetType("System.String"));
                    dt.Columns.Add("NOMETABELA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODTABELA"] = "02";
                    dr["NOMETABELA"] = "150H JOR./LOC.";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODTABELA"] = "03";
                    dr["NOMETABELA"] = "GLOB SP 220 H/210 H/180 H";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODTABELA"] = "04";
                    dr["NOMETABELA"] = "GLOB SP JOR. 150 H/ LOC.150 H";
                    dt.Rows.Add(dr);
                    return dt;

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao buscar Opcao Tabela Salarial: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }


        /// <summary>
        /// Busca opções de Tabela Salarial no Banco
        /// </summary>
        /// <param name="coligada">Coligada do Gestor</param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable GetOpcaoTabelaSalarial(string coligada)
        {
            try
            {
                if (ambiente_producao)
                {
                    command = new SqlCommand("SELECT DISTINCT CODTABELA, NOMETABELA FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODCOLIGADA = '" + coligada + "' AND NOMETABELA NOT LIKE '%ESTAG%' ORDER BY NOMETABELA", BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CODTABELA", Type.GetType("System.String"));
                    dt.Columns.Add("NOMETABELA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODTABELA"] = "02";
                    dr["NOMETABELA"] = "150H JOR./LOC.";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODTABELA"] = "03";
                    dr["NOMETABELA"] = "GLOB SP 220 H/210 H/180 H";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODTABELA"] = "04";
                    dr["NOMETABELA"] = "GLOB SP JOR. 150 H/ LOC.150 H";
                    dt.Rows.Add(dr);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao buscar Opcao Tabela Salarial: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Busca Tabela Salarial de Colaboradores
        /// </summary>
        /// <param name="codigoTabela">Código da Tabela Salarial</param>
        /// <param name="coligada">Coligada</param>
        /// <param name="nivel">Classe</param>
        /// <returns></returns>
        public static DataTable GetTabelaSalarial(string codigoTabela, string coligada)
        {
            try
            {
                if (true)
                {
                    command = new SqlCommand("SELECT CODCOLIGADA, CODTABELA, NOMETABELA, NIVEL, FAIXA, CAST(SALARIO as decimal(10,2)) SALARIO FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODTABELA = '" + codigoTabela + "' AND CODCOLIGADA = '" + coligada + "' ORDER BY NIVEL,FAIXA", BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));
                    dt.Columns.Add("CODTABELA", Type.GetType("System.String"));
                    dt.Columns.Add("NOMETABELA", Type.GetType("System.String"));
                    dt.Columns.Add("NIVEL", Type.GetType("System.String"));
                    dt.Columns.Add("FAIXA", Type.GetType("System.String"));
                    dt.Columns.Add("SALARIO", Type.GetType("System.String"));


                    DataRow dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "A";
                    dr["SALARIO"] = "938.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "B";
                    dr["SALARIO"] = "996.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "C";
                    dr["SALARIO"] = "1054.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "D";
                    dr["SALARIO"] = "1111.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "E";
                    dr["SALARIO"] = "1169.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "F";
                    dr["SALARIO"] = "1228.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "G";
                    dr["SALARIO"] = "1288.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "H";
                    dr["SALARIO"] = "1345.00";
                    dt.Rows.Add(dr);


                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "01";
                    dr["FAIXA"] = "I";
                    dr["SALARIO"] = "1403.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "A";
                    dr["SALARIO"] = "938.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "B";
                    dr["SALARIO"] = "996.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "C";
                    dr["SALARIO"] = "1054.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "D";
                    dr["SALARIO"] = "1111.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "E";
                    dr["SALARIO"] = "1169.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "F";
                    dr["SALARIO"] = "1228.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "G";
                    dr["SALARIO"] = "1288.00";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "H";
                    dr["SALARIO"] = "1345.00";
                    dt.Rows.Add(dr);


                    dr = null;
                    dr = dt.NewRow();
                    dr["CODCOLIGADA"] = "1";
                    dr["CODTABELA"] = "01";
                    dr["NOMETABELA"] = "220H/210H/180H";
                    dr["NIVEL"] = "02";
                    dr["FAIXA"] = "I";
                    dr["SALARIO"] = "1403.00";
                    dt.Rows.Add(dr);
                    return dt;

                }
            }
            catch (Exception ex)
            {
                Logger.Write("Erro ao buscar Opcao Tabela Salarial: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }

        /// <summary>
        ///  Função que busca os dados do funcionário no banco de dados
        /// </summary>
        /// <param name="matricula">
        /// Matrícula do Funcionário
        /// </param>
        /// <returns>Tabela com todos os funcionarios</returns>
        public static DataTable GetCentroCusto(string matricula, string coligada)
        {
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO
                    string sql = string.Format(@"
                                                SELECT DISTINCT CCI.CODSECAO,CCI.DESCRICAO, CCI.CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
                                INNER JOIN LK_RM..RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE.CODSECAO
                                WHERE PCE.CODEXTERNO = '{0}' AND CODCOLSUBST = {1}
                                UNION
                                SELECT distinct C.CODSECAO, C.DESCRICAO, C.CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                LEFT JOIN LK_RM..RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C.CODSECAO
                                where C.CHAPASUBST = '{0}' AND C.CODCOLIGADA = {1}", matricula, coligada);

                    command = new SqlCommand(sql, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    //CODSECAO,DESCRICAO, CODCOLIGADA, CODSECAO, DESCRICAO, CODCOLIGADA
                    dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                    dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                    dt.Columns.Add("COD_DESC", Type.GetType("System.String"));
                    dt.Columns.Add("CODSECAO_ESTADO", Type.GetType("System.String"));
                    dt.Columns.Add("CODSECAO_ESTADO_COLIGADA", Type.GetType("System.String"));
                    dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CODSECAO"] = "01.202.107";
                    dr["DESCRICAO"] = "TESTE";
                    dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                    dr["CODSECAO_ESTADO"] = "01.202.107_RJ";
                    dr["CODSECAO_ESTADO_COLIGADA"] = "01.202.107_RJ_1";
                    dr["CODCOLIGADA"] = "1";
                    dt.Rows.Add(dr);

                    dr = null;
                    dr = dt.NewRow();
                    dr["CODSECAO"] = "01.202.106";
                    dr["DESCRICAO"] = "REMUNERAÇÃO";
                    dr["COD_DESC"] = dr["CODSECAO"] + " - " + dr["DESCRICAO"];
                    dr["CODSECAO_ESTADO"] = "01.202.106_RJ";
                    dr["CODSECAO_ESTADO_COLIGADA"] = "01.202.106_RJ_1";
                    dr["CODCOLIGADA"] = "1";
                    dt.Rows.Add(dr);
                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Recupera todos os centro de custos com funcionários ativos e inativos.
        /// </summary>
        /// <param name="matricula"></param>
        /// <param name="coligada"></param>
        /// <returns></returns>
        public static DataTable GetCentroCustoAtivos(string matricula, string coligada)
        {
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO
                    string sql = string.Format(@"
                                                SELECT DISTINCT CCI.CODSECAO,CCI.DESCRICAO, CCI.CODCOLIGADA, (CCI.CODSECAO + ' - ' + CCI.DESCRICAO) AS COD_DESC, (CCI.CODSECAO + '_' + CCI.ESTADO) AS CODSECAO_ESTADO, (CCI.CODSECAO + '_' + CCI.ESTADO + '_' + cast(CCI.CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP CCI
                                INNER JOIN LK_RM..RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE.CODSECAO
                                WHERE PCE.CODEXTERNO = '{0}' AND CODCOLSUBST = {1}
                                UNION
                                SELECT distinct C.CODSECAO, C.DESCRICAO, C.CODCOLIGADA, (C.CODSECAO + ' - ' + C.DESCRICAO) AS COD_DESC, (C.CODSECAO + '_' + C.ESTADO) AS CODSECAO_ESTADO, (C.CODSECAO + '_' + C.ESTADO + '_' + CAST(C.CODCOLIGADA as VARCHAR)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP C
                                LEFT JOIN LK_RM..RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C.CODSECAO
                                where C.CHAPASUBST = '{0}' AND C.CODCOLIGADA = {1}", matricula, coligada);

                    command = new SqlCommand(sql, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
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
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetCentroCustoAtivosD(string matricula, string coligada)
        {
            try
            {
                string sql = string.Format(@"SELECT DISTINCT CCI.CODSECAO,CCI.DESCRICAO, CCI.CODCOLIGADA, (CCI.CODSECAO + ' - ' + CCI.DESCRICAO) AS COD_DESC, (CCI.CODSECAO + '_' + CCI.ESTADO) AS CODSECAO_ESTADO, (CCI.CODSECAO + '_' + CCI.ESTADO + '_' + cast(CCI.CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA 
	                                            FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP CCI
	                                            WHERE CCI.CHAPASUBST = '{0}' AND CCI.CODCOLIGADA = {1}
                                            UNION SELECT DISTINCT C.CODSECAO, C.DESCRICAO, C.CODCOLIGADA, (C.CODSECAO + ' - ' + C.DESCRICAO) AS COD_DESC, (C.CODSECAO + '_' + C.ESTADO) AS CODSECAO_ESTADO, (C.CODSECAO + '_' + C.ESTADO + '_' + CAST(C.CODCOLIGADA as VARCHAR)) AS CODSECAO_ESTADO_COLIGADA 
	                                            FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP C
	                                            LEFT JOIN LK_RM..RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C.CODSECAO
	                                            WHERE C.CHAPASUBST = '{0}' AND C.CODCOLIGADA = {1}", matricula, coligada);

                    command = new SqlCommand(sql, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca todos centros de custos para o relatório de prêmios do comercial.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllCentroCustoToPremios()
        {
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO

                    string sql = @"SELECT * FROM OPENQUERY(LK_RM, '
	                                    SELECT DISTINCT 
		                                     CC.CODCOLIGADA
		                                    ,CC.DESCRICAO
		                                    ,CC.CODSECAO
		                                    ,CC.ESTADO
	                                    FROM 
		                                    INT_SHAREP_RM.VW_CCUSTO_INTRANET CC
	                                    INNER JOIN 
		                                    RM.PFUNC PF 
	                                    ON 
		                                    CC.CODCOLIGADA = PF.CODCOLIGADA
	                                    AND 
		                                    CC.CHAPA = PF.CHAPA 
	                                    INNER JOIN 
		                                    INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_GLOBOSAT RP 
	                                    ON 
		                                    PF.CODFUNCAO = RP.CODIGO
	                                    WHERE 
		                                    RP.CODIGO IN (''G000168'',''G001263'',''G001267'',''G001266'',''G001265'',''G001264'',''G000405'',''G000415'',''G000752'',''G000751'',''G001009'',''G000790'',''G000466'',''G000400'',''G000401'',''G000404'',''G001041'',''G001131'',''G000454'',''G000978'',''G000413'')
                                    ')";

                    using (command = new SqlCommand(sql, BaseDados.GetConnection()))
                    {
                        using (adapter = new SqlDataAdapter(command))
                        {
                            using (tableDados = new DataTable())
                            {
                                adapter.Fill(tableDados);
                                return tableDados;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                        dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["CODSECAO"] = "01.202.107";
                        dr["DESCRICAO"] = "TESTE";
                        dt.Rows.Add(dr);

                        dr = null;
                        dr = dt.NewRow();
                        dr["CODSECAO"] = "01.202.106";
                        dr["DESCRICAO"] = "REMUNERAÇÃO";
                        dt.Rows.Add(dr);
                        return dt;
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca os centros de custos para o relatório de prêmios do comercial.
        /// </summary>
        /// <param name="matricula"></param>
        /// <param name="codigoColigada"></param>
        /// <returns></returns>
        public static DataTable GetCentroCustoToPremios(string matricula, string codigoColigada)
        {
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO

                    string sql = string.Format(@"SELECT * FROM OPENQUERY(LK_RM, '
	                                    SELECT DISTINCT 
		                                    CC.CODCOLIGADA
		                                    ,CC.DESCRICAO
		                                    ,CC.CODSECAO
		                                    ,CC.ESTADO
		                                    ,CC.CHAPASUBST
	                                    FROM 
		                                    INT_SHAREP_RM.VW_CCUSTO_INTRANET CC
	                                    INNER JOIN 
		                                    RM.PFUNC PF 
	                                    ON 
		                                    CC.CODCOLIGADA = PF.CODCOLIGADA
	                                    AND 
		                                    CC.CHAPA = PF.CHAPA 
	                                    INNER JOIN 
		                                    INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_GLOBOSAT RP 
	                                    ON 
		                                    PF.CODFUNCAO = RP.CODIGO
	                                    WHERE 
		                                    CC.CHAPASUBST = ''{0}'' 
	                                    AND 
		                                    CC.CODCOLIGADA = {1}
	                                    AND 
		                                    RP.CODIGO IN (''G000168'',''G001263'',''G001267'',''G001266'',''G001265'',''G001264'',''G000405'',''G000415'',''G000752'',''G000751'',''G001009'',''G000790'',''G000466'',''G000400'',''G000401'',''G000404'',''G001041'',''G001131'',''G000454'',''G000978'',''G000413'')
                                    ')", matricula, codigoColigada);

                    using (command = new SqlCommand(sql, BaseDados.GetConnection()))
                    {
                        using (adapter = new SqlDataAdapter(command))
                        {
                            using (tableDados = new DataTable())
                            {
                                adapter.Fill(tableDados);
                                return tableDados;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                        dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["CODSECAO"] = "01.202.107";
                        dr["DESCRICAO"] = "TESTE";
                        dt.Rows.Add(dr);

                        dr = null;
                        dr = dt.NewRow();
                        dr["CODSECAO"] = "01.202.106";
                        dr["DESCRICAO"] = "REMUNERAÇÃO";
                        dt.Rows.Add(dr);
                        return dt;
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca todos os colaboradores do centro de custo
        /// </summary>
        /// <param name="centroCusto">Centro de custo</param>
        /// <returns>Data Table com os dados</returns>
        public static DataTable GetTodosColaboradores(string centroCusto, string coligada, string matricula)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    //command = new SqlCommand("SELECT distinct CHAPA, NOME, CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET where CODSECAO = '" + centroCusto.Trim() + "' AND CODCOLIGADA = '" + coligada + "' ORDER BY NOME", BaseDados.GetConnection());
                    command = new SqlCommand(string.Format(@"SELECT * FROM OPENQUERY (LK_RM , 'SELECT distinct CHAPA, NOME , CODCOLIGADA
                                                        FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET
                                                        where CODSECAO = ''{0}''  AND CODCOLIGADA in (
                                                        SELECT DISTINCT CCI .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
                                                        INNER JOIN RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE. CODSECAO
                                                        WHERE PCE. CODEXTERNO = ''{1}'' AND CODCOLSUBST = {2}
                                                        UNION
                                                        SELECT distinct C .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                                        LEFT JOIN RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C. CODSECAO
                                                        where C. CHAPASUBST = ''{1}'' AND C. CODCOLIGADA = {2}
                                                        ) ORDER BY NOME' );", centroCusto, matricula, coligada), BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));
                    dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00051";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO 1";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);

                    return dt;
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetTodosColaboradores(string centroCusto)
        {
            try
            {
                if (ambiente_producao)
                {
                    command = new SqlCommand(string.Format(@"SELECT * FROM OPENQUERY (LK_RM, 'SELECT distinct CHAPA, NOME, CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET where CODSECAO = ''{0}'' ORDER BY NOME');", centroCusto), BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));
                    dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00051";
                    dr["NOME"] = "FABIANO OLIVEIRA";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00052";
                    dr["NOME"] = "ANTONIO FERNANDEZ SOUZA";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00053";
                    dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetTodosColaboradoresToPremios(string centroCusto)
        {
            try
            {
                if (ambiente_producao)
                {
                    using (command = new SqlCommand(string.Format(@"SELECT DISTINCT CC.CHAPA, CC.NOME, CC.CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET CC
                                            INNER JOIN LK_RM..RM.PFUNC PF ON CC.CODCOLIGADA = PF.CODCOLIGADA
                                            AND CC.CHAPA = PF.CHAPA
                                            AND PF.CODFUNCAO IN ('G000168','G001263','G001267','G001266','G001265','G001264','G000405','G000415','G000752','G000751','G001009',
                                                'G000790','G000466','G000400','G000401','G000404','G001041','G001131','G000454','G000978','G000413')
                                            WHERE CC.CODSECAO = '{0}' 
                                            ORDER BY NOME", centroCusto), BaseDados.GetConnection()))
                    {
                        using (adapter = new SqlDataAdapter(command))
                        {
                            using (tableDados = new DataTable())
                            {
                                adapter.Fill(tableDados);
                                return tableDados;
                            }
                        }
                    }
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                        dt.Columns.Add("NOME", Type.GetType("System.String"));
                        dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["CHAPA"] = "00050";
                        dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);
                        dr = null;

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00051";
                        dr["NOME"] = "FABIANO OLIVEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00052";
                        dr["NOME"] = "ANTONIO FERNANDEZ SOUZA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        return dt;
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetTodosColaboradoresToPremiosIN(string centroCusto)
        {
            try
            {
                if (ambiente_producao)
                {
                    using (command = new SqlCommand(string.Format(@"SELECT DISTINCT CC.CHAPA, CC.NOME, CC.CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET CC
                                            INNER JOIN LK_RM..RM.PFUNC PF ON CC.CODCOLIGADA = PF.CODCOLIGADA
                                            AND CC.CHAPA = PF.CHAPA
                                            AND PF.CODFUNCAO IN ('G000168','G001263','G001267','G001266','G001265','G001264','G000405','G000415','G000752','G000751','G001009',
                                                'G000790','G000466','G000400','G000401','G000404','G001041','G001131','G000454','G000978','G000413')
                                            WHERE CC.CODSECAO IN ({0})
                                            ORDER BY NOME", centroCusto), BaseDados.GetConnection()))
                    {
                        using (adapter = new SqlDataAdapter(command))
                        {
                            using (tableDados = new DataTable())
                            {
                                adapter.Fill(tableDados);
                                return tableDados;
                            }
                        }
                    }
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                        dt.Columns.Add("NOME", Type.GetType("System.String"));
                        dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["CHAPA"] = "00050";
                        dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);
                        dr = null;

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00051";
                        dr["NOME"] = "FABIANO OLIVEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00052";
                        dr["NOME"] = "ANTONIO FERNANDEZ SOUZA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        dr = dt.NewRow();
                        dr["CHAPA"] = "00053";
                        dr["NOME"] = "RICARDO GOMEZ TEIXEIRA";
                        dr["CODCOLIGADA"] = "3";
                        dt.Rows.Add(dr);

                        return dt;
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca Historico Salarial do Colaborador
        /// </summary>
        /// <param name="matricula">Matricula do Colaborador</param>
        /// <param name="coligada">Coligada do Colaborador</param>
        /// <returns>Tabela com o histórico salarial do colaborador</returns>
        public static DataTable GetHistoricoSalarialAcordoColetivo(string matriculaFuncionario, string coligada)
        {
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    // command = new SqlCommand("SELECT H.DTMUDANCA Data, SALARIO Salário, DESCRICAO Motivo, F.NOME Cargo, CODNIVEL Classe, CODFAIXA Nível FROM LK_RM..INT_SHAREP_RM.VW_HISTORICO_SALARIAL H LEFT JOIN LK_RM..INT_SHAREP_RM.VW_HISTORICO_FUNCIONAL F ON CONVERT(VARCHAR,H.DTMUDANCA,103) = CONVERT(VARCHAR,F.DTMUDANCA,103) AND F.CHAPA = H.CHAPA AND H.CODCOLIGADA = F.CODCOLIGADA WHERE H.CHAPA = '" + matriculaFuncionario + "' AND H.CODCOLIGADA = '" + coligada + "' AND YEAR(GETDATE()) - year(H.DTMUDANCA) <= '10' ORDER BY H.DTMUDANCA", BaseDados.GetConnection());
                    command = new SqlCommand("SELECT * FROM OPENQUERY(LK_RM, 'SELECT H.DTMUDANCA Data, SALARIO Salário, DESCRICAO Motivo, F.NOME Cargo, CODNIVEL Classe, CODFAIXA Nível FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H LEFT JOIN INT_SHAREP_RM.VW_HISTORICO_FUNCIONAL F ON substr(H.DTMUDANCA,0,9) = substr(F.DTMUDANCA,0,9) AND F.CHAPA = H.CHAPA AND H.CODCOLIGADA = F.CODCOLIGADA WHERE H.CHAPA = ''" + matriculaFuncionario + "'' AND H.CODCOLIGADA = ''" + coligada + "'' AND EXTRACT(year from sysdate) - extract(year from H.DTMUDANCA) <= ''10'' ORDER BY H.DTMUDANCA')", BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);
                }
                else
                {
                    tableDados = new DataTable();
                    tableDados.Columns.Add("DATA", Type.GetType("System.String"));
                    tableDados.Columns.Add("SALÁRIO", Type.GetType("System.String"));
                    tableDados.Columns.Add("MOTIVO", Type.GetType("System.String"));
                    tableDados.Columns.Add("CARGO", Type.GetType("System.String"));
                    tableDados.Columns.Add("CLASSE", Type.GetType("System.String"));
                    tableDados.Columns.Add("NÍVEL", Type.GetType("System.String"));

                    DataRow row0 = tableDados.NewRow();
                    row0["DATA"] = "2013-04-17 00:00:00.0000000";
                    row0["SALÁRIO"] = "3496.00";
                    row0["MOTIVO"] = "ACORDO COLETIVO";
                    row0["CARGO"] = "AUXILIAR";
                    row0["CLASSE"] = "NULL";
                    row0["NÍVEL"] = "NULL";
                    tableDados.Rows.Add(row0);

                    DataRow row1 = tableDados.NewRow();
                    row1["DATA"] = "2013-05-01 00:10:00.0000000";
                    row1["SALÁRIO"] = "3854.00";
                    row1["MOTIVO"] = "ACORDO COLETIVO";
                    row1["CARGO"] = "AUXILIARI";
                    row1["CLASSE"] = "NULL";
                    row1["NÍVEL"] = "NULL";
                    tableDados.Rows.Add(row1);

                    DataRow row2 = tableDados.NewRow();
                    row2["DATA"] = "2013-06-06 00:10:00.0000000";
                    row2["SALÁRIO"] = "4200.00";
                    row2["MOTIVO"] = "NULL";
                    row2["CARGO"] = "ASSISTENTE";
                    row2["CLASSE"] = "NULL";
                    row2["NÍVEL"] = "NULL";
                    tableDados.Rows.Add(row2);

                    DataRow row3 = tableDados.NewRow();
                    row3["DATA"] = "2013-07-01 00:10:00.0000000";
                    row3["SALÁRIO"] = "4500.00";
                    row3["MOTIVO"] = "NULL";
                    row3["CARGO"] = "ASSSISTENTEII";
                    row3["CLASSE"] = "NULL";
                    row3["NÍVEL"] = "NULL";
                    tableDados.Rows.Add(row3);
                }

                return tableDados;

            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca nível salarial do Gestor
        /// </summary>
        /// <param name="codigoTabela">Código da Tabela</param>
        /// <param name="coligada">Coligada</param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable GetNivelTabelaSalarial(string codigoTabela, string coligada)
        {
            if (ambiente_producao)
            {
                command = new SqlCommand("SELECT distinct NIVEL FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODTABELA = '" + codigoTabela + "' AND CODCOLIGADA = '" + coligada + "'", BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            else
            {
                tableDados = new DataTable();
                tableDados.Columns.Add("NIVEL", Type.GetType("System.String"));

                DataRow row = tableDados.NewRow();
                row["NIVEL"] = "01";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "02";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "03";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "04";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "05";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "06";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "07";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "08";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "09";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "10";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "11";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "12";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "13";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "14";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "15";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "16";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "17";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "18";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "18";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "19";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "20";
                tableDados.Rows.Add(row);
                row = null;

                row = tableDados.NewRow();
                row["NIVEL"] = "21";
                tableDados.Rows.Add(row);
                row = null;
                return tableDados;


            }

        }

        /// <summary>
        /// Busca Classes da Tabela Salarial
        /// </summary>
        /// <param name="codigoTabela">Código da Tabela Salarial</param>
        /// <param name="coligada">Coligada</param>
        /// <returns>Data Table com as informações</returns>
        public static DataTable GetClasseTabelaSalarial(string codigoTabela, string coligada)
        {
            if (ambiente_producao)
            {
                command = new SqlCommand("SELECT distinct FAIXA FROM LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL WHERE CODTABELA = '" + codigoTabela + "' AND CODCOLIGADA = '" + coligada + "'", BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            else
            {
                tableDados = new DataTable();
                tableDados.Columns.Add("FAIXA", Type.GetType("System.String"));
                DataRow rowA = tableDados.NewRow();
                rowA["FAIXA"] = "A";
                tableDados.Rows.Add(rowA);

                DataRow rowB = tableDados.NewRow();
                rowB["FAIXA"] = "B";
                tableDados.Rows.Add(rowB);

                DataRow rowC = tableDados.NewRow();
                rowC["FAIXA"] = "C";
                tableDados.Rows.Add(rowC);

                DataRow rowD = tableDados.NewRow();
                rowD["FAIXA"] = "D";
                tableDados.Rows.Add(rowD);

                DataRow rowE = tableDados.NewRow();
                rowE["FAIXA"] = "E";
                tableDados.Rows.Add(rowE);

                DataRow rowF = tableDados.NewRow();
                rowF["FAIXA"] = "F";
                tableDados.Rows.Add(rowF);

                DataRow rowG = tableDados.NewRow();
                rowG["FAIXA"] = "G";
                tableDados.Rows.Add(rowG);

                DataRow rowH = tableDados.NewRow();
                rowH["FAIXA"] = "H";
                tableDados.Rows.Add(rowH);

                DataRow rowI = tableDados.NewRow();
                rowI["FAIXA"] = "I";
                tableDados.Rows.Add(rowI);

                return tableDados;

            }
        }

        /// <summary>
        /// Busca no banco todos os centros de custo disponíveis
        /// </summary>
        /// <returns>DataTable com as informações</returns>
        public static DataTable GetAllCentrosCusto()
        {
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, CODCOLIGADA, (CODSECAO + ' - ' + DESCRICAO) AS COD_DESC, (CODSECAO + '_' + ESTADO) AS CODSECAO_ESTADO, (CODSECAO + '_' + ESTADO + '_' + CAST(CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET ORDER BY DESCRICAO", BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
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
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar todos os Centros de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetAllCentrosCustoAtivos()
        {
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, CODCOLIGADA, (CODSECAO + ' - ' + DESCRICAO) AS COD_DESC, (CODSECAO + '_' + ESTADO) AS CODSECAO_ESTADO, (CODSECAO + '_' + ESTADO + '_' + CAST(CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP ORDER BY DESCRICAO", BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
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
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar todos os Centros de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetAllCentrosCustoAtivosD()
        {
            try
            {
                command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, CODCOLIGADA, (CODSECAO + ' - ' + DESCRICAO) AS COD_DESC, (CODSECAO + '_' + ESTADO) AS CODSECAO_ESTADO, (CODSECAO + '_' + ESTADO + '_' + CAST(CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP UNION SELECT DISTINCT CCI.CODIGO,CCI.DESCRICAO, CCI.CODCOLIGADA, (CCI.CODIGO + ' - ' + CCI.DESCRICAO) AS COD_DESC, (CCI.CODIGO + '_' + CCI.ESTADO) AS CODSECAO_ESTADO, (CCI.CODIGO + '_' + CCI.ESTADO + '_' + cast(CCI.CODCOLIGADA as varchar)) AS CODSECAO_ESTADO_COLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_ATIVO CCI WHERE CCI.CODIGO NOT IN (SELECT DISTINCT CODSECAO FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET_RP) ORDER BY DESCRICAO", BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar todos os Centros de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca no banco todos os centros de custo disponíveis, EXCETO a coligada 5 (G2C)
        /// </summary>
        /// <returns>DataTable com as informações</returns>
        public static DataTable GetAllCentrosCustoParaRV()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region Produção
                    command = new SqlCommand("SELECT distinct CODSECAO, DESCRICAO, CODCOLIGADA FROM LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET ORDER BY DESCRICAO", BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);
                    return tableDados;
                    #endregion
                }
                else
                {
                    #region Desenvolvimento
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CODSECAO", Type.GetType("System.String"));
                    dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                    dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CODSECAO"] = "3.201.100";
                    dr["DESCRICAO"] = "ADMINIST/FINANCEIRA";
                    dr["CODCOLIGADA"] = "3";
                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CODSECAO"] = "5.201.102";
                    dr["DESCRICAO"] = "ADMINIST/FINANCEIRA 2";
                    dr["CODCOLIGADA"] = "5";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar todos os Centros de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Busca nome de colaborador
        /// </summary>
        /// <param name="coligada">Coligada do colaborador</param>
        /// <param name="matricula">Matrícula do colaborador</param>
        /// <returns>Linha da tabela com a informação</returns>
        public static DataRow GetNomeColaborador(string coligada, string matricula)
        {
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    // A utilização da instrução OPENQUERY, faz com que a consulta seja executada com maior agilidade.
                    command = new SqlCommand(string.Format(@"SELECT * FROM OPENQUERY(LK_RM, 'SELECT  DISTINCT NOME_FUNC FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL WHERE CHAPA = ''{0}'' AND CODCOLIGADA = ''{1}''');", matricula, coligada), BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);
                    if (tableDados.Rows.Count == 1)
                    {
                        return tableDados.Rows[0];
                    }
                    return null;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("NOME_FUNC", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["NOME_FUNC"] = "FLAVIO JOSE FERREIRA VELASCO";

                    dt.Rows.Add(dr);
                    return dt.Rows[0];
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }

        }

        public static DataTable GetFolhaPagamento(string centroCusto, string coligada, string matricula)
        {
            try
            {
                if (ambiente_producao)
                {
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM, 'SELECT DISTINCT C.CHAPA,
                    C.NOME,
                    (SELECT FUNCAO FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM HF WHERE CHAPA = C.CHAPA AND CODCOLIGADA = C.CODCOLIGADA AND DTMUDANCA <= SYSDATE AND FUNCAO IS NOT NULL AND ROWNUM = 1) CARGO, 
                    P.CODNIVELSAL, P.GRUPOSALARIAL,
                        (SELECT MAX(SALARIO)
                                FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                        WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                                                   where H.CHAPA = C.CHAPA
                                                                   AND H.CODCOLIGADA = C.CODCOLIGADA)
                                                                   AND H.CHAPA = C.CHAPA
                                                                   AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO, P.DTBASE Admissao
                                                FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                                JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                                ON P.CHAPA = C.CHAPA
                                                AND P.CODCOLIGADA = C.CODCOLIGADA
                                                JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                                ON R.MATRICULA = P.CHAPA AND R.COLIGADA = P.CODCOLIGADA
                                               JOIN INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM HS
                                              ON HS.CHAPA = C.CHAPA AND HS.CODCOLIGADA = HS.CODCOLIGADA
                                                WHERE C.CODSECAO = ''{0}''
                                                AND C.CODCOLIGADA in (
                                                    SELECT DISTINCT CCI .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
                                                        INNER JOIN RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE. CODSECAO
                                                            WHERE PCE. CODEXTERNO <> ''{1}'' AND CODCOLSUBST = ''{2}''
                                                        UNION
                                                            SELECT distinct C .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                                             LEFT JOIN RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C. CODSECAO
                                                             where C. CHAPASUBST <> ''{1}'' AND C. CODCOLIGADA = ''{2}'') 
                                                                ORDER BY NOME'); ", centroCusto, matricula, coligada);

                    command = new SqlCommand(commandText, BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("NOME", Type.GetType("System.String"));
                        dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                        dt.Columns.Add("CARGO", Type.GetType("System.String"));
                        dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                        dt.Columns.Add("Admissao", Type.GetType("System.String"));
                        dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                        dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["NOME"] = "USUARIO DA SILVA PEREIRA GOMES CASTRO";
                        dr["SALARIO"] = "150000,00";
                        dr["CARGO"] = "Gerente de Canais";
                        dr["CHAPA"] = "25529";
                        dr["Admissao"] = "05/11/1986";
                        dr["CODNIVELSAL"] = "11";
                        dr["GRUPOSALARIAL"] = "C";
                        dt.Rows.Add(dr);
                        return dt;
                    }
                    #endregion

                }


            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
        }

        public static DataTable GetFolhaPagamento(string centroCusto, string ano)
        {
            try
            {
                if (true)
                {
                    #region Produção
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM,  
                                'SELECT DISTINCT C.CHAPA,  
                                C.NOME,  
                                (SELECT FUNCAO FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM HF WHERE CHAPA = C.CHAPA AND CODCOLIGADA = C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA) <= R.ANOCOMP AND FUNCAO IS NOT NULL AND ROWNUM = 1) CARGO,
       CASE (SELECT CODNIVEL FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM WHERE CHAPA=C.CHAPA AND CODCOLIGADA=C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA)<=R.ANOCOMP AND FUNCAO IS NOT NULL AND CODNIVEL IS NOT NULL AND CODFAIXA IS NOT NULL AND ROWNUM = 1)
            WHEN NULL 
               THEN (SELECT CODNIVEL FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM WHERE CHAPA=C.CHAPA AND CODCOLIGADA=C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA)<=R.ANOCOMP AND FUNCAO IS NOT NULL AND CODNIVEL IS NOT NULL AND CODFAIXA IS NOT NULL AND ROWNUM = 1)
               ELSE P.CODNIVELSAL 
       END as CODNIVELSAL,
        CASE (SELECT CODFAIXA FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM WHERE CHAPA=C.CHAPA AND CODCOLIGADA=C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA)<=R.ANOCOMP AND FUNCAO IS NOT NULL AND CODNIVEL IS NOT NULL AND CODFAIXA IS NOT NULL AND ROWNUM = 1)
            WHEN NULL 
               THEN (SELECT CODFAIXA FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM WHERE CHAPA=C.CHAPA AND CODCOLIGADA=C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA)<=R.ANOCOMP AND FUNCAO IS NOT NULL AND CODNIVEL IS NOT NULL AND CODFAIXA IS NOT NULL AND ROWNUM = 1)
               ELSE P.GRUPOSALARIAL
       END as GRUPOSALARIAL,
                                 (SELECT SALARIO FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H 
        WHERE H.CHAPA = C.CHAPA AND H.CODCOLIGADA = C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA)<=R.ANOCOMP AND ROWNUM = 1) SALARIO, 
                                    P.DTBASE Admissao  
                                    FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C  
                                    Join INT_SHAREP_RM.VW_PERFIL_INTRANET P  
                                    ON P.CHAPA = C.CHAPA AND P.CODCOLIGADA = C.CODCOLIGADA  
                                    JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                    ON R.MATRICULA = P.CHAPA AND R.COLIGADA = P.CODCOLIGADA
                                    WHERE C.CODSECAO = ''{0}'' 
                                    AND R.ANOCOMP = ''{1}''
                                    ORDER BY NOME');", centroCusto, ano);
                    #endregion

                    #region Desenvolvimento
                    //string commandText = "SELECT * FROM REMUNERACAOVARIAVEL";
                    #endregion

                    command = new SqlCommand(commandText, BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("NOME", Type.GetType("System.String"));
                        dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                        dt.Columns.Add("CARGO", Type.GetType("System.String"));
                        dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                        dt.Columns.Add("Admissao", Type.GetType("System.String"));
                        dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                        dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["NOME"] = "USUARIO DA SILVA PEREIRA GOMES CASTRO";
                        dr["SALARIO"] = "150000,00";
                        dr["CARGO"] = "Gerente de Canais";
                        dr["CHAPA"] = "25529";
                        dr["Admissao"] = "05/11/1986";
                        dr["CODNIVELSAL"] = "11";
                        dr["GRUPOSALARIAL"] = "C";
                        dt.Rows.Add(dr);
                        return dt;
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
        }

        #region VINICIUS
        public static DataTable GetFolhaPagamento(string centroCusto)
        {
            try
            {
                if (ambiente_producao)
                {
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM,  
                                'SELECT DISTINCT C.CHAPA, C.NOME, P.CARGO, P.CODNIVELSAL,                                 
(SELECT DISTINCT CODFAIXA FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM WHERE CHAPA=C.CHAPA AND CODCOLIGADA=HS.CODCOLIGADA AND CODFAIXA IS NOT NULL AND ROWNUM = 1) GRUPOSALARIAL,
(SELECT DISTINCT SALARIO  FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM WHERE CHAPA=C.CHAPA AND CODCOLIGADA=HS.CODCOLIGADA AND SALARIO IS NOT NULL AND ROWNUM = 1) SALARIO, 
                                    P.DTBASE Admissao  
                                    FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C  
                                    JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P  
                                    ON P.CHAPA = C.CHAPA AND P.CODCOLIGADA = C.CODCOLIGADA  
                                    JOIN INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM HS
                                    ON HS.CHAPA = C.CHAPA AND HS.CODCOLIGADA = C.CODCOLIGADA
                                    WHERE C.CODSECAO = ''{0}''                                
                                    ORDER BY NOME');", centroCusto);


                    command = new SqlCommand(commandText, BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    #region DESENVOLVIMENTO
                    using (DataTable dt = new DataTable())
                    {
                        dt.Columns.Add("NOME", Type.GetType("System.String"));
                        dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                        dt.Columns.Add("CARGO", Type.GetType("System.String"));
                        dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                        dt.Columns.Add("Admissao", Type.GetType("System.String"));
                        dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                        dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));

                        DataRow dr = dt.NewRow();
                        dr["NOME"] = "USUARIO DA SILVA PEREIRA GOMES CASTRO";
                        dr["SALARIO"] = "150000,00";
                        dr["CARGO"] = "Gerente de Canais";
                        dr["CHAPA"] = "25529";
                        dr["Admissao"] = "05/11/1986";
                        dr["CODNIVELSAL"] = "11";
                        dr["GRUPOSALARIAL"] = "C";
                        dt.Rows.Add(dr);
                        return dt;
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
        }
#endregion

        #region Dados de Remuneração Variável no Ano
        public static DataTable GetRemuneracaoVariavelAno(string centroCusto, string chapa, int? ano = null)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM ,
                                   'SELECT DISTINCT C.CHAPA,
                                C.NOME,
(SELECT FUNCAO FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM HF WHERE CHAPA = C.CHAPA AND CODCOLIGADA = C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA) <= ''{2}'' AND FUNCAO IS NOT NULL AND ROWNUM = 1) CARGO,
                                P.CODNIVELSAL, P.GRUPOSALARIAL,
                                      (SELECT MAX(SALARIO)
                                      FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                      WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                 FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                                                 WHERE H.CHAPA = C.CHAPA
                                                                 AND H.CODCOLIGADA = C.CODCOLIGADA)
                                         AND H.CHAPA = C.CHAPA
                                         AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                                   P.DTBASE ADMISSAO,
                                   R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = ''{2}''
                                   AND C.CHAPA = ''{1}''
                                   AND R.MESCOMP = 7
                    UNION
                                   SELECT DISTINCT C.CHAPA,
                                C.NOME,
(SELECT FUNCAO FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM HF WHERE CHAPA = C.CHAPA AND CODCOLIGADA = C.CODCOLIGADA AND EXTRACT(YEAR FROM DTMUDANCA) <= ''{2}'' AND FUNCAO IS NOT NULL AND ROWNUM = 1) CARGO,
                                P.CODNIVELSAL, P.GRUPOSALARIAL,
                                      (SELECT MAX(SALARIO)
                                      FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                      WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                 FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                                                 WHERE H.CHAPA = C.CHAPA
                                                                 AND H.CODCOLIGADA = C.CODCOLIGADA)
                                         AND H.CHAPA = C.CHAPA
                                         AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                                   P.DTBASE ADMISSAO,
                                   R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = CAST(''{2}'' AS INT) + 1
                                   AND C.CHAPA= ''{1}''
                                   AND R.MESCOMP IN (1,2)
                                   ORDER BY NOME,ANOCOMP,MESCOMP')",
                           centroCusto, chapa, ano);

                    command = new SqlCommand(commandText, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));
                    dt.Columns.Add("CARGO", Type.GetType("System.String"));
                    dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                    dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));
                    dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                    dt.Columns.Add("ADMISSAO", Type.GetType("System.String"));
                    dt.Columns.Add("VALOR", Type.GetType("System.String"));
                    dt.Columns.Add("MESCOMP", Type.GetType("System.String"));
                    dt.Columns.Add("ANOCOMP", Type.GetType("System.String"));
                    dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dr["CARGO"] = "COORDENADOR FINANCEIRO";
                    dr["CODNIVELSAL"] = "13";
                    dr["GRUPOSALARIAL"] = "B";
                    dr["SALARIO"] = "10619";
                    dr["ADMISSAO"] = "2003-05-15 00:00:00.0000000";
                    dr["VALOR"] = "2583.90";
                    dr["MESCOMP"] = "7";
                    dr["ANOCOMP"] = "2012";
                    dr["DESCRICAO"] = "PARTICIPE";
                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00051";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO 1";
                    dr["CARGO"] = "COORDENADOR FINANCEIRO 1";
                    dr["CODNIVELSAL"] = "131";
                    dr["GRUPOSALARIAL"] = "B1";
                    dr["SALARIO"] = "106191";
                    dr["ADMISSAO"] = "2003-05-15 00:00:00.0000000";
                    dr["VALOR"] = "2583.90";
                    dr["MESCOMP"] = "7";
                    dr["ANOCOMP"] = "2012";
                    dr["DESCRICAO"] = "PARTICIPE";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetRemuneracaoVariavelAno(string centroCusto, string chapa)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM ,
                                   'SELECT DISTINCT C.CHAPA,
                                C.NOME,
                                P.CARGO, P.CODNIVELSAL, P.GRUPOSALARIAL,
                                      (SELECT MAX(SALARIO)
                                      FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H
                                      WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                 FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H
                                                                 WHERE H.CHAPA = C.CHAPA
                                                                 AND H.CODCOLIGADA = C.CODCOLIGADA)
                                         AND H.CHAPA = C.CHAPA
                                         AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                                   P.DTBASE ADMISSAO,
                                   R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND C.CHAPA = ''{1}''
                                   AND R.MESCOMP = 7
                    UNION
                                   SELECT DISTINCT C.CHAPA,
                                C.NOME,
                                P.CARGO, P.CODNIVELSAL, P.GRUPOSALARIAL,
                                      (SELECT MAX(SALARIO)
                                      FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H
                                      WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                 FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H
                                                                 WHERE H.CHAPA = C.CHAPA
                                                                 AND H.CODCOLIGADA = C.CODCOLIGADA)
                                         AND H.CHAPA = C.CHAPA
                                         AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                                   P.DTBASE ADMISSAO,
                                   R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND C.CHAPA= ''{1}''
                                   AND R.MESCOMP IN (1,2)
                                   ORDER BY NOME,ANOCOMP,MESCOMP')",
                           centroCusto, chapa);

                    command = new SqlCommand(commandText, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));
                    dt.Columns.Add("CARGO", Type.GetType("System.String"));
                    dt.Columns.Add("CODNIVELSAL", Type.GetType("System.String"));
                    dt.Columns.Add("GRUPOSALARIAL", Type.GetType("System.String"));
                    dt.Columns.Add("SALARIO", Type.GetType("System.String"));
                    dt.Columns.Add("ADMISSAO", Type.GetType("System.String"));
                    dt.Columns.Add("VALOR", Type.GetType("System.String"));
                    dt.Columns.Add("MESCOMP", Type.GetType("System.String"));
                    dt.Columns.Add("ANOCOMP", Type.GetType("System.String"));
                    dt.Columns.Add("DESCRICAO", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dr["CARGO"] = "COORDENADOR FINANCEIRO";
                    dr["CODNIVELSAL"] = "13";
                    dr["GRUPOSALARIAL"] = "B";
                    dr["SALARIO"] = "10619";
                    dr["ADMISSAO"] = "2003-05-15 00:00:00.0000000";
                    dr["VALOR"] = "2583.90";
                    dr["MESCOMP"] = "7";
                    dr["ANOCOMP"] = "2012";
                    dr["DESCRICAO"] = "PARTICIPE";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetRemuneracaoVariavelAno(string centroCusto)
        {
            try
            {

                string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM ,
                                   'SELECT DISTINCT C.CHAPA,
                                C.NOME,
                                P.FUNCAO, P.CODNIVEL, P.CODFAIXA,
                                      (SELECT MAX(SALARIO)
                                      FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                      WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                 FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H
                                                                 WHERE H.CHAPA = C.CHAPA
                                                                 AND H.CODCOLIGADA = C.CODCOLIGADA)
                                         AND H.CHAPA = C.CHAPA
                                         AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                                   P.DTMUDANCA ADMISSAO,
                                   R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)-1
                                   AND R.MESCOMP = 7
                    UNION
                                   SELECT DISTINCT C.CHAPA,
                                C.NOME,
                                P.FUNCAO, P.CODNIVEL, P.CODFAIXA,
                                      (SELECT MAX(SALARIO)
                                      FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                                      WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                                                                 FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM H
                                                                 WHERE H.CHAPA = C.CHAPA
                                                                 AND H.CODCOLIGADA = C.CODCOLIGADA)
                                         AND H.CHAPA = C.CHAPA
                                         AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                                   P.DTMUDANCA ADMISSAO,
                                   R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_HISTORICO_SALARIAL_REM P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)
                                   AND R.MESCOMP IN (1,2)
                                   ORDER BY NOME,ANOCOMP,MESCOMP')",
                       centroCusto);

                //@"SELECT * FROM OPENQUERY(LK_RM , 'SELECT DISTINCT C.CHAPA,
                //                    C.NOME,
                //                    P.CARGO, P.CODNIVELSAL, P.GRUPOSALARIAL,
                //                       (SELECT MAX(SALARIO)
                //                                FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                //                                        WHERE DTMUDANCA = (SELECT MAX(DTMUDANCA)
                //                                                                FROM INT_SHAREP_RM.VW_HISTORICO_SALARIAL H
                //                                                                        where H.CHAPA = C.CHAPA
                //                                                                        AND H.CODCOLIGADA = C.CODCOLIGADA)
                //                                        AND H.CHAPA = C.CHAPA
                //                                        AND H.CODCOLIGADA = C.CODCOLIGADA) SALARIO,
                //                        P.DTBASE Admissao,
                //                        R.VALOR, R.MESCOMP, R.ANOCOMP,  R.DESCRICAO
                //                        FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                //                        Join INT_SHAREP_RM.VW_PERFIL_INTRANET P
                //                        ON P.CHAPA = C.CHAPA
                //                        AND P.CODCOLIGADA = C.CODCOLIGADA
                //                        Join INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                //                        ON R.MATRICULA = C.CHAPA
                //                        AND R.COLIGADA = C.CODCOLIGADA
                //                                    where C.CODSECAO = ''" + centroCusto + @"''
                //                                    AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)
                //                                    ORDER BY NOME' )";

                #region Desenvolvimento
                //commandText = "SELECT * FROM REMUNERACAOVARIAVEL";
                #endregion


                command = new SqlCommand(commandText, BaseDados.GetConnection());

                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar nome de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
        }

        internal static DataTable GetMatriculaRemuneracaoVariavelAno(string centroCusto)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM ,
                                   'SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)-1
                                   AND R.MESCOMP = 7
                    UNION
                                   SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)
                                   AND R.MESCOMP = 1
                                   ORDER BY NOME')",
                           centroCusto);

                    command = new SqlCommand(commandText, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00051";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO 1";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar matricula de colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        internal static DataTable GetMatriculaRemuneracaoVariavelAno(string centroCusto, int ano)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM ,
                                   'SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = ''{1}''
                                   AND R.MESCOMP = 7
                    UNION
                                   SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = CAST({1} AS INT) + 1
                                   AND R.MESCOMP = 1
                                   ORDER BY NOME')",
                           centroCusto, ano);

                    command = new SqlCommand(commandText, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00051";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO 1";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar matricula de colaboradores: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        internal static DataTable GetMatriculaRemuneracaoVariavelAno(string centroCusto, string coligada, string matricula)
        {
            try
            {
                string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM , 'SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)-1
                                   AND R.MESCOMP = 7
                                   AND C.CODCOLIGADA in (
SELECT DISTINCT CCI .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
INNER JOIN RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE. CODSECAO
WHERE PCE. CODEXTERNO = ''{1}'' AND CODCOLSUBST = {2}
UNION
SELECT distinct C .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
LEFT JOIN RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C. CODSECAO
where C. CHAPASUBST = ''{1}'' AND C. CODCOLIGADA = {2}
)
                    UNION
                                   SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = EXTRACT(YEAR FROM SYSDATE)
                                   AND R.MESCOMP = 1
                                   AND C.CODCOLIGADA in (
SELECT DISTINCT CCI .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
INNER JOIN RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE. CODSECAO
WHERE PCE. CODEXTERNO = ''{1}'' AND CODCOLSUBST = {2}
UNION
SELECT distinct C .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
LEFT JOIN RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C. CODSECAO
where C. CHAPASUBST = ''{1}'' AND C. CODCOLIGADA = {2}
) ORDER BY NOME')", centroCusto, matricula, coligada);

                command = new SqlCommand(commandText, BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar matricula de colaboradores (com coligada): " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

            }

        }

        internal static DataTable GetMatriculaRemuneracaoVariavelAno(string centroCusto, string coligada, string matricula, int ano)
        {
            try
            {
                string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM , 'SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = ''{3}''
                                   AND R.MESCOMP = 7
                                   AND C.CODCOLIGADA in (
SELECT DISTINCT CCI .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
INNER JOIN RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE. CODSECAO
WHERE PCE. CODEXTERNO = ''{1}'' AND CODCOLSUBST = {2}
UNION
SELECT distinct C .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
LEFT JOIN RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C. CODSECAO
where C. CHAPASUBST = ''{1}'' AND C. CODCOLIGADA = {2}
)
                    UNION
                                   SELECT DISTINCT C.CHAPA CHAPA, C.NOME NOME
                                   FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
                                   JOIN INT_SHAREP_RM.VW_PERFIL_INTRANET P
                                   ON P.CHAPA = C.CHAPA
                                   AND P.CODCOLIGADA = C.CODCOLIGADA
                                   JOIN INT_SHAREP_RM.VW_REMUNERACAO_VARIAVEL R
                                   ON R.MATRICULA = C.CHAPA
                                   AND R.COLIGADA = C.CODCOLIGADA
                                   WHERE C.CODSECAO = ''{0}''
                                   AND R.ANOCOMP = CAST({3} AS INT) + 1
                                   AND R.MESCOMP = 1
                                   AND C.CODCOLIGADA in (
SELECT DISTINCT CCI .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET CCI
INNER JOIN RM.PCHEFEEXTERNO PCE ON CCI.CODSECAO = PCE. CODSECAO
WHERE PCE. CODEXTERNO = ''{1}'' AND CODCOLSUBST = {2}
UNION
SELECT distinct C .CODCOLIGADA FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET C
LEFT JOIN RM.PCHEFEEXTERNO P ON P.CODEXTERNO = C. CODSECAO
where C. CHAPASUBST = ''{1}'' AND C. CODCOLIGADA = {2}
) ORDER BY NOME')", centroCusto, matricula, coligada, ano);

                command = new SqlCommand(commandText, BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar matricula de colaboradores (com coligada): " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }

        }

        public static DataTable GetSalarioColaborador(string matricula, string coligada, int? ano = null)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    string dtMudanca = "2012-12-31"; // Valor padrão é referente ao ano de 2012.
                    //MUDANÇA DA DATA PARA QUERY COM INFORMAÇÕES DO SALARIO DO FUNCIONARIO VINICIUS_27/06/2014
                    if ((ano.HasValue) && (ano == 2012))
                    {
                        dtMudanca = "2012-12-31";
                    }
                    if ((ano.HasValue) && (ano == 2013))
                    {
                        dtMudanca = "2013-12-30";
                    }
                    if ((ano.HasValue) && (ano == 2014))
                    {
                        dtMudanca = "2014-12-31";
                    }
                     string commandText = string.Format(@"SELECT * FROM OPENQUERY(LK_RM, 
                                                        'SELECT MAX(SALARIO) SALARIO FROM 
                                                            INT_SHAREP_RM.VW_HISTORICO_SALARIAL 
                                                            WHERE CHAPA = ''{0}'' 
                                                           AND CODCOLIGADA = ''{1}'' 
                                                            AND DTMUDANCA <= ''" + dtMudanca + "''')", matricula, coligada);

                    command = new SqlCommand(commandText, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    DataTable dt = new DataTable();
                    dt.Columns.Add("SALARIO", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["SALARIO"] = "7555";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar matricula de colaboradores (com coligada): " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }
        #endregion

        public static DataTable GetLoginColaborador(string matricula, string coligada, SqlConnection conn)
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]))
                {
                    #region PRODUCAO
                    string commandText = "SELECT UPF1.NTNAME AS LOGIN, UPV1.PROPERTYVAL AS MATRICULA " +
                                                        "FROM USERPROFILE_FULL UPF1 " +
                                                        "INNER JOIN USERPROFILEVALUE UPV1 ON UPV1.RECORDID = UPF1.RECORDID " +
                                                        "INNER JOIN PROPERTYLIST PL1 ON PL1.PROPERTYID = UPV1.PROPERTYID " +
                                                        "AND PL1.PROPERTYNAME = 'MATRICULA' " +
                                                        "INNER JOIN USERPROFILE_FULL UPF2 ON UPF2.RECORDID = UPF1.RECORDID " +
                                                        "INNER JOIN USERPROFILEVALUE UPV2 ON UPV2.RECORDID = UPF2.RECORDID " +
                                                        "INNER JOIN PROPERTYLIST PL2 ON PL2.PROPERTYID = UPV2.PROPERTYID " +
                                                        "AND PL2.PROPERTYNAME = 'COLIGADA' " +
                                                        "WHERE UPV1.PROPERTYVAL = '" + matricula + "'" +
                                                        "AND UPV2.PROPERTYVAL = '" + coligada + "'";

                    command = new SqlCommand(commandText, conn);

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                    #endregion
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("LOGIN", Type.GetType("System.String"));
                    dt.Columns.Add("MATRICULA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["LOGIN"] = "CIT\\bcontarini";
                    dr["MATRICULA"] = "25529";
                    dt.Rows.Add(dr);

                    return dt;
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar login de colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                return null;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetDadosSimulador(string matricula, string coligada)
        {
            try
            {
                int mes = DateTime.Now.AddMonths(-1).Month;
                int ano = DateTime.Now.AddMonths(-1).Year;

                #region Produção
                command = new SqlCommand(string.Format(@"SELECT * FROM
OPENQUERY (LK_RM , 'SELECT VALOR,
CASE
WHEN (CODEVENTO = ''0001'')
THEN ''SALARIOBASE''
WHEN (CODEVENTO = ''1096'' OR CODEVENTO = ''1098'')
THEN ''PENSEPREV''
else ''?''
END TIPO
 FROM INT_SHAREP_RM.VW_CONTRACHEQUE
    WHERE CODEVENTO IN (''0001'',''1096'',''1098'')
    AND ANOCOMP = {2}
    AND MESCOMP = {3}
    AND CHAPA = ''{0}''
    AND CODCOLIGADA = {1}
   
    UNION
    (SELECT (VALORMENSALDEFÉRIAS + VALORMENSALDE13SALARIO) VALOR, ''FERIAS13MENSAL'' TIPO
FROM INT_SHAREP_RM.VW_SIMULADOR
    WHERE
     ANO = {2}
     AND MES = {3}
    AND CHAPA =''{0}''
     AND CODCOLIGADA = {1}

UNION

     SELECT (VALORANUALDEFÉRIAS + VALORANUALDE13SALARIO) VALOR, ''FERIAS13ANUAL'' TIPO
     FROM INT_SHAREP_RM.VW_SIMULADOR
    WHERE
     ANO = {2}
     AND MES = {3}
    AND CHAPA =''{0}''
     AND CODCOLIGADA = {1})
    
     UNION
    
     (SELECT count(chapa) DEPENDENTES, ''DEPENDENTES'' TIPO
FROM INT_SHAREP_RM.VW_DEPENDENTES_FUNC
     WHERE
    CHAPA = ''{0}''
     AND CODCOLIGADA = {1})
    ORDER BY TIPO DESC' )", matricula, coligada, ano, mes), BaseDados.GetConnection());
                #endregion

                #region Desenvolvimento
                //command = new SqlCommand("SELECT * FROM SIMULADOR_REMUNERACAO", BaseDados.GetConnection());
                #endregion

                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar os dados do simulador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        public static DataTable GetFuncionariosEvolucaoSalarial(string matricula, string coligada)
        {
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_PRODUCAO"]))
                {
                    command = new SqlCommand(string.Format(@"SELECT * FROM OPENQUERY(LK_RM , 'SELECT DISTINCT CHAPA, NOME, CODCOLIGADA
                        FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET WHERE CODSECAO = (SELECT DISTINCT CODSECAO
                        FROM INT_SHAREP_RM.VW_CCUSTO_INTRANET WHERE CHAPA=''{0}'' AND CODCOLIGADA=''{1}'' ) ORDER BY NOME');", matricula.Trim(), coligada.Trim()), BaseDados.GetConnection());

                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
                }
                else
                {
                    #region DESENVOLVIMENTO

                    DataTable dt = new DataTable();
                    dt.Columns.Add("CHAPA", Type.GetType("System.String"));
                    dt.Columns.Add("NOME", Type.GetType("System.String"));
                    dt.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                    DataRow dr = dt.NewRow();
                    dr["CHAPA"] = "00050";
                    dr["NOME"] = "FLAVIO JOSE FERREIRA VELASCO";
                    dr["CODCOLIGADA"] = "3";

                    dt.Rows.Add(dr);
                    dr = null;

                    dr = dt.NewRow();
                    dr["CHAPA"] = "00051";
                    dr["NOME"] = "ANTONIO TECONOLOGICO";
                    dt.Rows.Add(dr);

                    return dt;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Colaboradores em Evolução Salarial para preenchimento do DropDownList: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Buscar cargo pelo centro de custo para utilização do pagina custom field type CCustoCargo
        /// </summary>
        /// <param name="strCargo"></param>
        /// <returns></returns>
        public static DataTable GetCargo(string codCentroCusto)
        {
            try
            {
                string sql = string.Format(@"
                                                SELECT DISTINCT P.CODCOLIGADA AS CODCOLIGADA, P.CODSECAO AS CODCENTROCUSTO, P.CODFUNCAO AS CODFUNCAO, P.CODFILIAL AS CODFILIAL, F.NOME
                                                    FROM [LK_RM]..[RM].[PFUNC] P
                                                        INNER JOIN [LK_RM]..[RM].[PFUNCAO] F ON P.CODFUNCAO = F.CODIGO
                                                        INNER JOIN [LK_RM]..[INT_SHAREP_RM].[VW_CCUSTO_ATIVO] C ON P.CODSECAO = C.CODIGO
                                                    WHERE F.INATIVA='0' AND P.CODSECAO = '{0}'
                                                    ORDER BY NOME", codCentroCusto);

                    command = new SqlCommand(sql, BaseDados.GetConnection());
                    adapter = new SqlDataAdapter(command);
                    tableDados = new DataTable();
                    adapter.Fill(tableDados);

                    return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Cargo do Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Buscar cargo pelo centro de custo e codigo do cargo para utilização do pagina custom field type CCustoCargo - Cargo
        /// </summary>
        /// <param name="strCargo"></param>
        /// <returns></returns>
        public static DataTable GetCargo(string codCentroCusto, string codCargo)
        {
            try
            {
                string sql = string.Format(@"
                                                SELECT DISTINCT P.CODCOLIGADA AS CODCOLIGADA, P.CODSECAO AS CODCENTROCUSTO, P.CODFUNCAO AS CODFUNCAO, P.CODFILIAL AS CODFILIAL, F.NOME
                                                    FROM [LK_RM]..[RM].[PFUNC] P
                                                        INNER JOIN [LK_RM]..[RM].[PFUNCAO] F ON P.CODFUNCAO = F.CODIGO
                                                        INNER JOIN [LK_RM]..[INT_SHAREP_RM].[VW_CCUSTO_ATIVO] C ON P.CODSECAO = C.CODIGO
                                                    WHERE F.INATIVA='0' AND P.CODSECAO = '{0}' AND P.CODFUNCAO = '{1}'
                                                    ORDER BY NOME", codCentroCusto, codCargo);

                command = new SqlCommand(sql, BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar Cargo do Centro de Custo: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }

        /// <summary>
        /// Buscar cargo para utilização do formulário de requisição de pessoal
        /// </summary>
        /// <param name="strCargo"></param>
        /// <returns></returns>
        public static DataTable GetCargo(string strCargo, int codColigada, string nivel)
        {
            try
            {
                if (codColigada.Equals(2))//JV
                {
                    command = new SqlCommand(string.Format(@"SELECT * FROM
                    OPENQUERY (LK_RM , 'SELECT CODCOLIGADA, NOME, JORNADA, NIVEL, FAIXA, SALARIO 
                    FROM INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_JV WHERE NOME LIKE ''{0}%'' AND FAIXA = ''A''
                    AND (CAST(NIVEL AS INT)) <= (CAST(''{1}'' AS INT))')", strCargo, nivel), BaseDados.GetConnection());
                }
                else
                {
                    command = new SqlCommand(string.Format(@"SELECT * FROM
                    OPENQUERY (LK_RM , 'SELECT CODCOLIGADA, NOME, JORNADA, NIVEL, FAIXA, SALARIO 
                    FROM INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_GLOBOSAT WHERE NOME LIKE ''{0}%'' AND FAIXA = ''A''
                    AND (CAST(NIVEL AS INT)) <= (CAST(''{1}'' AS INT))')", strCargo, nivel), BaseDados.GetConnection());
                }

                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar cargos para o Formulário de Requisição de Pessoal: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        public static DataTable GetCargoRequisicaoPessoal(string strCargo, int codColigada, string nivel, string tipo, int codFilial)
        {
            try
            {
                string query = string.Empty;
                if ((codColigada == 1) || (codColigada == 2)) // Globosat ou Telecine
                {
                    if (codColigada == 2) // Telecine
                    {
                        if (codFilial == 2)
                            codFilial = 3;
                    }                  

                    query = "   SELECT" +
                            "       (CODIGO + ' - ' + NOME) AS CODNOME" +
                            "       ,CODCOLIGADA" +
                            "       ,NOME" +
                            "       ,JORNADA" +
                            "       ,NIVEL" +
                            "       ,FAIXA" +
                            "       ,SALARIO " +
                            "   FROM" +
                            "       LK_RM..INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_GLOBOSAT" +
                            "   WHERE" +
                            "       NOME LIKE '" + strCargo + "%'" +
                            "   AND" +
                            "       FAIXA = 'A'" +
                            "   AND" +
                            "       NIVEL <> '00'" +
                            "   AND" +
                            "       CODFILIAL = '" + codFilial + "'" +
                            "   AND" +
                            "       (CAST(NIVEL AS INT)) <= (CAST('" + nivel + "' AS INT))" +
                            "   AND" +
                            "       JORNADA IS NOT NULL" +
                            "   AND" +
                            "       INATIVA = 0" +
                            "   AND" +
                            "       CODCOLIGADA = " + codColigada;
                }
                else // Joint Ventures
                {
                    query = "   SELECT" +
                            "       (CODIGO + ' - ' + NOME) AS CODNOME" +
                            "       ,CODCOLIGADA" +
                            "       ,NOME" +
                            "       ,JORNADA" +
                            "       ,NIVEL" +
                            "       ,FAIXA" +
                            "       ,SALARIO " +
                            "   FROM" +
                            "       LK_RM..INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_JV" +
                            "   WHERE" +
                            "       NOME LIKE '" + strCargo + "%'" +
                            "   AND" +
                            "       FAIXA = 'A'" +
                            "   AND" +
                            "       NIVEL <> '00'" +
                            "   AND" +
                            "       CODFILIAL = '" + codFilial + "'" +
                            "   AND" +
                            "       (CAST(NIVEL AS INT)) <= (CAST('" + nivel + "' AS INT))" +
                            "   AND" +
                            "       JORNADA IS NOT NULL" +
                            "   AND" +
                            "       INATIVA = 0" +
                            "   AND" +
                            "       CODCOLIGADA = " + codColigada;
                }

                command = new SqlCommand(query, BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                return tableDados;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar cargos para o Formulário de Requisição de Pessoal: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        public static int GetJornada(string cargo, string faixa, string nivel, int codFilial, int codColigada)
        {
            try
            {
                string query = string.Empty;
                if ((codColigada == 1) || (codColigada == 2)) // Globosat ou  Telecine
                {
                    if (codColigada == 2) // Telecine
                    {
                        if (codFilial == 2)
                            codFilial = 3;
                    }                    

                    query = "  SELECT" +
                            "       ISNULL(JORNADA, 0) AS JORNADA" +
                            "   FROM" +
                            "       OPENQUERY (LK_RM ," +
                            "                           'SELECT" +
                            "                               JORNADA" +
                            "                           FROM" +
                            "                               INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_GLOBOSAT" +
                            "                           WHERE" +
                            "                               NOME LIKE ''" + cargo + "%''" +
                            "                           AND" +
                            "                               FAIXA = ''" + faixa + "''" +
                            "                           AND" +
                            "                               NIVEL = ''" + nivel + "''" +
                            "                           AND" +
                            "                               CODFILIAL = ''" + codFilial + "''" +
                            "       ')";
                }
                else // Joint Ventures
                {
                    query = "  SELECT" +
                         "       ISNULL(JORNADA, 0) AS JORNADA" +
                         "   FROM" +
                         "       OPENQUERY (LK_RM ," +
                         "                           'SELECT" +
                         "                               JORNADA" +
                         "                           FROM" +
                         "                               INT_SHAREP_RM.VW_REQUISICAO_PESSOAL_JV" +
                         "                           WHERE" +
                         "                               NOME LIKE ''" + cargo + "%''" +
                         "                           AND" +
                         "                               FAIXA = ''" + faixa + "''" +
                         "                           AND" +
                         "                               NIVEL = ''" + nivel + "''" +
                         "                           AND" +
                         "                               CODFILIAL = ''" + codFilial + "''" +
                         "       ')";
                }

                command = new SqlCommand(query, BaseDados.GetConnection());
                adapter = new SqlDataAdapter(command);
                tableDados = new DataTable();
                adapter.Fill(tableDados);

                if (tableDados.Rows.Count > 0)
                {
                    return Convert.ToInt32(tableDados.Rows[0][0].ToString());
                }

                return 0;
            }
            catch (SqlException ex)
            {
                Logger.Write("Erro ao buscar cargos para o Formulário de Requisição de Pessoal: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        public static DataRow GetNivelColaborador(string matricula, string coligada)
        {
            try
            {
                command = new SqlCommand(string.Format(@"SELECT * FROM OPENQUERY(LK_RM , 'SELECT CODNIVELSAL AS NIVEL FROM INT_SHAREP_RM.VW_PERFIL_INTRANET
                        WHERE CHAPA = ''{0}''
                        AND CODCOLIGADA = {1}')", matricula, coligada), BaseDados.GetConnection());

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
                Logger.Write("Erro ao buscar o nível do colaborador: " + ex.Message + " " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        public static DataTable GetDadosFuncionario(int codigoColigada, string matricula)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringProducaoRM"].ConnectionString))
            {
                using (SqlCommand sqlCommandFunc = new SqlCommand())
                {
                    sqlCommandFunc.Connection = sqlConnection;
                    sqlCommandFunc.CommandText = "RetornaDadosFuncionarioComSalario";
                    sqlCommandFunc.CommandType = CommandType.StoredProcedure;
                    sqlCommandFunc.Parameters.Add("CODIGOCOLIGADA", SqlDbType.VarChar);
                    sqlCommandFunc.Parameters.Add("MATRICULAFUNCIONARIO", SqlDbType.VarChar);

                    sqlCommandFunc.Parameters["CODIGOCOLIGADA"].Value = codigoColigada;
                    sqlCommandFunc.Parameters["MATRICULAFUNCIONARIO"].Value = matricula;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommandFunc);
                    sqlDataAdapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public static DataTable GetPremios(int codigoColigada, string matricula)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringProducaoRM"].ConnectionString))
            {
                using (SqlCommand sqlCommandPremios = new SqlCommand())
                {
                    sqlCommandPremios.Connection = sqlConnection;
                    sqlCommandPremios.CommandText = "RetornaPremiosFuncionario";
                    sqlCommandPremios.CommandType = CommandType.StoredProcedure;
                    sqlCommandPremios.Parameters.Add("CODIGOCOLIGADA", SqlDbType.VarChar);
                    sqlCommandPremios.Parameters.Add("MATRICULAFUNCIONARIO", SqlDbType.VarChar);

                    sqlCommandPremios.Parameters["CODIGOCOLIGADA"].Value = codigoColigada;
                    sqlCommandPremios.Parameters["MATRICULAFUNCIONARIO"].Value = matricula;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommandPremios);
                    sqlDataAdapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public static DataTable GetPremios(string centroCusto, string idFuncionarios)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringProducaoRM"].ConnectionString))
            {
                using (SqlCommand sqlCommandPremios = new SqlCommand())
                {
                    idFuncionarios = string.Format("'{0}'", idFuncionarios).Replace(",","','");
                    sqlCommandPremios.Connection = sqlConnection;
                    sqlCommandPremios.CommandText = "RetornaFuncionarioPremios";
                    sqlCommandPremios.CommandType = CommandType.StoredProcedure;
                    sqlCommandPremios.Parameters.Add("CENTROCUSTO", SqlDbType.NVarChar);
                    sqlCommandPremios.Parameters.Add("FUNCIONARIOS", SqlDbType.NVarChar);

                    sqlCommandPremios.Parameters["CENTROCUSTO"].Value = centroCusto;
                    sqlCommandPremios.Parameters["FUNCIONARIOS"].Value = idFuncionarios;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommandPremios);
                    sqlDataAdapter.Fill(dataTable);
                    
                }
            }
            return dataTable;
        }

        }
}
