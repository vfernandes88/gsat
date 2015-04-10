using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace Cit.Globosat.Remuneracao.DAL
{
    public class BaseDAL
    {
        public static SqlConnection GetConnection()
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringProducao"].ConnectionString);
            return cn;
        }
    }
}
