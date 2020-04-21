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


namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DepartamentoController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;

        public DepartamentoController(ILogger<DepartamentoController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }

        // GET: Departamento
        public ActionResult Departamento()
        {
            return View();
        }


        [HttpGet]
        public async Task<JsonResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<Departamento> _Departamento = new List<Departamento>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Departamento/GetDepartamento");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Departamento = JsonConvert.DeserializeObject<List<Departamento>>(valorrespuesta);
                    _Departamento = _Departamento.OrderByDescending(q => q.IdDepartamento).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Departamento.ToDataSourceResult(request));

        }
        //--------------------------------------------------------------------------------------
        [HttpGet("[action]")]
        public async Task<JsonResult> GetDepartamento([DataSourceRequest]DataSourceRequest request)
        {
            List<Departamento> _Departamento = new List<Departamento>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Departamento/GetDepartamento");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Departamento = JsonConvert.DeserializeObject<List<Departamento>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Departamento);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwAddDepartamento([FromBody]DepartamentoDTO _sarpara)
        {
            DepartamentoDTO _Departamento = new DepartamentoDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Departamento/GetDepartamentoById/" + _sarpara.IdDepartamento);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Departamento = JsonConvert.DeserializeObject<DepartamentoDTO>(valorrespuesta);

                }

                if (_Departamento == null)
                {
                    _Departamento = new DepartamentoDTO();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_Departamento);

        }


        [HttpPost]
        public async Task<ActionResult<Departamento>> SaveDepartamento([FromBody] DepartamentoDTO _TipoGastosS)
        {
            {
                string valorrespuesta = "";
                try
                {
                    Departamento _TipoGastos = new Departamento();
                    Departamento _TipoGastosP = new Departamento();
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/Departamento/GetDepartamentoByDescripcion/" + _TipoGastosS.NombreDepartamento);
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _TipoGastos = JsonConvert.DeserializeObject<Departamento>(valorrespuesta);
                        _TipoGastosP = JsonConvert.DeserializeObject<Departamento>(valorrespuesta);
                        if (_TipoGastos != null && _TipoGastosS.IdDepartamento == 0)
                        {
                            if (_TipoGastos.NombreDepartamento.ToLower() == _TipoGastosS.NombreDepartamento.ToLower())
                            {
                                {
                                    return await Task.Run(() => BadRequest());
                                }
                            }
                        }
                    }
                    result = await _client.GetAsync(baseadress + "api/Departamento/GetDepartamentoById/" + _TipoGastosS.IdDepartamento);

                    _TipoGastosS.FechaModificacion = DateTime.Now;
                    _TipoGastosS.Usuariomodificacion = HttpContext.Session.GetString("user");
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _TipoGastos = JsonConvert.DeserializeObject<Departamento>(valorrespuesta);
                    }
                    if (_TipoGastos == null) { _TipoGastos = new Models.Departamento(); }
                    if (_TipoGastos.IdDepartamento == 0)
                    {
                        _TipoGastosS.FechaCreacion = DateTime.Now;
                        _TipoGastosS.Usuariocreacion = HttpContext.Session.GetString("user");
                        var insertresult = await Insert(_TipoGastosS);
                    }
                    else
                    {
                        if (_TipoGastosP != null)
                        {
                            if (_TipoGastosP.NombreDepartamento.ToLower() == _TipoGastosS.NombreDepartamento.ToLower() && _TipoGastosP.IdDepartamento != _TipoGastosS.IdDepartamento)
                            {
                                return await Task.Run(() => BadRequest());
                            }
                        }
                        _TipoGastosS.FechaModificacion = DateTime.Now;
                        _TipoGastosS.Usuariomodificacion = HttpContext.Session.GetString("user");
                        var updateresult = await Update(_TipoGastosS.IdDepartamento, _TipoGastosS);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    throw ex;
                }
                return Json(_TipoGastosS);
            }
        }


        //--------------------------------------------------------------------------------------
        // POST: CAI/Insert
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(Departamento _DepartamentoS)
        {
            Departamento _Departamento = _DepartamentoS;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Departamento.Usuariocreacion = HttpContext.Session.GetString("user");
                _Departamento.Usuariomodificacion = HttpContext.Session.GetString("user");
                _Departamento.FechaCreacion = DateTime.Now;
                _Departamento.FechaModificacion = DateTime.Now;
                var result = await _client.PostAsJsonAsync(baseadress + "api/Departamento/Insert", _Departamento);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Departamento = JsonConvert.DeserializeObject<Departamento>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Departamento }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Int64 IdDepartamento, Departamento _Departamentop)
        {
            Departamento _Departamento = _Departamentop;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _Departamento.FechaModificacion = DateTime.Now;
                _Departamento.Usuariomodificacion = HttpContext.Session.GetString("user");
                var result = await _client.PutAsJsonAsync(baseadress + "api/Departamento/Update", _Departamento);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Departamento = JsonConvert.DeserializeObject<Departamento>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _Departamento }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<Departamento>> Delete([FromBody]Departamento _Branch)
        {

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Departamento/Delete", _Branch);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Branch = JsonConvert.DeserializeObject<Departamento>(valorrespuesta);
                    return new ObjectResult(new DataSourceResult { Data = new[] { _Branch }, Total = 1 });
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }

        }


    }
}