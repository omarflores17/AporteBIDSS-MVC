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
    public class Contrato_DetalleController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public Contrato_DetalleController(ILogger<Contrato_DetalleController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        //get
        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetContratoDetalle([DataSourceRequest]DataSourceRequest request, Contrato_Detalle _ContratoDetallep)
        {
            List<Contrato_Detalle> _ContratoDetalle = new List<Contrato_Detalle>();
            
            try
            {
                string baseadress = config.Value.urlbase;
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
                    _ContratoDetallep.UsuarioCreacion = HttpContext.Session.GetString("user");
                    _ContratoDetallep.UsuarioModificacion = HttpContext.Session.GetString("user");
                    _ContratoDetallep.CreatedDate = DateTime.Now;
                    _ContratoDetallep.ModifiedDate = DateTime.Now;
                    _ContratoDetalle = JsonConvert.DeserializeObject<List<Contrato_Detalle>>(HttpContext.Session.GetString("listadoproductos"));
                }
                if (_ContratoDetallep.ContratoId > 0)
                {
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/Contrato_detalle/GetContrato_detalleByContratoId/" + _ContratoDetallep.ContratoId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _ContratoDetalle = JsonConvert.DeserializeObject<List<Contrato_Detalle>>(valorrespuesta);
                    }
                }
                else
                {

                    List<Contrato_Detalle> _existelinea = new List<Contrato_Detalle>();
                    if (HttpContext.Session.GetString("listadoproductos") != "" && HttpContext.Session.GetString("listadoproductos") != null)
                    {
                        _ContratoDetalle = JsonConvert.DeserializeObject<List<Contrato_Detalle>>(HttpContext.Session.GetString("listadoproductos"));
                        _existelinea = _ContratoDetalle.Where(q => q.Contrato_detalleId== _ContratoDetallep.Contrato_detalleId).ToList();
                    }

                    var id = _ContratoDetallep.Contrato_detalleId;

                    if (_ContratoDetalle.Count > 0)
                    {
                        id--;
                        foreach (var item in _ContratoDetalle)
                        {
                            if (item.ProductId == _ContratoDetallep.ProductId)
                            {
                                item.Cantidad = item.Cantidad + _ContratoDetallep.Cantidad;
                                item.Monto = item.Monto + _ContratoDetallep.Monto;
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                                break;
                            }
                            else if (item.Contrato_detalleId == id && _existelinea.Count == 0)
                            {
                                _ContratoDetalle.Add(_ContratoDetallep);
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                                break;
                            }
                        }
                    }
                    else if (_ContratoDetallep.Contrato_detalleId > 0 && _existelinea.Count == 0)
                    {
                        _ContratoDetalle.Add(_ContratoDetallep);
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                    }
                    else
                    {
                        var obj = _ContratoDetalle.Where(x => x.Contrato_detalleId == _ContratoDetallep.Contrato_detalleId).FirstOrDefault();
                        if (obj != null)
                        {

                            obj.ProductId = _ContratoDetallep.ProductId;
                            obj.Cantidad = _ContratoDetallep.Cantidad;
                            obj.Precio = _ContratoDetallep.Precio;
                            //obj.Amount = _ContratoDetallep.Amount;
                            obj.ProductId = _ContratoDetallep.ProductId;
                            //obj.Total = _ContratoDetallep.Total;
                            obj.Monto = _ContratoDetallep.Monto;
                            obj.Serie = _ContratoDetallep.Serie;
                            //obj.TaxCode = _ContratoDetallep.TaxCode;
                            //obj.TaxPercentage = _ContratoDetallep.TaxPercentage;
                            //obj.TaxAmount = _ContratoDetallep.TaxAmount;
                            //obj.SubTotal = _ContratoDetallep.SubTotal;
                            obj.Modelo = _ContratoDetallep.Modelo;
                            
                           
                            //obj.DiscountAmount = _ContratoDetallep.DiscountAmount;
                            //obj.DiscountPercentage = _ContratoDetallep.DiscountPercentage;
                        }
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
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

        [HttpGet("[controller]/[action]")]
        public async Task<DataSourceResult> GetContratoDetaleByContratoId([DataSourceRequest]DataSourceRequest request, int ContratoId)
        {
            List<Contrato_Detalle> _ContratoDetalle = new List<Contrato_Detalle>();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_detalle/GetContrato_detalleByContratoId/" + ContratoId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ContratoDetalle = JsonConvert.DeserializeObject<List<Contrato_Detalle>>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _ContratoDetalle.ToDataSourceResult(request);
        }

        //metodo de guardar registro
        public async Task<ActionResult<Contrato_Detalle>> SaveContratoDetalle([FromBody]Contrato_Detalle _ContratodetalleP)
        {
            Contrato_Detalle _ContratoDetalle = _ContratodetalleP;
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/Contrato_detalle/GetContrato_detalleById/" + _ContratoDetalle.Contrato_detalleId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ContratoDetalle = JsonConvert.DeserializeObject<Contrato_Detalle>(valorrespuesta);
                }

                if (_ContratoDetalle == null) { _ContratoDetalle = new Models.Contrato_Detalle(); }

                if (_ContratodetalleP.Contrato_detalleId == 0)
                {
                    var insertresult = await Insert(_ContratodetalleP);
                }
                else
                {
                    var updateresult = await Update(_ContratoDetalle.Contrato_detalleId, _ContratodetalleP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_ContratodetalleP);
        }

        //metodo de inserta reguistro
        [HttpPost("[action]")]
        public async Task<ActionResult<Contrato_Detalle>> Insert(Contrato_Detalle _ContratoDetalle)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                var result = await _client.PostAsJsonAsync(baseadress + "api/Contrato_detalle/Insert", _ContratoDetalle);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ContratoDetalle = JsonConvert.DeserializeObject<Contrato_Detalle>(valorrespuesta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok("Datos Guardados Correctamente! ");
        }
        //metodo de actualizar el reguistro

        [HttpPut("[action]")]
        public async Task<ActionResult<Contrato_Detalle>> Update(Int64 Id, Contrato_Detalle _ContratoDetalle)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/Contrato_detalle/Update", _ContratoDetalle);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _ContratoDetalle = JsonConvert.DeserializeObject<Contrato_Detalle>(valorrespuesta);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _ContratoDetalle }, Total = 1 });

        }

        //metodo para eliminar
        public async Task<ActionResult<Contrato_Detalle>> Delete([FromBody]Contrato_Detalle _ContratoDetalle)
        {
            try
            {
                List<Contrato_Detalle> _Contrato_DetalleLIST =
                 JsonConvert.DeserializeObject<List<Contrato_Detalle>>(HttpContext.Session.GetString("listadoproductos"));

                if (_Contrato_DetalleLIST != null)
                {
                    _Contrato_DetalleLIST = _Contrato_DetalleLIST
                          .Where(q => q.Contrato_detalleId != _ContratoDetalle.Contrato_detalleId)
                           //.Where(q => q.ContratoId != _ContratoDetalle.ContratoId)
                           .Where(q => q.ProductId != _ContratoDetalle.ProductId)
                          .ToList();

                    HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_Contrato_DetalleLIST));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            return await Task.Run(() => Ok(_ContratoDetalle));
            //return new ObjectResult(new DataSourceResult { Data = new[] { _ContratoDetalle }, Total = 1 });
        }
    }
}