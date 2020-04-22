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
    public class ProformaInvoiceController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public ProformaInvoiceController(ILogger<ProformaInvoiceController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        async Task<IEnumerable<Product>> Obtenerproducto()
        {
            IEnumerable<Product> Product = null;
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
                    Product = JsonConvert.DeserializeObject<IEnumerable<Product>>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            ViewData["producto"] = Product.FirstOrDefault();
            return Product;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddModalCustomer([FromBody]Customer _sarpara)
        {
            Customer _Customer = new Customer();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Branch/GetBranchById/" + _sarpara.CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Customer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);

                }

                if (_Customer == null)
                {
                    _Customer = new Customer();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return PartialView(_Customer);

        }

        public IActionResult IndexCredito()
        {
            return View();
        }
        public async Task<ActionResult> Details(Int64 InvoiceId)
        {
            ViewData["producto"] = await Obtenerproducto();
            ProformaInvoiceDTO _customers = new ProformaInvoiceDTO();
            if (InvoiceId == 0)
            {
                return await Task.Run(() => View(_customers));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoiceById/" + InvoiceId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<ProformaInvoiceDTO>(valorrespuesta);
                    
                }
                ViewData["customer"] = _customers;
                ViewData["customerName"] = _customers.CustomerName.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_customers));
        }
        public async Task<ActionResult> DetailsCredito(Int64 InvoiceId)
        {
            ProformaInvoiceDTO _customers = new ProformaInvoiceDTO();
            if (InvoiceId == 0)
            {
                return await Task.Run(() => View(_customers));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoiceById/" + InvoiceId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<ProformaInvoiceDTO>(valorrespuesta);

                }
                ViewData["customer"] = _customers;
                ViewData["customerName"] = _customers.CustomerName.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_customers));
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwModalCliente([FromBody]Customer _Cust)
        {
            Customer _Customer = new Customer();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerById/" + _Cust.CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Customer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Customer);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwModalProduct([FromBody]Product _vendorT)
        {
            Product _VendorType = new Product();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProductById/" + _vendorT.ProductId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<Product>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_VendorType);
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> pvwProformaInvoice([FromBody]ProformaInvoiceDTO _ProformaId)
        {
            ProformaInvoiceDTO _ProformaInvoice = new ProformaInvoiceDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoiceById/" + _ProformaId.ProformaId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<ProformaInvoiceDTO>(valorrespuesta);

                }

                if (_ProformaInvoice == null)
                {
                    _ProformaInvoice = new ProformaInvoiceDTO { ExpirationDate = DateTime.Now.AddDays(30), OrderDate = DateTime.Now, editar = 1 };
                }
                else
                {
                    _ProformaInvoice.editar = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_ProformaInvoice);

        }
        public async Task<ActionResult> pvwProformaInvoiceCredito([FromBody]ProformaInvoiceDTO _ProformaId)
        {
            ProformaInvoiceDTO _ProformaInvoice = new ProformaInvoiceDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoiceById/" + _ProformaId.ProformaId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<ProformaInvoiceDTO>(valorrespuesta);

                }

                if (_ProformaInvoice == null)
                {
                    _ProformaInvoice = new ProformaInvoiceDTO { ExpirationDate = DateTime.Now.AddDays(30), OrderDate = DateTime.Now, editar = 1 };
                }
                else
                {
                    _ProformaInvoice.editar = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_ProformaInvoice);

        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<ProformaInvoice> _ProformaInvoice = new List<ProformaInvoice>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoice");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<List<ProformaInvoice>>(valorrespuesta);
                    _ProformaInvoice = _ProformaInvoice.OrderByDescending(q => q.ProformaId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _ProformaInvoice.ToDataSourceResult(request);

        }


        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetProformaInvoice([DataSourceRequest]DataSourceRequest request)
        {
            List<ProformaInvoice> _ProformaInvoice = new List<ProformaInvoice>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoice");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<List<ProformaInvoice>>(valorrespuesta);
                    _ProformaInvoice = _ProformaInvoice.OrderByDescending(q => q.ProformaId).ToList();
                }
                
                //else if(result.StatusCode== 401)
                //{
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
            }


            return _ProformaInvoice.ToDataSourceResult(request);
        }
        ////////////////////////////factura contado
        /// 
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetProformaInvoiceContado([DataSourceRequest]DataSourceRequest request)
        {
            List<ProformaInvoice> _ProformaInvoice = new List<ProformaInvoice>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoicefiltrar/"+ 2);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<List<ProformaInvoice>>(valorrespuesta);
                }

                //else if(result.StatusCode== 401)
                //{
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
            }


            return _ProformaInvoice.ToDataSourceResult(request);
        }
        ////////////////////////////factura credito
        /// 
        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetProformaInvoicecredito([DataSourceRequest]DataSourceRequest request)
        {
            List<ProformaInvoice> _ProformaInvoice = new List<ProformaInvoice>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoicefiltrar/" + 3);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<List<ProformaInvoice>>(valorrespuesta);
                }

                //else if(result.StatusCode== 401)
                //{
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
            }


            return _ProformaInvoice.ToDataSourceResult(request);
        }










        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetProformaInvoiceByCustomer([DataSourceRequest]DataSourceRequest request, Int64 CustomerId)
        {
            List<ProformaInvoice> _ProformaInvoice = new List<ProformaInvoice>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoiceByCustomer/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<List<ProformaInvoice>>(valorrespuesta);
                }
                //else if(result.StatusCode== 401)
                //{
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
            }


            return _ProformaInvoice.ToDataSourceResult(request);
        }

        

        [HttpPost("[action]")]
        // [ValidateAntiForgeryToken]
        // public async Task<ActionResult<ProformaInvoice>> SaveProformaInvoice([FromBody]ProformaInvoice _ProformaInvoice)
        public async Task<ActionResult<ProformaInvoice>> SaveProformaInvoice([FromBody]dynamic dto)
        {
            
            ProformaInvoice _ProformaInvoiceP = JsonConvert.DeserializeObject<ProformaInvoice>(dto.ToString());

            ProformaInvoice _ProformaInvoice = _ProformaInvoiceP;
            try
            {
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/ProformaInvoice/GetProformaInvoiceById/" + _ProformaInvoice.ProformaId);
                    string valorrespuesta = "";
                    _ProformaInvoice.FechaModificacion = DateTime.Now;
                    _ProformaInvoice.UsuarioModificacion = HttpContext.Session.GetString("user");
                    _ProformaInvoice.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<ProformaInvoice>(valorrespuesta);
                    }

                    if (_ProformaInvoice == null) { _ProformaInvoice = new ProformaInvoice(); }
                    if (_ProformaInvoiceP.ProformaId == 0)
                    {
                        _ProformaInvoiceP.FechaCreacion = DateTime.Now;
                        _ProformaInvoice.UsuarioCreacion = HttpContext.Session.GetString("user");
                        
                       var  insertresult = await Insert(_ProformaInvoiceP);

                }
                    else
                    {
                    _ProformaInvoiceP.FechaCreacion = _ProformaInvoice.FechaCreacion;
                    _ProformaInvoiceP.UsuarioCreacion = _ProformaInvoice.UsuarioCreacion;
                    var updateresult = await Update(_ProformaInvoice.ProformaId, _ProformaInvoiceP);
                     }       
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_ProformaInvoice);
        }

        // POST: ProformaInvoice/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ProformaInvoice>> Insert(ProformaInvoice _ProformaInvoice)
        {
            try
            {
                string valorrespuesta3 = "";
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _ProformaInvoice.UsuarioCreacion = HttpContext.Session.GetString("user");
                _ProformaInvoice.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/ProformaInvoice/InsertWithInventory", _ProformaInvoice);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<ProformaInvoice>(valorrespuesta);

                    foreach (var item in _ProformaInvoice.ProformaInvoiceLine)
                    {
                        KardexViale kardexViale = new KardexViale();

                        var result3 = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexByProductId/" + item.ProductId + "/" + Convert.ToInt32(HttpContext.Session.GetString("BranchId")));
                        if (result3.IsSuccessStatusCode)
                        {
                            valorrespuesta3 = await (result3.Content.ReadAsStringAsync());
                            kardexViale = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta3);
                            kardexViale.QuantityOut = item.Quantity;
                            kardexViale.QuantityEntry = 0;
                            kardexViale.SaldoAnterior = kardexViale.Total;
                            kardexViale.Total = kardexViale.Total - item.Quantity;
                            kardexViale.Id = 0;
                            kardexViale.KardexDate = DateTime.Now;
                            kardexViale.TypeOperationId = 2;
                            kardexViale.TypeOperationName = "Salida";
                            kardexViale.UsuarioCreacion = HttpContext.Session.GetString("user");
                            kardexViale.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                            kardexViale.TypeOfDocumentId = 1;
                            kardexViale.TypeOfDocumentName = "Factura al Contado";
                            kardexViale.DocumentId = _ProformaInvoice.ProformaId;
                            result3 = await _client.PostAsJsonAsync(baseadress + "api/KardexViale/Insert", kardexViale);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_ProformaInvoice);
            // return new ObjectResult(new DataSourceResult { Data = new[] { _ProformaInvoice }, Total = 1 });
        }

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ProformaInvoice>> Update(Int64 id, ProformaInvoice _ProformaInvoice)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/ProformaInvoice/Update", _ProformaInvoice);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<ProformaInvoice>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _ProformaInvoice }, Total = 1 });
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ProformaInvoice>> Delete([FromBody]ProformaInvoice _ProformaInvoice)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/ProformaInvoice/Delete", _ProformaInvoice);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ProformaInvoice = JsonConvert.DeserializeObject<ProformaInvoice>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            return new ObjectResult(new DataSourceResult { Data = new[] { _ProformaInvoice }, Total = 1 });
        }







    }
}