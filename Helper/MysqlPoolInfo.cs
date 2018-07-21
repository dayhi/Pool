using System.Threading;
using MySql.Data.MySqlClient;

namespace MySql_Pool.Helper {
    public class MysqlInfo {
        private bool _isRun;
        private MySqlConnection _conn;
        private MySqlCommand _comm;

        public bool isRun { get { return _isRun; } }
        public MySqlConnection conn { get { return _conn; } }

        public MySqlCommand comm { get { return _comm; } }

        public MysqlInfo (string connStr) {
            this._conn = new MySqlConnection (connStr);
            this._comm = _conn.CreateCommand ();
            _isRun = false;
        }

        public MysqlInfo (MySqlConnection conn, MySqlCommand comm) {
            this._conn = conn;
            this._comm = comm;
            comm.Connection = conn;
            _isRun = false;
        }

        private readonly object objLock = new object ();

        /// <summary>
        /// 占用此对象
        /// </summary>
        /// <returns>成功返回true，失败返回false</returns>
        public bool TackUp () {

            bool result = false;
            if (Monitor.TryEnter (objLock)) {
                if (!_isRun) {
                    _isRun = true;
                    result = true;
                }
                Monitor.Exit (objLock);
            }
            return result;
        }

        public void Free () {
            _isRun = false;
        }
    }
}