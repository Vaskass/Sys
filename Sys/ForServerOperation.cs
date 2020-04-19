using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Sys
{
    class ForServerOperation
    {

    public SqlDataReader GetReader(string request)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.CS))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(request, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    
                    if (reader.HasRows) // если есть данные
                    {
                        return reader;
                    }
                    else
                    {
                       return null;
                    }
                }
            }
            catch
            {

            }
          return null;  
        }



    }
}

