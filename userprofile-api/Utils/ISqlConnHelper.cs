using System.Data;

namespace userprofile_api.Utils
{
    public interface ISqlConnHelper
    {
        IDbConnection Connection { get; }
    }
}
