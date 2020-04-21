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
    public class RegistroGastosController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public RegistroGastosController(ILogger<RegistroGastosController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddRegistroGastos([FromBody]RegistroGastosDTO _sarpara)
        {
            RegistroGastosDTO _RegistroGastos = new RegistroGastosDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/RegistroGastos/GetRegistroGastosById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _RegistroGastos = JsonConvert.DeserializeObject<RegistroGastosDTO>(valorrespuesta);

                }
                if (_RegistroGastos == null)
                {
                    _RegistroGastos = new RegistroGastosDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return PartialView(_RegistroGastos);
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<RegistroGastos> _RegistroGastos = new List<RegistroGastos>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/RegistroGastos/GetRegistroGastos");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _RegistroGastos = JsonConvert.DeserializeObject<List<RegistroGastos>>(valorrespuesta);
                    _RegistroGastos = _RegistroGastos.OrderByDescending(q => q.Id).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _RegistroGastos.ToDataSourceResult(request);
        }

        [HttpPost]
        public async Task<ActionResult> Insert(RegistroGastos _RegistroGastosP)
        {
            RegistroGastos _RegistroGastos = _RegistroGastosP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _RegistroGastos.UsuarioCreacion = HttpContext.Session.GetString("user");
                _RegistroGastos.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/RegistroGastos/Insert", _RegistroGastos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _RegistroGastos = JsonConvert.DeserializeObject<RegistroGastos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { _RegistroGastos }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult> Update(Int64 Id, RegistroGastos _RegistroGastosP)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/RegistroGastos/Update", _RegistroGastosP);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _RegistroGastosP = JsonConvert.DeserializeObject<RegistroGastos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_RegistroGastosP);
        }

        [HttpPost]
        public async Task<ActionResult<RegistroGastos>> Delete([FromBody]RegistroGastos _RegistroGastos)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/RegistroGastos/Delete", _RegistroGastos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _RegistroGastos = JsonConvert.DeserializeObject<RegistroGastos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { _RegistroGastos }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<RegistroGastos>> SaveRegistroGastos([FromBody] RegistroGastosDTO _RegistroGastosS)
        {
            {
                string valorrespuesta = "";
                try
                {
                    RegistroGastos _RegistroGastos = new RegistroGastos();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                  
                    var result = await _client.GetAsync(baseadress + "api/RegistroGastos/GetRegistroGastosById/" + _RegistroGastosS.Id);

                    _RegistroGastosS.FechaModificacion = DateTime.Now;
                    _RegistroGastosS.UsuarioModificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _RegistroGastos = JsonConvert.DeserializeObject<RegistroGastos>(valorrespuesta);
                    }
                    if (_RegistroGastos == null) { _RegistroGastos = new Models.RegistroGastos(); }
                    if (_RegistroGastos.Id == 0)
                    {
                        _RegistroGastosS.FechaCreacion = DateTime.Now;
                        _RegistroGastosS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(_RegistroGastosS);
                    }
                    else
                    {
                        _RegistroGastosS.FechaCreacion = DateTime.Now;
                        _RegistroGastosS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var updateresult = await Update(_RegistroGastosS.Id, _RegistroGastosS);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
                return Json(_RegistroGastosS);
            }
        }
    }
}