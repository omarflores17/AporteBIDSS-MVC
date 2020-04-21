﻿using System;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class KardexController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public KardexController(ILogger<KardexController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> pvwKardex(Int64 Id = 0)
        {
            Kardex _Kardex = new Kardex();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Kardex/GetKardexById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<Kardex>(valorrespuesta);

                }

                if (_Kardex == null)
                {
                    _Kardex = new Kardex();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Kardex);

        }


        //[HttpGet]
        
        //public async Task<ActionResult> SFKardex(Int32 id)
        //{
        //    try
        //    {
        //        Kardex _Kardexdto = new Kardex { KardexId = id, };
        //        return await Task.Run(() => View(_Kardexdto));
        //    }
        //    catch (Exception)
        //    {

        //        return await Task.Run(() => BadRequest("Ocurrio un error"));
        //    }

        //}

        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Kardex> _Kardex = new List<Kardex>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Kardex/GetKardex");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<List<Kardex>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Kardex.ToDataSourceResult(request);

        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> GetSaldoProductoByCertificado([DataSourceRequest]DataSourceRequest request, [FromBody]KardexParam _Kardexp)
        //public async Task<ActionResult> GetSaldoProductoByCertificado([DataSourceRequest]DataSourceRequest request, [FromBody]dynamic _Kardexd)
        {
            List<KardexLine> _KardexLine = new List<KardexLine>();
            KardexLine _saldo = new KardexLine();
            Kardex _Kardexq = new Kardex { DocumentId = _Kardexp.DocumentId, DocumentName = _Kardexp.DocumentName };
            try
            {
                // _Kardexq = JsonConvert.DeserializeObject<Kardex>(_Kardexd);

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CertificadoDeposito/GetCertificadoDepositoByNoCD/" + _Kardexq.DocumentId);
                string valorrespuesta = "";
                CertificadoDeposito _cd = new CertificadoDeposito();
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _cd = JsonConvert.DeserializeObject<CertificadoDeposito>(valorrespuesta);

                }

                if (_cd != null)
                {

                    _Kardexq.DocumentId = _cd.IdCD;

                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    result = await _client.PostAsJsonAsync(baseadress + "api/Kardex/GetSaldoProductoByCertificado", _Kardexq);
                    valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _KardexLine = JsonConvert.DeserializeObject<List<KardexLine>>(valorrespuesta);

                    }

                    _saldo = _KardexLine.Where(q => q.SubProducId == _Kardexp.SubProducId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_saldo);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult<Kardex>> SaveKardex([FromBody]Kardex _Kardex)
        {

            try
            {
                Kardex _listKardex = new Kardex();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Kardex/GetKardexById/" + _Kardex.KardexId);
                string valorrespuesta = "";
                _Kardex.FechaModificacion = DateTime.Now;
                _Kardex.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listKardex = JsonConvert.DeserializeObject<Kardex>(valorrespuesta);
                }

                if (_listKardex.KardexId == 0)
                {
                    _Kardex.FechaCreacion = DateTime.Now;
                    _Kardex.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_Kardex);
                }
                else
                {
                    var updateresult = await Update(_Kardex.KardexId, _Kardex);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Kardex);
        }

        // POST: Kardex/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Kardex>> Insert(Kardex _Kardex)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Kardex.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Kardex.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Kardex/Insert", _Kardex);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<Kardex>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_Kardex);
            // return new ObjectResult(new DataSourceResult { Data = new[] { _Kardex }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Kardex>> Update(Int64 id, Kardex _Kardex)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Kardex/Update", _Kardex);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<Kardex>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Kardex }, Total = 1 });
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<Kardex>> Delete([FromBody]Kardex _Kardex)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Kardex/Delete", _Kardex);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<Kardex>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            return new ObjectResult(new DataSourceResult { Data = new[] { _Kardex }, Total = 1 });
        }





    }
}