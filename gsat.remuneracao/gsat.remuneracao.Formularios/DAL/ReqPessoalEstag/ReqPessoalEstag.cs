using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using Cit.Globosat.Common;
using System.Configuration;
using System.Data.SqlClient;
using Cit.Globosat.Remuneracao.Formularios.DAL.AltFuncCargo;
using System.Globalization;

namespace Cit.Globosat.Remuneracao.Formularios.DAL.ReqPessoalEstag
{
    public class ReqPessoalEstag
    {
        private static bool ambiente_producao = Convert.ToBoolean(ConfigurationManager.AppSettings["ambiente_producao"]);

        public static DataTable GetDados(string centroCusto, int codigoColigada)
        {
            SqlCommand sqlCommand = null;
            SqlDataAdapter sqlDataAdapter = null;
            DataTable dataTable = null;
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO

                    string query = "SELECT TOP 1" +
                                    "   R.CODSECAO" +
                                    "   ,F.DEPARTAMENTO" +
                                    "   ,F.ENDERECOPAGTO" +
                                    "   ,F.CODCOLIGADA" +
                                    " FROM" +
                                    "   LK_RM..INT_SHAREP_RM.VW_PERFIL_INTRANET AS F" +
                                    " INNER JOIN" +
                                    "   LK_RM..INT_SHAREP_RM.VW_CCUSTO_INTRANET AS R" +
                                    " ON" +
                                    "   R.CHAPA = F.CHAPA" +
                                    " AND" +
                                    "   R.CODCOLIGADA = F.CODCOLIGADA" +
                                    " WHERE" +
                                    "   R.CODSECAO = '" + centroCusto + "'" +
                                    " AND" +
                                    "   F.CODCOLIGADA = " + codigoColigada.ToString();

                    sqlCommand = new SqlCommand(query, BaseDAL.GetConnection());
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);

                    return dataTable;

                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    dataTable = new DataTable();
                    dataTable.Columns.Add("CODSECAO", Type.GetType("System.String"));
                    dataTable.Columns.Add("DEPARTAMENTO", Type.GetType("System.String"));
                    dataTable.Columns.Add("ENDERECOPAGTO", Type.GetType("System.String"));
                    dataTable.Columns.Add("CODCOLIGADA", Type.GetType("System.String"));

                    DataRow dr = dataTable.NewRow();
                    dr["CODSECAO"] = "01.504.201";
                    dr["DEPARTAMENTO"] = "EVENTOS - RJ";
                    dr["ENDERECOPAGTO"] = "TECNOLOGIA";
                    dr["CODCOLIGADA"] = "3";
                    dataTable.Rows.Add(dr);

                    return dataTable;
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (sqlCommand != null)
                    sqlCommand.Dispose();

                if (sqlDataAdapter != null)
                    sqlDataAdapter.Dispose();
            }
        }

        public static string GetValorAuxilioBolsa(int codigoColigada, int filial, bool nivelTecnico)
        {
            SqlCommand sqlCommand = null;
            SqlDataAdapter sqlDataAdapter = null;
            DataTable dataTable = null;
            try
            {
                if (ambiente_producao)
                {
                    #region PRODUCAO

                    if (codigoColigada != 1) // Joint Ventures
                    {
                        if (codigoColigada == 2) // Telecine
                        {
                            if (filial == 2)
                                filial = 3;
                        }
                        else
                        {
                            filial = 1;
                        }
                    }

                    string query = "SELECT" + 
                                    "   SALARIO" + 
                                    " FROM" + 
                                    "   LK_RM..INT_SHAREP_RM.VW_TABELA_SALARIAL" +
                                    " WHERE" + 
                                    "   NIVEL = '00'" + 
                                    " AND" + 
                                    "   FAIXA = 'A'" +
                                    " AND" + 
                                    "   FILIAL = " + filial +
                                    " AND" + 
                                    "   CODCOLIGADA = " + codigoColigada;

                    if (nivelTecnico)
                        query += " AND ";
                    else
                        query += " AND NOT ";
                        
                    query += "NOMETABELA LIKE '%TÉCNICO%'";

                    sqlCommand = new SqlCommand(query, BaseDAL.GetConnection());
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        return Convert.ToDecimal(dataTable.Rows[0]["SALARIO"].ToString()).ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                    }

                    return string.Empty;

                    #endregion
                }
                else
                {
                    #region DESENVOLVIMENTO
                    return Convert.ToDecimal("938.0000").ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                CIT.Sharepoint.Util.Logger.Write(string.Format("Um erro acorreu enquanto tentava executar o método: {0}. Classe: {1}. Erro: {2}",
                    Utility.GetCurrentMethod(), Utility.GetCurrentClass(), ex.Message), EventLogEntryType.Error, 2, 3);
                throw;
            }
            finally
            {
                if (sqlCommand != null)
                    sqlCommand.Dispose();

                if (sqlDataAdapter != null)
                    sqlDataAdapter.Dispose();

                if (dataTable != null)
                    dataTable.Dispose();
            }
        }
    }
}
