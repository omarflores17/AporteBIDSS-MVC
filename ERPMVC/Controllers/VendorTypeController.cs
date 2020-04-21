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
    public class VendorTypeController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public VendorTypeController(ILogger<VendorTypeController> logger, IOptions<MyConfig> config)
        {
            this._config = config;
            this._logger = logger;
        }

        public IActionResult VendorTypeList()
        {
            return View();
        }

        //Vista de Edición/Ingreso
        [HttpPost("[action]")]
        public async Task<ActionResult> pvwModalVendorType([FromBody]VendorType _vendorT)
        {
            VendorType _VendorType = new VendorType();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorType/GetVendorTypeById/" + _vendorT.VendorTypeId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                    //
                    //Obtener los estados. (Activo/Inactivo)
                   
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return PartialView(_VendorType);

        }
        
        
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> GetVendorTypeJson([DataSourceRequest]DataSourceRequest request)
        {
            List<VendorType> _VendorType = new List<VendorType>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorType/GetUnitOfMeasure");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<List<VendorType>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return Json(_VendorType.ToDataSourceResult(request));

        }

        
        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetVendorType([DataSourceRequest]DataSourceRequest request)
        {
            List<VendorType> _VendorType = new List<VendorType>();
            try
            {

                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorType/GetVendorType");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<List<VendorType>>(valorrespuesta);
                    _VendorType = _VendorType.OrderByDescending(q => q.VendorTypeId).ToList();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _VendorType.ToDataSourceResult(request);

        }


        [HttpPost]
        public async Task<ActionResult<VendorType>> SaveVendorType([FromBody]VendorType _VendorType)
        {
            string valorrespuesta = "";
            try
            {
                VendorType _listVendorType = new VendorType();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/VendorType/GetVendorTypeByName/" + _VendorType.VendorTypeName);
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                    if (_listVendorType!=null)
                    {
                        if (_listVendorType.VendorTypeId != _VendorType.VendorTypeId)
                        {
                            if (_listVendorType.VendorTypeName == _VendorType.VendorTypeName)
                            {
                                //result = await _client.GetAsync(baseadress + "api/VendorType/GetVendorTypeByDescription/" + _VendorType.Description);
                                //if (result.IsSuccessStatusCode)
                                //{
                                //    valorrespuesta = await (result.Content.ReadAsStringAsync());
                                //    _listVendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                                //    if (_listVendorType != null)
                                //    {
                                //        if (_listVendorType.Description == _VendorType.Description)
                                //        {
                                //            return await Task.Run(() => BadRequest($"Ya existe un Tipo Proveedor registrado con ese nombre."));
                                //        }
                                //    }

                                //}
                                return await Task.Run(() => BadRequest($"Ya existe un Tipo Proveedor registrado con ese nombre."));
                            }
                        }
                    }
                    
                }

                 result = await _client.GetAsync(baseadress + "api/VendorType/GetVendorTypeById/" + _VendorType.VendorTypeId);
               
                _VendorType.FechaModificacion = DateTime.Now;
                _VendorType.UsuarioModificacion = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _listVendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                }

                if (_listVendorType == null) {_listVendorType = new Models.VendorType();}

                if (_listVendorType.VendorTypeId == 0)
                {
                    _VendorType.FechaCreacion = DateTime.Now;
                    _VendorType.UsuarioCreacion = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_VendorType);
                }
                else
                {
                    var updateresult = await Update(_VendorType.VendorTypeId,_VendorType);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_VendorType);
        }

        // POST: VendorType/Insert
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<VendorType>> Insert(VendorType _VendorType)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _VendorType.UsuarioCreacion = HttpContext.Session.GetString("user");
                _VendorType.UsuarioModificacion = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/VendorType/Insert", _VendorType);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_VendorType);
        }

        [HttpPut("[controller]/[action]/{id}")]
        public async Task<ActionResult<VendorType>> Update(Int64 id, VendorType _VendorType)
        {
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/VendorType/Update", _VendorType);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok(_VendorType);
        }

        [HttpPost]
        public async Task<ActionResult<VendorType>> Delete([FromBody]VendorType _VendorType)
        {
            
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/VendorType/Delete", _VendorType);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _VendorType = JsonConvert.DeserializeObject<VendorType>(valorrespuesta);
                    if(_VendorType.VendorTypeId==0)
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
            return new ObjectResult(new DataSourceResult { Data = new[] { _VendorType }, Total = 1 });
        }





    }
}