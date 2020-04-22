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
using System.Security.Claims;

namespace ERPMVC.Controllers
{

    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CustomerController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        private readonly ClaimsPrincipal _principal;

        public CustomerController(ILogger<CustomerController> logger, IOptions<MyConfig> config, IHttpContextAccessor httpContextAccessor)
        {
            this.config = config;
            this._logger = logger;
            _principal = httpContextAccessor.HttpContext.User;
        }


        // GET: Customer
        public async Task<ActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        public async Task<ActionResult> SalesOrderCustomer()
        {
            return await Task.Run(() => PartialView());
        }

        public ActionResult CustomerProduct()
        {
            return PartialView();
        }

        public ActionResult Phones()
        {
            return PartialView();
        }

        public ActionResult PurchaseCustomer(PurchaseOrder purchase)
        {
            return PartialView(purchase);
        }

        public async Task<ActionResult> pvwDatos(Customer customer) {
            ViewData["permisos"] = _principal;
            if (customer.CustomerId == 0)
            {
                var BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                Branch _customers = new Branch();
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
                        _customers = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                    }
                    customer.CountryId = _customers.CountryId;
                    customer.CityId = _customers.CityId;
                    customer.StateId = _customers.StateId;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
            }
            return await Task.Run(() => View(customer));
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetCityAndState()
        {
            var BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
            Branch _customers = new Branch();
            Customer _Customer = new Customer();
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
                    _customers = JsonConvert.DeserializeObject<Branch>(valorrespuesta);
                }
                _Customer.CityId = _customers.CityId;
                _Customer.CountryId = _customers.CountryId;
                _Customer.StateId = _customers.StateId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => Json(_Customer));
        }

        public async Task<ActionResult> CertificadoDepositoCustomer()
        {
            return await Task.Run(() => PartialView());
        }

        public async Task<ActionResult> ProformaInvoiceCustomer()
        {
            return await Task.Run(() => PartialView());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetCustomerById(Int64 CustomerId)
        {
            Customer _customers = new Customer();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerById/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<Customer>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return await Task.Run(() => Json(_customers));
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetCustomerConcatenado([DataSourceRequest]DataSourceRequest request)
        {
            List<Customer> __customers = new List<Customer>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                //Error
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomer");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    __customers = JsonConvert.DeserializeObject<List<Customer>>(valorrespuesta);
                    __customers = (from c in __customers.OrderBy(q => q.CustomerId)
                                   select new Customer
                                   {
                                       CustomerId = c.CustomerId,
                                       CustomerName = c.RTN + "--" + c.CustomerName,
                                       Address = c.Address,
                                       CityId = c.CityId,
                                       RTN = c.RTN,
                                       StateId = c.StateId,
                                       CustomerTypeId = c.CustomerTypeId,
                                       CountryId = c.CountryId,
                                       Phone = c.Phone,
                                       Identidad = c.Identidad,
                                       Email = c.Email,
                                       IdEstado = c.IdEstado,
                                       ZipCode = c.ZipCode,
                                       WorkPhone = c.WorkPhone,
                                       ContactPerson = c.ContactPerson,
                                       GrupoEconomico = c.GrupoEconomico,
                                       MontoActivos = c.MontoActivos,
                                       MontoIngresosAnuales = c.MontoIngresosAnuales,
                                       Proveedor1 = c.Proveedor1,
                                       Proveedor2 = c.Proveedor2,
                                       PerteneceEmpresa = c.PerteneceEmpresa,
                                       ClienteRecoger = c.ClienteRecoger,
                                       EnviarlaMensajero = c.EnviarlaMensajero,
                                       ConfirmacionCorreo = c.ConfirmacionCorreo,
                                       DireccionEnvio = c.DireccionEnvio,
                                       State= c.State,
                                       City = c.City,
                                       CountryName = c.CountryName,
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

        [HttpPost("[action]")]
        public async Task<ActionResult> CustomerByRTN([FromBody]Customer customer)
        {
            Customer _customer = new Customer();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerByRTN/" + customer.RTN);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => Ok(_customer));
        }

        //[HttpPost("[controller]/[action]")]
        //public async Task<ActionResult<Customer>> SaveCustomer([FromBody]Customer _Customer)
        //{

        //    try
        //    {
        //        Customer _listCustomer = new Customer();
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerById/" + _Customer.CustomerId);
        //        string valorrespuesta = "";
        //        _Customer.FechaModificacion = DateTime.Now;
        //        _Customer.UsuarioModificacion = HttpContext.Session.GetString("user");
        //        //_Customer.CustomerTypeId = 2;
        //        if (result.IsSuccessStatusCode)
        //        {

        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _listCustomer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
        //        }

        //        if (_listCustomer==null|| _listCustomer.CustomerId == 0)
        //        {
        //            _Customer.FechaCreacion = DateTime.Now;
        //            _Customer.UsuarioCreacion = HttpContext.Session.GetString("user");
        //            var insertresult = await Post(_Customer);
        //            var value = (insertresult.Result as ObjectResult).Value;
        //            _Customer = ((Customer)(value));
        //            if (_Customer.CustomerId == 0)
        //            {
        //                return await Task.Run(() => BadRequest("El RTN/Identidad ya existe!"));
        //            }
        //        }
        //        else
        //        {
        //            _Customer.UsuarioCreacion = _listCustomer.UsuarioCreacion == "" ? HttpContext.Session.GetString("user") : _listCustomer.UsuarioCreacion;
        //            _Customer.FechaCreacion = _listCustomer.FechaCreacion==null?DateTime.Now: _listCustomer.FechaCreacion;

        //            var updateresult = await Put(_Customer);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }

        //    return Json(_Customer);
        //}

        [HttpPost]
        public async Task<ActionResult<Customer>> SaveCustomer([FromBody]Customer _FundingInterestRateS)
        {
            {
                _FundingInterestRateS.RTN=_FundingInterestRateS.RTN.Replace("-","");
                _FundingInterestRateS.Identidad = _FundingInterestRateS.RTN.Replace("-", "");
                string valorrespuesta = "";
                try
                {
                    Customer _listVendorType = new Customer();
                    Customer CustomerC = new Customer();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerByRTN/" + _FundingInterestRateS.RTN);
                   
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _listVendorType = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
                        if (_listVendorType != null && _FundingInterestRateS.CustomerId == 0)
                        {
                            if (_listVendorType.RTN == _FundingInterestRateS.RTN)
                            {
                                return await Task.Run(() => BadRequest($"Ya existe un cliente con ese RTN."));
                            }
                        }

                    }

                    result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerById/" + _FundingInterestRateS.CustomerId);
                    _FundingInterestRateS.FechaModificacion = DateTime.Now;
                    _FundingInterestRateS.UsuarioModificacion = HttpContext.Session.GetString("user");

                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
                }



                if (_listVendorType == null) { _listVendorType = new Models.Customer(); }

                    if (_listVendorType.CustomerId == 0)
                    {
                    _listVendorType.FechaCreacion = DateTime.Now;
                    _listVendorType.UsuarioCreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Post(_FundingInterestRateS);
                    }
                    else
                    {
                    if (CustomerC != null) {
                        if (CustomerC.RTN == _FundingInterestRateS.RTN && CustomerC.CustomerId != _listVendorType.CustomerId)
                        {
                            return await Task.Run(() => BadRequest($"Ya existe una cliente con este RTN/Identidad."));
                        }
                    }
                    _listVendorType.FechaCreacion = DateTime.Now;
                    _FundingInterestRateS.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var updateresult = await Put(_FundingInterestRateS);
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
        public async Task<ActionResult> Details(Int64 CustomerId)
        {
            Customer _customers = new Customer();
            
            if (CustomerId==0)
            {
                return await Task.Run(() => View(_customers));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomerById/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<Customer>(valorrespuesta);

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

        // GET: Customer/Create
        public async Task<ActionResult> Create()
        {
            return await Task.Run(() => View());
        }


        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Customer> _customers = new List<Customer>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomer");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Customer>>(valorrespuesta);
                    _customers = _customers.OrderByDescending(q => q.CustomerId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return await Task.Run(() => _customers.ToDataSourceResult(request));

        }

        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetUsuario([DataSourceRequest]DataSourceRequest request)
        {
            List<Customer> _customers = new List<Customer>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomer");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customers = JsonConvert.DeserializeObject<List<Customer>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return await Task.Run(() => _customers.ToDataSourceResult(request));

        }

        


        // POST: Customer/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<Customer>> Post(Customer _customer)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _customer.UsuarioCreacion = HttpContext.Session.GetString("user");
                _customer.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Customer/Insert", _customer);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return await Task.Run(() => BadRequest($"Ocurrio un error{ex.Message}"));
            }

           // return await Task.Run(() => Ok(_customer));
             return await Task.Run(() => new ObjectResult(new DataSourceResult { Data = new[] { _customer }, Total = 1 }));
        }

        [HttpPost]
        public async Task<IActionResult> Put(Customer _customer)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Customer/Update", _customer);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _customer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return await Task.Run(() => new ObjectResult(new DataSourceResult { Data = new[] { _customer }, Total = 1 }));
        }

        // GET: Customer/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await Task.Run(() => View());
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return View();
            }
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> Delete([FromBody]Customer _Customer)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Customer/Remove", _Customer);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Customer = JsonConvert.DeserializeObject<Customer>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _Customer }, Total = 1 });
        }



    }
}