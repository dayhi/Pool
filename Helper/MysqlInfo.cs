using System.Threading;
using MySql.Data.MySqlClient;

namespace MySql_Pool.Helper {
    public class MysqlInfo {

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

    }
}