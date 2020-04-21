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
    public class FixedAssetController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public FixedAssetController(ILogger<FixedAssetController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        public IActionResult FixedAsset()
        {
            return View();
        }

        public  ActionResult pvwFixedAsset(FixedAsset FixedAsset)
        {
            
            return PartialView(FixedAsset);
        }

        public async Task<ActionResult> Details(Int64 FixedAssetId)
        {
            FixedAsset _Products = new FixedAsset();
            if (FixedAssetId == 0)
            {
                _Products.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                _Products.AssetDate = DateTime.Now;
                return await Task.Run(() => View(_Products));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/FixedAsset/GetFixedAssetById/" + FixedAssetId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Products = JsonConvert.DeserializeObject<FixedAsset>(valorrespuesta);

                }
                ViewData["FixedAsset"] = _Products;
                ViewData["FixedAssetName"] = _Products.FixedAssetName.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Products));
        }
        [HttpGet]

        public async Task<ActionResult> SFFixed(Int32 id)
        {
            try
            {
                FixedAsset _fixeddto = new FixedAsset { FixedAssetId = id, };
                return await Task.Run(() => View(_fixeddto));
            }
            catch (Exception)
            {

                return await Task.Run(() => BadRequest("Ocurrio un error"));
            }

        }



        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<FixedAsset> _FixedAsset = new List<FixedAsset>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/FixedAsset/GetFixedAsset");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FixedAsset = JsonConvert.DeserializeObject<List<FixedAsset>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _FixedAsset.ToDataSourceResult(request);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult<FixedAsset>> SaveFixedAsset([FromBody]FixedAsset _FixedAsset)
        {
            FixedAsset fixedAsset = new FixedAsset();
            try
            {
                FixedAsset _listFixedAsset = new FixedAsset();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/FixedAsset/GetFixedAssetById/" + _FixedAsset.FixedAssetId);
                string valorrespuesta = "";
                _FixedAsset.FechaModificacion = DateTime.Now;
                _FixedAsset.UsuarioModificacion = HttpContext.Session.GetString("user");
                _FixedAsset.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listFixedAsset = JsonConvert.DeserializeObject<FixedAsset>(valorrespuesta);
                }

                if (_listFixedAsset == null) { _listFixedAsset = new FixedAsset(); }

                if (_listFixedAsset.FixedAssetId == 0)
                {
                    _FixedAsset.FechaCreacion = DateTime.Now;
                    _FixedAsset.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_FixedAsset);
                    fixedAsset = (FixedAsset)insertresult;
                }
                else
                {
                    var updateresult = await Update(_FixedAsset.FixedAssetId, _FixedAsset);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(fixedAsset);
        }

        // POST: FixedAsset/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<FixedAsset> Insert(FixedAsset _FixedAsset)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _FixedAsset.UsuarioCreacion = HttpContext.Session.GetString("user");
                _FixedAsset.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/FixedAsset/Insert", _FixedAsset);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FixedAsset = JsonConvert.DeserializeObject<FixedAsset>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return null;
            }
            return _FixedAsset;
            // return new ObjectResult(new DataSourceResult { Data = new[] { _FixedAsset }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FixedAsset>> Update(Int64 id, FixedAsset _FixedAsset)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/FixedAsset/Update", _FixedAsset);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FixedAsset = JsonConvert.DeserializeObject<FixedAsset>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            //return new ObjectResult(new DataSourceResult { Data = new[] { _FixedAsset }, Total = 1 });
            return Ok(_FixedAsset);
        }

        [HttpPost]
        public async Task<ActionResult<FixedAsset>> Delete([FromBody]FixedAsset _FixedAsset)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/FixedAsset/Delete", _FixedAsset);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _FixedAsset = JsonConvert.DeserializeObject<FixedAsset>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _FixedAsset }, Total = 1 });
        }


        private async Task<ActionResult> DeleteDepreciation(Int64 _id)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/DepreciationFixedAsset/DeleteByFixedAssetId", _id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }

            return Ok();
            //return new ObjectResult(new DataSourceResult { Data = new[] { _DepreciationFixedAsset }, Total = 1 });
        }


    }
}