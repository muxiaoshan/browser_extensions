using System;
using System.Collections;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;

namespace DiagnoseAssistant1.database
{
    public class MySqlOp
    {
        Log log = new Log();
        static string connStr = "server=172.26.111.11;User Id=root;password=root;Database=hnszlyy";
        /// <summary>
        /// 执行统计查询，返回查询到的条数
        /// </summary>
        /// <param name="sql">统计条数的sql</param>
        /// <returns>查询到的条数</returns>
        public int executeCount(string sql)
        {
            MySqlConnection mycon = new MySqlConnection(connStr);
            MySqlCommand mycmd = new MySqlCommand(sql, mycon);
            //查询结果读取器
            MySqlDataReader reader = null;
            int ret = 0;
            try
            {
                //打开连接
                mycon.Open();
                //执行查询，并将结果返回给读取器
                reader = mycmd.ExecuteReader();

                if (reader.Read())
                {
                    ret = reader.GetInt32(0);
                }
            }
            finally
            {
                reader.Close();
                mycon.Close();
            }
            return ret;
        }
        /// <summary>
        /// 执行集合查询，返回查询到的集合
        /// </summary>
        /// <param name="sql">集合查询sql</param>
        /// <returns>查询到的集合</returns>
        public ArrayList executeQuery(string sql, Type type)
        {
            MySqlConnection mycon = new MySqlConnection(connStr);           
            MySqlCommand mycmd = new MySqlCommand(sql, mycon);
            //查询结果读取器
            MySqlDataReader reader = null;
            ArrayList rets = new ArrayList();
            try
            {
                //打开连接
                mycon.Open();
                //执行查询，并将结果返回给读取器
                reader = mycmd.ExecuteReader();
                
                while (reader.Read())
                {
                    object o = Activator.CreateInstance(type);
                    PropertyInfo[] pis = type.GetProperties();
                    foreach (PropertyInfo pi in pis)
                    {
                        if (pi.PropertyType == typeof(DateTime))
                        {
                            pi.SetValue(o, reader.GetDateTime(pi.Name), null);
                        }
                        else
                        {
                            pi.SetValue(o, reader.GetString(pi.Name), null);
                        }
                        
                    }                
                    rets.Add(o);
                }
            }
            finally
            {
                reader.Close();
                mycon.Close();
            }
            return rets;
        }
        /// <summary>
        /// 执行单条数据更新
        /// </summary>
        /// <param name="sql"></param>
        public void executeUpdate(string sql)
        {

            MySqlConnection mycon = new MySqlConnection(connStr);
            mycon.Open();
            MySqlCommand mycmd = new MySqlCommand(sql, mycon);
            if (mycmd.ExecuteNonQuery() > 0)
            {
                log.WriteLog("数据更新成功！" + sql);
            }
            Console.ReadLine();
            mycon.Close();
        }
        /// <summary>
        /// 执行批量更新
        /// </summary>
        /// <param name="SQLStringList">批量更新sql集合</param>
        public void executeUpdateBatch(ArrayList SQLStringList)
        {

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                        //后来加上的  
                        if (n > 0 && (n % 500 == 0 || n == SQLStringList.Count - 1))
                        {
                            tx.Commit();
                            tx = conn.BeginTransaction();
                        }
                    }
                    //tx.Commit();//原来一次性提交  
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }                
                conn.Close();
            }  
        }
    }
}
