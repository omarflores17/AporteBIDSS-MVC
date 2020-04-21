using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.Helpers;
using ERPMVC.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ERPMVC.DTO;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class KardexVialeController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public KardexVialeController(ILogger<KardexVialeController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<KardexViale> _Kardex = new List<KardexViale>();
            try
            {
                int branch = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexViale/" + branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<List<KardexViale>>(valorrespuesta);
                    _Kardex = _Kardex.OrderByDescending(q => q.KardexDate).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Kardex.ToDataSourceResult(request);
        }

        public async Task<ActionResult> VerSaldo([FromBody] int Codigo)
        {
            string valorrespuesta = "";
            KardexViale _KardexVialeP = new KardexViale();
            string baseadress = config.Value.urlbase;
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var result = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexByProductId/" + Codigo +"/"+ Convert.ToInt32(HttpContext.Session.GetString("BranchId")));
            if (result.IsSuccessStatusCode)
            {
                valorrespuesta = await (result.Content.ReadAsStringAsync());
                _KardexVialeP = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta);
                if (_KardexVialeP == null)
                {
                    int Total = 0;
                    return Ok(Total);
                }
                else
                {
                    return Ok(_KardexVialeP.Total);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Save([FromBody]KardexViale _CompanyInfoS)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _CompanyInfoS.SaldoAnterior = _CompanyInfoS.Total;
                _CompanyInfoS.Total = _CompanyInfoS.Total + _CompanyInfoS.QuantityEntry;
                _CompanyInfoS.QuantityOut = 0;
                _CompanyInfoS.KardexDate = DateTime.Now;
                _CompanyInfoS.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                _CompanyInfoS.UsuarioCreacion= HttpContext.Session.GetString("user");

                var result = await _client.PostAsJsonAsync(baseadress + "api/KardexViale/Insert", _CompanyInfoS);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CompanyInfoS = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta);
                    return RedirectToAction("Menu", "CompanyInfo");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_CompanyInfoS);
        }
        [HttpGet]

        public async Task<ActionResult> SFKardex()
        {
           
                return await Task.Run(() => View());
            

        }
    }
}