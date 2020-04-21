using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.Helpers;
using ERPMVC.Models;
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MarcasController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public MarcasController(ILogger<MarcasController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }

        public IActionResult MarcasList()
        {
            return View();
        }

        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwMarcas([FromBody]Marca _Marcas)

        {
           Marca _marcas = new Marca();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Marca/GetMarcaById/" + _Marcas.MarcaId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _marcas = JsonConvert.DeserializeObject<Marca>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)
                    var result2 = await _client.GetAsync(baseadress + "api/Estados/GetEstadosByGrupo/" + 1);
                    if (result2.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result2.Content.ReadAsStringAsync());
                        var estados = JsonConvert.DeserializeObject<List<Estados>>(valorrespuesta);
                        if (_marcas == null)
                        {
                            ViewData["estados"] = new SelectList(estados, "IdEstado", "NombreEstado");
                            _marcas = new Marca();
                        }
                        else
                        {
                            ViewData["estados"] = new SelectList(estados, "IdEstado", "NombreEstado", _marcas.IdEstado);
                            //ViewData["estadoMarcas"] = _Marca.IdEstado;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView(_marcas);

        }
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetMarcas([DataSourceRequest]DataSourceRequest request)
        {
            List<Marca> _Marca = new List<Marca>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Marca/GetMarcaByEstado/" + 1);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Marca = JsonConvert.DeserializeObject<List<Marca>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Marca.ToDataSourceResult(request);

        }


        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetMarcaJson([DataSourceRequest]DataSourceRequest request)
        {
            List<Marca> _Marca = new List<Marca>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Marca/GetMarca");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Marca = JsonConvert.DeserializeObject<List<Marca>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_Marca.ToDataSourceResult(request));

        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetMarca([DataSourceRequest]DataSourceRequest request)
        {
            List<Marca> _Marca = new List<Marca>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Marca/GetMarca");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Marca = JsonConvert.DeserializeObject<List<Marca>>(valorrespuesta);
                    _Marca = (from c in _Marca.OrderBy(q => q.MarcaId)
                              select new Marca
                              {
                                  MarcaId = c.MarcaId,
                                  Description = c.MarcaId + "--" + c.Description,
                                  IdEstado = c.IdEstado,
                                  Estado = c.Estado

                              }).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _Marca.ToDataSourceResult(request);

        }


        [HttpPost]
        public async Task<ActionResult<Marca>> SaveMarca([FromBody]Marca _marca)
        {

            try
            {
               Marca marca = new Marca();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Marca/GetMarcaById/" + _marca.MarcaId);
                string valorrespuesta = "";
                _marca.FechaModificacion = DateTime.Now;
                _marca.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    marca = JsonConvert.DeserializeObject<Marca>(valorrespuesta);
                }

                if (marca == null) { marca = new Models.Marca(); }

                if (marca.MarcaId == 0)
                {
                    _marca.FechaCreacion = DateTime.Now;
                    _marca.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_marca);
                }
                else
                {
                    var updateresult = await Update(_marca.MarcaId, _marca);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_marca);
        }

        // POST:Marcas/Insert
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Marca>> Insert(Marca _marca)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _marca.UsuarioCreacion = HttpContext.Session.GetString("user");
                _marca.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/Marca/Insert", _marca);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _marca = JsonConvert.DeserializeObject<Marca>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_marca);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Marca }, Total = 1 });
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<Marca>> Update(Int64 id,Marca _marca)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/Marca/Update", _marca);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _marca = JsonConvert.DeserializeObject<Marca>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_marca);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Marca }, Total = 1 });
        }

        [HttpPost]
        public async Task<ActionResult<Marca>> Delete([FromBody]Marca _Marca)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/Marca/Delete", _Marca);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _Marca = JsonConvert.DeserializeObject<Marca>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }


            return Ok(_Marca);
            //  return new ObjectResult(new DataSourceResult { Data = new[] { _Marca }, Total = 1 });
        }





    }
}