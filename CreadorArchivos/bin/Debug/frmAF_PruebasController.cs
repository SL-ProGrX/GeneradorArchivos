using Microsoft.AspNetCore.Mvc;
using PgxAPI.BusinessLogic;
using PgxAPI.Models;
using PgxAPI.Models.AF;

namespace PgxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class frmAF_PruebasController : Controller
    {
        private readonly IConfiguration? _config;
         frmAF_PruebasBL PruebasBL;
        public frmAF_PruebasController(IConfiguration config)
        {
            _config = config;
            PruebasBL = new frmAF_PruebasBL(_config);
        }
    }
}