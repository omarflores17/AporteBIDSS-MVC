using System;
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class BranchController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;


        public BranchController(ILogger<HomeController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }


        // GET: Branch
        public ActionResult Brach()
        {
            return View();
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetBranch([DataSourceRequest]DataSourceRequest request)
        {
            List<Branch> _customers = new List<Branch>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Branch/GetBranch");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Branch>>(valorrespuesta);
                    _customers = _customers.OrderByDescending(q => q.BranchId).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return Json(_customers.ToDataSourceResult(request));

        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetBranchById(Int64 BranchId)
        {
            List<Branch> _customers = new List<Branch>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Branch/GetBranchById/" + BranchId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Branch>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return await Task.Run(() => Json(_customers));
        }


        [HttpGet]
        public async Task<JsonResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Branch> _customers = new List<Branch>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Branch/GetBranch");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Branch>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
         


               return Json(_customers.ToDataSourceResult(request));

        }


        public async Task<ActionResult<Branch>> SaveBranch([FromBody] BranchDTO BranchA)
        {
            {
                string valorrespuesta = "";
                try
                {
                    Branch BranchB = new Branch();
                    Branch BranchC = new Branch();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/Branch/GetBranchByName/" + BranchA.BranchName);
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        BranchB = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                        BranchC = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                        if (BranchB != null && BranchA.BranchId == 0)
                        {
                            if (BranchB.BranchName.ToLower() == BranchA.BranchName.ToLower())
                            {
                                {
                                    return await Task.Run(() => BadRequest($"Ya existe una sucursal con ese nombre."));
                                }
                            }
                        }
                    }
                    result = await _client.GetAsync(baseadress + "api/Branch/GetBranchById/" + BranchA.BranchId);

                    BranchA.FechaModificacion = DateTime.Now;
                    BranchA.UsuarioModificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        BranchB = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                    }
                    if (BranchB == null) { BranchB = new Models.Branch(); }
                    if (BranchB.BranchId == 0)
                    {
                        BranchB.FechaCreacion = DateTime.Now;
                        BranchB.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(BranchA);
                    }
                    else
                    {
                        if (BranchC != null)
                        {
                            if (BranchC.BranchName.ToLower() == BranchA.BranchName.ToLower() && BranchC.BranchId != BranchA.BranchId)
                            {
                                return await Task.Run(() => BadRequest($"Ya existe una sucursal con ese nombre."));
                            }
                        }
                        BranchA.FechaCreacion = DateTime.Now;
                        BranchA.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var updateresult = await Update(BranchA.BranchId, BranchA);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
                return Json(BranchA);
            }
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddBranch([FromBody]BranchDTO _sarpara)
        {
            BranchDTO _Branch = new BranchDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Branch/GetBranchById/" + _sarpara.BranchId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Branch = JsonConvert.DeserializeObject<BranchDTO>(valorrespuesta);

                }

                if (_Branch == null)
                {
                    _Branch = new BranchDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Branch);

        }

        // POST: Branch/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Branch _Branch)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Branch.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Branch.FechaCreacion = DateTime.Now;
                _Branch.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Branch/Insert", _Branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Branch = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Branch }, Total = 1 });
        }

        [HttpPut("BranchId")]
        public async Task<IActionResult> Update(Int64 BranchId, Branch _Branch)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Branch/Update", _Branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Branch = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Branch }, Total = 1 });
        }
        [HttpPost]
        public async Task<ActionResult<Branch>> Delete([FromBody]Branch _Branch)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Branch/Delete", _Branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Branch = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                    return new ObjectResult(new DataSourceResult { Data = new[] { _Branch }, Total = 1 });
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            
        }
     
        

    }
}