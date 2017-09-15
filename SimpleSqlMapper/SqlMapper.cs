using System;
using System.Collections.Generic;
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
        private string _connectionString;
        /// <summary>
        /// Not implemented yet
        /// </summary>
        public bool IsCaseSensitive { get; set; }

        /// <summary>
        /// Create an instance of the SqlMapper
        /// </summary>
        /// <param name="connectionString">Connectionstring to SQL Server</param>
        public SqlMapper(string connectionString)
        {
            _connectionString = connectionString;
            IsCaseSensitive = true;
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
            return GetList<T>(procedureName, CommandType.StoredProcedure, param);
        }

        /// <summary>
        /// Returns a List of the specified type
        /// </summary>
        /// <typeparam name="T">Type of the List</typeparam>
        /// <param name="command">Name of the stored procedure on the server</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="param">A dynamic object containing parameters, e.g. new { para_Value = 12 }</param>
        /// <returns></returns>
        public List<T> GetList<T>(string command, CommandType commandType, dynamic param = null)
        {
            SqlConnection conn = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(_connectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand(command, conn) { CommandType = commandType };

                // Parameter aus dynamic lesen und als cmdParameters einfügen
                PropertyInfo[] sqlParameter = param?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                if (sqlParameter != null)
                {
                    foreach (var propertyInfo in sqlParameter)
                    {
                        cmd.Parameters.Add(new SqlParameter($@"{propertyInfo.Name}", param.GetType().GetProperty(propertyInfo.Name).GetValue(param, null)));
                    }
                }

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
    }
}
