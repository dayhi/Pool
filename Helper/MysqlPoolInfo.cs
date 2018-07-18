using MySql.Data.MySqlClient;

namespace MySql_Pool.Helper {
    public class MysqlInfo {
        public bool isRun { get; set; }
        public MySqlConnection conn { get; set; }
        public MySqlCommand comm { get; set; }
    }
}