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
    public class InventoryTransferController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public InventoryTransferController(ILogger<InventoryTransferController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public ActionResult Index()
        {
            
            return View();
        }

        public IActionResult Menu()
        {
            HttpContext.Session.SetString("listadoproductos", "");
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Index2()
        {
            return View();
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<InventoryTransfer> _Contrado = new List<InventoryTransfer>();
            try
            {
                int branch = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/InventoryTransfer/GetInventoryTransferBySourceBranch/" + branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrado = JsonConvert.DeserializeObject<List<InventoryTransfer>>(valorrespuesta);
                    _Contrado = _Contrado.OrderByDescending(q => q.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Contrado.ToDataSourceResult(request);
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get2([DataSourceRequest]DataSourceRequest request)
        {
            List<InventoryTransfer> _Contrado = new List<InventoryTransfer>();
            try
            {
                int branch = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/InventoryTransfer/GetInventoryTransferByTargetBranch/" + branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrado = JsonConvert.DeserializeObject<List<InventoryTransfer>>(valorrespuesta);
                    _Contrado = _Contrado.OrderByDescending(q => q.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Contrado.ToDataSourceResult(request);
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

        public async Task<ActionResult> Details(Int64 ContratoId)
        {
            ViewData["branch"]= Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
            ViewData["producto"] = await Obtenerproducto();
            InventoryTransfer _Contrato = new InventoryTransfer();
            if (ContratoId == 0)
            {
                _Contrato.DateGenerated = DateTime.Now;
                _Contrato.DepartureDate = DateTime.Now;
                _Contrato.DateReceived = DateTime.Now;
                return await Task.Run(() => View(_Contrato));
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/InventoryTransfer/GetInventoryTransferById/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<InventoryTransfer>(valorrespuesta);
                    ViewBag.Estado = _Contrato.EstadoId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Contrato));
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<InventoryTransfer>> SaveInventoryTransfer([FromBody]InventoryTransferDTO _ContratoP)
        {
            string valorrespuesta = "";
            InventoryTransfer _Contrato = _ContratoP;
            foreach (var item in _Contrato.InventoryTransferLines)
            {
                if (item.CantidadRecibida == 0)
                {
                    item.CantidadRecibida = item.QtyStock;
                }
            }
            if (_ContratoP.EstadoId == 10)
            {
                foreach (var item in _Contrato.InventoryTransferLines)
                {
                    KardexViale _KardexVialeP = new KardexViale();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexByProductId/" + item.ProductId + "/" + Convert.ToInt32(HttpContext.Session.GetString("BranchId")));
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _KardexVialeP = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta);
                        _KardexVialeP.QuantityEntry = item.CantidadRecibida;
                        _KardexVialeP.QuantityOut = 0;
                        _KardexVialeP.SaldoAnterior = _KardexVialeP.Total;
                        _KardexVialeP.Total = _KardexVialeP.Total + item.CantidadRecibida;
                        _KardexVialeP.Id = 0;
                        _KardexVialeP.KardexDate = DateTime.Now;
                        _KardexVialeP.TypeOperationId = 1;
                        _KardexVialeP.TypeOperationName = "Entrada";
                        _KardexVialeP.UsuarioCreacion = HttpContext.Session.GetString("user");
                        _KardexVialeP.TypeOfDocumentId = 2;
                        _KardexVialeP.TypeOfDocumentName = "Transferencia de Inventario";
                        _KardexVialeP.DocumentId = _ContratoP.Id;
                        result = await _client.PostAsJsonAsync(baseadress + "api/KardexViale/Insert", _KardexVialeP);
                    }
                }
            }
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/InventoryTransfer/GetInventoryTransferById/" + _Contrato.Id);
                
                _Contrato.FechaModificacion = DateTime.Now;
                _Contrato.UsuarioModificacion = HttpContext.Session.GetString("user");
                _Contrato.FechaCreacion = DateTime.Now;
                _Contrato.UsuarioCreacion = HttpContext.Session.GetString("user");
                //_Contrato.SourceBranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<InventoryTransfer>(valorrespuesta);
                }

                if (_Contrato == null) { _Contrato = new Models.InventoryTransfer(); }

                if (_ContratoP.Id == 0)
                {
                    _ContratoP.FechaCreacion = DateTime.Now;
                    _ContratoP.UsuarioCreacion = HttpContext.Session.GetString("user");
                    _ContratoP.SourceBranchId= Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                    var insertresult = await Insert(_ContratoP);
                }
                else
                {
                    _ContratoP.FechaModificacion = DateTime.Now;
                    _ContratoP.UsuarioModificacion = HttpContext.Session.GetString("user");
                    var updateresult = await Update(_Contrato.Id, _ContratoP);
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
        public async Task<ActionResult<InventoryTransfer>> Insert(InventoryTransfer _ContratoP)
        {
            //Contrato _Contrato = _ContratoP;
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _ContratoP.UsuarioCreacion = HttpContext.Session.GetString("user");
                _ContratoP.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/InventoryTransfer/Insert", _ContratoP);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ContratoP = JsonConvert.DeserializeObject<InventoryTransfer>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            if (_ContratoP.EstadoId == 8)
            {
                foreach (var item in _ContratoP.InventoryTransferLines)
                {
                    KardexViale _KardexVialeP = new KardexViale();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexByProductId/" + item.ProductId + "/" + Convert.ToInt32(HttpContext.Session.GetString("BranchId")));
                    if (result.IsSuccessStatusCode)
                    {
                        string valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _KardexVialeP = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta);
                        _KardexVialeP.QuantityOut = item.QtyStock;
                        _KardexVialeP.QuantityEntry = 0;
                        _KardexVialeP.SaldoAnterior = _KardexVialeP.Total;
                        _KardexVialeP.Total = _KardexVialeP.Total - item.QtyStock;
                        _KardexVialeP.Id = 0;
                        _KardexVialeP.KardexDate = DateTime.Now;
                        _KardexVialeP.TypeOperationId = 2;
                        _KardexVialeP.TypeOperationName = "Salida";
                        _KardexVialeP.UsuarioCreacion = HttpContext.Session.GetString("user");
                        _KardexVialeP.TypeOfDocumentId = 2;
                        _KardexVialeP.TypeOfDocumentName = "Transferencia de Inventario";
                        _KardexVialeP.DocumentId = _ContratoP.Id;
                        result = await _client.PostAsJsonAsync(baseadress + "api/KardexViale/Insert", _KardexVialeP);
                    }
                }
            }
            HttpContext.Session.SetString("listadoproductos", "");
            return new ObjectResult(new DataSourceResult { Data = new[] { _ContratoP }, Total = 1 });
            //return Ok(_Contrato);
        }

        public async Task<ActionResult<InventoryTransfer>> Update(Int64 id, InventoryTransfer _Contrato)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/InventoryTransfer/Update", _Contrato);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<InventoryTransfer>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            if (_Contrato.EstadoId == 8)
            {
                foreach (var item in _Contrato.InventoryTransferLines)
                {
                    KardexViale _KardexVialeP = new KardexViale();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexByProductId/" + item.ProductId + "/" + Convert.ToInt32(HttpContext.Session.GetString("BranchId")));
                    if (result.IsSuccessStatusCode)
                    {
                        string valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _KardexVialeP = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta);
                        _KardexVialeP.QuantityOut = item.QtyStock;
                        _KardexVialeP.QuantityEntry = 0;
                        _KardexVialeP.SaldoAnterior = _KardexVialeP.Total;
                        _KardexVialeP.Total = _KardexVialeP.Total - item.QtyStock;
                        _KardexVialeP.Id = 0;
                        _KardexVialeP.KardexDate = DateTime.Now;
                        _KardexVialeP.TypeOperationId = 2;
                        _KardexVialeP.TypeOperationName = "Salida";
                        _KardexVialeP.UsuarioCreacion = HttpContext.Session.GetString("user");
                        _KardexVialeP.TypeOfDocumentId = 2;
                        _KardexVialeP.TypeOfDocumentName = "Transferencia de Inventario";
                        _KardexVialeP.DocumentId = _Contrato.Id;
                        result = await _client.PostAsJsonAsync(baseadress + "api/KardexViale/Insert", _KardexVialeP);
                    }
                }
            }
            HttpContext.Session.SetString("listadoproductos", "");
            return Ok(_Contrato);
        }
    }
}