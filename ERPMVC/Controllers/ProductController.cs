using System;
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
using ERP.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    public class ProductController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        
        public ProductController(ILogger<ProductController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
           
        }

        public IActionResult Product()
        {
            return View();
        }
        public async Task<IActionResult> Inventario(Product product)
        {
            ViewData["Sucursal"] = await ObtenerSucusal();
            return PartialView(product);
        }

        //metodo para llamar la descripcion de la sucursal
        async Task<IEnumerable<Branch>> ObtenerSucusal()
        {
            IEnumerable<Branch> Branch = null;
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
                    Branch = JsonConvert.DeserializeObject<IEnumerable<Branch>>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            ViewData["Sucursal"] = Branch.FirstOrDefault();
            return Branch;
        }
        // GET: api/Product/GetProductVendorsByProductID
        /// <summary>
        ///   Obtiene el listado de Proveedores por Producto.        
        /// </summary>
        /// <returns></returns>


        public ActionResult pvwAddProduct(Product Producto)
        {
            return PartialView(Producto);
        }
     
        [HttpGet("[action]")]
        public async Task<ActionResult> GetProductById(Int64 ProductId)
        {
            Product _Product = new Product();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProductById/" + ProductId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Product = JsonConvert.DeserializeObject<Product>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => Json(_Product));
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult>GetKardex([DataSourceRequest]DataSourceRequest request, int ProductId)
        {
             List<KardexViale>  _Kardex = new List<KardexViale>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/KardexViale/Get/" + ProductId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<List<KardexViale>>(valorrespuesta);
                    _Kardex = _Kardex.OrderByDescending(q => q.KardexDate).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Kardex.ToDataSourceResult(request);
        }


        public async Task<ActionResult> Details(Int64 ProductId)
        {
            Product _Products = new Product();
            if (ProductId == 0)
            {
                return await Task.Run(() => View(_Products));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProductById/" + ProductId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Products = JsonConvert.DeserializeObject<Product>(valorrespuesta);

                }
                ViewData["Product"] = _Products;
                ViewData["ProductName"] = _Products.ProductName.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Products));
        }


        [HttpGet("[action]")]
        public async Task<JsonResult> GetProductConcatenado([DataSourceRequest]DataSourceRequest request)
        {
            List<Product> __customers = new List<Product>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                //Error
                var result = await _client.GetAsync(baseadress + "api/Product/GetProduct");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    __customers = JsonConvert.DeserializeObject<List<Product>>(valorrespuesta);
                    __customers = (from c in __customers.OrderBy(q => q.ProductId)
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
            return Json(__customers.ToDataSourceResult(request));

        }

        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Product> _Product = new List<Product>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProduct");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Product = JsonConvert.DeserializeObject<List<Product>>(valorrespuesta);
                    _Product = _Product.OrderByDescending(q => q.ProductId).ToList();

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Product.ToDataSourceResult(request);

        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Product>> ValidarCodigoProducto(string ProductCode)
        {
            Product _listProduct = new Product();
            string valorrespuesta = "";
            try
            {
                
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProductValidarProductCode/" + ProductCode);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listProduct = JsonConvert.DeserializeObject<Product>(valorrespuesta);
                    if (_listProduct != null)
                    {
                        if (_listProduct.ProductCode == ProductCode)
                        {
                            return await Task.Run(() => Json(_listProduct));
                            //return await Task.Run(() => BadRequest($"El código del producto ya existe."));
                        }
                    }

                }
             
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return Json(_listProduct);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<Product>> SaveProduct([FromBody]ProductDTO _ProductS)
        {
            Product _Product = _ProductS;
            try
            {
                Product _listProduct = new Product();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProductById/" + _Product.ProductId);
                string valorrespuesta = "";
                _Product.FechaModificacion = DateTime.Now;
                _Product.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Product = JsonConvert.DeserializeObject<ProductDTO>(valorrespuesta);
                }

                if (_Product == null) { _Product = new Models.Product(); }

                if (_ProductS.ProductId == 0)
                {
                    _ProductS.FechaCreacion = DateTime.Now;
                    _ProductS.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_ProductS);
                }
                else
                {
                    _ProductS.UsuarioCreacion = _Product.UsuarioCreacion;
                    _ProductS.FechaCreacion = _Product.FechaCreacion;
                    var updateresult = await Update(_Product.ProductId, _ProductS);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Product);
        }

        // POST: Product/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Product>> Insert(Product _Productp)
        {
            Product _Product = _Productp;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Productp.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Productp.UsuarioModificacion = HttpContext.Session.GetString("user");
                _Productp.FechaCreacion = DateTime.Now;
                _Productp.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Product/Insert", _Product);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Product = JsonConvert.DeserializeObject<Product>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Product }, Total = 1 });
        }

        [HttpPut("{ProductId}")]
        public async Task<ActionResult<Product>> Update(Int64 ProductId, Product _Productp)
        {
            Product _Product = _Productp;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Product.FechaModificacion = DateTime.Now;
                _Product.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PutAsJsonAsync(baseadress + "api/Product/Update", _Product);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Product = JsonConvert.DeserializeObject<Product>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Product }, Total = 1 });
        }


        [HttpPost("CurrencyId")]
        public async Task<ActionResult<Product>> Delete([FromBody]ProductDTO _Currencyp)

        {
            Product _Currency = _Currencyp;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/Product/Delete", _Currency);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Currency = JsonConvert.DeserializeObject<Product>(valorrespuesta);
                    if (_Currency.ProductId == 0)
                    {
                        return BadRequest("No se puede eliminar el producto porque ya esta siendo utilizado");
                    }
                }
               


            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
                throw ex;
            }
            
            return new ObjectResult(new DataSourceResult { Data = new[] { _Currency }, Total = 1 });
        }


        //[HttpPost("[action]")]
        //public async Task<ActionResult> GetProveedor([FromBody]VendorProduct _vendor)
        //{
        //    VendorProduct _vendorp = new VendorProduct();
        //    try
        //    {
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "/api/Product/GetProductVendorsByProductID/" + _vendor.VendorId);
        //        string valorrespuesta = "";
        //        if (result.IsSuccessStatusCode)
        //        {
        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _vendor = JsonConvert.DeserializeObject<VendorProduct>(valorrespuesta);

        //        }
        //        ViewData["product"] = _vendor;
        //        if (_vendor == null)
        //        {
        //            _vendor = new VendorProduct();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }



        //    return PartialView(_vendor);
        //}





    }
}