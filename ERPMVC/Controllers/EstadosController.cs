﻿using System;
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
    public class EstadosController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;

        public EstadosController(ILogger<EstadosController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
            
        }

        public IActionResult Estados()
        {
            return View();
        }

        public async Task<ActionResult> pvwAddEstado([FromBody]Estados _sarpara)
        {

            EstadoDTO _Estados = new EstadoDTO();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Estados/GetEstadosById/" + _sarpara.IdEstado);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<EstadoDTO>(valorrespuesta);

                }

                if (_Estados == null)
                {
                    _Estados = new EstadoDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Estados);

        }


        /// <summary>
        /// Obitiene el listado de los estados!
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Estados> _Estados = new List<Estados>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Estados/Get");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                    _Estados = _Estados.OrderByDescending(q => q.IdEstado).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Estados.ToDataSourceResult(request);

        }

        [HttpPost]
        public async Task<ActionResult<Estados>> SaveEstados([FromBody]EstadoDTO _EstadosP)
        {
            Estados _Estados = _EstadosP;
            try
            {
                Estados _listEstados = new Estados();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Estados/GetEstadosById/" + _Estados.IdEstado);
                string valorrespuesta = "";
                _Estados.FechaModificacion = DateTime.Now;
                _Estados.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<Estados>(valorrespuesta);
                }

                if (_Estados == null) { _Estados = new Models.Estados(); }


                if (_EstadosP.IdEstado == 0)
                {
                    _EstadosP.FechaCreacion = DateTime.Now;
                    _EstadosP.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_EstadosP);
                }
                else
                {
                    _EstadosP.UsuarioCreacion = _Estados.UsuarioCreacion;
                    _EstadosP.FechaCreacion = _Estados.FechaCreacion;
                    var updateresult = await Update(_Estados.IdEstado, _EstadosP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Estados);
        }

        // POST: Estados/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<Estados>> Insert(Estados _Estadosp)
        {
             Estados _Estados = _Estadosp;
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Estados.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Estados.UsuarioModificacion = HttpContext.Session.GetString("user");
                _Estados.FechaCreacion = DateTime.Now;
                _Estados.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Estados/Insert", _Estados);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<Estados>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Estados);
            //return new ObjectResult(new DataSourceResult { Data = new[] { _Estados }, Total = 1 });
        }

        [HttpPut("IdEstado")]
        public async Task<ActionResult<Estados>> Update(Int64 IdEstado, Estados _Estados)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Estados/Update", _Estados);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<Estados>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Estados);
            //return new ObjectResult(new DataSourceResult { Data = new[] { _Estados }, Total = 1 });
        }

        [HttpDelete("IdEstado")]
        public async Task<ActionResult<Estados>> Delete(Estados _Estados)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Estados/Delete", _Estados);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<Estados>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            return new ObjectResult(new DataSourceResult { Data = new[] { _Estados }, Total = 1 });
        }


        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetGrupoEstadoUno([DataSourceRequest]DataSourceRequest request)
        {
            List<Estados> _clientes = new List<Estados>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/estados/GetEstadosByGrupo/" + 1);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            //  JsonSerializerSettings settings = new JsonSerializerSettings();
            //  settings.Formatting = Formatting.Indented;
            //  settings.TypeNameHandling = TypeNameHandling.Auto;
            //settingall
            // return Json(new { _clientes });



            return Json(_clientes.ToDataSourceResult(request));
        }


        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetEstadoByGrupo([DataSourceRequest]DataSourceRequest request, int pIdEstado)
        {
            List<Estados> _Estados = new List<Estados>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/estados/GetEstadosByGrupo/" + pIdEstado);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Estados = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            //  JsonSerializerSettings settings = new JsonSerializerSettings();
            //  settings.Formatting = Formatting.Indented;
            //  settings.TypeNameHandling = TypeNameHandling.Auto;
            //settingall
            // return Json(new { _clientes });



            return Json(_Estados.ToDataSourceResult(request));
        }

        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetGrupoEstadoCuatro([DataSourceRequest]DataSourceRequest request)
        {
            List<Estados> _clientes = new List<Estados>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/estados/GetEstadosByGrupo/" + 4);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            //  JsonSerializerSettings settings = new JsonSerializerSettings();
            //  settings.Formatting = Formatting.Indented;
            //  settings.TypeNameHandling = TypeNameHandling.Auto;
            //settingall
            // return Json(new { _clientes });



            return Json(_clientes.ToDataSourceResult(request));
        }

        /// <summary>
        /// Obtiene los estados por grupo de cotizacion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetEnviosCotizacion([DataSourceRequest]DataSourceRequest request)
        {
            List<Estados> _clientes = new List<Estados>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/estados/GetEstadosByGrupo/"+2);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_clientes.ToDataSourceResult(request));
        }


    }
}