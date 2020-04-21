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
    public class MotivosAjusteController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public MotivosAjusteController(ILogger<MotivosAjusteController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddMotivosAjuste([FromBody]MotivosAjusteDTO _sarpara)
        {
            MotivosAjusteDTO MotivosAjuste = new MotivosAjusteDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/MotivosAjuste/GetMotivosAjustesById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    MotivosAjuste = JsonConvert.DeserializeObject<MotivosAjusteDTO>(valorrespuesta);

                }
                if (MotivosAjuste == null)
                {
                    MotivosAjuste = new MotivosAjusteDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return PartialView(MotivosAjuste);
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<MotivosAjuste> MotivosAjuste = new List<MotivosAjuste>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/MotivosAjuste/GetMotivosAjuste");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    MotivosAjuste = JsonConvert.DeserializeObject<List<MotivosAjuste>>(valorrespuesta);
                    MotivosAjuste = MotivosAjuste.OrderByDescending(q => q.Id).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return MotivosAjuste.ToDataSourceResult(request);
        }

        [HttpPost]
        public async Task<ActionResult> Insert(MotivosAjuste _MotivosAjuste)
        {
            MotivosAjuste MotivosAjuste = _MotivosAjuste;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                MotivosAjuste.UsuarioCreacion = HttpContext.Session.GetString("user");
                MotivosAjuste.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/MotivosAjuste/Insert", MotivosAjuste);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    MotivosAjuste = JsonConvert.DeserializeObject<MotivosAjuste>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { MotivosAjuste }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult> Update(Int64 Id, MotivosAjuste MotivosAjuste)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/MotivosAjuste/Update", MotivosAjuste);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    MotivosAjuste = JsonConvert.DeserializeObject<MotivosAjuste>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(MotivosAjuste);
        }

        [HttpPost]
        public async Task<ActionResult<MotivosAjuste>> Delete([FromBody]MotivosAjuste MotivosAjuste)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/MotivosAjuste/Delete", MotivosAjuste);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    MotivosAjuste = JsonConvert.DeserializeObject<MotivosAjuste>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { MotivosAjuste }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<MotivosAjuste>> SaveMotivosAjuste([FromBody] MotivosAjuste MotivosAjuste)
        {
            {
                string valorrespuesta = "";
                try
                {
                    MotivosAjuste _MotivosAjuste = new MotivosAjuste();
                    MotivosAjuste _MotivosAjusteP = new MotivosAjuste();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/MotivosAjuste/GetMotivosAjusteByDescripcion/" + MotivosAjuste.Descripcion);
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _MotivosAjuste = JsonConvert.DeserializeObject<MotivosAjuste>(valorrespuesta);
                        _MotivosAjusteP = JsonConvert.DeserializeObject<MotivosAjuste>(valorrespuesta);
                        if (_MotivosAjuste != null && MotivosAjuste.Id == 0)
                        {
                            if (_MotivosAjuste.Descripcion.ToLower() == MotivosAjuste.Descripcion.ToLower())
                            {
                                {
                                    return await Task.Run(() => BadRequest($"Ya existe un motivo de ajuste con ese nombre."));
                                }
                            }
                        }
                    }
                    result = await _client.GetAsync(baseadress + "api/MotivosAjuste/GetMotivosAjustesById/" + MotivosAjuste.Id);

                    MotivosAjuste.FechaModificacion = DateTime.Now;
                    MotivosAjuste.UsuarioModificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _MotivosAjuste = JsonConvert.DeserializeObject<MotivosAjuste>(valorrespuesta);
                    }
                    if (_MotivosAjuste == null) { _MotivosAjuste = new Models.MotivosAjuste(); }
                    if (_MotivosAjuste.Id == 0)
                    {
                        MotivosAjuste.FechaCreacion = DateTime.Now;
                        MotivosAjuste.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(MotivosAjuste);
                    }
                    else
                    {
                        if (_MotivosAjusteP != null)
                        {
                            if (_MotivosAjusteP.Descripcion.ToLower() == MotivosAjuste.Descripcion.ToLower() && _MotivosAjusteP.Id != MotivosAjuste.Id)
                            {
                                return await Task.Run(() => BadRequest($"Ya existe un motivo de ajuste con ese nombre."));
                            }
                        }
                        MotivosAjuste.FechaModificacion = DateTime.Now;
                        MotivosAjuste.UsuarioModificacion = HttpContext.Session.GetString("user");
                        var updateresult = await Update(MotivosAjuste.Id, MotivosAjuste);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
                return Json(MotivosAjuste);
            }
        }
    }
}