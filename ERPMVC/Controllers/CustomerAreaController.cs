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
    public class CustomerAreaController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public CustomerAreaController(ILogger<CustomerAreaController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }


        [HttpGet("[controller]/[action]")]
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> pvwCustomerArea([FromBody]CustomerArea _CustomerAreap)
        {
            CustomerArea _CustomerArea = new CustomerArea();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerArea/GetCustomerAreaById/" + _CustomerAreap.CustomerAreaId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<CustomerArea>(valorrespuesta);
                    if (_CustomerArea != null)
                    {
                        if (_CustomerArea.CustomerAreaProduct != null)
                        {

                            int i = 1;
                            foreach (var item in _CustomerArea.CustomerAreaProduct)
                            {
                                _CustomerArea.Productos.Add(item.ProductId);
                                _CustomerArea.listaproductos += item.ProductId + (i< _CustomerArea.CustomerAreaProduct.Count?",":"");
                            }
                        }
                    }
                }

                if (_CustomerArea == null)
                {
                    _CustomerArea = new CustomerArea();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_CustomerArea);

        }


        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<CustomerArea> _CustomerArea = new List<CustomerArea>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerArea/GetCustomerArea");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<List<CustomerArea>>(valorrespuesta);
                    _CustomerArea = _CustomerArea.OrderByDescending(q => q.CustomerAreaId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _CustomerArea.ToDataSourceResult(request);

        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> GetCustomerAreaById([DataSourceRequest]DataSourceRequest request, [FromBody]CustomerAreaDTO _CustomerAreap)
        {
            CustomerArea _CustomerArea = new CustomerArea();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerArea/GetCustomerAreaById/" + _CustomerAreap.CustomerAreaId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<CustomerArea>(valorrespuesta);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_CustomerArea);

        }




        public async Task<ActionResult> Virtualization_Read([DataSourceRequest] DataSourceRequest request)
        {
            var res = await GetCustomerArea();
            return Json(res.ToDataSourceResult(request));
        }

        public async Task<ActionResult> Orders_ValueMapper(Int64[] values)
        {
            var indices = new List<Int64>();

            if (values != null && values.Any())
            {
                var index = 0;

                foreach (var order in await GetCustomerArea())
                {
                    if (values.Contains(order.CustomerAreaId))
                    {
                        indices.Add(index);
                    }

                    index += 1;
                }
            }

            return Json(indices);
        }

        private async Task<List<CustomerArea>> GetCustomerArea()
        {
            List<CustomerArea> _CustomerArea = new List<CustomerArea>();

            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerArea/GetCustomerArea");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<List<CustomerArea>>(valorrespuesta);
                    _CustomerArea = (from c in _CustomerArea
                                     select new CustomerArea
                                    {                                     
                                       CustomerAreaId = c.CustomerAreaId,
                                       CustomerName = "Id: " + c.CustomerAreaId + " || Nombre: " + c.CustomerName + "|| Fecha:" + c.DocumentDate + "|| Area utilizada:" + c.UsedArea,
                                       DocumentDate = c.DocumentDate,
                                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return _CustomerArea;
        }




        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<CustomerArea>> SaveCustomerArea([FromBody]CustomerArea _CustomerArea)
      //  public async Task<ActionResult<CustomerArea>> SaveCustomerArea([FromBody]dynamic dto)
        {
            CustomerArea _listCustomerArea = new CustomerArea();
            try
            {
              //  CustomerArea _CustomerArea = JsonConvert.DeserializeObject<CustomerArea>(dto);
               
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerArea/GetCustomerAreaById/" + _CustomerArea.CustomerAreaId);
                string valorrespuesta = "";
                _CustomerArea.FechaModificacion = DateTime.Now;
                _CustomerArea.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listCustomerArea = JsonConvert.DeserializeObject<CustomerArea>(valorrespuesta);
                }

                if (_listCustomerArea == null) { _listCustomerArea = new CustomerArea(); }

                if (_listCustomerArea.CustomerAreaId == 0)
                {
                    _CustomerArea.FechaCreacion = DateTime.Now;
                    _CustomerArea.UsuarioCreacion = HttpContext.Session.GetString("user");
                    foreach (var item in _CustomerArea.Productos)
                    {
                        if (_CustomerArea.CustomerAreaProduct == null) { _CustomerArea.CustomerAreaProduct = new List<CustomerAreaProduct>(); } 
                        _CustomerArea.CustomerAreaProduct.Add(new CustomerAreaProduct { ProductId = item,
                            CustomerAreaId = _CustomerArea.CustomerAreaId, FechaCreacion = DateTime.Now, FechaMoficacion = DateTime.Now });
                    }
                    var insertresult = await Insert(_CustomerArea);
                }
                else
                {
                  

                    var updateresult = await Update(_CustomerArea.CustomerAreaId, _CustomerArea);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_listCustomerArea);
        }

        // POST: CustomerArea/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<CustomerArea>> Insert(CustomerArea _CustomerArea)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _CustomerArea.UsuarioCreacion = HttpContext.Session.GetString("user");
                _CustomerArea.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/CustomerArea/Insert", _CustomerArea);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<CustomerArea>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_CustomerArea);
            // return new ObjectResult(new DataSourceResult { Data = new[] { _CustomerArea }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerArea>> Update(Int64 id, CustomerArea _CustomerArea)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/CustomerArea/Update", _CustomerArea);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<CustomerArea>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _CustomerArea }, Total = 1 });
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerArea>> Delete([FromBody]CustomerArea _CustomerArea)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/CustomerArea/Delete", _CustomerArea);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerArea = JsonConvert.DeserializeObject<CustomerArea>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            return new ObjectResult(new DataSourceResult { Data = new[] { _CustomerArea }, Total = 1 });
        }





    }
}