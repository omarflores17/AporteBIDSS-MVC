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
    public class SalesTypeController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public SalesTypeController(ILogger<SalesTypeController> logger
            , IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<JsonResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<SalesType> _customers = new List<SalesType>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesType");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<SalesType>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_customers.ToDataSourceResult(request));

        }




    }
}