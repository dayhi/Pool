using System.Threading;

namespace MySql_Pool.Helper {
    public class BoolRunTime {
        private bool[] isRuns;
        private readonly object[] objLocks;

        public BoolRunTime (int max) {
            isRuns = new bool[max];
            objLocks = new object[max];
            for (int i = 0; i < max; i++) {
                objLocks[i] = new object ();
            }
        }

        /// <summary>
        /// 获取释放的对象
        /// </summary>
        /// <param name="max">可用数量</param>
        /// <returns>返回索引，无则未-1</returns>
        public int GetFree (int max) {
            for (int i = 0; i < max; i++) {
                if (!isRuns[i]) {
                    if (TackUp (i))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 修改为正在运行
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        private bool TackUp (int index) {
            bool result = false;
            if (Monitor.TryEnter (objLocks[index])) {
                if (!isRuns[index]) {
                    isRuns[index] = true;
                    result = true;
                }
                Monitor.Exit (objLocks[index]);
            }
            return result;
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="index">索引</param>
        public void Free (int index) {
            isRuns[index] = false;
        }
    }
}