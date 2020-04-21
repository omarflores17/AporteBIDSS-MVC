using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.Helpers;
using ERPMVC.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    [Authorize]


    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class LineasController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public LineasController(ILogger<LineasController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }

        public IActionResult LineaList()
        {
            return View();
        }

        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwLineas([FromBody]Linea _unidad)

        {
            Linea _Linea = new Linea();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Linea/GetLineaById/" + _unidad.LineaId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<Linea>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)
                    var result2 = await _client.GetAsync(baseadress + "api/Estados/GetEstadosByGrupo/" + 2);
                    if (result2.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result2.Content.ReadAsStringAsync());
                        var estados = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                        if (_Linea == null)
                        {
                            ViewData["estados"] = new SelectList(estados, "IdEstado", "NombreEstado");
                            _Linea = new Linea();
                        }
                        else
                        {
                            ViewData["estados"] = new SelectList(estados, "IdEstado", "NombreEstado", _Linea.IdEstado);
                            //ViewData["estadoUnidad"] = _Linea.IdEstado;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Linea);

        }

        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetLineaJson([DataSourceRequest]DataSourceRequest request)
        {
            List<Linea> _Linea = new List<Linea>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Linea/GetLinea");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<List<Linea>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Linea.ToDataSourceResult(request));

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetLinea([DataSourceRequest]DataSourceRequest request)
        {
            List<Linea> _Linea = new List<Linea>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Linea/GetLinea");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<List<Linea>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Linea.ToDataSourceResult(request);

        }
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetLineas([DataSourceRequest]DataSourceRequest request)
        {
            List<Linea> _Linea = new List<Linea>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Linea/GetLinea");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<List<Linea>>(valorrespuesta);
                    _Linea = (from c in _Linea.OrderBy(q => q.LineaId)
                              select new Linea
                              {
                                  LineaId = c.LineaId,
                                  Descripcion = c.LineaId + "--" + c.Descripcion,
                                  IdEstado = c.IdEstado,
                                  Estado = c.Estado
                              }).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Linea.ToDataSourceResult(request);

        }



        [HttpPost]
        public async Task<ActionResult<Linea>> SaveLinea([FromBody]Linea _Linea)
        {

            try
            {
                Linea _listLinea = new Linea();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Linea/GetLineaById/" + _Linea.LineaId);
                string valorrespuesta = "";
                _Linea.FechaModificacion = DateTime.Now;
                _Linea.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listLinea = JsonConvert.DeserializeObject<Linea>(valorrespuesta);
                }

                if (_listLinea == null) { _listLinea = new Models.Linea(); }

                if (_listLinea.LineaId == 0)
                {
                    _Linea.FechaCreacion = DateTime.Now;
                    _Linea.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_Linea);
                }
                else
                {
                    _Linea.FechaCreacion = _listLinea.FechaCreacion;
                    _Linea.UsuarioCreacion = _listLinea.UsuarioCreacion;
                    var updateresult = await Update(_Linea.LineaId, _Linea);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Linea);
        }

        // POST: Linea/Insert
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Linea>> Insert(Linea _Linea)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Linea.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Linea.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Linea/Insert", _Linea);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<Linea>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Linea);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Linea }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<Linea>> Update(Int64 id, Linea _Linea)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Linea/Update", _Linea);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<Linea>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Linea);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Linea }, Total = 1 });
        }


        [HttpPost]
        public async Task<ActionResult<Linea>> Delete([FromBody]Linea _Linea)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Linea/Delete", _Linea);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Linea = JsonConvert.DeserializeObject<Linea>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _Linea }, Total = 1 });
        }





    }
}