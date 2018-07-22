using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using MySql_Pool.Interface;

namespace MySql_Pool {
    public class MysqlInfo : IPoolInfo {

        private MySqlConnection _conn;
        private MySqlCommand _comm;

        public MySqlConnection conn { get { return _conn; } }

        public MySqlCommand comm { get { return _comm; } }

        public MysqlInfo (string connStr) {
            this._conn = new MySqlConnection (connStr);
            this._comm = _conn.CreateCommand ();
        }

        public MysqlInfo (MySqlConnection conn) {
            this._conn = conn;
            this._comm = conn.CreateCommand ();
        }

        /// <summary>
        /// 重制
        /// </summary>
        public void Remakes () {
            _comm.CommandType = CommandType.Text;
            _comm.CommandText = string.Empty;
            _comm.CommandTimeout = 30;
            _comm.Transaction = null;
        }

    }
}