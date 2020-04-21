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
    public class PagoPrimaController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;

        public PagoPrimaController(ILogger<PagoPrimaController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Sucursal"] = await ObtenerSucusal();
            ViewData["Empleado"] = await ObtenerEmpleado();
            return View();
        }

        async Task<IEnumerable<Employees>> ObtenerEmpleado()
        {
            IEnumerable<Employees> Empleado = null;
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployees");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    Empleado = JsonConvert.DeserializeObject<IEnumerable<Employees>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            ViewData["Empleado"] = Empleado.FirstOrDefault();
            return Empleado;

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

        public async Task<ActionResult> Details(Int64 ContratoId)
        {
            Contrato_movimientos _Contrato_movimientos = new Contrato_movimientos();
            if (ContratoId == 0)
            {
                return await Task.Run(() => View(_Contrato_movimientos));
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
                    _Contrato_movimientos = JsonConvert.DeserializeObject<Contrato_movimientos>(valorrespuesta);
                    _Contrato_movimientos.BranchId =  Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                    _Contrato_movimientos.Fechamovimiento = DateTime.Now;
                    

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => View(_Contrato_movimientos));
        }

        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwContrato_movimientos(Contrato_movimientos Contrato_movimientos)
        {
            Contrato_movimientosDTO _Contrato_movimientoss = new Contrato_movimientosDTO();
            if (Contrato_movimientos.Contrato_movimientosId == 0)
            {
                return await Task.Run(() => View(_Contrato_movimientoss));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_movimientos/GetContrato_movimientosById/" + Contrato_movimientos.Contrato_movimientosId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_movimientoss = JsonConvert.DeserializeObject<Contrato_movimientosDTO>(valorrespuesta);
                }
                if (_Contrato_movimientoss == null)
                {
                    // _Contrato_movimientoss = new Contrato_movimientosDTO { Fecha_inicio = DateTime.Now, Fecha = DateTime.Now, Fecha_de_vencimiento = DateTime.Now, Proxima_fecha_de_pago = DateTime.Now, Ultima_fecha_de_pago = DateTime.Now, editar = 1 };
                }
                else
                {
                    _Contrato_movimientoss.editar = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Contrato_movimientoss);

        }
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetContrato_movimientosJson([DataSourceRequest]DataSourceRequest request)
        {
            List<Contrato_movimientos> _Contrato_movimientos = new List<Contrato_movimientos>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_movimientos/GetUnitOfMeasure");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_movimientos = JsonConvert.DeserializeObject<List<Contrato_movimientos>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Contrato_movimientos.ToDataSourceResult(request));

        }


        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetContrato_movimientos([DataSourceRequest]DataSourceRequest request)
        {
            List<Contrato_movimientos> _Contrado = new List<Contrato_movimientos>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_movimientos/GetContrato_movimientos");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrado = JsonConvert.DeserializeObject<List<Contrato_movimientos>>(valorrespuesta);
                    _Contrado = _Contrado.OrderByDescending(q => q.Contrato_movimientosId).ToList();
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
        public async Task<ActionResult> GetContrato_movimientosByIdTipo(Int64 ContratoId)
        {
            List<Contrato_movimientos> _Contrato = new List<Contrato_movimientos>();
           
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_movimientos/GetContrato_movimientosByContratoIdTipo/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    
                    _Contrato = JsonConvert.DeserializeObject<List<Contrato_movimientos>>(valorrespuesta);
                    

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return await Task.Run(() => Json(_Contrato));
        }


        [HttpPost]
        public async Task<ActionResult<Contrato_movimientos>> SaveContrato_movimientos([FromBody]Contrato_movimientos _Contrato_movimientos)
        {

            try
            {
                List<Contrato_movimientos> _Contrato = new List<Contrato_movimientos>();
                string valorrespuesta = "";
                Contrato_movimientos _listContrato_movimientos = new Contrato_movimientos();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.GetAsync(baseadress + "api/Contrato_movimientos/GetContrato_movimientosByContratoIdTipo/" + _Contrato_movimientos.ContratoId);
                
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());

                    _Contrato = JsonConvert.DeserializeObject<List<Contrato_movimientos>>(valorrespuesta);


                }

                if (_Contrato.Count > 0)
                {
                    return await Task.Run(() => BadRequest($"La prima de este contrato ya está pagada."));
                }


                 result = await _client.GetAsync(baseadress + "api/Contrado/GetContrato_movimientosById/" + _Contrato_movimientos.Contrato_movimientosId);

                        _Contrato_movimientos.ModifiedDate = DateTime.Now;
                        _Contrato_movimientos.Fechamovimiento = DateTime.Now;
                        _Contrato_movimientos.UsuarioModificacion = HttpContext.Session.GetString("user");
                        _Contrato_movimientos.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                        if (result.IsSuccessStatusCode)
                        {

                            valorrespuesta = await (result.Content.ReadAsStringAsync());
                            _listContrato_movimientos = JsonConvert.DeserializeObject<Contrato_movimientos>(valorrespuesta);
                        }

                        if (_listContrato_movimientos == null) { _listContrato_movimientos = new Models.Contrato_movimientos(); }

                        if (_listContrato_movimientos.Contrato_movimientosId == 0)
                        {
                            _Contrato_movimientos.CreatedDate = DateTime.Now;
                            _Contrato_movimientos.UsuarioCreacion = HttpContext.Session.GetString("user");
                            var insertresult = await Insert(_Contrato_movimientos);
                        }
                        else
                        {
                            var updateresult = await Update(_Contrato_movimientos.Contrato_movimientosId, _Contrato_movimientos);
                        }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_Contrato_movimientos);
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Contrato_movimientos>> Insert(Contrato_movimientos _Contrato_movimientos)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Contrato_movimientos.UsuarioCreacion = HttpContext.Session.GetString("user");
                _Contrato_movimientos.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato_movimientos/Insertar_Prima", _Contrato_movimientos);
                var result2 = await _client.GetAsync(baseadress + "api/Contrato_detalle/GetContrato_detalleByContratoId/" + _Contrato_movimientos.ContratoId);
                string valorrespuesta = "";
                string valorrespuesta2 = "";
                string valorrespuesta3 = "";
                if (result.IsSuccessStatusCode && result2.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_movimientos = JsonConvert.DeserializeObject<Contrato_movimientos>(valorrespuesta);

                    valorrespuesta2 = await (result2.Content.ReadAsStringAsync());
                    List<Contrato_Detalle> contrato_Detalle = new List<Contrato_Detalle>();
                    contrato_Detalle = JsonConvert.DeserializeObject<List<Contrato_Detalle>>(valorrespuesta2);

                    foreach (var item in contrato_Detalle) {
                        KardexViale kardexViale = new KardexViale();

                        var result3 = await _client.GetAsync(baseadress + "api/KardexViale/GetKardexByProductId/" + item.ProductId + "/" + Convert.ToInt32(HttpContext.Session.GetString("BranchId")));
                        if (result3.IsSuccessStatusCode) {
                        valorrespuesta3 = await (result3.Content.ReadAsStringAsync());
                            kardexViale = JsonConvert.DeserializeObject<KardexViale>(valorrespuesta3);
                            kardexViale.QuantityOut = item.Cantidad;
                            kardexViale.QuantityEntry = 0;
                            kardexViale.SaldoAnterior = kardexViale.Total;
                            kardexViale.Total = kardexViale.Total - item.Cantidad;
                            kardexViale.Id = 0;
                            kardexViale.KardexDate = DateTime.Now;
                            kardexViale.TypeOperationId = 2;
                            kardexViale.TypeOperationName = "Salida";
                            kardexViale.UsuarioCreacion = HttpContext.Session.GetString("user");
                            kardexViale.BranchId = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                            kardexViale.TypeOfDocumentId = 3;
                            kardexViale.TypeOfDocumentName = "Factura al Credito";
                            kardexViale.DocumentId = Convert.ToInt32(_Contrato_movimientos.ContratoId);

                            result3 = await _client.PostAsJsonAsync(baseadress + "api/KardexViale/Insert", kardexViale );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Contrato_movimientos);
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<Contrato_movimientos>> Update(Int64 id, Contrato_movimientos _Contrato_movimientos)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Contrato_movimientos/Update", _Contrato_movimientos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_movimientos = JsonConvert.DeserializeObject<Contrato_movimientos>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_Contrato_movimientos);
        }

        public async Task<ActionResult<Contrato_movimientos>> Delete([FromBody]Contrato_movimientos _Contrato_movimientos)
        {

            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato_movimientos/Delete", _Contrato_movimientos);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato_movimientos = JsonConvert.DeserializeObject<Contrato_movimientos>(valorrespuesta);
                    if (_Contrato_movimientos.Contrato_movimientosId == 0)
                    {
                        return await Task.Run(() => BadRequest($"No se puede eliminar porque ya esta siendo usada."));
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _Contrato_movimientos }, Total = 1 });
        }

    }
}