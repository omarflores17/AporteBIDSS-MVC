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
    public class ContratoController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public ContratoController(ILogger<ContratoController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Sucursal"] = await ObtenerSucusal();
            ViewData["Cliente"] = await ObtenerCliente();
            ViewData["Fechapagar"] = await ObtenerFechapagar();
            return View();
        }
        //metodo para llamar Fecha pagar de plan de pago
        async Task<IEnumerable<Contrato_plan_pagos>> ObtenerFechapagar()
        {
            IEnumerable<Contrato_plan_pagos> Fechapagar = null;
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetContrato_plan_pagos");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    Fechapagar = JsonConvert.DeserializeObject<IEnumerable<Contrato_plan_pagos>>(valorrespuesta);
                    Fechapagar = Fechapagar.OrderByDescending(q => q.Fechacuota).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            ViewData["Fechapagar"] = Fechapagar.FirstOrDefault();
            return Fechapagar;
        }
        //metodo para llamar nombre y identidad del cliente
        async Task<IEnumerable<Customer>> ObtenerCliente()
        {
            IEnumerable<Customer> Customer = null;
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Customer/GetCustomer");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    Customer = JsonConvert.DeserializeObject<IEnumerable<Customer>>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            ViewData["Cliente"] = Customer.FirstOrDefault();
            return Customer;
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

        //paara  cargar la vista de consultas 
        public async Task<IActionResult> Consulta_General_Cuentas_Details()
        {
            return await Task.Run(() => View());
        }
        //metodo para el cargar vista parcial de customer
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddCustomerContrato([FromBody]Customer _sarpara)
        {
            Customer _Customer = new Customer();
            try
            {
                string baseadress = _config.Value.urlbase;
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
        //metodo para el cargar vista
        public async Task<ActionResult> Details(Int64 ContratoId)
        {
            ViewData["producto"] = await Obtenerproducto();
            ContratoDTO _Contrato = new ContratoDTO();
            if (ContratoId == 0)
            {
                _Contrato.Fecha = DateTime.Now;
                _Contrato.Fecha_inicio = DateTime.Now;
                _Contrato.Proxima_fecha_de_pago = DateTime.Now;
                _Contrato.Ultima_fecha_de_pago = DateTime.Now;
                return await Task.Run(() => View(_Contrato));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<ContratoDTO>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Contrato));
        }

        //metodo para ver sin modificar vista
        public async Task<ActionResult> VerDetails(Int64 ContratoId)
        {
            ViewData["producto"] = await Obtenerproducto();
            ContratoDTO _Contrato = new ContratoDTO();
            if (ContratoId == 0)
            {
                _Contrato.Fecha = DateTime.Now;
                _Contrato.Fecha_inicio = DateTime.Now;
                _Contrato.Proxima_fecha_de_pago = DateTime.Now;
                _Contrato.Ultima_fecha_de_pago = DateTime.Now;
                return await Task.Run(() => View(_Contrato));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<ContratoDTO>(valorrespuesta);
                    _Contrato.ver = 1;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Contrato));
        }

        public async Task<IActionResult> CuotasPagadas(Product product)
        {
            ViewData["Sucursal"] = await ObtenerSucusal();
            return PartialView(product);
        }
        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwContrato(Contrato contrato)
        {
            ContratoDTO _Contratos = new ContratoDTO();
            if (contrato.ContratoId == 0)
            {
                return await Task.Run(() => View(_Contratos));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + contrato.ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contratos = JsonConvert.DeserializeObject<ContratoDTO>(valorrespuesta);
                }
                if (_Contratos == null)
                {
                    _Contratos = new ContratoDTO { Fecha_inicio = DateTime.Now, Fecha = DateTime.Now, Fecha_de_vencimiento = DateTime.Now, Proxima_fecha_de_pago = DateTime.Now, Ultima_fecha_de_pago = DateTime.Now, editar = 1 };
                }
                else
                {
                    _Contratos.editar = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Contratos);

        }
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetContratoJson([DataSourceRequest]DataSourceRequest request)
        {
            List<Contrato> _Contrato = new List<Contrato>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetUnitOfMeasure");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<List<Contrato>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Contrato.ToDataSourceResult(request));

        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetContratoByCustomerId(Int64 CustomerId)
        {
            Contrato _Customer = new Contrato();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoByCustomerId/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Customer = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return await Task.Run(() => Json(_Customer));
        }


        //get
        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetContrato_Detalle([DataSourceRequest]DataSourceRequest request, Contrato _ContratoDetallep)
        {
            List<Contrato> _ContratoDetalle = new List<Contrato>();
            
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                if (HttpContext.Session.Get("listadoproductos") == null
                    || HttpContext.Session.GetString("listadoproductos") == "")
                {
                    if (_ContratoDetallep.ContratoId > 0)
                    {
                        string serialzado = JsonConvert.SerializeObject(_ContratoDetallep).ToString();
                        HttpContext.Session.SetString("listadoproductos", serialzado);
                    }
                }
                else
                {
                    _ContratoDetalle = JsonConvert.DeserializeObject<List<Contrato>>(HttpContext.Session.GetString("listadoproductos"));
                }
                if (_ContratoDetallep.ContratoId > 0)
                {
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoByCustomerId/" + _ContratoDetallep.CustomerId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                      Contrato  contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                         if (_ContratoDetallep.ContratoId > 0)
                        {
                            _ContratoDetalle.Add(_ContratoDetallep);
                            HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                        }
                        else
                        {
                            var obj = _ContratoDetalle.Where(x => x.ContratoId == _ContratoDetallep.ContratoId).FirstOrDefault();
                            if (obj != null)
                            {

                                obj.CustomerId = _ContratoDetallep.CustomerId;
                                obj.Plazo = _ContratoDetallep.Plazo;
                                obj.TotalContrato = _ContratoDetallep.TotalContrato;
                                obj.ValorPrima = _ContratoDetallep.ValorPrima;
                                obj.ValorCuota = _ContratoDetallep.ValorCuota;
                                obj.InteresesFinaciar = _ContratoDetallep.InteresesFinaciar;
                            }
                            HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                        }
                    }
                }
                else
                {
                    List<Contrato> _existelinea = new List<Contrato>();
                    if (HttpContext.Session.GetString("listadoproductos") != "" && HttpContext.Session.GetString("listadoproductos") != null)
                    {
                        _ContratoDetalle = JsonConvert.DeserializeObject<List<Contrato>>(HttpContext.Session.GetString("listadoproductos"));
                        _existelinea = _ContratoDetalle.Where(q => q.ContratoId == _ContratoDetallep.ContratoId).ToList();
                    }
                    
                    var id = _ContratoDetallep.ContratoId;
                    
                    if (_ContratoDetalle.Count > 0)
                    {
                        id--;
                        foreach (var item in _ContratoDetalle)
                        {
                            if (item.CustomerId == _ContratoDetallep.CustomerId)
                            {
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                                break;
                            }
                            else if (item.ContratoId == id && _existelinea.Count == 0)
                            {
                                _ContratoDetalle.Add(_ContratoDetallep);
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                                break;
                            }
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _ContratoDetalle.ToDataSourceResult(request);
        }


        //metodo para buscar contrato por id

        [HttpGet("[action]")]
        public async Task<ActionResult> GetContratoById(Int64 ContratoId)
        {
            Contrato _Contrato = new Contrato();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return await Task.Run(() => Json(_Contrato));
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetContrato([DataSourceRequest]DataSourceRequest request)
        {
            List<Contrato> _Contrado = new List<Contrato>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContrato");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrado = JsonConvert.DeserializeObject<List<Contrato>>(valorrespuesta);
                    _Contrado = _Contrado.OrderByDescending(q => q.ContratoId).ToList();
                }


            }
           catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Contrado.ToDataSourceResult(request);

        }

        public async Task<IActionResult> Reporte_Mora_TiendaDetails()
        {
            return await Task.Run(() => View());
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetContratoByBranch([DataSourceRequest]DataSourceRequest request,DateTime Fecha1, DateTime Fecha2,string Dias)
        {
            string baseadress = _config.Value.urlbase;
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            List<ContratoDTO> _Contrado = new List<ContratoDTO>();
            List<ContratoDTO> _Contradop = new List<ContratoDTO>();
            Contrato Contrato = new Contrato();           
            try
            {
                if (Dias == null)
                {
                    Contrato.IdEmpleado = 1;
                    Contrato.CustomerId = 1000000;
                }
                else if (int.Parse(Dias)==1)
                {
                    Contrato.IdEmpleado = 1;
                    Contrato.CustomerId = 30;
                }
                else if (int.Parse(Dias) == 2)
                {
                    Contrato.IdEmpleado = 31;
                    Contrato.CustomerId = 60;
                }
                else if (int.Parse(Dias) == 3)
                {
                    Contrato.IdEmpleado = 61;
                    Contrato.CustomerId = 90;
                }
                else if (int.Parse(Dias) == 4)
                {
                    Contrato.IdEmpleado = 91;
                    Contrato.CustomerId = 120;
                }
                else if (int.Parse(Dias) == 5)
                {
                    Contrato.IdEmpleado = 120;
                    Contrato.CustomerId = 365;
                }
                else if (int.Parse(Dias) == 6)
                {
                    Contrato.IdEmpleado = 365;
                    Contrato.CustomerId = 1000000;
                }
                else
                {
                    Contrato.IdEmpleado = 1;
                    Contrato.CustomerId = 1000000;
                }
                
                Contrato.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                Contrato.Proxima_fecha_de_pago = Fecha1;
                Contrato.Ultima_fecha_de_pago = Fecha2;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato/GetContratoByBranch", Contrato);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrado = JsonConvert.DeserializeObject<List<ContratoDTO>>(valorrespuesta);
                    _Contrado = _Contrado.OrderByDescending(q => q.ContratoId).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            foreach(var item in _Contrado)
            {
                var result = await _client.GetAsync(baseadress + "api/Contrato_movimientos/GetContrato_movimientosByContrato/" + item.ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contradop = JsonConvert.DeserializeObject<List<ContratoDTO>>(valorrespuesta);
                }
                item.Cuotas_pagadas = _Contradop.Count();
                item.Cuotas_pendiente = item.Plazo - item.Cuotas_pagadas;
                item.TotalPagado = item.TotalContrato - item.SaldoActual;
                item.TotalPagado = Math.Round(((item.TotalPagado * 100) / 100),2);
                item.meses = (DateTime.Now.Month-item.Fecha.Month)+12*(DateTime.Now.Year-item.Fecha.Year);
                item.Difmeses = item.meses - item.Cuotas_pagadas;
            }
            return Json(_Contrado.ToDataSourceResult(request));
        }


        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<Contrato>> SaveContrato([FromBody]ContratoDTO _ContratoP)

        {
            Contrato _Contrato = _ContratoP;
            
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + _Contrato.ContratoId);
                string valorrespuesta = "";
                _Contrato.FechaModificacion = DateTime.Now;
                _Contrato.UsuarioModificacion = HttpContext.Session.GetString("user");
                _Contrato.Fecha_de_vencimiento = DateTime.Now.AddMonths(_ContratoP.Plazo);
                _Contrato.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                }

                if (_Contrato == null) { _Contrato = new Models.Contrato(); }

                if (_ContratoP.ContratoId == 0)
                {
                    _ContratoP.FechaCreacion = DateTime.Now;
                    _Contrato.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_ContratoP);
                }
                else
                {
                    _ContratoP.FechaCreacion = _Contrato.FechaCreacion;
                    _ContratoP.UsuarioCreacion = _Contrato.UsuarioCreacion;
                    var updateresult = await Update(_Contrato.ContratoId, _ContratoP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Contrato);
        }

        [HttpPost("[action]")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<Contrato>> Insert(Contrato _ContratoP)
        {
            //Contrato _Contrato = _ContratoP;
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _ContratoP.UsuarioCreacion = HttpContext.Session.GetString("user");
                _ContratoP.UsuarioModificacion = HttpContext.Session.GetString("user");
                _ContratoP.SaldoActual = _ContratoP.TotalContrato;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato/Insert", _ContratoP);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ContratoP = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return new ObjectResult(new DataSourceResult { Data = new[] { _ContratoP }, Total = 1 });
            //return Ok(_Contrato);
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<Contrato>> Update(Int64 id, Contrato _Contrato)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Contrato/Update", _Contrato);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Contrato);
        }
        
        public async Task<ActionResult<Contrato>> Delete([FromBody]Contrato _Contrato)
        {

            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato/Delete", _Contrato);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                    if (_Contrato.ContratoId == 0)
                    {
                        return await Task.Run(() => BadRequest($"No se puede eliminar El Tipo de Proveedor porque ya esta siendo usada."));
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _Contrato }, Total = 1 });
        }

        //metodo para contratos pendientes y pagados
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetContratoPendientesAndPagados([DataSourceRequest]DataSourceRequest request, Int64 CustomerId)
        {
            List<Contrato> _Contrato = new List<Contrato>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoPendientesAndPagadosByCustomerId/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<List<Contrato>>(valorrespuesta);
                    _Contrato = _Contrato.OrderByDescending(q => q.ContratoId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Contrato.ToDataSourceResult(request); ;

        }

        //metodo para contratos pendientes
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetContratoPendiente(Int64 CustomerId)
        {
            List<Contrato> _Contrato = new List<Contrato>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoPendientesByCustomerId/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<List<Contrato>>(valorrespuesta);
                    _Contrato = _Contrato.OrderByDescending(q => q.ContratoId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => Json(_Contrato));

        }

        //metodo para contratos pendientes
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetContratoPagados(Int64 CustomerId)
        {
            List<Contrato> _Contrato = new List<Contrato>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoPagadosByCustomerId/" + CustomerId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<List<Contrato>>(valorrespuesta);
                    _Contrato = _Contrato.OrderByDescending(q => q.ContratoId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return await Task.Run(() => Json(_Contrato));

        }

        [HttpGet]
        public async Task<ActionResult> SFImpresionContrato(Int32 id)
        {
            try
            {
                Contrato _Kardexdto = new Contrato { ContratoId = id, };
                return await Task.Run(() => View(_Kardexdto));
            }
            catch (Exception)
            {
                return await Task.Run(() => BadRequest("Ocurrio un error"));
            }
        }
        [HttpGet]

        public async Task<ActionResult> SFContratoPorSucursal()
        {

            return await Task.Run(() => View());


        }
    }
}
