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
    public class CurrencyController : Controller
    {

        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public CurrencyController(ILogger<CurrencyController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;

        }

        public ActionResult CurrencyList()
        {
            return View();
        }



        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Currency> _Currency = new List<Currency>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Currency/GetCurrency");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<List<Currency>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Json(_Currency.ToDataSourceResult(request));

        }


        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetCurrency([DataSourceRequest]DataSourceRequest request)
        {
            List<Currency> _Currency = new List<Currency>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Currency/GetCurrency");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<List<Currency>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Json(_Currency.ToDataSourceResult(request));

        }


        [HttpPost]
        public async Task<ActionResult<Currency>> SaveCurrency([FromBody]CurrencyDTO _CurrencyS)
        {

            Currency _Currency = _CurrencyS;
            try
            {
                // DTO_NumeracionSAR _liNumeracionSAR = new DTO_NumeracionSAR();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Currency/GetCurrencyById/" + _Currency.CurrencyId);
                string valorrespuesta = "";
                _Currency.FechaModificacion = DateTime.Now;
                _Currency.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<CurrencyDTO>(valorrespuesta);
                }

                if (_Currency == null) { _Currency = new Models.Currency(); }

                if (_CurrencyS.CurrencyId == 0)
                {
                    _Currency.FechaCreacion = DateTime.Now;
                    _Currency.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_CurrencyS);
                }
                else
                {
                    _CurrencyS.UsuarioCreacion = _Currency.UsuarioCreacion;
                    _CurrencyS.FechaCreacion = _Currency.FechaCreacion;
                    var updateresult = await Update(_Currency.CurrencyId, _CurrencyS);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Currency);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwCurrency([FromBody]CurrencyDTO _sarpara)
        {
            CurrencyDTO _Currency = new CurrencyDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Currency/GetCurrencyById/" + _sarpara.CurrencyId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<CurrencyDTO>(valorrespuesta);

                }

                if (_Currency == null)
                {
                    _Currency = new CurrencyDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Currency);

        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Currency _Currencyp)
        {
            Currency _Currency = _Currencyp;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Currency.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Currency.UsuarioModificacion = HttpContext.Session.GetString("user");
                _Currency.FechaCreacion = DateTime.Now;
                _Currency.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Currency/Insert", _Currency);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<Currency>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Currency }, Total = 1 });
        }



        [HttpPost]
        public async Task<IActionResult> Update(Int64 CurrencyId, Currency _Currencyp)
        {
            Currency _Currency = _Currencyp;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Currency.FechaModificacion = DateTime.Now;
                _Currency.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Currency/Update", _Currency);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<Currency>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Currency }, Total = 1 });
        }


        [HttpDelete("CurrencyId")]
        public async Task<ActionResult<Currency>> Delete(Int64 CurrencyId, Currency _Currencyp)
        {
            Currency _Currency = _Currencyp;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/Currency/Delete", _Currency);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<Currency>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Currency }, Total = 1 });
        }


        public async Task<ActionResult> GetCurrencyP([DataSourceRequest]DataSourceRequest request)
        {
            List<Currency> _cais = new List<Currency>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Currency/GetCurrency");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _cais = JsonConvert.DeserializeObject<List<Currency>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_cais.ToDataSourceResult(request));

        }


    }
}