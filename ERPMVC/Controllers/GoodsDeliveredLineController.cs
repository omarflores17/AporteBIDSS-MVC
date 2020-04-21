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
    public class GoodsDeliveredLineController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public GoodsDeliveredLineController(ILogger<GoodsDeliveredLineController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        [HttpGet("[controller]/[action]")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> pvwGoodsDeliveredLine([FromBody]GoodsDeliveredLine _GoodsDeliveredLinep)
        {
            GoodsDeliveredLine _GoodsDeliveredLine = new GoodsDeliveredLine();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GoodsDeliveredLine/GetGoodsDeliveredLineById/" + _GoodsDeliveredLinep.GoodsDeliveredLinedId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GoodsDeliveredLine = JsonConvert.DeserializeObject<GoodsDeliveredLine>(valorrespuesta);

                }

                if (_GoodsDeliveredLine == null)
                {
                    _GoodsDeliveredLine = new GoodsDeliveredLine();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView("~/Views/GoodsDelivered/pvwGoodsDeliveredDetailMant.cshtml", _GoodsDeliveredLine);

        }


        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<GoodsDeliveredLine> _GoodsDeliveredLine = new List<GoodsDeliveredLine>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GoodsDeliveredLine/GetGoodsDeliveredLine");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GoodsDeliveredLine = JsonConvert.DeserializeObject<List<GoodsDeliveredLine>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _GoodsDeliveredLine.ToDataSourceResult(request);

        }


        [HttpPost("[controller]/[action]")]
          //public async Task<ActionResult<GoodsDeliveredLine>> SetLinesInSession([FromBody]dynamic dto)
          public  ActionResult<GoodsDeliveredLine> SetLinesInSession([FromBody]GoodsDeliveredLine _GoodsDeliveryAuthorizationLine)
        {

           
           // GoodsDeliveredLine _GoodsDeliveryAuthorizationLine = new GoodsDeliveredLine();
            try
            {
             //    _GoodsDeliveryAuthorizationLine = JsonConvert.DeserializeObject<GoodsDeliveredLine>(dto);
                List <GoodsDeliveredLine> _GoodsDeliveredLine = new List<GoodsDeliveredLine>();
                _GoodsDeliveredLine = JsonConvert.DeserializeObject<List<GoodsDeliveredLine>>(HttpContext.Session.GetString("listadoproductosGoodsDelivered"));

                if (_GoodsDeliveredLine == null) { _GoodsDeliveredLine = new List<GoodsDeliveredLine>(); }
                if(_GoodsDeliveryAuthorizationLine!=null)
                _GoodsDeliveredLine.Add(_GoodsDeliveryAuthorizationLine);
                //  GoodsDeliveryAuthorizationLine _listGoodsDeliveryAuthorizationLine = new GoodsDeliveryAuthorizationLine();
                // string serialzado = JsonConvert.SerializeObject(_GoodsDeliveryAuthorizationLine).ToString();
                HttpContext.Session.SetString("listadoproductosGoodsDelivered", JsonConvert.SerializeObject(_GoodsDeliveredLine).ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_GoodsDeliveryAuthorizationLine);
        }


        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetGoodsDeliveredLineByGoodsDeliveredId([DataSourceRequest]DataSourceRequest request, GoodsDeliveredLine _GoodsDeliveredLinep)
        {
            List<GoodsDeliveredLine> _GoodsDeliveredLine = new List<GoodsDeliveredLine>();
            try
            {
                if (HttpContext.Session.Get("listadoproductosGoodsDelivered") == null
                   || HttpContext.Session.GetString("listadoproductosGoodsDelivered") == "")
                {
                    if (_GoodsDeliveredLinep.GoodsDeliveredId > 0)
                    {
                        string serialzado = JsonConvert.SerializeObject(_GoodsDeliveredLinep).ToString();
                        HttpContext.Session.SetString("listadoproductosGoodsDelivered", serialzado);
                    }
                }
                else
                {
                    _GoodsDeliveredLine = JsonConvert.DeserializeObject<List<GoodsDeliveredLine>>(HttpContext.Session.GetString("listadoproductosGoodsDelivered"));
                }


                if (_GoodsDeliveredLinep.GoodsDeliveredId > 0)
                {

                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();

                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/GoodsDeliveredLine/GetGoodsReceivedLineByGoodsDeliveredId/" + _GoodsDeliveredLinep.GoodsDeliveredId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _GoodsDeliveredLine = JsonConvert.DeserializeObject<List<GoodsDeliveredLine>>(valorrespuesta);
                        HttpContext.Session.SetString("listadoproductosGoodsDelivered", JsonConvert.SerializeObject(_GoodsDeliveredLine).ToString());
                    }
                }
                else
                {

                    List<GoodsDeliveredLine> _existelinea = new List<GoodsDeliveredLine>();
                    if (HttpContext.Session.GetString("listadoproductosGoodsDelivered") != "")
                    {
                        _GoodsDeliveredLine = JsonConvert.DeserializeObject<List<GoodsDeliveredLine>>(HttpContext.Session.GetString("listadoproductosGoodsDelivered"));
                        _existelinea = _GoodsDeliveredLine.Where(q => q.GoodsDeliveredLinedId == _GoodsDeliveredLinep.GoodsDeliveredLinedId).ToList();
                    }

                    if (_GoodsDeliveredLinep.GoodsDeliveredLinedId > 0 && _existelinea.Count == 0)
                    {
                        _GoodsDeliveredLine.Add(_GoodsDeliveredLinep);
                        HttpContext.Session.SetString("listadoproductosGoodsDelivered", JsonConvert.SerializeObject(_GoodsDeliveredLine).ToString());
                    }
                    else
                    {
                        var obj = _GoodsDeliveredLine.FirstOrDefault(x => x.GoodsDeliveredLinedId == _GoodsDeliveredLinep.GoodsDeliveredLinedId);
                        if (obj != null)
                        {
                            obj.Description = _GoodsDeliveredLinep.Description;
                            obj.Price = _GoodsDeliveredLinep.Price;
                            obj.Quantity = _GoodsDeliveredLinep.Quantity;
                            obj.Total = _GoodsDeliveredLinep.Total;
                            obj.SubProductId = _GoodsDeliveredLinep.SubProductId;
                            obj.SubProductName = _GoodsDeliveredLinep.SubProductName;
                            obj.Price = _GoodsDeliveredLinep.Price;
                            obj.SubProductId = _GoodsDeliveredLinep.SubProductId;
                            obj.SubProductName = _GoodsDeliveredLinep.SubProductName;
                            obj.QuantitySacos = _GoodsDeliveredLinep.QuantitySacos;
                            obj.UnitOfMeasureId = _GoodsDeliveredLinep.UnitOfMeasureId;
                            obj.UnitOfMeasureName = _GoodsDeliveredLinep.UnitOfMeasureName;
                            obj.WareHouseId = _GoodsDeliveredLinep.WareHouseId;
                            obj.WareHouseName = _GoodsDeliveredLinep.WareHouseName;
                            obj.NoCD = _GoodsDeliveredLinep.NoCD;
                            obj.ControlPalletsId = _GoodsDeliveredLinep.ControlPalletsId;
                            obj.CenterCostId = _GoodsDeliveredLinep.CenterCostId;
                            obj.Description = _GoodsDeliveredLinep.Description;

                        }

                        HttpContext.Session.SetString("listadoproductosGoodsDelivered", JsonConvert.SerializeObject(_GoodsDeliveredLine).ToString());

                    }


                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _GoodsDeliveredLine.ToDataSourceResult(request);

        }







        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<GoodsDeliveredLine>> SaveGoodsDeliveredLine([FromBody]GoodsDeliveredLine _GoodsDeliveredLine)
        {

            try
            {
                GoodsDeliveredLine _listGoodsDeliveredLine = new GoodsDeliveredLine();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/GoodsDeliveredLine/GetGoodsDeliveredLineById/" + _GoodsDeliveredLine.GoodsDeliveredLinedId);
                string valorrespuesta = "";
                _GoodsDeliveredLine.FechaModificacion = DateTime.Now;
                _GoodsDeliveredLine.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listGoodsDeliveredLine = JsonConvert.DeserializeObject<GoodsDeliveredLine>(valorrespuesta);
                }

                if (_listGoodsDeliveredLine.GoodsDeliveredLinedId == 0)
                {
                    _GoodsDeliveredLine.FechaCreacion = DateTime.Now;
                    _GoodsDeliveredLine.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_GoodsDeliveredLine);
                }
                else
                {
                    var updateresult = await Update(_GoodsDeliveredLine.GoodsDeliveredLinedId, _GoodsDeliveredLine);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_GoodsDeliveredLine);
        }

        // POST: GoodsDeliveredLine/Insert
        [HttpPost("[controller]/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<GoodsDeliveredLine>> Insert(GoodsDeliveredLine _GoodsDeliveredLine)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _GoodsDeliveredLine.UsuarioCreacion = HttpContext.Session.GetString("user");
                _GoodsDeliveredLine.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/GoodsDeliveredLine/Insert", _GoodsDeliveredLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GoodsDeliveredLine = JsonConvert.DeserializeObject<GoodsDeliveredLine>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_GoodsDeliveredLine);
            // return new ObjectResult(new DataSourceResult { Data = new[] { _GoodsDeliveredLine }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<GoodsDeliveredLine>> Update(Int64 id, GoodsDeliveredLine _GoodsDeliveredLine)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/GoodsDeliveredLine/Update", _GoodsDeliveredLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GoodsDeliveredLine = JsonConvert.DeserializeObject<GoodsDeliveredLine>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _GoodsDeliveredLine }, Total = 1 });
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<GoodsDeliveredLine>> Delete([FromBody]GoodsDeliveredLine _GoodsDeliveredLine)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/GoodsDeliveredLine/Delete", _GoodsDeliveredLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _GoodsDeliveredLine = JsonConvert.DeserializeObject<GoodsDeliveredLine>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            return new ObjectResult(new DataSourceResult { Data = new[] { _GoodsDeliveredLine }, Total = 1 });
        }





    }
}