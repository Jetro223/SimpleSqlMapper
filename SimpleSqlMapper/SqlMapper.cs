using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace SimpleSqlMapper
{
    /// <summary>
    /// SqlMapper 
    /// </summary>
    public class SqlMapper
    {
        /// <summary>
        /// The ConnectionString used to connect to SqlServer
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Not implemented yet
        /// </summary>
        public bool IsCaseSensitive {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        
        /// <summary>
        /// Create an instance of the SqlMapper
        /// </summary>
        /// <param name="connectionString">Connectionstring to SQL Server</param>
        public SqlMapper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Create an instance of the SqlMapper using the connectionstring with the name SqlMapperDefaultConnection
        /// </summary>
        public SqlMapper()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SqlMapperDefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("No connectionstring with name \"SqlMapperDefaultConnection\" in config. Please set this connectionstring or use the constructor with the connectionstring parameter");

            ConnectionString = connectionString;
        }

        /// <summary>
        /// Executes SQL without returning a resultset
        /// </summary>
        /// <param name="command">SQL command or stored procedure</param>
        /// <param name="commandType">CommantType</param>
        /// <param name="param">A dynamic object containing parameters, e.g. new { para_Value = 12 }</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(string command, SqlCommandType commandType, dynamic param = null)
        {
            SqlConnection conn = null;
            
            try
            {
                conn = new SqlConnection(ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand(command, conn) { CommandType = commandType == SqlCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text };

                cmd = AddParameters(cmd, param);

                // Ausführen
                var rowCount = cmd.ExecuteNonQuery();
                return rowCount;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Returns a List of the specified type from a stored procedure
        /// Column names in the result set must match the property names of the type
        /// </summary>
        /// <typeparam name="T">Type of the List</typeparam>
        /// <param name="procedureName">Name of the stored procedure on the server</param>
        /// <param name="param">A dynamic object containing parameters, e.g. new { para_Value = 12 }</param>
        /// <returns></returns>
        public List<T> GetListFromStoredProcedure<T>(string procedureName, dynamic param = null)
        {
            return GetList<T>(procedureName, SqlCommandType.StoredProcedure, param);
        }

        /// <summary>
        /// Returns a List of the specified type
        /// </summary>
        /// <typeparam name="T">Type of the List</typeparam>
        /// <param name="command">Name of the stored procedure on the server</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="param">A dynamic object containing parameters, e.g. new { para_Value = 12 }</param>
        /// <returns></returns>
        public List<T> GetList<T>(string command, SqlCommandType commandType, dynamic param = null)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand(command, conn) { CommandType = commandType == SqlCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text };

                cmd = AddParameters(cmd, param);

                // Ausführen
                rdr = cmd.ExecuteReader();

                var rows = new List<T>();
                while (rdr.Read())
                {
                    //Für jede Zeile ein neues dynamisches Objekt erzeugen => PropertyNames = Spaltennamen
                    var row = new ExpandoObject() as IDictionary<string, object>;
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        row.Add(rdr.GetName(i), rdr[i]);
                    }

                    // die Properties aus dem übergebenen Typ lesen
                    var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetSetMethod() != null);
                    var obj = Activator.CreateInstance(typeof(T));

                    // wenn es ein Property im Ergebnis und im Typ gibt, dann zuweisen
                    foreach (var prop in props)
                    {
                        if (row.ContainsKey(prop.Name))
                        {
                            prop.SetValue(obj, row[prop.Name]);
                        }
                    }

                    //zur Ergebnisliste des übergebenen Typs hinzufügen
                    rows.Add((T)obj);
                }

                return rows;
            }
            finally
            {
                conn?.Close();
                rdr?.Close();
            }
        }

        private SqlCommand AddParameters(SqlCommand sqlCommand, dynamic param = null)
        {
            // Parameter aus dynamic lesen und als cmdParameters einfügen
            PropertyInfo[] sqlParameter = param?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (sqlParameter != null)
            {
                foreach (var propertyInfo in sqlParameter)
                {
                    sqlCommand.Parameters.Add(new SqlParameter($@"{propertyInfo.Name}", param.GetType().GetProperty(propertyInfo.Name).GetValue(param, null)));
                }
            }

            return sqlCommand;
        }
    }
}
