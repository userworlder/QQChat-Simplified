using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QQClient.DataAccess
{
    public class DbHelper
    {
        // 客户端本地数据库连接字符串
        private static readonly string ConnectionString = "Data Source=.;Initial Catalog=QQClientDB;Integrated Security=True;";

        // private static readonly string ConnectionString = "Data Source=LAPTOP-9JMTHEU8\\SQLEXPRESS;Initial Catalog=QQClientDB;Integrated Security=True;";


        // 获取数据库连接

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }


        // 执行查询，返回DataTable（同步版本）

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


        // 执行查询，返回DataTable（异步版本）

        public static async Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters)
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
                        await Task.Run(() => adapter.Fill(dt));
                    }
                    return dt;
                }
            }
        }
        // 执行非查询，返回受影响的行数（同步版本）
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
        // 执行非查询，返回受影响的行数（异步版本）
        public static async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                await conn.OpenAsync();
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
                        int result = await cmd.ExecuteNonQueryAsync();
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


        //执行标量查询，返回第一行第一列的值（同步版本）

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


        // 执行标量查询，返回第一行第一列的值（异步版本）

        public static async Task<object> ExecuteScalarAsync(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return await cmd.ExecuteScalarAsync();
                }
            }
        }
    }
}