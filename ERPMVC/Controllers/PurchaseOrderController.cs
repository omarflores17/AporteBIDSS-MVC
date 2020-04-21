using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
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
    public class PurchaseOrderController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //  private readonly ILogger _logger;
        private readonly IOptions<MyConfig> _config;
        private readonly IMapper mapper;
        private readonly ILogger _logger;
        private readonly IViewRenderService _viewRenderService;
        private readonly ViewRender view;
        private readonly ClaimsPrincipal _principal;

        //public PurchaseOrderController(ILogger<PurchaseOrderController> logger,IOptions<MyConfig> config)
        public PurchaseOrderController(ILogger<PurchaseOrderController> logger, IOptions<MyConfig> config
            , IMapper mapper, IViewRenderService viewRenderService
            , ViewRender view , IHttpContextAccessor httpContextAccessor
            )
        {
            this.mapper = mapper;
            this._logger = logger;
            this._config = config;
            this._viewRenderService = viewRenderService;
            this.view = view;
            _principal = httpContextAccessor.HttpContext.User;
        }

        [Authorize(Policy = "Ordenes de Compras")]
        public async Task<IActionResult> Index()
        {
          ViewData["Sucursal"] = await ObtenerSucusal();
            return View();
        }

        [Authorize(Policy = "Inventarios.Ordenes de Compra.Ingresadas")]
        public async Task<IActionResult> Ingresadas()
        {
            ViewData["EstadoId"] = 3;
            ViewData["Permisos"] = _principal;
            ViewData["Sucursal"] = await ObtenerSucusal();
            return View("Index");
        }
        [Authorize(Policy = "Inventarios.Ordenes de Compra.Enviadas")]
        public async Task<IActionResult> Enviadas()
        {
            ViewData["EstadoId"] = 4;
            ViewData["Permisos"] = _principal;
            ViewData["Sucursal"] = await ObtenerSucusal();
            return View("Index");
        }
        [Authorize(Policy = "Inventarios.Ordenes de Compra.Aprobadas")]
        public async Task<IActionResult> Aprobadas()
        {
            ViewData["EstadoId"] = 5;
            ViewData["Permisos"] = _principal;
            ViewData["Sucursal"] = await ObtenerSucusal();
            return View("Index");
        }
        [Authorize(Policy = "Inventarios.Ordenes de Compra.Cerradas")]
        public async Task<IActionResult> Cerradas()
        {
            ViewData["EstadoId"] = 6;
            ViewData["Permisos"] = _principal;
            ViewData["Sucursal"] = await ObtenerSucusal();
            return  View("Index");
        }

        //metodo para llamar la descripcion de la sucursal
        async Task<IEnumerable<Branch>> ObtenerSucusal()
        {
            IEnumerable<Branch> Branch = null;
            try
            {
                string baseadress = _config.Value.urlbase;
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

        public async Task<ActionResult> VerDetalles(Int64 Id, string proceso)
        {
            PurchaseOrderDTO _PurchaseOrderDTO = new PurchaseOrderDTO();
            HttpClient _client = new HttpClient();
            if (Id == 0)
            {
                _PurchaseOrderDTO.DatePlaced = DateTime.Now;
                _PurchaseOrderDTO.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                _PurchaseOrderDTO.EstadoId = 3;
                _PurchaseOrderDTO.proceso = proceso;
                ///_PurchaseOrderDTO.Estados.NombreEstado;
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(_config.Value.urlbase + "api/PurchaseOrder/GetPONumberCorrelative");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    // _PurchaseOrderDTO = JsonConvert.DeserializeObject<string>(valorrespuesta);
                }
                _PurchaseOrderDTO.PONumber = valorrespuesta;
                return await Task.Run(() => View(_PurchaseOrderDTO));
            }
            try
            {
                string baseadress = _config.Value.urlbase;

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderDTO = JsonConvert.DeserializeObject<PurchaseOrderDTO>(valorrespuesta);
                    _PurchaseOrderDTO.proceso = proceso;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => View(_PurchaseOrderDTO));
        }


        //[HttpGet("[controller]/[action]/{Id}/{proceso}")]
        public async Task<ActionResult> Details(Int64 Id,string proceso)
        {
            PurchaseOrderDTO _PurchaseOrderDTO = new PurchaseOrderDTO();
            HttpClient _client = new HttpClient();
            if (Id == 0)
            {
                _PurchaseOrderDTO.DatePlaced = DateTime.Now;
                _PurchaseOrderDTO.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                _PurchaseOrderDTO.EstadoId = 3;
                _PurchaseOrderDTO.proceso = proceso;
                ///_PurchaseOrderDTO.Estados.NombreEstado;
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(_config.Value.urlbase + "api/PurchaseOrder/GetPONumberCorrelative");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                   // _PurchaseOrderDTO = JsonConvert.DeserializeObject<string>(valorrespuesta);
                }
                _PurchaseOrderDTO.PONumber = valorrespuesta;
                return await Task.Run(() => View(_PurchaseOrderDTO));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
               
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderDTO = JsonConvert.DeserializeObject<PurchaseOrderDTO>(valorrespuesta);
                    _PurchaseOrderDTO.proceso = proceso;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => View(_PurchaseOrderDTO));
        }
  

      
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwPurchaseOrderDetailMant([FromBody]PurchaseOrderLine _PurchaseOrderline)
        {
            PurchaseOrderLine _PurchaseOrderf = new PurchaseOrderLine();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/PurchaseOrderLine/GetPurchaseOrderLineById/", _PurchaseOrderline);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderf = JsonConvert.DeserializeObject<PurchaseOrderLine>(valorrespuesta);
                }

                if (_PurchaseOrderf == null) { _PurchaseOrderf = new PurchaseOrderLine { ProductDescription = "" }; }
                //_PurchaseOrderf.editar = _PurchaseOrderline.editar;



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView("~/Views/PurchaseOrder/pvwPurchaseOrderDetailMant.cshtml", _PurchaseOrderf);
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
        public async Task<ActionResult> pvwPurchaseOrder([FromBody]PurchaseOrderDTO _PurchaseOrder)
        {
            PurchaseOrderDTO _PurchaseOrderf = new PurchaseOrderDTO();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + _PurchaseOrder.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderf = JsonConvert.DeserializeObject<PurchaseOrderDTO>(valorrespuesta);
                }

                if (_PurchaseOrderf == null)
                {
                    _PurchaseOrderf = new PurchaseOrderDTO
                    {
                        DatePlaced = DateTime.Now
,
                        editar = _PurchaseOrder.editar,
                        Id = _PurchaseOrder.Id
,
                        BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"))
                    };
                }
                _PurchaseOrderf.editar = _PurchaseOrder.editar;



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return View(_PurchaseOrderf);
        }


        [HttpGet("[action]")]
        public async Task<JsonResult> GetPurchaseOrder([DataSourceRequest]DataSourceRequest request)
        {
            List<PurchaseOrder> _PurchaseOrders = new List<PurchaseOrder>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrder");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrders = JsonConvert.DeserializeObject<List<PurchaseOrder>>(valorrespuesta);
                    _PurchaseOrders = _PurchaseOrders.OrderByDescending(q => q.Id).ToList();
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

            return Json(_PurchaseOrders.ToDataSourceResult(request));
            //return _PurchaseOrders.ToDataSourceResult(request);
        }


       
        public async Task<ActionResult> GetPurchaseOrderById(Int64 Id)
        {
            PurchaseOrder _ControlPallets = new PurchaseOrder();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ControlPallets = JsonConvert.DeserializeObject<PurchaseOrder>(valorrespuesta);

                }

                if (_ControlPallets == null)
                {
                    _ControlPallets = new PurchaseOrder();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_ControlPallets);
        }



        public async Task<ActionResult> EnviarCotizacionA([DataSourceRequest]DataSourceRequest request, PurchaseOrderDTO _PurchaseOrderDTO)
        {

            try
            {
                PurchaseOrderDTO _PurchaseOrderf = new PurchaseOrderDTO();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + _PurchaseOrderDTO.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderf = JsonConvert.DeserializeObject<PurchaseOrderDTO>(valorrespuesta);
                }

                _PurchaseOrderf.EstadoId = _PurchaseOrderDTO.EstadoId;

                var resultado = await Update(_PurchaseOrderDTO.Id, _PurchaseOrderf);
                var value = (resultado.Result as ObjectResult).Value;
                PurchaseOrder _result = (PurchaseOrder)value;

                switch (_PurchaseOrderf.EstadoId)
                {
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
        public async Task<ActionResult<PurchaseOrderDTO>> Aprobar([FromBody]PurchaseOrderDTO _PurchaseOrder)
        {
            PurchaseOrderDTO _so = new PurchaseOrderDTO();
            if (_PurchaseOrder != null)
            {
                try
                {
                    string baseadress = _config.Value.urlbase;
                    HttpClient _client = new HttpClient();

                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + _PurchaseOrder.Id);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _so = JsonConvert.DeserializeObject<PurchaseOrderDTO>(valorrespuesta);


                        //  _PurchaseOrder.UsuarioCreacion = HttpContext.Session.GetString("user");
                        // _PurchaseOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
                        _so.EstadoId = 6;
                        var resultPurchaseOrder = await Update(_so.Id, _so);

                        var value = (resultPurchaseOrder.Result as ObjectResult).Value;
                        PurchaseOrder resultado = ((PurchaseOrder)(value));
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

        


        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<PurchaseOrder>> SavePurchaseOrder([FromBody]dynamic dto)
       
        {
            PurchaseOrderDTO _PurchaseOrderP = new PurchaseOrderDTO(); 
            PurchaseOrder _PurchaseOrder = new PurchaseOrder();


            try
            {
                _PurchaseOrderP = JsonConvert.DeserializeObject<PurchaseOrderDTO>(dto.ToString());
                _PurchaseOrder = _PurchaseOrderP;
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrderById/" + _PurchaseOrder.Id);
                string valorrespuesta = "";
                _PurchaseOrder.FechaModificacion = DateTime.Now;
                _PurchaseOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
                _PurchaseOrder.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                _PurchaseOrder.DatePlaced = DateTime.Now;
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrder>(valorrespuesta);
                }

                if (_PurchaseOrder == null) { _PurchaseOrder = new Models.PurchaseOrder(); }

                if (_PurchaseOrderP.Id == 0)
                {
                    _PurchaseOrder.FechaCreacion = DateTime.Now;
                    _PurchaseOrder.UsuarioCreacion = HttpContext.Session.GetString("user");
                    
                    var insertresult = await Insert(_PurchaseOrderP);

                 
                }

                else
                {
                    _PurchaseOrderP.UsuarioCreacion = _PurchaseOrder.UsuarioCreacion;
                    _PurchaseOrderP.FechaCreacion = _PurchaseOrder.FechaCreacion;
                    var updateresult = await Update(_PurchaseOrder.Id, _PurchaseOrderP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_PurchaseOrder);

        }



        private bool RemoteServerCertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            //Console.WriteLine(certificate);
            return true;
        }

        

        [HttpPost("[action]")]
        public async Task<ActionResult<PurchaseOrder>> Insert(PurchaseOrder _PurchaseOrder)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _PurchaseOrder.UsuarioCreacion = HttpContext.Session.GetString("user");
                _PurchaseOrder.FechaCreacion = DateTime.Now;
                _PurchaseOrder.UsuarioModificacion = HttpContext.Session.GetString("user");
                _PurchaseOrder.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/PurchaseOrder/Insert", _PurchaseOrder);
                string jsonresult = "";
                jsonresult = JsonConvert.SerializeObject(_PurchaseOrder);

                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrder>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_PurchaseOrder);
        }


        [HttpPut("[action]")]
        public async Task<ActionResult<PurchaseOrder>> Update(Int64 Id, PurchaseOrderDTO _PurchaseOrderLine)
        {
            try
            {

                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/PurchaseOrder/Update", _PurchaseOrderLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderLine = JsonConvert.DeserializeObject<PurchaseOrderDTO>(valorrespuesta);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            // return new ObjectResult(new DataSourceResult { Data = new[] { _PurchaseOrderLine }, Total = 1 });
            return Ok(_PurchaseOrderLine);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> Delete([FromBody]PurchaseOrder _rol)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/PurchaseOrder/Delete", _rol);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _rol = JsonConvert.DeserializeObject<PurchaseOrder>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _rol }, Total = 1 });
        }


        
        [HttpGet]
        public ActionResult SFCotizacion(Int32 id)
        {

            PurchaseOrderDTO _PurchaseOrderdto = new PurchaseOrderDTO { Id = id, };

            return View(_PurchaseOrderdto);
        }




        public async Task<ActionResult> Virtualization_Read([DataSourceRequest] DataSourceRequest request)
        {
            var res = await GetPurchaseOrder();
            return Json(res.ToDataSourceResult(request));
        }

        public async Task<ActionResult> Orders_ValueMapper(Int64[] values)
        {
            var indices = new List<Int64>();

            if (values != null && values.Any())
            {
                var index = 0;

                foreach (var order in await GetPurchaseOrder())
                {
                    if (values.Contains(order.Id))
                    {
                        indices.Add(index);
                    }

                    index += 1;
                }
            }

            return Json(indices);
        }

        private async Task<List<PurchaseOrder>> GetPurchaseOrder()
        {
            List<PurchaseOrder> _PurchaseOrder = new List<PurchaseOrder>();

            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrder");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrder = JsonConvert.DeserializeObject<List<PurchaseOrder>>(valorrespuesta);
                    _PurchaseOrder = _PurchaseOrder.ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _PurchaseOrder;
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetPurchaseOrderbyEstado([DataSourceRequest]DataSourceRequest request , int pEstadoId)
        {
            List<PurchaseOrder> _PurchaseOrders = new List<PurchaseOrder>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrder/GetPurchaseOrder");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrders = JsonConvert.DeserializeObject<List<PurchaseOrder>>(valorrespuesta);
                    _PurchaseOrders = _PurchaseOrders.Where(q =>q.EstadoId == pEstadoId).OrderByDescending(q => q.Id).ToList();
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

            return Json(_PurchaseOrders.ToDataSourceResult(request));
            //return _PurchaseOrders.ToDataSourceResult(request);
        }





    }
}