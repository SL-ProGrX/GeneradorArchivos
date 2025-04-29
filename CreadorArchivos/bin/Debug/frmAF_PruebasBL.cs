using PgxAPI.DataBaseTier;
using PgxAPI.Models;
using PgxAPI.Models.AF;

namespace PgxAPI.BusinessLogic
{
    public class frmAF_PruebasBL
    {
        private readonly IConfiguration? _config;
        frmAF_PruebasDB PruebasDb;

        public frmAF_PruebasBL(IConfiguration config)
        {
            _config = config;
            PruebasDb = new frmAF_PruebasDB(_config);
        }
    }
}