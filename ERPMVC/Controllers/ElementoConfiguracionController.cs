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
    public class ElementoConfiguracionController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public ElementoConfiguracionController(ILogger<ElementoConfiguracionController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        public IActionResult ElementoConfiguracion()
        {
            return View();
        }

        [HttpPost("[action]")]
        //public async Task<ActionResult> pvwElementoConfiguracion(Int64 Id = 0)
        public async Task<ActionResult> pvwElementoConfiguracion([FromBody]ElementoConfiguracionDTO _sarpara)
        {
            //ElementoConfiguracion _ElementoConfiguracion = new ElementoConfiguracion();
            ElementoConfiguracionDTO _ElementoConfiguracion = new ElementoConfiguracionDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ElementoConfiguracion/GetElementoConfiguracionById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ElementoConfiguracion = JsonConvert.DeserializeObject<ElementoConfiguracionDTO>(valorrespuesta);

                }

                if (_ElementoConfiguracion == null)
                {
                    _ElementoConfiguracion = new ElementoConfiguracionDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_ElementoConfiguracion);

        }


        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetElementoByIdConfiguracion([DataSourceRequest]DataSourceRequest request,Int64 Id)
        {
            List<ElementoConfiguracion> _clientes = new List<ElementoConfiguracion>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ElementoConfiguracion/GetElementoConfiguracionByIdConfiguracion/"+Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<ElementoConfiguracion>>(valorrespuesta);
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
            List<ElementoConfiguracion> _ElementoConfiguracion = new List<ElementoConfiguracion>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ElementoConfiguracion/GetElementoConfiguracion");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ElementoConfiguracion = JsonConvert.DeserializeObject<List<ElementoConfiguracion>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _ElementoConfiguracion.ToDataSourceResult(request);

        }


        //public async Task<ActionResult<ElementoConfiguracion>> SaveElementoConfiguracion([FromBody]ElementoConfiguracion _ElementoConfiguracion)
        //[HttpPost("[action]")]
        //public async Task<ActionResult<ElementoConfiguracion>> SaveProduct([FromBody]ElementoConfiguracionDTO _ElementoConfiguracionS)
        //{

        //    try
        //    {
        //        ElementoConfiguracion _listElementoConfiguracion = new ElementoConfiguracion();
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/ElementoConfiguracion/GetElementoConfiguracionById/" + _ElementoConfiguracionS.Id);
        //        string valorrespuesta = "";
        //        _ElementoConfiguracion.FechaModificacion = DateTime.Now;
        //        _ElementoConfiguracion.UsuarioModificacion = HttpContext.Session.GetString("user");
        //        if (result.IsSuccessStatusCode)
        //        {

        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _listElementoConfiguracion = JsonConvert.DeserializeObject<List<ElementoConfiguracion>>(valorrespuesta);


        //        }

        //        if (_listElementoConfiguracion.Id == 0)
        //        {
        //            _ElementoConfiguracion.FechaCreacion = DateTime.Now;
        //            _ElementoConfiguracion.UsuarioCreacion = HttpContext.Session.GetString("user");
        //            var insertresult = await Insert(_ElementoConfiguracion);
        //        }
        //        else
        //        {
        //            var updateresult = await Update(_ElementoConfiguracion.Id, _ElementoConfiguracion);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }

        //    return Json(_ElementoConfiguracion);
        //}

        [HttpPost("[action]")]
        public async Task<ActionResult<ElementoConfiguracion>> SaveElementoConfiguracion([FromBody]ElementoConfiguracionDTO _ElementoConfiguracionS)
        {
            ElementoConfiguracion _ElementoConfiguracion = _ElementoConfiguracionS;
            
            try
            {
                ElementoConfiguracion _listElementoConfiguracion = new ElementoConfiguracion();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ElementoConfiguracion/GetElementoConfiguracionById/" + _ElementoConfiguracion.Id);
                string valorrespuesta = "";
                _ElementoConfiguracion.FechaModificacion = DateTime.Now;
                _ElementoConfiguracion.UsuarioModificacion = HttpContext.Session.GetString("user");
                
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ElementoConfiguracion = JsonConvert.DeserializeObject<ElementoConfiguracionDTO>(valorrespuesta);
                }

                if (_ElementoConfiguracion == null) { _ElementoConfiguracion = new Models.ElementoConfiguracion(); }

                if (_ElementoConfiguracionS.Id == 0)
                {
                    _ElementoConfiguracionS.FechaCreacion = DateTime.Now;
                    _ElementoConfiguracionS.UsuarioCreacion = HttpContext.Session.GetString("user");

                    //if (_ElementoConfiguracionS.Estado == "Activo")
                    //{
                    //    _ElementoConfiguracionS.Estado = "A";
                    //}
                    //else {
                    //    _ElementoConfiguracionS.Estado = "I";

                    //}
                    
                    var insertresult = await Insert(_ElementoConfiguracionS);
                }
                else
                {
                    _ElementoConfiguracionS.UsuarioCreacion = _ElementoConfiguracion.UsuarioCreacion;
                    _ElementoConfiguracionS.FechaCreacion = _ElementoConfiguracion.FechaCreacion;
                    var updateresult = await Update(_ElementoConfiguracion.Id, _ElementoConfiguracionS);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_ElementoConfiguracion);
        }

        // POST: ElementoConfiguracion/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ElementoConfiguracion>> Insert(ElementoConfiguracion _ElementoConfiguracion)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _ElementoConfiguracion.UsuarioCreacion = HttpContext.Session.GetString("user");
                _ElementoConfiguracion.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/ElementoConfiguracion/Insert", _ElementoConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ElementoConfiguracion = JsonConvert.DeserializeObject<ElementoConfiguracion>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _ElementoConfiguracion }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ElementoConfiguracion>> Update(Int64 id, ElementoConfiguracion _ElementoConfiguracion)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/ElementoConfiguracion/Update", _ElementoConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ElementoConfiguracion = JsonConvert.DeserializeObject<ElementoConfiguracion>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _ElementoConfiguracion }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<ElementoConfiguracion>> Delete(Int64 Id, ElementoConfiguracion _ElementoConfiguracionP)
        {
            ElementoConfiguracion _ElementoConfiguracion = _ElementoConfiguracionP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/ElementoConfiguracion/Delete", _ElementoConfiguracion);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ElementoConfiguracion = JsonConvert.DeserializeObject<ElementoConfiguracion>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _ElementoConfiguracion }, Total = 1 });
        }





    }
}