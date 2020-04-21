﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.Helpers;
using ERPMVC.DTO;
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
    public class GrupoConfiguracionController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public GrupoConfiguracionController(ILogger<GrupoConfiguracionController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        public IActionResult GrupoConfiguracion()
        {
            return View();
        }

        [HttpPost("[action]")]
        //public async Task<ActionResult> pvwGrupoConfiguracion(Int64 Id = 0)
        public async Task<ActionResult> pvwGrupoConfiguracion([FromBody]GrupoConfiguracionDTO _sarpara)
        {
            //GrupoConfiguracion _GrupoConfiguracion = new GrupoConfiguracion();
            GrupoConfiguracionDTO _GrupoConfiguracion = new GrupoConfiguracionDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracion/GetGrupoConfiguracionById/" + _sarpara.IdConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracion = JsonConvert.DeserializeObject<GrupoConfiguracionDTO>(valorrespuesta);

                }

                if (_GrupoConfiguracion == null)
                {
                    _GrupoConfiguracion = new GrupoConfiguracionDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_GrupoConfiguracion);

        }


        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetElementoByIdConfiguracion([DataSourceRequest]DataSourceRequest request, Int64 Id)
        {
            List<GrupoConfiguracion> _clientes = new List<GrupoConfiguracion>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracion/GetGrupoConfiguracionByIdConfiguracion/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<GrupoConfiguracion>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_clientes.ToDataSourceResult(request));

        }

        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<GrupoConfiguracion> _GrupoConfiguracion = new List<GrupoConfiguracion>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracion/GetGrupoConfiguracion");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracion = JsonConvert.DeserializeObject<List<GrupoConfiguracion>>(valorrespuesta);
                    _GrupoConfiguracion = _GrupoConfiguracion.OrderByDescending(q => q.IdConfiguracion).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _GrupoConfiguracion.ToDataSourceResult(request);

        }


        //public async Task<ActionResult<GrupoConfiguracion>> SaveGrupoConfiguracion([FromBody]GrupoConfiguracion _GrupoConfiguracion)
        //[HttpPost("[action]")]
        //public async Task<ActionResult<GrupoConfiguracion>> SaveProduct([FromBody]GrupoConfiguracionDTO _GrupoConfiguracionS)
        //{

        //    try
        //    {
        //        GrupoConfiguracion _listGrupoConfiguracion = new GrupoConfiguracion();
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracion/GetGrupoConfiguracionById/" + _GrupoConfiguracionS.Id);
        //        string valorrespuesta = "";
        //        _GrupoConfiguracion.FechaModificacion = DateTime.Now;
        //        _GrupoConfiguracion.UsuarioModificacion = HttpContext.Session.GetString("user");
        //        if (result.IsSuccessStatusCode)
        //        {

        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _listGrupoConfiguracion = JsonConvert.DeserializeObject<List<GrupoConfiguracion>>(valorrespuesta);


        //        }

        //        if (_listGrupoConfiguracion.Id == 0)
        //        {
        //            _GrupoConfiguracion.FechaCreacion = DateTime.Now;
        //            _GrupoConfiguracion.UsuarioCreacion = HttpContext.Session.GetString("user");
        //            var insertresult = await Insert(_GrupoConfiguracion);
        //        }
        //        else
        //        {
        //            var updateresult = await Update(_GrupoConfiguracion.Id, _GrupoConfiguracion);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }

        //    return Json(_GrupoConfiguracion);
        //}

        [HttpPost("[action]")]
        public async Task<ActionResult<GrupoConfiguracion>> SaveGrupoConfiguracion([FromBody]GrupoConfiguracionDTO _GrupoConfiguracionS)
        {
            GrupoConfiguracion _GrupoConfiguracion = _GrupoConfiguracionS;

            try
            {
                GrupoConfiguracion _listGrupoConfiguracion = new GrupoConfiguracion();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GrupoConfiguracion/GetGrupoConfiguracionById/" + _GrupoConfiguracion.IdConfiguracion);
                string valorrespuesta = "";
                _GrupoConfiguracion.FechaModificacion = DateTime.Now;
                _GrupoConfiguracion.UsuarioModificacion = HttpContext.Session.GetString("user");

                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracion = JsonConvert.DeserializeObject<GrupoConfiguracionDTO>(valorrespuesta);
                }

                if (_GrupoConfiguracion == null) { _GrupoConfiguracion = new Models.GrupoConfiguracion(); }

                if (_GrupoConfiguracionS.IdConfiguracion == 0)
                {
                    _GrupoConfiguracionS.FechaCreacion = DateTime.Now;
                    _GrupoConfiguracionS.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_GrupoConfiguracionS);
                }
                else
                {
                    _GrupoConfiguracionS.UsuarioCreacion = _GrupoConfiguracion.UsuarioCreacion;
                    _GrupoConfiguracionS.FechaCreacion = _GrupoConfiguracion.FechaCreacion;
                    var updateresult = await Update(_GrupoConfiguracion.IdConfiguracion, _GrupoConfiguracionS);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_GrupoConfiguracion);
        }

        // POST: GrupoConfiguracion/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<GrupoConfiguracion>> Insert(GrupoConfiguracion _GrupoConfiguracion)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _GrupoConfiguracion.UsuarioCreacion = HttpContext.Session.GetString("user");
                _GrupoConfiguracion.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/GrupoConfiguracion/Insert", _GrupoConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracion = JsonConvert.DeserializeObject<GrupoConfiguracion>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _GrupoConfiguracion }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GrupoConfiguracion>> Update(Int64 id, GrupoConfiguracion _GrupoConfiguracion)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/GrupoConfiguracion/Update", _GrupoConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracion = JsonConvert.DeserializeObject<GrupoConfiguracion>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _GrupoConfiguracion }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<GrupoConfiguracion>> Delete(Int64 Id, GrupoConfiguracion _GrupoConfiguracionP)
        {
            GrupoConfiguracion _GrupoConfiguracion = _GrupoConfiguracionP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/GrupoConfiguracion/Delete", _GrupoConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GrupoConfiguracion = JsonConvert.DeserializeObject<GrupoConfiguracion>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _GrupoConfiguracion }, Total = 1 });
        }





    }
}