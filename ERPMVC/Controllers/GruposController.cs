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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class GruposController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public GruposController(ILogger<GruposController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }

        //public IActionResult GrupoList()
        //{
        //    return View();
        //}
        public async Task<IActionResult> GrupoList()
        {
          
            ViewData["Linea"] = await ObtenerLinea();

            return View();
        }

        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwGrupo([FromBody]GrupoDTO _unidad)

        {
            GrupoDTO _Grupo = new GrupoDTO();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupoById/" + _unidad.GrupoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<GrupoDTO>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)
                    var result2 = await _client.GetAsync(baseadress + "api/Estados/GetEstadosByGrupo/" + 1);
                    if (result2.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result2.Content.ReadAsStringAsync());
                        var estados = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                        if (_Grupo == null)
                        {
                            ViewData["estados"] = new SelectList(estados, "IdEstado", "NombreEstado");
                            _Grupo = new GrupoDTO();
                        }
                        else
                        {
                            ViewData["estados"] = new SelectList(estados, "IdEstado", "NombreEstado", _Grupo.IdEstado);
                            //ViewData["estadoUnidad"] = _Grupo.IdEstado;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Grupo);

        }
        /// <summary>
        /// Obitiene el listado de los estados!
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpGet]
        //public async Task<ActionResult> GetGrupo([DataSourceRequest]DataSourceRequest request)
        //{
        //    List<Grupo> _Grupo = new List<Grupo>();
        //    try
        //    {

        //        string baseadress = _config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupo");
        //        string valorrespuesta = "";
        //        if (result.IsSuccessStatusCode)
        //        {
        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _Grupo = JsonConvert.DeserializeObject<List<Grupo>>(valorrespuesta);

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }


        //    return _Grupo.ToDataSourceResult(request);

        //}



        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetGrupoJson([DataSourceRequest]DataSourceRequest request)
        {
            List<Grupo> _Grupo = new List<Grupo>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupo");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<List<Grupo>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Grupo.ToDataSourceResult(request));

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetGrupo([DataSourceRequest]DataSourceRequest request)
        {
            List<Grupo> _Grupo = new List<Grupo>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupo");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<List<Grupo>>(valorrespuesta);
                    _Grupo = (from c in _Grupo.OrderBy(q => q.GrupoId)
                              select new Grupo
                              {

                                  GrupoId = c.GrupoId,
                                  Description = c.GrupoId + "--" + c.Description,
                                  LineaId = c.LineaId,
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


            return _Grupo.ToDataSourceResult(request);

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetGrupos([DataSourceRequest]DataSourceRequest request)
        {
            List<Grupo> _Grupo = new List<Grupo>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupoByEstado/" + 1);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<List<Grupo>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Grupo.ToDataSourceResult(request);

        }
        [HttpPost]
        public async Task<ActionResult<Grupo>> SaveGrupo([FromBody]GrupoDTO _Grupo)
        {
            string valorrespuesta = "";
            try
            {
                Grupo _listVendorType = new Grupo();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupoByDescripcion/" + _Grupo.Description);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<Grupo>(valorrespuesta);
                    if (_listVendorType != null && _Grupo.GrupoId == 0)
                    {
                        if (_listVendorType.Description == _Grupo.Description)
                        {
                            return await Task.Run(() => BadRequest($"Ya existe un grupo registrado con ese nombre."));
                        }
                    }

                }



                result = await _client.GetAsync(baseadress + "api/Grupo/GetGrupoById/" + _Grupo.GrupoId);

                _Grupo.FechaModificacion = DateTime.Now;
                _Grupo.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {



                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<Grupo>(valorrespuesta);
                }



                if (_listVendorType == null) { _listVendorType = new Models.Grupo(); }



                if (_listVendorType.GrupoId == 0)
                {
                    _Grupo.FechaCreacion = DateTime.Now;
                    _Grupo.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_Grupo);
                }
                else
                {
                    var updateresult = await Update(_Grupo.GrupoId, _Grupo);
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return Json(_Grupo);
        }

        // POST: Grupo/Insert
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Grupo>> Insert(Grupo _Grupo)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Grupo.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Grupo.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Grupo/Insert", _Grupo);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<Grupo>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Grupo);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Grupo }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<Grupo>> Update(Int64 id, Grupo _Grupo)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Grupo/Update", _Grupo);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<Grupo>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Grupo);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Grupo }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<Grupo>> Delete([FromBody]Grupo _Grupo)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Grupo/Delete", _Grupo);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Grupo = JsonConvert.DeserializeObject<Grupo>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }


            return Ok(_Grupo);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Grupo }, Total = 1 });
        }


        async Task<IEnumerable<Linea>> ObtenerLinea()
        {
            IEnumerable<Linea> Linea = null;
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
                    Linea = JsonConvert.DeserializeObject<IEnumerable<Linea>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            ViewData["Linea"] = Linea.FirstOrDefault();
            return Linea;

        }


    }
}