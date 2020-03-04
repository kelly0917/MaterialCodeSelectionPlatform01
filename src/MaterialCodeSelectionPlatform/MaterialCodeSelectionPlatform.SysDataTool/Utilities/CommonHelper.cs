using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Common.Logging;
using Oracle.ManagedDataAccess.Client;

namespace MaterialCodeSelectionPlatform.SysDataTool.Utilities
{
    public class CommonHelper
    {
        private static ILog log = LogManager.GetLogger<Object>();
        /// <summary>
        /// 从Oracle数据库抽取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> GetDataFromOracle<T>(string sql,string connString) where T:new()
        {
            OracleConnection conn = new OracleConnection(connString);
            try
            {
                conn.Open();
                OracleCommand OracleCommand = new OracleCommand(sql,conn);
                DataSet ds = new DataSet();

                OracleDataAdapter OracleDataAdapter = new OracleDataAdapter(OracleCommand);
                OracleDataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                List<T> result = new List<T>();

                foreach (DataRow tableRow in table.Rows)
                {
                    T t = new T();
                    Type type = t.GetType();
                    foreach (DataColumn tableColumn in table.Columns)
                    {
                        PropertyInfo pp = type.GetProperty(tableColumn.ColumnName);
                        if (pp != null)
                        {
                            var value = tableRow[tableColumn.ColumnName].ToString();
                            pp.SetValue(t,value);
                        }
                    }
                    result.Add(t);
                }

                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<T>();
            }
            finally
            {
                conn.Close();
            }


        }

        /// <summary>
        /// 从sql server读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static List<T> GetDataFromSqlServer<T>(string sql, string connString) where T:new()
        {
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sql, conn);
                DataSet ds = new DataSet();

                SqlDataAdapter OracleDataAdapter = new SqlDataAdapter(command);
                OracleDataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                List<T> result = new List<T>();

                foreach (DataRow tableRow in table.Rows)
                {
                    T t = new T();
                    Type type = t.GetType();
                    foreach (DataColumn tableColumn in table.Columns)
                    {
                        PropertyInfo pp = type.GetProperty(tableColumn.ColumnName);
                        if (pp != null)
                        {
                            var value = tableRow[tableColumn.ColumnName].ToString();
                            pp.SetValue(t, value);
                        }
                    }
                    result.Add(t);
                }

                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<T>();
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 从Oracle数据库抽取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataFromOracle(string sql, string connString)
        {
            OracleConnection conn = new OracleConnection(connString);
            try
            {
                conn.Open();
                OracleCommand OracleCommand = new OracleCommand(sql, conn);
                DataSet ds = new DataSet();

                OracleDataAdapter OracleDataAdapter = new OracleDataAdapter(OracleCommand);
                OracleDataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];
                return table;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>   
        /// 使用SqlBulkCopy方式插入数据   
        /// </summary>   
        /// <returns></returns>   
        public static long SqlBulkCopyInsert(DataTable table,string connString,string tableName)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DataTable dataTable = table;
       
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connString);
            sqlBulkCopy.DestinationTableName = tableName;
            log.Debug("dataTable.Rows.Count="+ dataTable.Rows.Count);
            if (dataTable != null && dataTable.Rows.Count != 0)
            {
                sqlBulkCopy.BulkCopyTimeout = 1800;
                sqlBulkCopy.WriteToServer(dataTable);
            }
            sqlBulkCopy.Close();


            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 执行 存储过程
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="connStrign"></param>
        public static void ExcuteSP(string sp, string connString,List<SqlParameter> parameters = null)
        {
            SqlConnection con = new SqlConnection(connString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(sp, con);
                com.CommandType = CommandType.StoredProcedure;
                if(parameters != null)
                    com.Parameters.AddRange(parameters.ToArray());
                com.CommandTimeout = 1800;
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                con.Close();
            }
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connString"></param>
        /// <param name="parameters"></param>
        public static void ExcuteSql(string sql, string connString, List<SqlParameter> parameters = null)
        {
            SqlConnection con = new SqlConnection(connString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(sql, con);
                if (parameters != null)
                    com.Parameters.AddRange(parameters.ToArray());
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                con.Close();
            }
        }
    }
}