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
    public class PurchaseOrderLineController : Controller
    {
        private readonly IOptions<MyConfig> _config;
        private readonly ILogger _logger;
        public PurchaseOrderLineController(ILogger<PurchaseOrderLineController> logger, IOptions<MyConfig> config)
        {
            this._logger = logger;
            this._config = config;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> pvwPurchaseOrderLine([FromBody]PurchaseOrderLineDTO _PurchaseOrderline)
        {
            PurchaseOrderLineDTO _PurchaseOrderf = new PurchaseOrderLineDTO();
            try
            {
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();


                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrderLine/GetPurchaseOrderLineById/"+ _PurchaseOrderline.Id);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderf = JsonConvert.DeserializeObject<PurchaseOrderLineDTO>(valorrespuesta);
                }

                if (_PurchaseOrderf == null)
                {
                    _PurchaseOrderf = new PurchaseOrderLineDTO();
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return PartialView("~/Views/PurchaseOrder/pvwPurchaseOrderDetailMant.cshtml", _PurchaseOrderf);
        }


        [HttpGet("[action]")]
        public async Task<DataSourceResult> GetPurchaseOrderLineById([DataSourceRequest]DataSourceRequest request, PurchaseOrderLine _PurchaseOrderLinep)
        {
            List<PurchaseOrderLine> _PurchaseOrderLine = new List<PurchaseOrderLine>();
            PurchaseOrder _PurchaseOrder = new PurchaseOrder();
            try
            {
                //string baseadress = _config.Value.urlbase;
                //HttpClient _client = new HttpClient();

                if (HttpContext.Session.Get("listadoproductos") == null
                   || HttpContext.Session.GetString("listadoproductos") == "")
                {
                    if (_PurchaseOrderLinep.PurchaseOrderId > 0)
                    {
                        string serialzado = JsonConvert.SerializeObject(_PurchaseOrderLinep).ToString();
                        HttpContext.Session.SetString("listadoproductos", serialzado);
                    }
                }
                else
                {
                    var result = HttpContext.Session.GetString("listadoproductos");
                    try
                    {
                        _PurchaseOrderLine = JsonConvert.DeserializeObject<List<PurchaseOrderLine>>(HttpContext.Session.GetString("listadoproductos"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                    }
                    
                }

                if (_PurchaseOrderLinep.PurchaseOrderId > 0)
                {
                    string baseadress = _config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.GetAsync(baseadress + "api/PurchaseOrderLine/GetPurchaseOrderLineByPurchaseOrderId/" + _PurchaseOrderLinep.PurchaseOrderId);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _PurchaseOrderLine = JsonConvert.DeserializeObject<List<PurchaseOrderLine>>(valorrespuesta);
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLine).ToString());
                    }
                }
                
                else
                {
                    List<PurchaseOrderLine> _existelinea = new List<PurchaseOrderLine>();
                    if (HttpContext.Session.GetString("listadoproductos") != "" && HttpContext.Session.GetString("listadoproductos") != null)
                    {
                        _PurchaseOrderLine = JsonConvert.DeserializeObject<List<PurchaseOrderLine>>(HttpContext.Session.GetString("listadoproductos"));
                        _existelinea = _PurchaseOrderLine.Where(q => q.PurchaseOrderId == _PurchaseOrderLinep.Id).ToList();
                    }
                    var id = _PurchaseOrderLinep.Id;
                    if (_PurchaseOrderLine.Count > 0)
                    {
                        id--;
                        foreach (var item in _PurchaseOrderLine)
                        {
                            if (item.ProductId == _PurchaseOrderLinep.ProductId)
                            {
                                item.QtyOrdered = item.QtyOrdered + _PurchaseOrderLinep.QtyOrdered;
                                item.SubTotal = item.SubTotal + _PurchaseOrderLinep.SubTotal;
                                item.TaxPercentage = item.TaxPercentage + _PurchaseOrderLinep.TaxPercentage;
                                item.Total = item.Total + _PurchaseOrderLinep.Total;
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLine).ToString());
                                break;
                            }
                            else if (item.Id == id && _existelinea.Count == 0)
                            {
                                _PurchaseOrderLine.Add(_PurchaseOrderLinep);
                                HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLine).ToString());
                                break;
                            }
                        }
                    }

                   else if (_PurchaseOrderLinep.Id > 0 && _existelinea.Count == 0)
                    {
                        _PurchaseOrderLine.Add(_PurchaseOrderLinep);
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLine).ToString());
                    }

                    else
                    {
                        
                        var obj = _PurchaseOrderLine.FirstOrDefault(x => x.Id == _PurchaseOrderLinep.Id);
                        if (obj != null)
                        {
                            obj.LineNumber = _PurchaseOrderLinep.LineNumber;
                            obj.ProductId = _PurchaseOrderLinep.ProductId;
                            obj.ProductDescription = _PurchaseOrderLinep.ProductDescription;
                            obj.QtyOrdered = _PurchaseOrderLinep.QtyOrdered;
                            obj.QtyReceived = _PurchaseOrderLinep.QtyReceived;
                            obj.QtyReceivedToDate = _PurchaseOrderLinep.QtyReceivedToDate;
                            obj.Price = _PurchaseOrderLinep.Price;
                            obj.TaxName = _PurchaseOrderLinep.TaxName;
                            obj.TaxRate = _PurchaseOrderLinep.TaxRate;
                            obj.TaxId = _PurchaseOrderLinep.TaxId;
                            obj.UnitOfMeasureId = _PurchaseOrderLinep.UnitOfMeasureId;
                            obj.UnitOfMeasureName = _PurchaseOrderLinep.UnitOfMeasureName;
                            obj.Amount = _PurchaseOrderLinep.Amount;
                            obj.TaxPercentage = _PurchaseOrderLinep.TaxPercentage;
                            obj.DiscountAmount = _PurchaseOrderLinep.DiscountAmount;
                            obj.TaxAmount = _PurchaseOrderLinep.TaxAmount;
                            obj.DiscountPercentage = _PurchaseOrderLinep.DiscountPercentage;
                            obj.SubTotal = _PurchaseOrderLinep.SubTotal;
                            obj.Total = _PurchaseOrderLinep.Total;

                        }
                        
                        HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLine).ToString());

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }
            
            return _PurchaseOrderLine.ToDataSourceResult(request);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult<PurchaseOrderLine>> SavePurchaseOrderLine([FromBody]PurchaseOrderLine _PurchaseOrderLineP)
        {
            PurchaseOrderLine _PurchaseOrderLine = _PurchaseOrderLineP;
            try
            {
                //PurchaseOrder _listItems = new PurchaseOrder();
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchaseOrderLine/GetPurchaseOrderById/" + _PurchaseOrderLine.Id);
                string valorrespuesta = "";
                //_PurchaseOrderLine.ModifiedDate = DateTime.Now;
                //_PurchaseOrderLine.ModifiedUser = HttpContext.Session.GetString("user");
                if (result.IsSuccessStatusCode)
                {

                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderLine = JsonConvert.DeserializeObject<PurchaseOrderLine>(valorrespuesta);
                }

                if (_PurchaseOrderLine == null) { _PurchaseOrderLine = new Models.PurchaseOrderLine(); }

                if (_PurchaseOrderLineP.Id == 0)
                {
                    //_PurchaseOrderLine.CreatedDate = DateTime.Now;
                    //_PurchaseOrderLine.CreatedUser = HttpContext.Session.GetString("user");
                    var insertresult = await Insert(_PurchaseOrderLineP);
                }
                else
                {
                    //_PurchaseOrderLineP.CreatedUser = _PurchaseOrderLine.CreatedUser;
                    //_PurchaseOrderLineP.CreatedDate = _PurchaseOrderLine.CreatedDate;
                    var updateresult = await Update(_PurchaseOrderLine.Id, _PurchaseOrderLineP);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_PurchaseOrderLineP);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<PurchaseOrderLine>> Insert(PurchaseOrderLine _PurchaseOrderLine)
        {
            try
            {
                // TODO: Add insert logic here
                string baseadress = _config.Value.urlbase;
                HttpClient _client = new HttpClient();
                var result = await _client.PostAsJsonAsync(baseadress + "api/PurchaseOrderLine/Insert", _PurchaseOrderLine);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchaseOrderLine = JsonConvert.DeserializeObject<PurchaseOrderLine>(valorrespuesta);
                }

          
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return Ok("Datos Guardados Correctamente! ");
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<PurchaseOrderLine>> Update(Int64 Id, PurchaseOrderLine _PurchaseOrderLine)
        {
            List<PurchaseOrderLine> _PurchaseOrderLinep = new List<PurchaseOrderLine>();

            try
            {
                if (_PurchaseOrderLine.PurchaseOrderId > 0 )
                {
                    // TODO: Add insert logic here
                    string baseadress = _config.Value.urlbase;
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                    var result = await _client.PutAsJsonAsync(baseadress + "api/PurchaseOrderLine/Update", _PurchaseOrderLine);
                    string valorrespuesta = "";
                    if (result.IsSuccessStatusCode)
                    {
                        valorrespuesta = await (result.Content.ReadAsStringAsync());
                        _PurchaseOrderLine = JsonConvert.DeserializeObject<PurchaseOrderLine>(valorrespuesta);
                    }

                }
                else 
                {
                            _PurchaseOrderLinep = JsonConvert.DeserializeObject<List<PurchaseOrderLine>>(HttpContext.Session.GetString("listadoproductos"));
                            var obj = _PurchaseOrderLinep.FirstOrDefault(x => x.Id == _PurchaseOrderLine.Id);
                            if (obj != null && _PurchaseOrderLine.QtyOrdered > 0 )
                            {
                                obj.LineNumber = _PurchaseOrderLine.LineNumber;
                                obj.ProductId = _PurchaseOrderLine.ProductId;
                                obj.ProductDescription = _PurchaseOrderLine.ProductDescription;
                                obj.QtyOrdered = _PurchaseOrderLine.QtyOrdered;
                                obj.QtyReceived = _PurchaseOrderLine.QtyReceived;
                                obj.QtyReceivedToDate = _PurchaseOrderLine.QtyReceivedToDate;
                                obj.Price = _PurchaseOrderLine.Price;
                                obj.TaxName = _PurchaseOrderLine.TaxName;
                                obj.TaxRate = _PurchaseOrderLine.TaxRate;
                                obj.TaxId = _PurchaseOrderLine.TaxId;
                                obj.UnitOfMeasureId = _PurchaseOrderLine.UnitOfMeasureId;
                                obj.UnitOfMeasureName = _PurchaseOrderLine.UnitOfMeasureName;
                                obj.Amount = Convert.ToInt32(_PurchaseOrderLine.QtyOrdered) * _PurchaseOrderLine.Price;
                                obj.TaxPercentage = _PurchaseOrderLine.TaxPercentage;
                                obj.DiscountAmount = Convert.ToInt32(_PurchaseOrderLine.Amount) * _PurchaseOrderLine.DiscountPercentage/100;
                                obj.TaxAmount = obj.SubTotal * _PurchaseOrderLine.TaxPercentage/100;
                                obj.DiscountPercentage = _PurchaseOrderLine.DiscountPercentage;
                                obj.SubTotal = obj.Amount - _PurchaseOrderLine.DiscountAmount;
                                obj.Total = obj.SubTotal + _PurchaseOrderLine.TaxAmount;

                            }
                    else if (obj != null && _PurchaseOrderLine.QtyAuthorized > 0)
                    {
                        obj.LineNumber = _PurchaseOrderLine.LineNumber;
                        obj.ProductId = _PurchaseOrderLine.ProductId;
                        obj.ProductDescription = _PurchaseOrderLine.ProductDescription;
                        obj.QtyOrdered = _PurchaseOrderLine.QtyOrdered;
                        obj.QtyReceived = _PurchaseOrderLine.QtyReceived;
                        obj.QtyAuthorized = _PurchaseOrderLine.QtyAuthorized;
                        obj.QtyReceivedToDate = _PurchaseOrderLine.QtyReceivedToDate;
                        obj.Price = _PurchaseOrderLine.Price;
                        obj.TaxName = _PurchaseOrderLine.TaxName;
                        obj.TaxRate = _PurchaseOrderLine.TaxRate;
                        obj.TaxId = _PurchaseOrderLine.TaxId;
                        obj.UnitOfMeasureId = _PurchaseOrderLine.UnitOfMeasureId;
                        obj.UnitOfMeasureName = _PurchaseOrderLine.UnitOfMeasureName;
                        obj.Amount = Convert.ToInt32(_PurchaseOrderLine.QtyAuthorized) * _PurchaseOrderLine.Price;
                        obj.TaxPercentage = _PurchaseOrderLine.TaxPercentage;
                        obj.DiscountAmount = Convert.ToInt32(_PurchaseOrderLine.Amount) * _PurchaseOrderLine.DiscountPercentage / 100;
                        obj.TaxAmount = obj.SubTotal * _PurchaseOrderLine.TaxPercentage / 100;
                        obj.DiscountPercentage = _PurchaseOrderLine.DiscountPercentage;
                        obj.SubTotal = obj.Amount - _PurchaseOrderLine.DiscountAmount;
                        obj.Total = obj.SubTotal + _PurchaseOrderLine.TaxAmount;

                    }
                    else if (obj != null && _PurchaseOrderLine.QtyReceived > 0)
                    {
                        obj.LineNumber = _PurchaseOrderLine.LineNumber;
                        obj.ProductId = _PurchaseOrderLine.ProductId;
                        obj.ProductDescription = _PurchaseOrderLine.ProductDescription;
                        obj.QtyOrdered = _PurchaseOrderLine.QtyOrdered;
                        obj.QtyReceived = _PurchaseOrderLine.QtyReceived;
                        obj.QtyAuthorized = _PurchaseOrderLine.QtyAuthorized;
                        obj.QtyReceivedToDate = _PurchaseOrderLine.QtyReceivedToDate;
                        obj.Price = _PurchaseOrderLine.Price;
                        obj.TaxName = _PurchaseOrderLine.TaxName;
                        obj.TaxRate = _PurchaseOrderLine.TaxRate;
                        obj.TaxId = _PurchaseOrderLine.TaxId;
                        obj.UnitOfMeasureId = _PurchaseOrderLine.UnitOfMeasureId;
                        obj.UnitOfMeasureName = _PurchaseOrderLine.UnitOfMeasureName;
                        obj.Amount = Convert.ToInt32(_PurchaseOrderLine.QtyReceived) * _PurchaseOrderLine.Price;
                        obj.TaxPercentage = _PurchaseOrderLine.TaxPercentage;
                        obj.DiscountAmount = Convert.ToInt32(_PurchaseOrderLine.Amount) * _PurchaseOrderLine.DiscountPercentage / 100;
                        obj.TaxAmount = obj.SubTotal * _PurchaseOrderLine.TaxPercentage / 100;
                        obj.DiscountPercentage = _PurchaseOrderLine.DiscountPercentage;
                        obj.SubTotal = obj.Amount - _PurchaseOrderLine.DiscountAmount;
                        obj.Total = obj.SubTotal + _PurchaseOrderLine.TaxAmount;

                    }
                    HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLinep).ToString());

                        }
                    
                

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _PurchaseOrderLine }, Total = 1 });

        }

       

        //metodo para eliminar
        public async Task<ActionResult<PurchaseOrderLine>> Delete([FromBody]PurchaseOrderLine _PurchaseOrderLine)
        {
            try
            {
                List<PurchaseOrderLine> _PurchaseOrderLineLIST =
                 JsonConvert.DeserializeObject<List<PurchaseOrderLine>>(HttpContext.Session.GetString("listadoproductos"));

                if (_PurchaseOrderLineLIST != null)
                {
                    _PurchaseOrderLineLIST = _PurchaseOrderLineLIST
                          .Where(q => q.Id != _PurchaseOrderLine.Id)
                          .Where(q => q.ProductId != _PurchaseOrderLine.ProductId)
                          .ToList();

                    HttpContext.Session.SetString("listadoproductos", JsonConvert.SerializeObject(_PurchaseOrderLineLIST));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
            return await Task.Run(() => Ok(_PurchaseOrderLine));
        }


    }
}