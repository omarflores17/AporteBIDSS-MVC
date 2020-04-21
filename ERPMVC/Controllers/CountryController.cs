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
    public class CountryController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public CountryController(ILogger<CountryController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        // GET: Customer
        public ActionResult Country()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Country> _cais = new List<Country>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Country/GetCountry");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _cais = JsonConvert.DeserializeObject<List<Country>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_cais.ToDataSourceResult(request));

        }
        //--------------------------------------------------------------------------------------
        [HttpGet]
        public async Task<JsonResult> GetBOX([DataSourceRequest]DataSourceRequest request)
        {
            List<Country> _Country = new List<Country>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Country/GetCountry");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Country = JsonConvert.DeserializeObject<List<Country>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Country);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddCountry([FromBody]CountryDTO _sarpara)
        {
            CountryDTO _Country = new CountryDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Country/GetCountryById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Country = JsonConvert.DeserializeObject<CountryDTO>(valorrespuesta);

                }

                if (_Country == null)
                {
                    _Country = new CountryDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Country);

        }


        [HttpPost]
        public async Task<ActionResult<Country>> SaveCountry([FromBody]CountryDTO _CountryP)
        {

            Country _Country = _CountryP;
            try
            {
                // DTO_NumeracionSAR _liNumeracionSAR = new DTO_NumeracionSAR();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Country/GetCountryById/" + _Country.Id);
                string valorrespuesta = "";
                _Country.FechaModificacion = DateTime.Now;
                _Country.Usuariomodificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Country = JsonConvert.DeserializeObject<CountryDTO>(valorrespuesta);
                }

                if (_Country == null) { _Country = new Models.Country(); }

                if (_CountryP.Id == 0)
                {
                    _Country.FechaCreacion = DateTime.Now;
                    _Country.Usuariocreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_CountryP);
                }
                else
                {
                    _CountryP.Usuariocreacion = _Country.Usuariocreacion;
                    _CountryP.FechaCreacion = _Country.FechaCreacion;
                    var updateresult = await Update(_Country.Id, _CountryP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Country);
        }


        //--------------------------------------------------------------------------------------
        // POST: Country/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Country _CountryP)
        {
            Country _Country = _CountryP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Country.Usuariocreacion = HttpContext.Session.GetString("user");
                _Country.Usuariomodificacion = HttpContext.Session.GetString("user");
                _Country.FechaCreacion = DateTime.Now;
                _Country.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Country/Insert", _Country);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Country = JsonConvert.DeserializeObject<Country>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Country }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Int64 Id, Country _CountryP)
        {
            Country _Country = _CountryP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Country.FechaModificacion = DateTime.Now;
                _Country.Usuariomodificacion = HttpContext.Session.GetString("user");
                var result = await _client.PutAsJsonAsync(baseadress + "api/Country/Update", _Country);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Country = JsonConvert.DeserializeObject<Country>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Country }, Total = 1 });
        }

        
        
        [HttpPost]
        public async Task<ActionResult<Country>> Delete(Int64 Id, Country _CountryP)
        {
            Country _Country = _CountryP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/Country/Delete", _Country);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Country = JsonConvert.DeserializeObject<Country>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Country }, Total = 1 });
        }






    }
}