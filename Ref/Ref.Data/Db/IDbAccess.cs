using System.Data;
using System.Data.SqlClient;

namespace Ref.Data.Db
{
    public interface IDbAccess
    {
        IDbConnection Connection { get; }
        void CloseConnection();
    }

    public class DbAccess : IDbAccess
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public DbAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    if(string.IsNullOrWhiteSpace(_connection.ConnectionString))
                        _connection.ConnectionString = _connectionString;

                    _connection.Open();
                }
                return _connection;
            }
        }
    }
}