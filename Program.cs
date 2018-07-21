using System;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using MySql_Pool.Helper;

namespace MySql_Pool {
    class Program {
        static void Main (string[] args) {
            //测试数据库使用4c2g docker
            test ();
            test1 ();

        }

        static void test () {
            string connStr = "Host=127.0.0.1;port=3308;database=mytest;uid=root;pwd=123456";
            Stopwatch sw = new Stopwatch ();
            sw.Start ();
            for (int i = 0; i < 10000; i++) {
                using (MySqlConnection conn = new MySqlConnection (connStr)) {
                    using (MySqlCommand comm = conn.CreateCommand ()) {
                        conn.Open ();
                        comm.CommandText = $"insert into `test.user`(id,name,pwd) value('{Guid.NewGuid()}','day','123456')";
                        comm.ExecuteNonQuery ();
                    }
                }
            }
            sw.Stop ();
            Console.WriteLine ("正常使用：" + sw.ElapsedMilliseconds); //正常使用：61598
        }

        static void test1 () {
            string connStr = "Host=127.0.0.1;port=3308;database=mytest;uid=root;pwd=123456";
            MysqlPool mysqlPool = new MysqlPool (100, 1000, connStr);

            Stopwatch sw = new Stopwatch ();
            sw.Start ();
            for (int i = 0; i < 10000; i++) {
                (MySqlConnection conn, MySqlCommand comm) = mysqlPool.GetMysqlInfo ();
                if (conn.State == ConnectionState.Closed)
                    conn.Open ();
                comm.CommandText = $"insert into `test.user`(id,name,pwd) value('{Guid.NewGuid()}','day','123456')";
                comm.ExecuteNonQuery ();
                mysqlPool.FreeInfo (comm);
            }
            sw.Stop ();
            Console.WriteLine ("使用池：" + sw.ElapsedMilliseconds); //使用池：40833
        }
    }
}