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
    public class CustomerTypeController : Controller
    {

        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public CustomerTypeController(ILogger<CustomerTypeController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;

        }

        public ActionResult CustomerTypeList()
        {
            return View();
        }



        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<CustomerType> _CustomerType = new List<CustomerType>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerType/Get");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerType = JsonConvert.DeserializeObject<List<CustomerType>>(valorrespuesta);
                    _CustomerType = _CustomerType.OrderByDescending(q => q.CustomerTypeId).ToList();

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Json(_CustomerType.ToDataSourceResult(request));

        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwCustomerType([FromBody]CustomerTypeDTO _sarpara)
        {
            CustomerTypeDTO _CustomerType = new CustomerTypeDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerType/GetCustomerTypeById/" + _sarpara.CustomerTypeId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerType = JsonConvert.DeserializeObject<CustomerTypeDTO>(valorrespuesta);

                }

                if (_CustomerType == null)
                {
                    _CustomerType = new CustomerTypeDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_CustomerType);
        }


        [HttpPost]
        public async Task<ActionResult<CustomerType>> SaveCustomerType([FromBody]CustomerTypeDTO _CustomerTypeS)
        {
            string valorrespuesta = "";
            try
            {
                CustomerType _CustomerType = new CustomerType();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/CustomerType/GetCustomerTypeByCustomerTypeName/" + _CustomerTypeS.CustomerTypeName);
                
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerType = JsonConvert.DeserializeObject<CustomerType>(valorrespuesta);
                    if (_CustomerType != null && _CustomerTypeS.CustomerTypeId == 0)
                    {
                        if (_CustomerType.CustomerTypeName == _CustomerTypeS.CustomerTypeName)
                        {
                            return await Task.Run(() => BadRequest($"Ya existe un Tipo de cliente con el mismo nombre."));
                        }
                    }
                }
                result = await _client.GetAsync(baseadress + "api/CustomerType/GetCustomerTypeById/" + _CustomerTypeS.CustomerTypeId);
                _CustomerTypeS.FechaModificacion = DateTime.Now;
                _CustomerTypeS.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerType = JsonConvert.DeserializeObject<CustomerTypeDTO>(valorrespuesta);
                }

                if (_CustomerType == null) { _CustomerType = new Models.CustomerType(); }

                if (_CustomerType.CustomerTypeId == 0)
                {
                    _CustomerTypeS.FechaCreacion = DateTime.Now;
                    _CustomerTypeS.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_CustomerTypeS);
                }
                else
                {
                    _CustomerTypeS.UsuarioCreacion = _CustomerType.UsuarioCreacion;
                    _CustomerTypeS.FechaCreacion = _CustomerType.FechaCreacion;
                    var updateresult = await Update(_CustomerType.CustomerTypeId, _CustomerTypeS);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_CustomerTypeS);
        }



        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(CustomerType _CustomerTypep)
        {
            CustomerType _CustomerType = _CustomerTypep;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _CustomerType.UsuarioCreacion = HttpContext.Session.GetString("user");
                _CustomerType.UsuarioModificacion = HttpContext.Session.GetString("user");
                _CustomerType.FechaCreacion = DateTime.Now;
                _CustomerType.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/CustomerType/Insert", _CustomerType);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerType = JsonConvert.DeserializeObject<CustomerType>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _CustomerType }, Total = 1 });
        }



        [HttpPut]
        public async Task<IActionResult> Update(Int64 CustomerTypeId, CustomerType _customertype)
        {
           
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _customertype.FechaModificacion = DateTime.Now;
                _customertype.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PutAsJsonAsync(baseadress + "api/CustomerType/Update", _customertype);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customertype = JsonConvert.DeserializeObject<CustomerType>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _customertype }, Total = 1 });
        }


        [HttpPost]
        public async Task<ActionResult<CustomerType>> Delete([FromBody]CustomerType _CustomerType)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/CustomerType/Delete", _CustomerType);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _CustomerType = JsonConvert.DeserializeObject<CustomerType>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _CustomerType }, Total = 1 });
        }



    }
}