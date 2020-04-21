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
     [Authorize]
     [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class FundingInterestRateController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public FundingInterestRateController(ILogger<FundingInterestRateController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        // GET: FundingInterestRate
        public IActionResult Index()
        {
            return View();
        }

      

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddFundingInterestRate([FromBody]FundingInterestRateDTO _sarpara)

        {
            FundingInterestRateDTO _FundingInterestRate = new FundingInterestRateDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRateById/" + _sarpara.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FundingInterestRate = JsonConvert.DeserializeObject<FundingInterestRateDTO>(valorrespuesta);

                }

                if (_FundingInterestRate == null)
                {
                    _FundingInterestRate = new FundingInterestRateDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return PartialView(_FundingInterestRate);

        }
            [HttpGet("[controller]/[action]")]
        public async Task<ActionResult<FundingInterestRate>> GetFundingInterestRate([DataSourceRequest]DataSourceRequest request, Int64 FundingInterestRateId)
        {
            FundingInterestRate _FundingInterestRatep = new FundingInterestRate();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRateById/" + FundingInterestRateId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FundingInterestRatep = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return _FundingInterestRatep;
        }


       // POST: TypeAccount/Insert
       [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(FundingInterestRate _FundingInterestRateP)
        {
            FundingInterestRate _FundingInterestRate = _FundingInterestRateP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _FundingInterestRate.UsuarioCreacion = HttpContext.Session.GetString("user");
                _FundingInterestRate.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/FundingInterestRate/Insert", _FundingInterestRate);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FundingInterestRate = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            //return Ok(_FundingInterestRate);
            return new ObjectResult(new DataSourceResult { Data = new[] { _FundingInterestRate }, Total = 1 });
        }


        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult> Update(Int64 Id, FundingInterestRate _FundingInterestRateP)
        {
            //FundingInterestRate _FundingInterestRate = _FundingInterestRateP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/FundingInterestRate/Update", _FundingInterestRateP);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FundingInterestRateP = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_FundingInterestRateP);
            //return new ObjectResult(new DataSourceResult { Data = new[] { _FundingInterestRate }, Total = 1 });
        }
      
        [HttpPost]
        public async Task<ActionResult<FundingInterestRate>> SaveFundingInterestRate([FromBody]FundingInterestRateDTO _FundingInterestRateS)
        {
            {
                string valorrespuesta = "";
                try
                {
                    FundingInterestRate _listVendorType = new FundingInterestRate();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRateByDescripcion/" + _FundingInterestRateS.Descripcion);
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _listVendorType = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);
                        if (_listVendorType != null && _FundingInterestRateS.Id == 0)
                        {
                            if (_listVendorType.Descripcion == _FundingInterestRateS.Descripcion)
                            {
                                result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRateByMonths/" + _FundingInterestRateS.Months);
                                if (result.IsSuccessStatusCode)
                                {
                                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                                    _listVendorType = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);
                                    if (_listVendorType != null && _FundingInterestRateS.Id == 0)
                                    {
                                        if (_listVendorType.Months == _FundingInterestRateS.Months)
                                        {
                                            return await Task.Run(() => BadRequest($"Ya existe una tasa de interes registrada con ese mes."));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    

                    result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRateById/" + _FundingInterestRateS.Id);

                    _FundingInterestRateS.FechaModificacion = DateTime.Now;
                    _FundingInterestRateS.UsuarioModificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _listVendorType = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);
                    }
                    
                    if (_listVendorType == null) { _listVendorType = new Models.FundingInterestRate(); }
                    
                    if (_listVendorType.Id == 0)
                    {
                        _FundingInterestRateS.FechaCreacion = DateTime.Now;
                        _FundingInterestRateS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(_FundingInterestRateS);
                    }
                    else
                    {
                        _FundingInterestRateS.FechaCreacion = DateTime.Now;
                        _FundingInterestRateS.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var updateresult = await Update(_FundingInterestRateS.Id, _FundingInterestRateS);
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
                
                return Json(_FundingInterestRateS);
            }
        }



        // GET: Customer/Details/5
        //public async Task<ActionResult> Details(Int64 FundingInterestRateId)
        //{
        //    FundingInterestRate _FundingInterestRates = new FundingInterestRate();
        //    if (FundingInterestRateId == 0)
        //    {
        //        return await Task.Run(() => View(_FundingInterestRates));
        //    }
        //    try
        //    {
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRateById/" + FundingInterestRateId);
        //        string valorrespuesta = "";
        //        if (result.IsSuccessStatusCode)
        //        {
        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _FundingInterestRates = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);

        //        }
        //        ViewData["FundingInterestRate"] = _FundingInterestRates;
        //        ViewData["FundingInterestRateName"] = _FundingInterestRates.Descripcion.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }

        //    return await Task.Run(() => View(_FundingInterestRates));
        //}
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<FundingInterestRate> _FundingInterestRate = new List<FundingInterestRate>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/FundingInterestRate/GetFundingInterestRate");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FundingInterestRate = JsonConvert.DeserializeObject<List<FundingInterestRate>>(valorrespuesta);
                    _FundingInterestRate = _FundingInterestRate.OrderByDescending(q => q.Id).ToList();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _FundingInterestRate.ToDataSourceResult(request);

        }

        [HttpPost]
        public async Task<ActionResult<FundingInterestRate>> Delete([FromBody]FundingInterestRate _FundingInterestRate)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/FundingInterestRate/Delete", _FundingInterestRate);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FundingInterestRate = JsonConvert.DeserializeObject<FundingInterestRate>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _FundingInterestRate }, Total = 1 });
        }

    }
}