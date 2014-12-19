using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace databaseConnection
{
    public class DbConnection
    {
        #region Attributes
        private string fileParameters;
        private string connectionString;
        private string error;
        private bool connectionExists;
        private SqlConnection cnn;        
        private SqlCommand cmd;
        private SqlDataReader dr;         
        #endregion

        #region Properties
        public string Error
        {
            get { return error; }
             
        }

        public SqlConnection Cnn
        {
            get { return cnn; }
             
        }

        public SqlDataReader Dr
        {
            get { return dr; }
           
            
        }
        #endregion

        #region Constructor
        public DbConnection(string fileParameters)
        {
            this.fileParameters = fileParameters;
            connectionExists = false;
            cnn = new SqlConnection();
            cmd = new SqlCommand();
        } 
        #endregion

        #region Private Methods
        private bool GenerateConnectionString()
        {
            try
            {
                Parameters parameter = new Parameters();
                if (!parameter.GenerateConnectionString(fileParameters))
                {
                    error = parameter.Error;
                    parameter = null;
                    return false;
                }
                else
                {
                    connectionString = parameter.ConnectionString;
                    parameter = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region Public Methods
        public bool OpenConnection()
        {
            if (!GenerateConnectionString()) return false; 
           
                cnn.ConnectionString = connectionString;
                try
                {
                    cnn.Open();
                    connectionExists = true;
                    return true;
                }
                catch (Exception ex)
                {
                    error = "Did not open the connection, " + ex.Message;
                    connectionExists = false;
                    return false;
                }
           
        }

        public void CloseConnection()
        {
            try
            {
                cnn.Close();
                connectionExists = false;
            }
            catch (Exception ex)
            {
                error = "Did not closed the connection, " + ex.Message;
            }
        }

        public DataTable GetData(string procedureName, string[] parameterName, 
            params Object[] parameterValues)
        {
            DataTable dt = new DataTable();
            cmd.Connection = cnn;
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            if (procedureName.Length != 0 && parameterName.Length == parameterValues.Length)
            {
                int i = 0;
                foreach (String parameter in parameterName)
                    cmd.Parameters.AddWithValue(parameter,parameterValues[i++]);
                try
                {
                    dr = cmd.ExecuteReader();
                    dt.Load(dr);
                    CloseConnection();
                    return dt;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return dt;
        }

        public int ExecuteProcedures(string procedureName, string[] parameterName, 
            params Object[] parameterValues)
        {
            cmd.Connection = cnn;
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            if (procedureName.Length != 0 && parameterName.Length == parameterValues.Length)
            {
                int i = 0;
                foreach (String parameter in parameterName)
                    cmd.Parameters.AddWithValue(parameter, parameterValues[i++]);
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return 0;
        }
        #endregion
    }
}
