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
    [Route("[controller]")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class SalesOrderLineController : Controller
    {
         private readonly IOptions<MyConfig> _config;
          private readonly ILogger _logger;
        public SalesOrderLineController(ILogger<SalesOrderLineController> logger, IOptions<MyConfig> config)
        {
              this._logger = logger;
             this._config = config;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwSalesOrderDetailMant([FromBody]SalesOrderLine _salesorderline)
        {
            SalesOrderLine _salesorderf = new SalesOrderLine();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrderLine/GetSalesOrderLineById/" ,_salesorderline);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _salesorderf = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);
                }

                if (_salesorderf == null) { _salesorderf = new SalesOrderLine { Description = ""  }; }
                //_salesorderf.editar = _salesorderline.editar;



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView("~/Views/SalesOrder/pvwSalesOrderDetailMant.cshtml",_salesorderf);
        }


        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetSalesOrderLine([DataSourceRequest]DataSourceRequest request,SalesOrderLineDTO _SalesOrderLinep)
        {
            List<SalesOrderLineDTO> _SalesOrderLine = new List<SalesOrderLineDTO>();
          
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                if (HttpContext.Session.Get("listadoproductos") == null 
                    || HttpContext.Session.GetString("listadoproductos") =="")
                {
                    if (_SalesOrderLinep.SalesOrderId > 0)
                    {
                        string serialzado = JsonConvert.SerializeObject(_SalesOrderLinep).ToString();
                        HttpContext.Session.SetString("listadoproductos", serialzado);
                    }
                }
                else
                {
                    _SalesOrderLine = JsonConvert.DeserializeObject<List<SalesOrderLineDTO>>(HttpContext.Session.GetString("listadoproductos"));
                }
                if (_SalesOrderLinep.SalesOrderId > 0)
                {

                    //string baseadress = _config.Value.urlbase;
                    //HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    //_client.DefaultRequestHeaders.Add("SalesOrderId", _salesorder.SalesOrderId.ToString());
                   // _client.DefaultRequestHeaders.Add("SalesOrderId", _SalesOrderLine.SalesOrderId.ToString());
                    var result = await _client.GetAsync(baseadress + "api/SalesOrderLine/GetSalesOrderIdBySalesOrderLineId/" + _SalesOrderLinep.SalesOrderId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _SalesOrderLine = JsonConvert.DeserializeObject<List<SalesOrderLineDTO>>(valorrespuesta);
                    }
                }
                else
                {

                    List<SalesOrderLineDTO> _existelinea = new List<SalesOrderLineDTO>();
                    if (HttpContext.Session.GetString("listadoproductos") != "" && HttpContext.Session.GetString("listadoproductos") != null)                        
                    {
                        _SalesOrderLine = JsonConvert.DeserializeObject<List<SalesOrderLineDTO>>(HttpContext.Session.GetString("listadoproductos"));
                        _existelinea = _SalesOrderLine.Where(q => q.SalesOrderLineId == _SalesOrderLinep.SalesOrderLineId).ToList();
                    }

                    var id = _SalesOrderLinep.SalesOrderLineId;
                    
                    if (_SalesOrderLine.Count > 0)
                    {
                        id--;
                        foreach (var item in _SalesOrderLine)
                        {
                            if (item.ProductId == _SalesOrderLinep.ProductId)
                            {
                                item.Cantidad = item.Cantidad + _SalesOrderLinep.Cantidad;
                                item.Monto = item.Monto + _SalesOrderLinep.Monto;
                                //item.TaxAmount = item.TaxAmount + _SalesOrderLinep.TaxAmount;
                                //item.DiscountAmount = item.DiscountAmount + _SalesOrderLinep.DiscountAmount;
                                //item.SubTotal = item.SubTotal + _SalesOrderLinep.SubTotal;

                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_SalesOrderLine).ToString());
                                break;
                            }
                            else if (item.SalesOrderLineId == id && _existelinea.Count == 0){
                                _SalesOrderLine.Add(_SalesOrderLinep);
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_SalesOrderLine).ToString());
                                break;
                            }
                        }
                    }
                    else if(_SalesOrderLinep.SalesOrderLineId > 0 && _existelinea.Count == 0)
                    {
                        _SalesOrderLine.Add(_SalesOrderLinep);
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_SalesOrderLine).ToString());
                    }
                    else
                    {
                        var obj = _SalesOrderLine.Where(x => x.SalesOrderLineId == _SalesOrderLinep.SalesOrderLineId).FirstOrDefault();
                        if (obj != null)
                        {
                            
                            //obj.Description = _SalesOrderLinep.Description;
                            obj.Precio = _SalesOrderLinep.Precio;
                            obj.Cantidad = _SalesOrderLinep.Cantidad;
                            obj.Monto = _SalesOrderLinep.Monto;
                            obj.ProductId = _SalesOrderLinep.ProductId;
                            obj.Serie = _SalesOrderLinep.Serie;
                            obj.Modelo = _SalesOrderLinep.Modelo;
                            //obj.SubProductName = _SalesOrderLinep.SubProductName;
                            //obj.Total = _SalesOrderLinep.Total;
                            //obj.UnitOfMeasureId = _SalesOrderLinep.UnitOfMeasureId;
                            //obj.UnitOfMeasureName = _SalesOrderLinep.UnitOfMeasureName;
                            //obj.TaxCode = _SalesOrderLinep.TaxCode;
                            //obj.TaxPercentage = _SalesOrderLinep.TaxPercentage;
                            //obj.TaxAmount = _SalesOrderLinep.TaxAmount;
                            //obj.SubTotal = _SalesOrderLinep.SubTotal;
                            //obj.ProductId = _SalesOrderLinep.ProductId;
                            //obj.DiscountAmount = _SalesOrderLinep.DiscountAmount;
                            //obj.DiscountPercentage = _SalesOrderLinep.DiscountPercentage;
                            //obj.Description = _SalesOrderLinep.Description;
                        }
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_SalesOrderLine).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
               _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            return _SalesOrderLine.ToDataSourceResult(request);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<SalesOrderLine>> SaveSalesOrderLine([FromBody]SalesOrderLine _SalesOrderLineP)
        {
            SalesOrderLine _SalesOrderLine = _SalesOrderLineP;
            try
            {
                //JournalEntry _listItems = new JournalEntry();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/SalesOrderLine/GetSalesOrderById/" + _SalesOrderLine.SalesOrderLineId);
                string valorrespuesta = "";
                //_JournalEntryLine.ModifiedDate = DateTime.Now;
                //_JournalEntryLine.ModifiedUser = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrderLine = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);
                }

                if (_SalesOrderLine == null) { _SalesOrderLine = new Models.SalesOrderLine(); }

                if (_SalesOrderLineP.SalesOrderLineId == 0)
                {
                    //_JournalEntryLine.CreatedDate = DateTime.Now;
                    //_JournalEntryLine.CreatedUser = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_SalesOrderLineP);
                }
                else
                {
                    //_SalesOrderLineP.CreatedUser = _JournalEntryLine.CreatedUser;
                    //_SalesOrderLineP.CreatedDate = _JournalEntryLine.CreatedDate;
                    var updateresult = await Update(_SalesOrderLine.SalesOrderLineId, _SalesOrderLineP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_SalesOrderLineP);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<SalesOrderLine>> Insert(SalesOrderLine _SalesOrderLine)
        {
            try
            {
                    // TODO: Add insert logic here
                    string baseadress = _config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrderLine/Insert", _SalesOrderLine);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _SalesOrderLine = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);
                    }

                //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " +  HttpContext.Session.GetString("token"));
                //        var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrderLine/Insert", _SalesOrderLine);

                        //string valorrespuesta = "";
                        //if (result.IsSuccessStatusCode)
                        //{
                        //    valorrespuesta = await (result.Content.ReadAsStringAsync());
                        //    _SalesOrderLine = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);

                        //}
                        //else
                        //{
                        //    string request = await result.Content.ReadAsStringAsync();
                        //    return BadRequest(request);
                        //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

              return Ok("Datos Guardados Correctamente! ");
           // return new ObjectResult(new DataSourceResult { Data = new[] { _role }, Total = 1 });
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<SalesOrderLine>> Update(Int64 Id, SalesOrderLine _SalesOrderLine)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();

                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.PutAsJsonAsync(baseadress + "api/SalesOrderLine/Update", _SalesOrderLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _SalesOrderLine = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _SalesOrderLine }, Total = 1 });

        }

        //[HttpPost("[action]")]
        public async Task<ActionResult<SalesOrderLine>> Delete([FromBody]SalesOrderLine _salesorder)
        {
            try
            {
                List<SalesOrderLine> _salesorderLIST =
                 JsonConvert.DeserializeObject<List<SalesOrderLine>>(HttpContext.Session.GetString("listadoproductos"));

                if (_salesorderLIST != null)
                {
                    _salesorderLIST =
                          _salesorderLIST.Where(q => q.SalesOrderLineId != _salesorder.SalesOrderLineId)
                           .Where(q => q.ProductId != _salesorder.ProductId)
                           //.Where(q => q.Cantidad != _salesorder.Cantidad)
                           //.Where(q => q.Precio != _salesorder.Precio)
                           //.Where(q => q.Serie != _salesorder.Serie)
                           //.Where(q => q.Modelo != _salesorder.Modelo)
                           //.Where(q => q.PorcentajeDescuento != _salesorder.PorcentajeDescuento)
                           //.Where(q => q.Tax != _salesorder.Tax)
                           //.Where(q => q.FundingInterestRate != _salesorder.FundingInterestRate)
                           //.Where(q => q.Monto != _salesorder.Monto)
                          .ToList();

                    HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_salesorderLIST));
                }


                //string baseadress = _config.Value.urlbase;
                //HttpClient _client = new HttpClient();
                //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                //var result = await _client.PostAsJsonAsync(baseadress + "api/SalesOrderLine/Delete", _rol);
                //string valorrespuesta = "";
                //if (result.IsSuccessStatusCode)
                //{
                //    valorrespuesta = await (result.Content.ReadAsStringAsync());
                //    _rol = JsonConvert.DeserializeObject<SalesOrderLine>(valorrespuesta);
                //}

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }

            return await Task.Run(() => Ok(_salesorder));
            //return new ObjectResult(new DataSourceResult { Data = new[] { _salesorder }, Total = 1 });
        }




    }
}