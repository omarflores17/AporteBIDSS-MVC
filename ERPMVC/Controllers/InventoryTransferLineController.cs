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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    [Authorize]
    [CustomAuthorization]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class InventoryTransferLineController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        public InventoryTransferLineController(ILogger<InventoryTransferLineController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
        }
        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetInventoryTransferLine([DataSourceRequest]DataSourceRequest request, InventoryTransferLine _ContratoDetallep)
        {
            
            List<InventoryTransferLine> _ContratoDetalle = new List<InventoryTransferLine>();

            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                if (HttpContext.Session.Get("listadoproductos") == null
                    || HttpContext.Session.GetString("listadoproductos") == "")
                {
                    if (_ContratoDetallep.InventoryTransferId > 0)
                    {
                        string serialzado = JsonConvert.SerializeObject(_ContratoDetallep).ToString();
                        HttpContext.Session.SetString("listadoproductos", serialzado);
                    }
                }
                else
                {
                    _ContratoDetallep.UsuarioCreacion = HttpContext.Session.GetString("user");
                    _ContratoDetallep.UsuarioModificacion = HttpContext.Session.GetString("user");
                    _ContratoDetallep.FechaCreacion = DateTime.Now;
                    _ContratoDetallep.FechaModificacion = DateTime.Now;
                    _ContratoDetalle = JsonConvert.DeserializeObject<List<InventoryTransferLine>>(HttpContext.Session.GetString("listadoproductos"));
                }
                if (_ContratoDetallep.InventoryTransferId > 0)
                {
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/InventoryTransferLine/GetInventoryTransferLineByInventoryTransferId/" + _ContratoDetallep.InventoryTransferId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _ContratoDetalle = JsonConvert.DeserializeObject<List<InventoryTransferLine>>(valorrespuesta);
                    }
                }
                else
                {

                    List<InventoryTransferLine> _existelinea = new List<InventoryTransferLine>();
                    if (HttpContext.Session.GetString("listadoproductos") != "" && HttpContext.Session.GetString("listadoproductos") != null)
                    {
                        _ContratoDetalle = JsonConvert.DeserializeObject<List<InventoryTransferLine>>(HttpContext.Session.GetString("listadoproductos"));
                        _existelinea = _ContratoDetalle.Where(q => q.Id == _ContratoDetallep.InventoryTransferId).ToList();
                    }

                    var id = _ContratoDetallep.Id;

                    if (_ContratoDetalle.Count > 0)
                    {
                        id--;
                        foreach (var item in _ContratoDetalle)
                        {
                            if (item.ProductId == _ContratoDetallep.ProductId)
                            {
                                item.QtyStock = item.QtyStock + _ContratoDetallep.QtyStock;
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                                break;
                            }
                            else if (item.Id == id && _existelinea.Count == 0)
                            {
                                _ContratoDetalle.Add(_ContratoDetallep);
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                                break;
                            }
                        }
                    }
                    else if (_ContratoDetallep.Id > 0 && _existelinea.Count == 0)
                    {
                        _ContratoDetalle.Add(_ContratoDetallep);
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_ContratoDetalle).ToString());
                    }
                    else
                    {
                        var obj = _ContratoDetalle.Where(x => x.Id == _ContratoDetallep.Id).FirstOrDefault();
                        if (obj != null)
                        {

                            obj.ProductId = _ContratoDetallep.ProductId;
                            obj.QtyStock = _ContratoDetallep.QtyStock;


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

        public async Task<ActionResult<InventoryTransferLine>> Delete([FromBody]InventoryTransferLine _ContratoDetalle)
        {
            try
            {
                List<InventoryTransferLine> _Contrato_DetalleLIST =
                 JsonConvert.DeserializeObject<List<InventoryTransferLine>>(HttpContext.Session.GetString("listadoproductos"));

                if (_Contrato_DetalleLIST != null)
                {
                    _Contrato_DetalleLIST = _Contrato_DetalleLIST
                          .Where(q => q.Id != _ContratoDetalle.Id)
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