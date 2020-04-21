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
    public class Contrato_plan_pagosController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public Contrato_plan_pagosController(ILogger<Contrato_plan_pagosController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        //metodo para el detalle
        public async Task<ActionResult> Details(Int64 Nro_cuota)
        {
            Contrato_plan_pagos _Contrato_plan_pagos = new Contrato_plan_pagos();
            if (Nro_cuota == 0)
            {
                return await Task.Run(() => View(_Contrato_plan_pagos));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetContrato_plan_pagosById/" + Nro_cuota);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_plan_pagos = JsonConvert.DeserializeObject<Contrato_plan_pagos>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Contrato_plan_pagos));
        }
        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwContrato_plan_pagos(Contrato_plan_pagos Contrato_plan_pagos)
        {
            Contrato_plan_pagosDTO _Contrato_plan_pagoss = new Contrato_plan_pagosDTO();
            if (Contrato_plan_pagos.Nro_cuota == 0)
            {
                return await Task.Run(() => View(_Contrato_plan_pagoss));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetContrato_plan_pagosById/" + Contrato_plan_pagos.Nro_cuota);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_plan_pagoss = JsonConvert.DeserializeObject<Contrato_plan_pagosDTO>(valorrespuesta);
                }
                if (_Contrato_plan_pagoss == null)
                {
                    _Contrato_plan_pagoss = new Contrato_plan_pagosDTO { editar = 1 };
                }
                else
                {
                    _Contrato_plan_pagoss.editar = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Contrato_plan_pagoss);

        }
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetContrato_plan_pagosJson([DataSourceRequest]DataSourceRequest request)
        {
            List<Contrato_plan_pagos> _Contrato_plan_pagos = new List<Contrato_plan_pagos>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetUnitOfMeasure");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_plan_pagos = JsonConvert.DeserializeObject<List<Contrato_plan_pagos>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Contrato_plan_pagos.ToDataSourceResult(request));

        }


        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetContrato_plan_pagos([DataSourceRequest]DataSourceRequest request)
        {
            List<Contrato_plan_pagos> _Contrado = new List<Contrato_plan_pagos>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetContrato_plan_pagos");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrado = JsonConvert.DeserializeObject<List<Contrato_plan_pagos>>(valorrespuesta);
                    _Contrado = _Contrado.OrderByDescending(q => q.Nro_cuota).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Contrado.ToDataSourceResult(request);

        }


        [HttpPost]
        public async Task<ActionResult<Contrato_plan_pagos>> SaveContrato_plan_pagos([FromBody]Contrato_plan_pagos _Contrato_plan_pagos)
        {

            try
            {
                Contrato_plan_pagos _listContrato_plan_pagos = new Contrato_plan_pagos();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrado/GetContrato_plan_pagosById/" + _Contrato_plan_pagos.Nro_cuota);
                string valorrespuesta = "";
                _Contrato_plan_pagos.ModifiedDate = DateTime.Now;
                _Contrato_plan_pagos.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listContrato_plan_pagos = JsonConvert.DeserializeObject<Contrato_plan_pagos>(valorrespuesta);
                }

                if (_listContrato_plan_pagos == null) { _listContrato_plan_pagos = new Models.Contrato_plan_pagos(); }

                if (_listContrato_plan_pagos.Nro_cuota == 0)
                {
                    _Contrato_plan_pagos.CreatedDate = DateTime.Now;
                    _Contrato_plan_pagos.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_Contrato_plan_pagos);
                }
                else
                {
                    var updateresult = await Update(_Contrato_plan_pagos.Nro_cuota, _Contrato_plan_pagos);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Contrato_plan_pagos);
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Contrato_plan_pagos>> Insert(Contrato_plan_pagos _Contrato_plan_pagos)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Contrato_plan_pagos.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Contrato_plan_pagos.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato_plan_pagos/Insert", _Contrato_plan_pagos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_plan_pagos = JsonConvert.DeserializeObject<Contrato_plan_pagos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Contrato_plan_pagos);
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<Contrato_plan_pagos>> Update(Int64 id, Contrato_plan_pagos _Contrato_plan_pagos)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Contrato_plan_pagos/Update", _Contrato_plan_pagos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_plan_pagos = JsonConvert.DeserializeObject<Contrato_plan_pagos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Contrato_plan_pagos);
        }

        public async Task<ActionResult<Contrato_plan_pagos>> Delete([FromBody]Contrato_plan_pagos _Contrato_plan_pagos)
        {

            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato_plan_pagos/Delete", _Contrato_plan_pagos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_plan_pagos = JsonConvert.DeserializeObject<Contrato_plan_pagos>(valorrespuesta);
                    if (_Contrato_plan_pagos.Nro_cuota == 0)
                    {
                        return await Task.Run(() => BadRequest($"No se puede eliminar porque ya esta siendo usada."));
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }


            return new ObjectResult(new DataSourceResult { Data = new[] { _Contrato_plan_pagos }, Total = 1 });
        }
    }
}
