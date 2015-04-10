using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace Globosat.Library.AcessoDados
{
    public class BaseDados
    {
        public static SqlConnection GetConnectionUP()
        {
            #region PRODUCAO
            // Erro ao buscar a string de conexão no web.config. 
            // Pode ser pelo fato da nomenclatura do database apresentar espaço em branco.
            // TODO: Verificar posteriormente como recuperar tal string no web.config. (boas práticas de programação)
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString_UP"].ConnectionString);  
            #endregion

            return cn;
        }

        public static SqlConnection GetConnectionProfile()
        {
            #region PRODUCAO
            // Erro ao buscar a string de conexão no web.config. 
            // Pode ser pelo fato da nomenclatura do database apresentar espaço em branco.
            // TODO: Verificar posteriormente como recuperar tal string no web.config. (boas práticas de programação)
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString_UP"].ConnectionString);
            #endregion

            return cn;
        }

        public static SqlConnection GetConnection()
        {
            #region PRODUCAO
            // Erro ao buscar a string de conexão no web.config. 
            // Pode ser pelo fato da nomenclatura do database apresentar espaço em branco.
            // TODO: Verificar posteriormente como recuperar tal string no web.config. (boas práticas de programação)
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringProducao"].ConnectionString);
            #endregion

            return cn;
        }
    }
}
