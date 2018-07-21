using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;

namespace MySql_Pool.Helper {
    public class MysqlPool {
        private List<MysqlInfo> dbPool = new List<MysqlInfo> ();
        private BoolRunTime runTime;
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
            runTime = new BoolRunTime (max);

            AddInfo (min);
        }

        /// <summary>
        /// 添加info对象，自动初始化连接字符串
        /// </summary>
        /// <param name="count">数量</param>
        private void AddInfo (int count) {
            for (int i = 0; i < count; i++) {

                if (dbPool.Count >= max) {
                    //TODO:连接不够用，写入日志
                    break;
                }

                var mysqlPoolinfo = new MysqlInfo (connStr);

                dbPool.Add (mysqlPoolinfo);
            }
        }

        /// <summary>
        /// 获取数据库操作对象
        /// </summary>
        /// <returns></returns>
        public MysqlInfo GetMysqlInfo () {
            int index = -1;
            while (true) {
                index = runTime.GetFree (dbPool.Count);
                if (index != -1) break;
                //否则添加数据
                AddInfo (dbPool.Count * 2 + 1);
                Thread.Sleep (5);
            }

            return dbPool[index];
        }

        /// <summary>
        /// 释放数据库操作对象
        /// </summary>
        /// <param name="conn"></param>
        public void FreeInfo (MysqlInfo mysqlInfo) {
            int index = dbPool.IndexOf (mysqlInfo);
            if (index != -1) {
                Remakes (mysqlInfo.comm);
                runTime.Free (index);
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