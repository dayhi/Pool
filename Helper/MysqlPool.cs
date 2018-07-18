using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;

namespace MySql_Pool.Helper {
    public class MysqlPool {
        private List<MysqlInfo> sqlPool = new List<MysqlInfo> ();

        public readonly string connStr;
        public readonly int min;
        public readonly int max;

        /// <summary>
        /// 创建数据库操作对象池，默认最小5，最大100
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public MysqlPool (string connStr) : this (5, 100, connStr) { }

        /// <summary>
        /// 创建数据库操作对象池
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="connStr">连接字符串</param>
        public MysqlPool (int min, int max, string connStr) {
            this.min = min;
            this.max = max;
            this.connStr = connStr;

            AddInfo (min);
        }

        /// <summary>
        /// 添加info对象，自动初始化连接字符串
        /// </summary>
        /// <param name="count">数量</param>
        private void AddInfo (int count) {
            for (int i = 0; i < count; i++) {

                if (sqlPool.Count >= max) {
                    //TODO:连接不够用，写入日志
                    break;
                }

                var mysqlPoolinfo = new MysqlInfo () {
                    isRun = false,
                    conn = new MySqlConnection (connStr),
                    comm = new MySqlCommand ()
                };

                mysqlPoolinfo.comm.Connection = mysqlPoolinfo.conn;

                sqlPool.Add (mysqlPoolinfo);
            }
        }

        private readonly object objGetLock = new object ();

        /// <summary>
        /// 获取数据库操作对象
        /// </summary>
        /// <returns></returns>
        public (MySqlConnection conn, MySqlCommand comm) GetMysqlInfo () {
            MysqlInfo result;

            lock (objGetLock) {
                while (true) {
                    result = sqlPool.Where (s => !s.isRun).FirstOrDefault ();
                    if (result != null) break;
                    //否则添加数据
                    AddInfo (sqlPool.Count * 2 + 1);
                    Thread.Sleep (5);
                }
            }

            result.isRun = true;
            return (result.conn, result.comm);
        }

        /// <summary>
        /// 释放数据库操作对象
        /// </summary>
        /// <param name="conn"></param>
        public void FreeInfo (MySqlCommand comm) {
            var info = sqlPool.Where (s => object.ReferenceEquals (s.comm, comm)).FirstOrDefault ();
            if (info != null) {
                Remakes (info.comm);
                info.isRun = false;
            }
        }

        /// <summary>
        /// 重制comm
        /// </summary>
        /// <param name="comm"></param>
        private static void Remakes (MySqlCommand comm) {
            comm.CommandType = CommandType.Text;
            comm.CommandText = string.Empty;
            comm.CommandTimeout = 30;
            comm.Transaction = null;
        }
    }

}