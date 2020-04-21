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
    public class VendorController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        

        public VendorController(ILogger<VendorController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
            
        }

        // GET: Vendor
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

        // GET: Vendor/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}
        public ActionResult pvwAddVendor(Vendor Vendor)
        {
            return PartialView(Vendor);
        }
        //Tab de productos por proveedor
        public ActionResult ListProduct(Product product)
        {
            return PartialView(product);
        }
        //Tab de Compras por proveedor 
        public ActionResult PurchaseVendor(PurchaseOrder purchase)
        {
            return PartialView(purchase);
        }

        // GET: api/Vendors/GetProductVendorsByVendorID
        /// <summary>
        ///   Obtiene el listado de Productos por Proveedor.        
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<ActionResult> GetProductos([FromBody]VendorProduct _vendor)
        {
            VendorProduct _vendorp = new VendorProduct();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "/api/Vendor/GetProductVendorsByVendorID/" + _vendor.ProductId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _vendor = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);

                }
                ViewData["product"] = _vendor;
                if (_vendor == null)
                {
                    _vendor = new VendorProduct();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return PartialView(_vendor);
        }

        // GET: api/Vendors/GetPurchaseByVendorId
        /// <summary>
        ///   Obtiene el listado de Productos por Proveedor.        
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<ActionResult> GetPurchase([FromBody]PurchaseOrder _vendor)
        {
            PurchaseOrder _vendorp = new PurchaseOrder();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "/api/Vendor/GetPurchaseByVendorId/" + _vendor.VendorId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _vendor = JsonConvert.DeserializeObject<PurchaseOrder>(valorrespuesta);

                }
                ViewData["purchase"] = _vendor;
                if (_vendor == null)
                {
                    _vendor = new PurchaseOrder();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_vendor);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Proveedores([FromBody]Vendor _vendor)
        {
            Vendor _vendorp = new Vendor();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Vendor/GetVendorById/" + _vendor.VendorId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _vendor = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);

                }
                ViewData["vendor"] = _vendor;
                ViewData["VendorName"] = _vendor.VendorName.ToString();
                if (_vendor == null)
                {
                    _vendor = new Vendor();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_vendor);
        }


        [HttpGet("[action]")]
        public async Task<JsonResult> GetVendor([DataSourceRequest]DataSourceRequest request)
        {
            List<Vendor> _customers = new List<Vendor>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Vendor/GetVendor");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Vendor>>(valorrespuesta);
                    _customers = _customers.OrderByDescending(q => q.VendorId).ToList();

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
        public async Task<ActionResult> Insert(Vendor _VendorP)
        {
            Vendor _Vendor = _VendorP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Vendor.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Vendor.FechaCreacion = DateTime.Now;
                _Vendor.UsuarioModificacion = HttpContext.Session.GetString("user");
                _Vendor.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Vendor/Insert", _Vendor);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Vendor = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Vendor }, Total = 1 });
        }


        [HttpPut("{VendorId}")]
        public async Task<IActionResult> Update(Int64 VendorId, Vendor _VendorP)
        {
            Vendor _Vendor = _VendorP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/Vendor/Update", _Vendor);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Vendor = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Vendor }, Total = 1 });
        }
 


        [HttpPost]
        public async Task<ActionResult<Vendor>> SaveVendor([FromBody]Vendor _VendorP)
        {
            string valorrespuesta = "";
            try
            {
                Vendor _listVendor = new Vendor();
                List<Vendor> _VendorValidacionRTN = new List<Vendor>();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Vendor/GetVendor");
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorValidacionRTN = JsonConvert.DeserializeObject<List<Vendor>>(valorrespuesta);
                    if (_VendorP.VendorId == 0)
                    {
                        _VendorValidacionRTN = _VendorValidacionRTN.Where(p => p.RTN == _VendorP.RTN).ToList();
                    }
                    else
                    {
                        _VendorValidacionRTN = _VendorValidacionRTN.Where(p => p.RTN == _VendorP.RTN && p.VendorId != _VendorP.VendorId).ToList();
                    }
                    if (_VendorValidacionRTN.Count > 0)
                    {
                        return await Task.Run(() => BadRequest($"Ya existe un Proveedor registrado con ese RTN."));
                    }

                }
                result = await _client.GetAsync(baseadress + "api/Vendor/GetVendorById/" + _VendorP.VendorId);

                _VendorP.FechaModificacion = DateTime.Now;
                _VendorP.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendor = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);
                }



                if (_listVendor == null) { _listVendor = new Models.Vendor(); }



                if (_listVendor.VendorId == 0)
                {
                    _VendorP.FechaCreacion = DateTime.Now;
                    _VendorP.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_VendorP);
                }
                else
                {
                    _VendorP.UsuarioCreacion = _listVendor.UsuarioCreacion == "" ? HttpContext.Session.GetString("user") : _listVendor.UsuarioCreacion;
                    _VendorP.FechaCreacion = _listVendor.FechaCreacion == null ? DateTime.Now : _listVendor.FechaCreacion;
                    var updateresult = await Update(_VendorP.VendorId, _VendorP);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return Json(_VendorP);
        }

        // GET: Customer/Details/5
        public async Task<ActionResult> Details(Int64 VendorId)
        {
            Vendor _vendors = new Vendor();
            if (VendorId == 0)
            {
                return await Task.Run(() => View(_vendors));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Vendor/GetVendorById/" + VendorId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _vendors = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);

                }
                ViewData["Vendor"] = _vendors;
                ViewData["VendorName"] = _vendors.VendorName.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_vendors));
        }
        [HttpGet("[action]")]
        public async Task<ActionResult> GetVendorById(Int64 VendorId)
        {
            Vendor _customers = new Vendor();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Vendor/GetVendorById/" + VendorId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return await Task.Run(() => Json(_customers));
        }
        //Productos por proveedor
        [HttpGet("[action]")]
        public async Task<ActionResult> GetProductsByVendorId([DataSourceRequest]DataSourceRequest request,int VendorId)
        {
            List<Product> _products = new List<Product>();
            try
            {
                if (VendorId > 0)
                {
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/Vendor/GetProductVendorsByVendorID/" + VendorId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _products = JsonConvert.DeserializeObject<List<Product>>(valorrespuesta);
                        _products = _products.OrderByDescending(q => q.ProductId).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            

            return  Json(_products.ToDataSourceResult(request));
        }

        //Compras por proveedor
        [HttpGet("[action]")]
        public async Task<ActionResult> GetPurchaseByVendorId([DataSourceRequest]DataSourceRequest request, int VendorId)
        {
            List<PurchaseOrder> _products = new List<PurchaseOrder>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Vendor/GetPurchaseByVendorId/" + VendorId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _products = JsonConvert.DeserializeObject<List<PurchaseOrder>>(valorrespuesta);
                    _products = _products.OrderByDescending(q => q.DatePlaced).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            

            return Json(_products.ToDataSourceResult(request));
        }

        public async Task<ActionResult<Vendor>> Delete([FromBody]Vendor _Vendor)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                
                var result = await _client.PostAsJsonAsync(baseadress + "api/Vendor/Delete", _Vendor);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Vendor = JsonConvert.DeserializeObject<Vendor>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            
            return new ObjectResult(new DataSourceResult { Data = new[] { _Vendor }, Total = 1 });
        }


    }
}