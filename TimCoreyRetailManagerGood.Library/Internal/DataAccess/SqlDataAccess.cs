using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimCoreyRetailManagerGood.Library.Internal.DataAccess
{
    internal class SqlDataAccess : IDisposable
    {
        //add constructor when fixing configuration for connection string
        public SqlDataAccess(IConfiguration config)
        {            
            this.config = config;
        }

        public string GetConnectionString(string name)
        {
            //from Web.Config .NET framework
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            //for debugging only
            //return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TRMData;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; 
            return this.config.GetConnectionString(name);
        }

        //load data from the database
        public List<T> LoadData<T,U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                //take the connection to the database and does the query.  
                List<T> rows = connection.Query<T>(storedProcedure, parameters, 
                    commandType: CommandType.StoredProcedure).ToList();
                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                //take the connection to the database and does the query.  
                connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        //Open connect/start transaction method        
        //load using the transaction
        //save using the transaction
        //Close connection/stop transaction method
        //Dispose

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

            IsClosed = false;
        }

        private bool IsClosed = false;
        private readonly IConfiguration config;

        public void CommitTransaction()
        {
            //we add the question mark (null check) in case we call it multiple times
            _transaction?.Commit();  //We only call if it succeeds
            _connection?.Close();
            IsClosed = true;
        }
        public void RollbackTransaction()
        {
            //we add the question mark (null check) in case we call it multiple times
            _transaction?.Rollback();  //We only call if it fails
            _connection?.Close();
            IsClosed = true;
        }

        public void Dispose()
        {
            if (IsClosed == false)
            {
                try
                {
                    CommitTransaction();
                }
                catch
                {
                    //don't so anything...just keep going since it's already closed
                    //TODO: Log this issue
                }
            }

            _transaction = null;
            _connection = null;
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {               
            //taken from SaveData above
            //associate the transaction with the call. 
            _connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _transaction);            
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            //taken from SaveData above
            //associate the transaction with the call. 
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();
            return rows;
        }

    }    
}
