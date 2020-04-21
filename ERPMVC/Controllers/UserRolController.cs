﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class UserRolController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        private readonly ClaimsPrincipal _principal;
        public UserRolController(ILogger<UserRolController> logger, IOptions<MyConfig> config, IHttpContextAccessor httpContextAccessor)
        {
            this.config = config;
            this._logger = logger;
            _principal = httpContextAccessor.HttpContext.User;

        }

        [Authorize(Policy = "Seguridad.Roles por Usuario")]
        public IActionResult UserRol()
        {
            return View();
        }

        [Authorize(Policy = "Seguridad.Roles por Usuario")]
        public IActionResult PorRol()
        {
            ViewData["permisos"] = _principal;
            return View();
        }

        [Authorize(Policy = "Seguridad.Roles por Usuario")]
        public IActionResult PorUsuario()
        {
            ViewData["permisos"] = _principal;
            return View();
        }

        public IActionResult pvwAddUserRol()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return View();
        }



        [HttpGet]
        public async Task<DataSourceResult> GetUserRol([DataSourceRequest]DataSourceRequest request)
        {
            List<ApplicationUserRole> _UserRrol = new List<ApplicationUserRole>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _UserRrol = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _UserRrol.ToDataSourceResult(request);

        }


        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetRolesByUserId([DataSourceRequest]DataSourceRequest request, Guid UserId)
        {
            List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);
                    _roles = _roles.Where(q => q.UserId == UserId).ToList();

                    foreach (var item in _roles)
                    {

                        var resultclient2 = await _client.GetAsync(baseadress + "api/Roles/GetRoleById/" + item.RoleId).Result.Content.ReadAsStringAsync();
                        item.RoleName = (JsonConvert.DeserializeObject<ApplicationRole>(resultclient2)).Name;
                    }
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw (new Exception(ex.Message));
            }
            //  return Json(_roles);
            return _roles.ToDataSourceResult(request);
        }

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetUsersByRoleId([DataSourceRequest]DataSourceRequest request, Guid RoleId)
        {
            List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);
                    _roles = _roles.Where(q => q.RoleId == RoleId).ToList();

                    foreach (var item in _roles)
                    {
                        var resultclient = await _client.GetAsync(baseadress + "api/Usuario/GetUserById/" + item.UserId).Result.Content.ReadAsStringAsync();
                        item.UserName = (JsonConvert.DeserializeObject<ApplicationUser>(resultclient)).UserName;


                    }

                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw (new Exception(ex.Message));
            }

            return _roles.ToDataSourceResult(request);
            //  return Json(_roles);
        }

        [HttpGet("[controller]/[action]")]
        public async Task<JsonResult> GetJsonRoles([DataSourceRequest]DataSourceRequest request)
        {
            List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);
                    // _roles = _roles.Where(q => q.UserId == UserId).ToList();

                    foreach (var item in _roles)
                    {

                        var resultclient2 = await _client.GetAsync(baseadress + "api/Roles/GetRoleById/" + item.RoleId).Result.Content.ReadAsStringAsync();
                        item.RoleName = (JsonConvert.DeserializeObject<ApplicationRole>(resultclient2)).Name;
                    }
                }
                // }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw (new Exception(ex.Message));
            }
            return Json(_roles);
            //    return _roles.ToDataSourceResult(request);
        }
        [HttpGet("[controller]/[action]")]
        public async Task<JsonResult> GetJsonUsersApi([DataSourceRequest]DataSourceRequest request)
        {
            List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);

                    foreach (var item in _roles)
                    {
                        var resultclient = await _client.GetAsync(baseadress + "api/Usuario/GetUserById/" + item.UserId).Result.Content.ReadAsStringAsync();
                        item.UserName = (JsonConvert.DeserializeObject<ApplicationUser>(resultclient)).UserName;
                    }



                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw (new Exception(ex.Message));
            }

            //  return _roles.ToDataSourceResult(request);
            return Json(_roles.ToDataSourceResult(request));
        }

        [HttpGet("[controller]/[action]")]
        public async Task<JsonResult> GetJsonUsers([DataSourceRequest]DataSourceRequest request)
        {
            List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);

                    foreach (var item in _roles)
                    {
                        var resultclient = await _client.GetAsync(baseadress + "api/Usuario/GetUserById/" + item.UserId).Result.Content.ReadAsStringAsync();
                        item.UserName = (JsonConvert.DeserializeObject<ApplicationUser>(resultclient)).UserName;
                    }



                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw (new Exception(ex.Message));
            }

            //  return _roles.ToDataSourceResult(request);
            return Json(_roles);
        }



        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetUsersRoles([DataSourceRequest]DataSourceRequest request)
        {
            List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta);

                    foreach (var item in _roles)
                    {
                        var resultclient = await _client.GetAsync(baseadress + "api/Usuario/GetUserById/" + item.UserId).Result.Content.ReadAsStringAsync();
                        item.UserName = (JsonConvert.DeserializeObject<ApplicationUser>(resultclient)).UserName;

                        var resultclient2 = await _client.GetAsync(baseadress + "api/Roles/GetRoleById/" + item.RoleId).Result.Content.ReadAsStringAsync();
                        item.RoleName = (JsonConvert.DeserializeObject<ApplicationRole>(resultclient2)).Name;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }




            return _roles.ToDataSourceResult(request);
        }

        [HttpGet]
        public async Task<ActionResult> Details(Int64 UserId)
        {
            ApplicationUser _usuario = new ApplicationUser();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/UserRol/GetUserById/" + UserId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _usuario = JsonConvert.DeserializeObject<ApplicationUser>(valorrespuesta);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return View(_usuario);
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<ApplicationUserRole>> Insert([FromBody]ApplicationUserRole _role)
        {
            try
            {



                if (_role.RoleId.ToString() != "" && _role.UserId.ToString() != "" && _role.RoleId != null && _role.UserId != null)
                {
                    // TODO: Add insert logic here
                    string baseadress = config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                    List<ApplicationUserRole> _roles = new List<ApplicationUserRole>();

                    var result1 = await _client.GetAsync(baseadress + "api/UserRol/GetUserRoles");

                    string valorrespuesta1 = "";
                    if (result1.IsSuccessStatusCode)
                    {
                        valorrespuesta1 = await (result1.Content.ReadAsStringAsync());
                        _roles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(valorrespuesta1);

                        _roles = _roles.Where(e => e.UserId == _role.UserId).ToList();

                        if (_roles.Count > 0)
                        {

                            return await Task.Run(() => BadRequest("Este usuario ya tiene un rol asignado"));

                        }

                    }

                    _role.FechaCreacion = DateTime.Now;
                    _role.FechaModificacion = DateTime.Now;
                    _role.UsuarioCreacion = HttpContext.Session.GetString("user");
                    _role.UsuarioModificacion = HttpContext.Session.GetString("user");


                    var result = await _client.PostAsJsonAsync(baseadress + "api/UserRol/Insert", _role);

                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _role = JsonConvert.DeserializeObject<ApplicationUserRole>(valorrespuesta);

                    }
                    else
                    {
                        string request = await result.Content.ReadAsStringAsync();
                        return BadRequest(request);
                    }

                }
                else
                {
                    return BadRequest("Ingrese todos los datos!");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok("Datos Guardados Correctamente! ");
            // return new ObjectResult(new DataSourceResult { Data = new[] { _role }, Total = 1 });
        }

        [HttpPut("[controller]/[action]")]
        public async Task<ActionResult<ApplicationUserRole>> Update(string Id, ApplicationUserRole _rol)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/UserRol/Update", _rol);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _rol = JsonConvert.DeserializeObject<ApplicationUserRole>(valorrespuesta);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _rol }, Total = 1 });

        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<ApplicationRole>> Delete([FromBody]ApplicationUserRole _rol)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                _rol.UsuarioCreacion = "l";

                _rol.UsuarioModificacion = "l";

                var result = await _client.PostAsJsonAsync(baseadress + "api/UserRol/Delete", _rol);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _rol = JsonConvert.DeserializeObject<ApplicationUserRole>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }


            //  return new ObjectResult(new DataSourceResult { Data = new[] { RoleId }, Total = 1 });
            return new ObjectResult(new DataSourceResult { Data = new[] { _rol }, Total = 1 });
        }






    }
}