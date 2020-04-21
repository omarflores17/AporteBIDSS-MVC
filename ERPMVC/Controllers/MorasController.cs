using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.DTO;
using ERPMVC.Helpers;
using ERPMVC.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MorasController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;


        public MorasController(ILogger<HomeController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }


        // GET: Moras
        public ActionResult Moras()
        {
            return View();
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetMora([DataSourceRequest]DataSourceRequest request)
        {
            List<Moras> _customers = new List<Moras>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Moras/GetMora");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Moras>>(valorrespuesta);
                    _customers = _customers.OrderByDescending(q => q.Id).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return Json(_customers.ToDataSourceResult(request));

        }


        [HttpGet]
        public async Task<JsonResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Moras> _customers = new List<Moras>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Moras/GetMoras");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Moras>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return Json(_customers.ToDataSourceResult(request));

        }

        [HttpPost]
        public async Task<ActionResult<Moras>> SaveMoras([FromBody]MorasDTO _MorasP)
        {

            Moras _Moras = new Moras();
            try
            {
                string valorrespuesta = "";
                Moras _listMoras = new Moras();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Moras/GetMorasByName/" + _MorasP.Nombre);
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Moras = JsonConvert.DeserializeObject<Moras>(valorrespuesta);
                    if (_Moras != null)
                    {
                        if (_Moras.Id != _MorasP.Id)
                        {
                            if (_Moras.Nombre == _MorasP.Nombre)
                            {
                                return await Task.Run(() => BadRequest($"Ya existe una regla registrada con ese nombre."));
                            }
                        }
                    }
                }
                
                result = await _client.GetAsync(baseadress + "api/Moras/GetMorasById/" + _MorasP.Id);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Moras = JsonConvert.DeserializeObject<Moras>(valorrespuesta);
                }

                if (_Moras == null) { _Moras = new Models.Moras(); }

                if (_MorasP.Id == 0)
                {
                    _MorasP.FechaCreacion = DateTime.Now;
                    _MorasP.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_MorasP);
                }
                else
                {
                    _MorasP.FechaCreacion = _Moras.FechaCreacion;
                    _MorasP.UsuarioCreacion = _Moras.UsuarioCreacion;
                    _MorasP.FechaModificacion = DateTime.Now;
                    _MorasP.UsuarioModificacion = HttpContext.Session.GetString("user");
                    var updateresult = await Update(_MorasP.Id, _MorasP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Moras);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddMoras([FromBody]MorasDTO _sarpara)
        {
            MorasDTO _Moras = new MorasDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Moras/GetMorasById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Moras = JsonConvert.DeserializeObject<MorasDTO>(valorrespuesta);

                }

                if (_Moras == null)
                {
                    _Moras = new MorasDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Moras);

        }

        // POST: Moras/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Moras _Mora)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Mora.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Mora.FechaCreacion = DateTime.Now;
                _Mora.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Moras/Insert", _Mora);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Mora = JsonConvert.DeserializeObject<Moras>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Mora }, Total = 1 });
        }

        [HttpPut("Id")]
        public async Task<IActionResult> Update(Int64 Id, Moras _MorasP)
        {
            Moras _Moras = _MorasP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/Moras/Update", _Moras);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Moras = JsonConvert.DeserializeObject<Moras>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Moras }, Total = 1 });
        }
        [HttpPost]
        public async Task<ActionResult<Moras>> Delete([FromBody]Moras _mora)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/Moras/Delete", _mora);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _mora = JsonConvert.DeserializeObject<Moras>(valorrespuesta);
                    if (_mora.Id == 0)
                    {
                        return await Task.Run(() => BadRequest($"No se puede eliminar la regla porque ya esta siendo usada."));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            
            return new ObjectResult(new DataSourceResult { Data = new[] { _mora }, Total = 1 });
        }
    }
}