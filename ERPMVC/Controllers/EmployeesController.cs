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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    public class EmployeesController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public EmployeesController(ILogger<EmployeesController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Listado Vacaciones
        public ActionResult Vacaciones()
        {
            return PartialView();
        }

        //Listado de Incapacidades.
        public ActionResult Incapacidades()
        {
            return PartialView();
        }

        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> Empleado([FromBody]Employees _empleado)
        {
            Employees _Employees = new Employees();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesById/" + _empleado.IdEmpleado);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<Employees>(valorrespuesta);   
                }

                if (_Employees == null)
                {
                    _Employees = new Employees();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_Employees);

        }


        //[HttpPost("[controller]/[action]")]
        //public async Task<ActionResult<Employees>> SaveEmployees([FromBody]EmployeesDTO _EmployeesP)
        ////public async Task<ActionResult<Employees>> SaveEmployees([FromBody]EmployeesDTO _EmployeesP)
        //{

        //    Employees _Employees = _EmployeesP;
        //    try
        //    {
        //        // DTO_NumeracionSAR _liNumeracionSAR = new DTO_NumeracionSAR();
        //        string baseadress = config.Value.urlbase;
        //        HttpClient _client = new HttpClient();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
        //        var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesById/" + _Employees.IdEmpleado);
        //        string valorrespuesta = "";
        //        Guid newGuid = Guid.Parse("FC405B7D-9FE3-43C9-97B5-D87A174CAB8A");
        //        _Employees.ApplicationUserId = newGuid;
        //        _Employees.FechaIngreso = DateTime.Now;
        //        //_Employees.FechaModificacion = DateTime.Now;
        //        //_Employees.Usuariomodificacion = HttpContext.Session.GetString("user");
        //        if (result.IsSuccessStatusCode)
        //        {
        //            valorrespuesta = await (result.Content.ReadAsStringAsync());
        //            _Employees = JsonConvert.DeserializeObject<EmployeesDTO>(valorrespuesta);
        //        }

        //        if (_Employees == null) { _Employees = new Models.Employees(); }

        //        if (_EmployeesP.IdEmpleado == 0)
        //        {
        //            _Employees.FechaCreacion = DateTime.Now;
        //            _Employees.Usuariocreacion = HttpContext.Session.GetString("user");
        //            var insertresult = await Insert(_EmployeesP);
        //        }
        //        else
        //        {
        //            _EmployeesP.Usuariocreacion = _Employees.Usuariocreacion;
        //            _EmployeesP.FechaCreacion = _Employees.FechaCreacion;
        //            var updateresult = await Update(_Employees.IdEmpleado, _EmployeesP);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
        //        throw ex;
        //    }

        //    return Json(_Employees);
        //}

        [HttpPost]
        public async Task<ActionResult<Employees>> SaveEmployees([FromBody]Employees _VendorType)
        {
            string valorrespuesta = "";
            try
            {
                Employees _listVendorType = new Employees();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesByIdentidad/" + _VendorType.Identidad);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<Employees>(valorrespuesta);
                    if (_listVendorType != null && _VendorType.IdEmpleado == 0)
                    {
                        if (_listVendorType.Identidad == _VendorType.Identidad)
                        {
                            return await Task.Run(() => BadRequest($"Ya existe un Empleado registrado con esa Identidad."));
                        }
                    }

                }
                result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesById/" + _VendorType.IdEmpleado);

                _VendorType.FechaModificacion = DateTime.Now;
                _VendorType.Usuariomodificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {
                    
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<Employees>(valorrespuesta);
                }
                
                if (_listVendorType == null) { _listVendorType = new Models.Employees(); }
                
                if (_listVendorType.IdEmpleado == 0)
                {
                    _VendorType.FechaCreacion = DateTime.Now;
                    _VendorType.Usuariocreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_VendorType);
                }
                else
                {
                    var updateresult = await Update(_VendorType.IdEmpleado, _VendorType);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_VendorType);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Employees _EmployeesP)
        {
            Employees _Employees = _EmployeesP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Employees.Usuariocreacion = HttpContext.Session.GetString("user");
                //_Employees.Usuariomodificacion = HttpContext.Session.GetString("user");
                _Employees.FechaCreacion = DateTime.Now;
                //_Employees.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Employees/Insert", _Employees);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<Employees>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Employees }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Int64 Id, Employees _EmployeesP)
        {
            Employees _Employees = _EmployeesP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Employees.FechaModificacion = DateTime.Now;
                _Employees.Usuariomodificacion = HttpContext.Session.GetString("user");
                var result = await _client.PutAsJsonAsync(baseadress + "api/Employees/Update", _Employees);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<Employees>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Employees }, Total = 1 });
        }



        [HttpPost]
        public async Task<ActionResult<Employees>> Delete([FromBody]Employees _Employees)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Employees/Delete", _Employees);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<Employees>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            //return Ok(_VendorType);
            return new ObjectResult(new DataSourceResult { Data = new[] { _Employees }, Total = 1 });
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddEmployees([FromBody]EmployeesDTO _sarpara)

        {
            EmployeesDTO _Employees = new EmployeesDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesById/" + _sarpara.IdEmpleado);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<EmployeesDTO>(valorrespuesta);

                }

                if (_Employees == null)
                {
                    _Employees = new EmployeesDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Employees);

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Employees> _Employees = new List<Employees>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployees");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<List<Employees>>(valorrespuesta);
                    _Employees = _Employees.OrderByDescending(q => q.IdEmpleado).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Employees.ToDataSourceResult(request);

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetEmployeesByBranch([DataSourceRequest]DataSourceRequest request)
        {
            List<Employees> Employees = new List<Employees>();
            try
            {
                int branch = Convert.ToInt32(HttpContext.Session.GetString("BranchId"));
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesByBranch/" + branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    Employees = JsonConvert.DeserializeObject<List<Employees>>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return Employees.ToDataSourceResult(request);
        }

        //metodo para cargar los empleados con puesto vendedores
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetEmployeesByPuesto([DataSourceRequest]DataSourceRequest request)
        {
            List<Employees> _Employees = new List<Employees>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Employees/GetEmployeesByPuesto");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Employees = JsonConvert.DeserializeObject<List<Employees>>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _Employees.ToDataSourceResult(request);

        }
    }
}
