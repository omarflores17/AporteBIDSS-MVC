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
using ERP.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class VendorProductController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        //private readonly ApplicationDbContext _context;

        public VendorProductController(ILogger<VendorProductController> logger, IOptions<MyConfig> config/*, ApplicationDbContext context*/)
        {
            this.config = config;
            this._logger = logger;
            //_context = context;
        }

        // GET: VendorProduct
        public IActionResult Index()
        {
            return View();
        }
        //public IActionResult ListProduct()
        //{
        //    return View();
        //}
        public IActionResult Compras()
        {
            return View();
        }
        //public ActionResult Proveedores()
        //{
        //    return View();
        //}

        // GET: VendorProduct/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}
        public ActionResult pvwAddVendorProduct(VendorProduct VendorProduct)
        {
            return PartialView(VendorProduct);
        }

        public ActionResult ListProduct(Product product)
        {
            return PartialView(product);
        }


      


        //[HttpPost("[action]")]
        //public async Task<ActionResult> Proveedores([FromBody]VendorProduct _VendorProduct)
        //{
        //    VendorProduct _VendorProductp = new VendorProduct();
        //    try
        //    {
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/VendorProduct/GetVendorProductById/" + _VendorProduct.VendorProductId);
        //        string valorrespuesta = "";
        //        if (result.IsSuccessStatusCode)
        //        {
        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _VendorProduct = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);

        //        }
        //        ViewData["VendorProduct"] = _VendorProduct;
        //        if (_VendorProduct == null)
        //        {
        //            _VendorProduct = new VendorProduct();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }



        //    return PartialView(_VendorProduct);
        //}


        [HttpGet("[action]")]
        public async Task<JsonResult> GetVendorProduct([DataSourceRequest]DataSourceRequest request)
        {
            List<VendorProduct> _customers = new List<VendorProduct>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorProduct/GetVendorProduct");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<VendorProduct>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_customers.ToDataSourceResult(request));

        }




        // POST: TypeAccount/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(VendorProduct _VendorProductP)
        {
            VendorProduct _VendorProduct = _VendorProductP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/VendorProduct/Insert", _VendorProduct);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorProduct = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _VendorProduct }, Total = 1 });
        }


        [HttpPut("{VendorProductId}")]
        public async Task<IActionResult> Update(Int64 VendorProductId, VendorProduct _VendorProductP)
        {
            VendorProduct _VendorProduct = _VendorProductP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/VendorProduct/Update", _VendorProduct);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorProduct = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _VendorProduct }, Total = 1 });
        }
        // GET: VendorProduct/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VendorProduct/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult<VendorProduct>> SaveVendorProduct([FromBody]VendorProduct _VendorProduct)
        {
            string valorrespuesta = "";
            try
            {
                VendorProduct _listVendorP = new VendorProduct();
                List<VendorProduct> Vendors = new List<VendorProduct>();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorProduct/GetVendorProduct");
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    Vendors = JsonConvert.DeserializeObject<List<VendorProduct>>(valorrespuesta);
                    Vendors = Vendors.Where(p => p.VendorId == _VendorProduct.VendorId).ToList();
                    if (Vendors.Count > 0)
                    {
                        Vendors = Vendors.Where(p => p.ProductId == _VendorProduct.ProductId).ToList();
                        if (Vendors.Count > 0)
                        {
                            return await Task.Run(() => BadRequest($"Ya existe ese producto registrado para este proveedor."));
                        }
                    }
                }
                
                result = await _client.GetAsync(baseadress + "api/VendorProduct/GetVendorProductById/" + _VendorProduct.Id);
                
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorP = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);
                }
                
                if (_listVendorP == null) { _listVendorP = new Models.VendorProduct(); }
                
                if (_listVendorP.Id == 0)
                {
                    var insertresult = await Insert(_VendorProduct);
                }
                else
                {
                    var updateresult = await Update(_VendorProduct.Id, _VendorProduct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return Json(_VendorProduct);
        }

        
        public async Task<ActionResult> Details(Int64 Id)
        {
            VendorProduct _VendorProducts = new VendorProduct();
            if (Id == 0)
            {
                return await Task.Run(() => View(_VendorProducts));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorProduct/GetVendorProductById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorProducts = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);

                }
                ViewData["VendorProduct"] = _VendorProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_VendorProducts));
        }
        [HttpGet("[action]")]
        public async Task<ActionResult> GetVendorProductById(Int64 Id)
        {
            VendorProduct _customers = new VendorProduct();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorProduct/GetVendorProductById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return await Task.Run(() => Json(_customers));
        }


    }
}