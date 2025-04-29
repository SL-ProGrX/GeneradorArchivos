using PgxAPI.Models;
using PgxAPI.Models.AF;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace PgxAPI.DataBaseTier
{
    public class frmAF_PruebasDB
    {
        private readonly IConfiguration? _config;

        public frmAF_PruebasDB(IConfiguration config)
        {
            _config = config;
        }
    }
}