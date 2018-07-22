using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using MySql_Pool.Interface;

namespace MySql_Pool.Helper {
    public class DayPool<T> where T:IPoolInfo{
        private List<T> poolInfos = new List<T> ();
        private BoolRunTime runTime;
        public readonly int min;
        public readonly int max;

        private Func<T> addFunc;

        /// <summary>
        /// 创建池
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="addAction">对象创建器</param>
        public DayPool (int min, int max, Func<T> addFunc) {
            if (addFunc == null) throw new Exception ("无法创建池，对象创建器为null");

            this.min = min;
            this.max = max;
            this.addFunc = addFunc;
            runTime = new BoolRunTime (max);

            AddInfo (min);
        }

        /// <summary>
        /// 添加info对象，自动初始化连接字符串
        /// </summary>
        /// <param name="count">数量</param>
        private void AddInfo (int count) {
            for (int i = 0; i < count; i++) {

                if (poolInfos.Count >= max) {
                    //TODO:连接不够用，写入日志
                    break;
                }

                poolInfos.Add (addFunc ());
            }
        }

        /// <summary>
        /// 获取数据库操作对象
        /// </summary>
        /// <returns></returns>
        public T GetMysqlInfo () {
            int index = -1;
            while (true) {
                index = runTime.GetFree (poolInfos.Count);
                if (index != -1) break;
                //否则添加数据
                AddInfo (poolInfos.Count * 2 + 1);
                Thread.Sleep (5);
            }

            return poolInfos[index];
        }

        /// <summary>
        /// 释放数据库操作对象
        /// </summary>
        /// <param name="conn"></param>
        public void FreeInfo (T info) {
            int index = poolInfos.IndexOf (info);
            if (index != -1) {
                info.Remakes ();
                runTime.Free (index);
            }
        }

    }

}