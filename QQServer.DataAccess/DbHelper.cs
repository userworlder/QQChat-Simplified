using System;
using System.Data;
using System.Data.SqlClient;

namespace QQServer.DataAccess
{
    public class DbHelper
    {
        // 数据库连接字符串
      // private static readonly string ConnectionString = "Data Source=.;Initial Catalog=QQChat;Integrated Security=True;";
        private static readonly string ConnectionString = "Data Source=LAPTOP-9JMTHEU8\\SQLEXPRESS;Initial Catalog=QQChat;Integrated Security=True;";
        // 获取数据库连接
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // 执行查询，返回DataTable
        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        // 执行非查询，返回受影响的行数
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    
                    if (parameters != null)
                    {
                        Console.WriteLine($"[DbHelper] SQL: {sql}");
                        Console.WriteLine($"[DbHelper] 参数数量: {parameters.Length}");
                        foreach (SqlParameter p in parameters)
                        {
                            Console.WriteLine($"[DbHelper] 参数名: {p.ParameterName}, 值: {p.Value}, 类型: {p.SqlDbType}");
                            cmd.Parameters.Add(p);
                        }
                    }
                    try
                    {
                        int result = cmd.ExecuteNonQuery();
                        Console.WriteLine($"[DbHelper] 执行结果: {result}");
                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DbHelper] 执行异常: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        // 执行标量查询，返回第一行第一列的值
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}