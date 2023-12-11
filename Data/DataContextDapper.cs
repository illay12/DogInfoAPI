using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DogInfoAPI.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }
        public int ExecuteSQLWithRowCount(string sql)
        {
            using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                return dbConnection.Execute(sql);
            }
        }

        public bool ExecuteSQL(string sql)
        {
            using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                return dbConnection.Execute(sql) > 0;
            }        
        }

        public bool ExecuteSQLWithParameters(string sql, List<SqlParameter> parameters)
        {
                SqlCommand commandWithParams = new SqlCommand(sql);

                foreach(SqlParameter parameter in parameters)
                {
                    commandWithParams.Parameters.Add(parameter);
                }

                SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                dbConnection.Open();

                commandWithParams.Connection = dbConnection;

                int rowsAffected = commandWithParams.ExecuteNonQuery();

                dbConnection.Close();

                return rowsAffected > 0;
            
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            using(IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                return dbConnection.Query<T>(sql);
            }
        }

        public T LoadDataSingle<T>(string sql)
        {
            using(IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                return dbConnection.QuerySingle<T>(sql);
            }
        }
    }
}