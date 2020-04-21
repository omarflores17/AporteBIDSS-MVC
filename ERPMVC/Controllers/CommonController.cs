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
    public class CommonController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;

        public CommonController(ILogger<CommonController> logger, IOptions<MyConfig> config)
        {
            _config = config;
            this._logger = logger;
        }


        [HttpPost("[controller]/[action]")]
        public  ActionResult ClearSession([FromBody]List<string> sesiones)
        {
            foreach (var item in sesiones)
            {
                HttpContext.Session.SetString(item, "");
            }
           
            return Ok();
        }

        [HttpGet("[controller]/GetUserByEmail")]
        public async Task<ActionResult> GetUserByEmail()
        {
            UserInfo _UserInfo = new UserInfo();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Usuario/GetUserByEmail/" + HttpContext.Session.GetString("user"));
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _UserInfo = JsonConvert.DeserializeObject<UserInfo>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_UserInfo);
        }
           
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetCustomer([DataSourceRequest]DataSourceRequest request)
        {
             List<Customer> _clientes = new List<Customer>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomer");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<Customer>>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_clientes.ToDataSourceResult(request));

        }



        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetProduct([DataSourceRequest]DataSourceRequest request)
        {
            List<Product> _clientes = new List<Product>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProduct");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _clientes = JsonConvert.DeserializeObject<List<Product>>(valorrespuesta);

                    _clientes = (from c in _clientes
                                 select new Product
                               {
                                   ProductId = c.ProductId,
                                   ProductCode = c.ProductCode,
                                   ProductName = c.ProductCode + "--" + c.ProductName,
                                   Barcode = c.Barcode,
                                   BranchId = c.BranchId,
                                   Correlative = c.Correlative,
                                   CurrencyId = c.CurrencyId,
                                   DefaultBuyingPrice = c.DefaultBuyingPrice,
                                   DefaultSellingPrice = c.DefaultSellingPrice,
                                   Description = c.Description,
                                   FlagConsignacion = c.FlagConsignacion,
                                   FundingInterestRateId = c.FundingInterestRateId,
                                   GrupoId = c.GrupoId,
                                   IdEstado = c.IdEstado,
                                   LineaId = c.LineaId,
                                   MarcaId = c.MarcaId,
                                   Modelo = c.Modelo,
                                   PorcentajeDescuento = c.PorcentajeDescuento,
                                   Prima = c.Prima,
                                   ProductTypeId = c.ProductTypeId,
                                   Serie = c.Serie,
                                   SerieChasis = c.SerieChasis,
                                   SerieMotor = c.SerieMotor,
                                   TaxId = c.TaxId,
                                   UnitOfMeasureId = c.UnitOfMeasureId,
                                   Valor_prima = c.Valor_prima,
                                   Branch = c.Branch,
                                   Grupo = c.Grupo,
                                   Linea = c.Linea,
                                   Estado = c.Estado
                               }
                              ).ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_clientes.ToDataSourceResult(request));

        }





    }
}