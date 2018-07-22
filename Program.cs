using System;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using MySql_Pool.Helper;

namespace MySql_Pool {
    class Program {
        static void Main (string[] args) {

            const int count = 10000;
            //测试机配置：2c3g
            test (count);
            test1 (count);

        }

        static void test (int count) {
            string connStr = "Host=127.0.0.1;port=3308;database=mytest;uid=root;pwd=123456";
            Stopwatch sw = new Stopwatch ();
            sw.Start ();
            for (int i = 0; i < count; i++) {
                using (MySqlConnection conn = new MySqlConnection (connStr)) {
                    using (MySqlCommand comm = conn.CreateCommand ()) {
                        conn.Open ();
                    }
                }
            }
            sw.Stop ();
            Console.WriteLine ("正常使用：" + sw.ElapsedMilliseconds); //正常使用：9633
        }

        static void test1 (int count) {
            string connStr = "Host=127.0.0.1;port=3308;database=mytest;uid=root;pwd=123456";
            DayPool<MysqlInfo> mysqlPool = new DayPool<MysqlInfo> (100, 1000, ()=>new MysqlInfo(connStr));

            Stopwatch sw = new Stopwatch ();
            sw.Start ();
            for (int i = 0; i < count; i++) {
                MysqlInfo mysqlInfo = mysqlPool.GetMysqlInfo ();
                mysqlPool.FreeInfo (mysqlInfo);
            }
            sw.Stop ();
            Console.WriteLine ("使用池：" + sw.ElapsedMilliseconds); //使用池：3
        }
    }
}