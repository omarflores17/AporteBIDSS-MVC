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
    public class GrupoConfiguracionInteresesController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public GrupoConfiguracionInteresesController(ILogger<GrupoConfiguracionInteresesController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddGrupoConfiguracionIntereses([FromBody]GrupoConfiguracionInteresesDTO _sarpara)

        {
            GrupoConfiguracionInteresesDTO _GrupoConfiguracionIntereses = new GrupoConfiguracionInteresesDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracionIntereses/GetGrupoConfiguracionInteresesById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracionIntereses = JsonConvert.DeserializeObject<GrupoConfiguracionInteresesDTO>(valorrespuesta);

                }

                if (_GrupoConfiguracionIntereses == null)
                {
                    _GrupoConfiguracionIntereses = new GrupoConfiguracionInteresesDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_GrupoConfiguracionIntereses);

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<GrupoConfiguracionIntereses> _GrupoConfiguracionIntereses = new List<GrupoConfiguracionIntereses>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracionIntereses/GetGrupoConfiguracionIntereses");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracionIntereses = JsonConvert.DeserializeObject<List<GrupoConfiguracionIntereses>>(valorrespuesta);
                    _GrupoConfiguracionIntereses = _GrupoConfiguracionIntereses.OrderByDescending(q => q.Id).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _GrupoConfiguracionIntereses.ToDataSourceResult(request);
        }

        [HttpPost]
        public async Task<ActionResult> Insert(GrupoConfiguracionIntereses _GrupoConfiguracionInteresesP)
        {
            GrupoConfiguracionIntereses _GrupoConfiguracionIntereses = _GrupoConfiguracionInteresesP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _GrupoConfiguracionIntereses.UsuarioCreacion = HttpContext.Session.GetString("user");
                _GrupoConfiguracionIntereses.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/GrupoConfiguracionIntereses/Insert", _GrupoConfiguracionIntereses);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracionIntereses = JsonConvert.DeserializeObject<GrupoConfiguracionIntereses>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            //return Ok(_FundingInterestRate);
            return new ObjectResult(new DataSourceResult { Data = new[] { _GrupoConfiguracionIntereses }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult> Update(Int64 Id, GrupoConfiguracionIntereses _GrupoConfiguracionInteresesP)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/GrupoConfiguracionIntereses/Update", _GrupoConfiguracionInteresesP);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracionInteresesP = JsonConvert.DeserializeObject<GrupoConfiguracionIntereses>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_GrupoConfiguracionInteresesP);
            //return new ObjectResult(new DataSourceResult { Data = new[] { _FundingInterestRate }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<GrupoConfiguracionIntereses>> SaveGrupoConfiguracionIntereses([FromBody]GrupoConfiguracionInteresesDTO _GrupoConfiguracionInteresesS)
        {
            {
                string valorrespuesta = "";
                try
                {
                    GrupoConfiguracionIntereses _GrupoConfiguracionIntereses = new GrupoConfiguracionIntereses();
                    GrupoConfiguracionIntereses _GrupoConfiguracionInteresesp = new GrupoConfiguracionIntereses();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracionIntereses/GetGrupoConfiguracionInteresesByDescripcion/" + _GrupoConfiguracionInteresesS.Nombre);
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _GrupoConfiguracionIntereses = JsonConvert.DeserializeObject<GrupoConfiguracionIntereses>(valorrespuesta);
                        _GrupoConfiguracionInteresesp = JsonConvert.DeserializeObject<GrupoConfiguracionIntereses>(valorrespuesta);
                        if (_GrupoConfiguracionIntereses != null && _GrupoConfiguracionInteresesS.Id == 0)
                        {
                            if (_GrupoConfiguracionIntereses.Nombre.ToLower() == _GrupoConfiguracionInteresesS.Nombre.ToLower())
                            {                              
                                        {
                                            return await Task.Run(() => BadRequest($"Ya existe un grupo de configuracion con ese nombre."));
                                        }
                                    }
                                }
                            }



                    result = await _client.GetAsync(baseadress + "api/GrupoConfiguracionIntereses/GetGrupoConfiguracionInteresesById/" + _GrupoConfiguracionInteresesS.Id);

                    _GrupoConfiguracionInteresesS.FechaModificacion = DateTime.Now;
                    _GrupoConfiguracionInteresesS.UsuarioModificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _GrupoConfiguracionIntereses = JsonConvert.DeserializeObject<GrupoConfiguracionIntereses>(valorrespuesta);
                    }

                    if (_GrupoConfiguracionIntereses == null) { _GrupoConfiguracionIntereses = new Models.GrupoConfiguracionIntereses(); }

                    if (_GrupoConfiguracionIntereses.Id == 0)
                    {
                        _GrupoConfiguracionInteresesS.FechaCreacion = DateTime.Now;
                        _GrupoConfiguracionInteresesS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(_GrupoConfiguracionInteresesS);
                    }
                    else
                    {
                        if(_GrupoConfiguracionInteresesp!=null)
                        {
                            if (_GrupoConfiguracionInteresesp.Nombre.ToLower() == _GrupoConfiguracionInteresesS.Nombre.ToLower() && _GrupoConfiguracionInteresesp.Id != _GrupoConfiguracionInteresesS.Id)
                            {
                                return await Task.Run(() => BadRequest($"Ya existe un grupo de configuracion con ese nombre."));
                            }
                        }
                            _GrupoConfiguracionInteresesS.FechaCreacion = DateTime.Now;
                            _GrupoConfiguracionInteresesS.UsuarioCreacion = HttpContext.Session.GetString("user");
                            var updateresult = await Update(_GrupoConfiguracionInteresesS.Id, _GrupoConfiguracionInteresesS);
                        
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }

                return Json(_GrupoConfiguracionInteresesS);
            }
        }

        [HttpPost]
        public async Task<ActionResult<GrupoConfiguracionIntereses>> Delete([FromBody]GrupoConfiguracionIntereses _GrupoConfiguracionIntereses)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/GrupoConfiguracionIntereses/Delete", _GrupoConfiguracionIntereses);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracionIntereses = JsonConvert.DeserializeObject<GrupoConfiguracionIntereses>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _GrupoConfiguracionIntereses }, Total = 1 });
        }
    }
}