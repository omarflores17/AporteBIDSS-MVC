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
    public class TipoGastosController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public TipoGastosController(ILogger<TipoGastosController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddTipoGastos([FromBody]TipoGastosDTO _sarpara)
        {
            TipoGastosDTO _TipoGastos = new TipoGastosDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/TipoGastos/GetTipoGastosById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _TipoGastos = JsonConvert.DeserializeObject<TipoGastosDTO>(valorrespuesta);

                }
                if (_TipoGastos == null)
                {
                    _TipoGastos = new TipoGastosDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return PartialView(_TipoGastos);
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<TipoGastos> _TipoGastos = new List<TipoGastos>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/TipoGastos/GetTipoGastos");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _TipoGastos = JsonConvert.DeserializeObject<List<TipoGastos>>(valorrespuesta);
                    _TipoGastos = _TipoGastos.OrderByDescending(q => q.Id).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _TipoGastos.ToDataSourceResult(request);
        }

        [HttpPost]
        public async Task<ActionResult> Insert(TipoGastos _TipoGastosP)
        {
            TipoGastos _TipoGastos = _TipoGastosP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _TipoGastos.UsuarioCreacion = HttpContext.Session.GetString("user");
                _TipoGastos.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/TipoGastos/Insert", _TipoGastos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _TipoGastos = JsonConvert.DeserializeObject<TipoGastos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { _TipoGastos }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult> Update(Int64 Id, TipoGastos _TipoGastosP)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/TipoGastos/Update", _TipoGastosP);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _TipoGastosP = JsonConvert.DeserializeObject<TipoGastos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_TipoGastosP);
        }

        [HttpPost]
        public async Task<ActionResult<TipoGastos>> Delete([FromBody]TipoGastos _TipoGastos)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/TipoGastos/Delete", _TipoGastos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _TipoGastos = JsonConvert.DeserializeObject<TipoGastos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { _TipoGastos }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<TipoGastos>> SaveTipoGastos([FromBody] TipoGastosDTO _TipoGastosS)
        {
            {
                string valorrespuesta = "";
                try
                {
                    TipoGastos _TipoGastos = new TipoGastos();
                    TipoGastos _TipoGastosP = new TipoGastos();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/TipoGastos/GetTipoGastosByDescripcion/" + _TipoGastosS.Descripcion);
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _TipoGastos = JsonConvert.DeserializeObject<TipoGastos>(valorrespuesta);
                        _TipoGastosP = JsonConvert.DeserializeObject<TipoGastos>(valorrespuesta);
                        if (_TipoGastos != null && _TipoGastosS.Id == 0)
                        {
                            if (_TipoGastos.Descripcion.ToLower() == _TipoGastosS.Descripcion.ToLower())
                            {
                                {
                                    return await Task.Run(() => BadRequest($"Ya existe un tipo de gasto con ese nombre."));
                                }
                            }
                        }
                    }
                    result = await _client.GetAsync(baseadress + "api/TipoGastos/GetTipoGastosById/" + _TipoGastosS.Id);

                    _TipoGastosS.FechaModificacion = DateTime.Now;
                    _TipoGastosS.UsuarioModificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _TipoGastos = JsonConvert.DeserializeObject<TipoGastos>(valorrespuesta);
                    }
                    if (_TipoGastos == null) { _TipoGastos = new Models.TipoGastos(); }
                    if (_TipoGastos.Id == 0)
                    {
                        _TipoGastosS.FechaCreacion = DateTime.Now;
                        _TipoGastosS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(_TipoGastosS);
                    }
                    else
                    {
                        if (_TipoGastosP != null)
                        {
                            if (_TipoGastosP.Descripcion.ToLower() == _TipoGastosS.Descripcion.ToLower() && _TipoGastosP.Id != _TipoGastosS.Id)
                            {
                                return await Task.Run(() => BadRequest($"Ya existe un tipo de gasto con ese nombre."));
                            }
                        }
                        _TipoGastosS.FechaCreacion = DateTime.Now;
                        _TipoGastosS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var updateresult = await Update(_TipoGastosS.Id, _TipoGastosS);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
                return Json(_TipoGastosS);
            }
        }
    }
}