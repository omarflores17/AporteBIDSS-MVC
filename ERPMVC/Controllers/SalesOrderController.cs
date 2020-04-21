using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ERPMVC.DTO;
using ERPMVC.Helpers;
using ERPMVC.Models;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static ERPMVC.Helpers.ViewRenderService;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class SalesOrderController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //  private readonly ILogger _logger;
        private readonly IOptions<MyConfig> _config;
        private readonly IMapper mapper;
        private readonly ILogger _logger;
        private readonly IViewRenderService _viewRenderService;
        private readonly ViewRender view;

        //public SalesOrderController(ILogger<SalesOrderController> logger,IOptions<MyConfig> config)
        public SalesOrderController(ILogger<SalesOrderController> logger, IOptions<MyConfig> config
            , IMapper mapper, IViewRenderService viewRenderService
            , ViewRender view
            )
        {
            this.mapper = mapper;
            this._logger = logger;
            this._config = config;
            this._viewRenderService = viewRenderService;
            this.view = view;
        }
        //metodo para llamar la descripcion, codigo del producto
        async Task<IEnumerable<Product>> Obtenerproducto()
        {
            IEnumerable<Product> Product = null;
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
        [CustomAuthorization]
        public IActionResult Index()
        {
            // SalesOrderDTO _dto = new SalesOrderDTO();
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        //[HttpPost("[action]")]
        public async Task<ActionResult> Details(Int64 SalesOrderId)
        {
            ViewData["producto"] = await Obtenerproducto();
            SalesOrderDTO _SalesOrderDTO = new SalesOrderDTO();
            if (SalesOrderId == 0)
            {
                _SalesOrderDTO = new SalesOrderDTO
                {
                    ExpirationDate = DateTime.Now.AddDays(30),
                    DeliveryDate = DateTime.Now,
                    OrderDate = DateTime.Now
,
                    editar = _SalesOrderDTO.editar,
                    SalesOrderId = _SalesOrderDTO.SalesOrderId
,
                    BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"))
                };
                return await Task.Run(() => View(_SalesOrderDTO));
            }
            try
            {
                //_SalesOrderP = JsonConvert.DeserializeObject<SalesOrderDTO>(dto.ToString());
                //_SalesOrder = _SalesOrderP;
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderById/" + SalesOrderId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrderDTO = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);
                }
                // var editresult = await pvwSalesOrder(_SalesOrderDTO);
                //ViewData["cotizacion"] = _SalesOrderDTO;
                //ViewData["GoodsReceivedId"] = _SalesOrderDTO.SalesOrderId.ToString();
//                if (_SalesOrderDTO == null)
//                {
//                    _SalesOrderDTO = new SalesOrderDTO
//                    {
//                        ExpirationDate = DateTime.Now.AddDays(30),
//                        DeliveryDate = DateTime.Now,
//                        OrderDate = DateTime.Now
//,
//                        editar = _SalesOrderDTO.editar,
//                        SalesOrderId = _SalesOrderDTO.SalesOrderId
//,
//                        BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"))
//                    };
//                }
//                _SalesOrderDTO.editar = _SalesOrderDTO.editar;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_SalesOrderDTO));
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwModalProduct([FromBody]Product _SalesO)
        {
            SalesOrder _SalesOrder = new SalesOrder();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Product/GetProductById/" + _SalesO.ProductId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrder = JsonConvert.DeserializeObject<SalesOrder>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_SalesOrder);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwSalesOrderDetailMant([FromBody]SalesOrderLine _salesorderline)
        {
            SalesOrderLine _salesorderf = new SalesOrderLine();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrderLine/GetSalesOrderLineById/", _salesorderline);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _salesorderf = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);
                }

                if (_salesorderf == null) { _salesorderf = new SalesOrderLine { Description = "" }; }
                //_salesorderf.editar = _salesorderline.editar;



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView("~/Views/SalesOrder/pvwSalesOrderDetailMant.cshtml", _salesorderf);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> pvwModalCustomer([FromBody]Customer _Cust)
        {
            Customer _Customer = new Customer();
            try
            {
                string baseadress = _config.Value.urlbase;
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
        public async Task<ActionResult> pvwSalesOrder([FromBody]SalesOrderDTO _salesorder)
        {
            SalesOrderDTO _salesorderf = new SalesOrderDTO();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetById/" + _salesorder.SalesOrderId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _salesorderf = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);
                }

                if (_salesorderf == null)
                {
                    _salesorderf = new SalesOrderDTO
                    {
                        ExpirationDate = DateTime.Now.AddDays(30),
                        DeliveryDate = DateTime.Now,
                        OrderDate = DateTime.Now
,
                        editar = _salesorder.editar,
                        SalesOrderId = _salesorder.SalesOrderId
,
                        BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"))
                    };
                }
                _salesorderf.editar = _salesorder.editar;



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return View(_salesorderf);
        }


        [HttpGet("[action]")]
        public async Task<JsonResult> GetSalesOrder([DataSourceRequest]DataSourceRequest request)
        {
            List<SalesOrder> _SalesOrders = new List<SalesOrder>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrder");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrders = JsonConvert.DeserializeObject<List<SalesOrder>>(valorrespuesta);
                    _SalesOrders = _SalesOrders.OrderByDescending(q => q.SalesOrderId).ToList();
                }
                //else if(result.StatusCode== 401)
                //{
                ViewData["cotizacion"] = request;
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
            }

            return Json(_SalesOrders.ToDataSourceResult(request));
            //return _SalesOrders.ToDataSourceResult(request);
        }


        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetSalesOrderByCustomerId([DataSourceRequest]DataSourceRequest request, Int64 CustomerId)
        {
            List<SalesOrder> _SalesOrders = new List<SalesOrder>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderByCustomerId/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrders = JsonConvert.DeserializeObject<List<SalesOrder>>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
            }


            return _SalesOrders.ToDataSourceResult(request);
        }

        public async Task<ActionResult> GetSalesOrderById(Int64 Id)
        {
            SalesOrder _ControlPallets = new SalesOrder();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ControlPallets = JsonConvert.DeserializeObject<SalesOrder>(valorrespuesta);

                }

                if (_ControlPallets == null)
                {
                    _ControlPallets = new SalesOrder();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_ControlPallets);
        }



        public async Task<ActionResult> EnviarCotizacionA([DataSourceRequest]DataSourceRequest request, SalesOrderDTO _SalesOrderDTO)
        {

            try
            {
                SalesOrderDTO _salesorderf = new SalesOrderDTO();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetById/" + _SalesOrderDTO.SalesOrderId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _salesorderf = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);
                }

                _salesorderf.IdEstado = _SalesOrderDTO.IdEstado;

                var resultado = await Update(_SalesOrderDTO.SalesOrderId, _salesorderf);
                var value = (resultado.Result as ObjectResult).Value;
                SalesOrder _result = (SalesOrder)value;

                switch (_salesorderf.IdEstado)
                {
                    //Factura proforma
                    case 3:

                        break;
                    //Factura fiscal
                    case 4:
                    case 6:
                        break;


                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return RedirectToAction("", "");
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<SalesOrderDTO>> Aprobar([FromBody]SalesOrderDTO _SalesOrder)
        {
            SalesOrderDTO _so = new SalesOrderDTO();
            if (_SalesOrder != null)
            {
                try
                {
                    string baseadress = _config.Value.urlbase;
                    HttpClient _client = new HttpClient();

                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderById/" + _SalesOrder.SalesOrderId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _so = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);


                        //  _SalesOrder.UsuarioCreacion = HttpContext.Session.GetString("user");
                        // _SalesOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
                        _so.IdEstado = 6;
                        _so.Estado = "Aprobado";
                        var resultsalesorder = await Update(_so.SalesOrderId, _so);

                        var value = (resultsalesorder.Result as ObjectResult).Value;
                        SalesOrder resultado = ((SalesOrder)(value));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
            }
            else
            {
                return await Task.Run(() => BadRequest("No llego correctamente el modelo!"));
            }

            return await Task.Run(() => Ok(_so));
        }


        //[HttpPost("[action]")]
        //public async Task<ActionResult<SalesOrder>> SaveSalesOrder([FromBody]SalesOrderDTO _SalesOrderP)

        //{
        //    SalesOrder _SalesOrder = _SalesOrderP;
        //    try
        //    {
        //        //SalesOrderDTO _listVendorType = new SalesOrderDTO();
        //        string baseadress = _config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderById/" + _SalesOrder.SalesOrderId);
        //        string valorrespuesta = "";
        //        _SalesOrder.FechaModificacion = DateTime.Now;
        //        _SalesOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
        //        if (result.IsSuccessStatusCode)
        //        {

        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _SalesOrder = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);
        //        }

        //        if (_SalesOrder == null) { _SalesOrder = new Models.SalesOrder(); }

        //        if (_SalesOrderP.SalesOrderId == 0)
        //        {
        //            _SalesOrder.FechaCreacion = DateTime.Now;
        //            _SalesOrder.UsuarioCreacion = HttpContext.Session.GetString("user");
        //            var insertresult = await Insert(_SalesOrderP);
        //        }
        //        else
        //        {
        //            _SalesOrderP.FechaCreacion = _SalesOrder.FechaCreacion;
        //            _SalesOrderP.UsuarioCreacion = _SalesOrder.UsuarioCreacion;
        //            var updateresult = await Update(_SalesOrder.SalesOrderId, _SalesOrderP);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }

        //    return Json(_SalesOrder);
        //}


        [HttpPost("[action]")]
        //public async Task<ActionResult<SalesOrder>> SaveSalesOrder([FromBody]SalesOrderDTO _SalesOrderP)
        public async Task<ActionResult<SalesOrder>> SaveSalesOrder([FromBody]dynamic dto)
        //  public async Task<ActionResult<SalesOrder>> SaveSalesOrder([FromBody]dynamic dto)
        // public async Task<ActionResult<SalesOrder>> SaveSalesOrder(Newtonsoft.Json.Linq.JObject datos)
        {
            SalesOrderDTO _SalesOrderP = new SalesOrderDTO(); //_JournalEntryP;
            SalesOrder _SalesOrder = new SalesOrder();


            try
            {
                _SalesOrderP = JsonConvert.DeserializeObject<SalesOrderDTO>(dto.ToString());
                _SalesOrder = _SalesOrderP;
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderById/" + _SalesOrder.SalesOrderId);
                string valorrespuesta = "";
                _SalesOrder.FechaModificacion = DateTime.Now;
                _SalesOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrder = JsonConvert.DeserializeObject<SalesOrder>(valorrespuesta);
                }

                if (_SalesOrder == null) { _SalesOrder = new Models.SalesOrder(); }

                if (_SalesOrderP.SalesOrderId == 0)
                {
                    _SalesOrder.FechaCreacion = DateTime.Now;
                    _SalesOrder.UsuarioCreacion = HttpContext.Session.GetString("user");

                    //foreach (var item in _SalesOrderP.JournalEntryLines)
                    //{
                    //    item.CreatedUser = HttpContext.Session.GetString("user");
                    //    item.ModifiedUser = HttpContext.Session.GetString("user");
                    //}

                    var insertresult = await Insert(_SalesOrderP);

                    //var value = (insertresult.Result as ObjectResult).Value;
                    //SalesOrder resultado = ((SalesOrder)(value));
                    //if (resultado.SalesOrderId <= 0)

                    //{
                    //    return BadRequest("No se guardo el formulario!");
                    //}
                }

                else
                {
                    _SalesOrderP.UsuarioCreacion = _SalesOrder.UsuarioCreacion;
                    _SalesOrderP.FechaCreacion = _SalesOrder.FechaCreacion;
                    var updateresult = await Update(_SalesOrder.SalesOrderId, _SalesOrderP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_SalesOrderP);

        }



        private bool RemoteServerCertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            //Console.WriteLine(certificate);
            return true;
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<SalesOrder>> GenerarContrato([FromBody]SalesOrderDTO _SalesOrderp)
        //  public async Task<ActionResult<SalesOrder>> GenerarContrato([FromBody]dynamic dto)
        {

            //     _SalesOrder = JsonConvert.DeserializeObject<SalesOrderDTO>(dto.ToString());           
            SalesOrder _SalesOrdermodel = new SalesOrder();
            if (_SalesOrderp != null)
            {
                try
                {

                    string baseadress = _config.Value.urlbase;
                    HttpClient _client = new HttpClient();

                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrderById/" + _SalesOrderp.SalesOrderId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _SalesOrdermodel = JsonConvert.DeserializeObject<SalesOrder>(valorrespuesta);
                    }

                    CustomerContract _customercontract = new CustomerContract();
                    _customercontract.SalesOrderId = _SalesOrderp.SalesOrderId;

                    try
                    {
                        _customercontract.UsedArea = _SalesOrdermodel.SalesOrderLines
                    .Where(q => q.SubProductName.Contains("Almacenaje")).Select(q => q.Price).FirstOrDefault();

                        _customercontract.UnitOfMeasureId = _SalesOrdermodel.SalesOrderLines
                          .Where(q => q.SubProductName.Contains("Almacenaje")).Select(q => q.UnitOfMeasureId).FirstOrDefault();

                        _customercontract.UnitOfMeasureName = _SalesOrdermodel.SalesOrderLines
                        .Where(q => q.SubProductName.Contains("Almacenaje")).Select(q => q.UnitOfMeasureName).FirstOrDefault();

                        _customercontract.ValueSecure = _SalesOrdermodel.SalesOrderLines
                               .Where(q => q.SubProductName.Contains("Seguro")).Select(q => q.Price).FirstOrDefault();

                        _customercontract.ValueBascula = _SalesOrdermodel.SalesOrderLines
                                               .Where(q => q.SubProductName.Contains("Bascula")).Select(q => q.Price).FirstOrDefault();
                        _customercontract.BandaTransportadora = _SalesOrdermodel.SalesOrderLines
                                                       .Where(q => q.SubProductName.Contains("Banda")).Select(q => q.Price).FirstOrDefault();
                        _customercontract.MontaCargas = _SalesOrdermodel.SalesOrderLines
                                                     .Where(q => q.SubProductName.Contains("Monta carga")).Select(q => q.Price).FirstOrDefault();
                        _customercontract.Papeleria = _SalesOrdermodel.SalesOrderLines
                                 .Where(q => q.SubProductName.Contains("Papeleria")).Select(q => q.Price).FirstOrDefault();
                        _customercontract.ExtraHours = _SalesOrdermodel.SalesOrderLines
                                .Where(q => q.SubProductName.Contains("Horas Extras")).Select(q => q.Price).FirstOrDefault();

                        _customercontract.FoodPayment = _SalesOrdermodel.SalesOrderLines
                               .Where(q => q.SubProductName.Contains("Alimentacion")).Select(q => q.Price).FirstOrDefault();

                        _customercontract.Transport = _SalesOrdermodel.SalesOrderLines
                             .Where(q => q.SubProductName.Contains("Transporte")).Select(q => q.Price).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                        return await Task.Run(() => BadRequest("Asegurese que la cotización tenga los elementos necesarios para generar un contrato  como: Almacenaje,Seguro,Bascula,Banda,Monta carga,Papeleria,Horas Extras,Alimentación,Transporte"));

                    }
                  


                    _logger.LogInformation($"Despues del transporte");

                    CustomerConditions _cc = new CustomerConditions();
                    List<CustomerConditions> _cclist = new List<CustomerConditions>();
                    _cc.DocumentId = _SalesOrdermodel.SalesOrderId;
                    _cc.IdTipoDocumento = 12;
                    _client = new HttpClient();

                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    result = await _client.PostAsJsonAsync(baseadress + "api/CustomerConditions/GetCustomerConditionsByClass", _cc);
                    valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _cclist = JsonConvert.DeserializeObject<List<CustomerConditions>>(valorrespuesta);
                    }

                    _logger.LogInformation($"Despues de consultar las condiciones del cliente Cantidad de condiciones: {_cclist.Count}");

                    _customercontract.Porcentaje1 = _cclist
                           .Where(q => q.CustomerConditionName.Contains("enor")).Select(q => q.ValueDecimal).FirstOrDefault();

                    _customercontract.Valor1 = _cclist
                         .Where(q => q.CustomerConditionName.Contains("enor")).Select(q => Convert.ToDouble(q.ValueToEvaluate)).FirstOrDefault();

                    _customercontract.Porcentaje2 = _cclist
                           .Where(q => q.CustomerConditionName.Contains("ayor")).Select(q => q.ValueDecimal).FirstOrDefault();

                    _customercontract.Valor2 = _cclist
                         .Where(q => q.CustomerConditionName.Contains("ayor")).Select(q => Convert.ToDouble(q.ValueToEvaluate)).FirstOrDefault();


                    _customercontract.CustomerId = _SalesOrdermodel.CustomerId.Value;
                    _customercontract.CustomerName = _SalesOrdermodel.CustomerName;
                    _customercontract.ProductId = _SalesOrdermodel.ProductId;
                    _customercontract.ProductName = _SalesOrdermodel.ProductName;


                    _logger.LogInformation($"Antes de recuperar la empresa");
                    CompanyInfo _company = new CompanyInfo { CompanyInfoId = 1 };
                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    result = await _client.GetAsync(baseadress + "api/CompanyInfo/GetCompanyInfoById/" + _company.CompanyInfoId);
                    valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _company = JsonConvert.DeserializeObject<CompanyInfo>(valorrespuesta);
                    }

                    _customercontract.Manager = _company.Manager;
                    _customercontract.RTNMANAGER = _company.RTNMANAGER;

                    _logger.LogInformation($"Despues de manager");

                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    result = await _client.PostAsJsonAsync(baseadress + "api/CustomerContract/Insert", _customercontract);

                    _logger.LogInformation($"Despues del insertar el contrato!");
                    valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _customercontract = JsonConvert.DeserializeObject<CustomerContract>(valorrespuesta);
                        _logger.LogInformation($"Fue satisfactorio la generacion!");
                    }
                    else
                    {
                        string request = await result.Content.ReadAsStringAsync();
                        return await Task.Run(() => BadRequest(request));
                    }


                    return await Task.Run(() => Json(_customercontract));
                }

                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    return await Task.Run(() => BadRequest($"{ex.ToString()}!"));
                }

            }
            else
            {
                return await Task.Run(() => BadRequest("No llego correctamente el modelo!"));
            }
        }



        [HttpPost("[action]")]
        public async Task<ActionResult<SalesOrder>> Insert(SalesOrder _SalesOrder)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _SalesOrder.UsuarioCreacion = HttpContext.Session.GetString("user");
                _SalesOrder.FechaCreacion = DateTime.Now;
                _SalesOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
                _SalesOrder.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrder/Insert", _SalesOrder);
                string jsonresult = "";
                jsonresult = JsonConvert.SerializeObject(_SalesOrder);

                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrder = JsonConvert.DeserializeObject<SalesOrder>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_SalesOrder);
            // return new ObjectResult(new DataSourceResult { Data = new[] { _role }, Total = 1 });
        }


        [HttpPut("[action]")]
        public async Task<ActionResult<SalesOrder>> Update(Int64 Id, SalesOrderDTO _SalesOrderLine)
        {
            try
            {

                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrder/Update", _SalesOrderLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrderLine = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            // return new ObjectResult(new DataSourceResult { Data = new[] { _SalesOrderLine }, Total = 1 });
            return Ok(_SalesOrderLine);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<SalesOrderDTO>> Delete([FromBody]SalesOrderDTO _rol)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrder/Delete", _rol);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _rol = JsonConvert.DeserializeObject<SalesOrderDTO>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _rol }, Total = 1 });
        }



        // [HttpGet("{SalesOrderId}")]
        //public  ActionResult AR(Int32 SalesOrderId)
        // {

        //     SalesOrderDTO _salesorderdto = new SalesOrderDTO { SalesOrderId = SalesOrderId, token = HttpContext.Session.GetString("token") };

        //     return View(_salesorderdto);
        // }


        // [HttpGet("[controller]/[action]/{SalesOrderId}")]
        //    [HttpGet("{SalesOrderId}")]
        [HttpGet]
        public ActionResult SFCotizacion(Int32 id)
        {

            SalesOrderDTO _salesorderdto = new SalesOrderDTO { SalesOrderId = id, }; //token = HttpContext.Session.GetString("token") };

            return View(_salesorderdto);
        }




        public async Task<ActionResult> Virtualization_Read([DataSourceRequest] DataSourceRequest request)
        {
            var res = await GetSalesOrder();
            return Json(res.ToDataSourceResult(request));
        }

        public async Task<ActionResult> Orders_ValueMapper(Int64[] values)
        {
            var indices = new List<Int64>();

            if (values != null && values.Any())
            {
                var index = 0;

                foreach (var order in await GetSalesOrder())
                {
                    if (values.Contains(order.SalesOrderId))
                    {
                        indices.Add(index);
                    }

                    index += 1;
                }
            }

            return Json(indices);
        }

        private async Task<List<SalesOrder>> GetSalesOrder()
        {
            List<SalesOrder> _SalesOrder = new List<SalesOrder>();

            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrder/GetSalesOrder");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrder = JsonConvert.DeserializeObject<List<SalesOrder>>(valorrespuesta);
                    _SalesOrder = (from c in _SalesOrder
                                   select new SalesOrder
                                   {
                                       RTN = c.RTN,
                                       SalesOrderId = c.SalesOrderId,
                                       SalesOrderName = "Id:" + c.SalesOrderId + "|| Nombre:" + c.SalesOrderName + "|| Fecha:" + c.OrderDate + "|| Total:" + c.Total,
                                       OrderDate = c.OrderDate,
                                   }).ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _SalesOrder;
        }





    }
}