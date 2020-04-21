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
    public class Contrato_movimientosController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public Contrato_movimientosController(ILogger<Contrato_movimientosController> logger, IOptions<MyConfig> config)
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
            List<Contrato_plan_pagos> _plan_pagos = new List<Contrato_plan_pagos>();
            Contrato _Contrato = new Contrato();
            Contrato_movimientos _Contrato_movimientos = new Contrato_movimientos();
            var DiasMora = 0;
            double InteresMora = 0;
            double Cuota = 0;
            double PagoMora = 0;
            if (ContratoId == 0)
            {
                return await Task.Run(() => View(_Contrato_movimientos));
            }
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetFechaPagoContrato_plan_pagosByContratoId/" + ContratoId);

                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _plan_pagos = JsonConvert.DeserializeObject<List<Contrato_plan_pagos>>(valorrespuesta);
                    _plan_pagos = _plan_pagos.OrderBy(q => q.Fechacuota).ToList();
                    var nuevafecha = _plan_pagos.FirstOrDefault();
                    var fecha = DateTime.Now;
                    DiasMora = _Contrato.Dias_mora;
                    InteresMora = _Contrato.InteresesMora;

                    if (nuevafecha.Fechacuota >= fecha)
                    {
                        DiasMora = 0;
                    }
                    else
                    {
                        TimeSpan dias = fecha - nuevafecha.Fechacuota.Date;
                        DiasMora = dias.Days;
                    }
                    _Contrato.Dias_mora = DiasMora;
                    ViewData["mora"] = DiasMora;
                }
                result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + ContratoId);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                    _Contrato_movimientos = JsonConvert.DeserializeObject<Contrato_movimientos>(valorrespuesta);
                    Cuota = _Contrato.ValorCuota;
                }
                if (DiasMora >= 0 && DiasMora <= 30)
                {
                    InteresMora = 0;
                    PagoMora = 0;
                }
                else if (DiasMora >= 31 && DiasMora <= 60)
                {
                    InteresMora = Cuota * 0.04;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 61 && DiasMora <= 90)
                {
                    InteresMora = Cuota * 0.06;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 91 && DiasMora <= 120)
                {
                    InteresMora = Cuota * 0.08;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 121 && DiasMora <= 150)
                {
                    InteresMora = Cuota * 0.10;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 151 && DiasMora <= 180)
                {
                    InteresMora = Cuota * 0.12;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 181 && DiasMora <= 210)
                {
                    InteresMora = Cuota * 0.15;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 211 && DiasMora <= 240)
                {
                    InteresMora = Cuota * 0.20;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 241 && DiasMora <= 270)
                {
                    InteresMora = Cuota * 0.25;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 271 && DiasMora <= 300)
                {
                    InteresMora = Cuota * 0.30;
                    PagoMora = InteresMora + Cuota;
                }
                else
                {
                    InteresMora = Cuota * 0.35;
                    PagoMora = InteresMora + Cuota;
                }

                _Contrato.InteresesMora = InteresMora;
                _Contrato.PagoPorMora = PagoMora;
                ViewData["intereses"] = InteresMora;
                ViewData["pago"] = PagoMora;
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


        [HttpPost]
        public async Task<ActionResult<Contrato_movimientos>> SaveContrato_movimientos([FromBody]Contrato_movimientos _Contrato_movimientos)
        {
            List<Contrato_plan_pagos> _plan_pagos = new List<Contrato_plan_pagos>();
            Contrato _Contrato = new Contrato();
            var DiasMora = 0;
            double InteresMora = 0;
            double Cuota = 0;
            double PagoMora = 0;
            try
            {
                Contrato_movimientos _listContrato_movimientos = new Contrato_movimientos();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_plan_pagos/GetFechaPagoContrato_plan_pagosByContratoId/" + _Contrato_movimientos.ContratoId);

                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _plan_pagos = JsonConvert.DeserializeObject<List<Contrato_plan_pagos>>(valorrespuesta);
                    _plan_pagos = _plan_pagos.OrderBy(q => q.Fechacuota).ToList();
                    var nuevafecha = _plan_pagos.FirstOrDefault();
                    var fecha = DateTime.Now;
                    DiasMora = _Contrato.Dias_mora;
                    InteresMora = _Contrato.InteresesMora;

                    if (nuevafecha.Fechacuota >= fecha)
                    {
                        DiasMora = 0;
                    }
                    else
                    {
                        TimeSpan dias = fecha - nuevafecha.Fechacuota.Date;
                        DiasMora = dias.Days;
                    }
                    _Contrato.Dias_mora = DiasMora;
                    _Contrato_movimientos.Dias_mora = DiasMora;
                }
                 result = await _client.GetAsync(baseadress + "api/Contrato/GetContratoById/" + _Contrato_movimientos.ContratoId);
                if (result.IsSuccessStatusCode)
                {
                     valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Contrato = JsonConvert.DeserializeObject<Contrato>(valorrespuesta);
                    Cuota = _Contrato.ValorCuota;
                }
                if (DiasMora >= 0 && DiasMora <= 30)
                {
                    InteresMora = 0;
                    PagoMora = 0;
                }
                else if (DiasMora >= 31 && DiasMora <= 60)
                {
                    InteresMora = Cuota * 0.04;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 61 && DiasMora <= 90)
                {
                    InteresMora = Cuota * 0.06;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 91 && DiasMora <= 120)
                {
                    InteresMora = Cuota * 0.08;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 121 && DiasMora <= 150)
                {
                    InteresMora = Cuota * 0.10;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 151 && DiasMora <= 180)
                {
                    InteresMora = Cuota * 0.12;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 181 && DiasMora <= 210)
                {
                    InteresMora = Cuota * 0.15;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 211 && DiasMora <= 240)
                {
                    InteresMora = Cuota * 0.20;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 241 && DiasMora <= 270)
                {
                    InteresMora = Cuota * 0.25;
                    PagoMora = InteresMora + Cuota;
                }
                else if (DiasMora >= 271 && DiasMora <= 300)
                {
                    InteresMora = Cuota * 0.30;
                    PagoMora = InteresMora + Cuota;
                }
                else
                {
                    InteresMora = Cuota * 0.35;
                    PagoMora = InteresMora + Cuota;
                }
                
                 _Contrato_movimientos.PagoPorMora = PagoMora;
                 _Contrato_movimientos.InteresesMora = InteresMora;

                 result = await _client.GetAsync(baseadress + "api/Contrado/GetContrato_movimientosById/" + _Contrato_movimientos.Contrato_movimientosId);
                 valorrespuesta = "";
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
                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato_movimientos/Insert", _Contrato_movimientos);
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


        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetContratoMovimientoByContratoId([DataSourceRequest]DataSourceRequest request, int ContratoId)
        {
            List<Contrato_movimientos> _Kardex = new List<Contrato_movimientos>();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_Movimientos/GetContrato_movimientosByContratoIdPago/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Kardex = JsonConvert.DeserializeObject<List<Contrato_movimientos>>(valorrespuesta);
                    _Kardex = _Kardex.OrderByDescending(q => q.Contrato_movimientosId).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Kardex.ToDataSourceResult(request);
        }
    }
}
