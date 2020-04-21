﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.Helpers;
using ERPMVC.Models;
using ERPMVC.DTO;
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
    public class PuestoController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public PuestoController(ILogger<PuestoController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        // GET: Customer
        public ActionResult Puesto()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Puesto> _cais = new List<Puesto>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Puesto/GetPuesto");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _cais = JsonConvert.DeserializeObject<List<Puesto>>(valorrespuesta);
                    _cais = _cais.OrderByDescending(q => q.IdPuesto).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_cais.ToDataSourceResult(request));

        }
        [HttpPost]
        public async Task<ActionResult<Puesto>> Delete([FromBody]Puesto _puesto)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Puesto/Delete", _puesto);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _puesto = JsonConvert.DeserializeObject<Puesto>(valorrespuesta);
                    if (_puesto.IdPuesto == 0)
                    {
                        return await Task.Run(() => BadRequest($"No se puede eliminar el Puesto porque ya esta siendo usada."));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _puesto }, Total = 1 });
        }

        //--------------------------------------------------------------------------------------
        [HttpGet]
        public async Task<JsonResult> GetBOX([DataSourceRequest]DataSourceRequest request)
        {
            List<Puesto> _Puesto = new List<Puesto>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Puesto/GetPuesto");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Puesto = JsonConvert.DeserializeObject<List<Puesto>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Puesto);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddPuesto([FromBody]PuestoDTO _sarpara)
        {
            PuestoDTO _Puesto = new PuestoDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Puesto/GetPuestoById/" + _sarpara.IdPuesto);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Puesto = JsonConvert.DeserializeObject<PuestoDTO>(valorrespuesta);

                }

                if (_Puesto == null)
                {
                    _Puesto = new PuestoDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Puesto);

        }
        [HttpPost]
        public async Task<ActionResult<Puesto>> SavePuesto([FromBody]Puesto _PuestoP)
        {
            string valorrespuesta = "";
            try
            {
                Puesto _listPuesto = new Puesto();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Puesto/GetPuestoByName/" + _PuestoP.NombrePuesto);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listPuesto = JsonConvert.DeserializeObject<Puesto>(valorrespuesta);
                    if (_listPuesto != null)
                    {
                        if (_listPuesto.NombrePuesto == _PuestoP.NombrePuesto)
                        {
                            return await Task.Run(() => BadRequest($"Ya existe un puesto para este departamento con el mismo nombre."));
                        }
                    }
                }

                result = await _client.GetAsync(baseadress + "api/Puesto/GetPuestoById/" + _PuestoP.IdPuesto);

                _PuestoP.FechaModificacion = DateTime.Now;
                _PuestoP.Usuariomodificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listPuesto = JsonConvert.DeserializeObject<Puesto>(valorrespuesta);
                }

                if (_listPuesto == null) { _listPuesto = new Models.Puesto(); }

                if (_listPuesto.IdPuesto == 0)
                {
                    _PuestoP.FechaCreacion = DateTime.Now;
                    _PuestoP.Usuariocreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_PuestoP);
                }
                else
                {
                    var updateresult = await Update(_PuestoP.IdPuesto, _PuestoP);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return Json(_PuestoP);
        }

        //--------------------------------------------------------------------------------------
        // POST: Puesto/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Puesto _PuestoP)
        {
            Puesto _Puesto = _PuestoP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Puesto.Usuariocreacion = HttpContext.Session.GetString("user");
                _Puesto.Usuariomodificacion = HttpContext.Session.GetString("user");
                _Puesto.FechaCreacion = DateTime.Now;
                _Puesto.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Puesto/Insert", _Puesto);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Puesto = JsonConvert.DeserializeObject<Puesto>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Puesto }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Int64 Id, Puesto _PuestoP)
        {
            Puesto _Puesto = _PuestoP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Puesto.FechaModificacion = DateTime.Now;
                _Puesto.Usuariomodificacion = HttpContext.Session.GetString("user");
                var result = await _client.PutAsJsonAsync(baseadress + "api/Puesto/Update", _Puesto);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Puesto = JsonConvert.DeserializeObject<Puesto>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Puesto }, Total = 1 });
        }

    }
}