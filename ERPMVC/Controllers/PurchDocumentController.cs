﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ERPMVC.DTO;
using ERPMVC.Helpers;
using ERPMVC.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPMVC.Controllers
{
    
    [Authorize]
    [CustomAuthorization]
    public class PurchDocumentController : Controller
    {
        private readonly IOptions<MyConfig> config;
        private readonly ILogger _logger;
        private IHostingEnvironment _hostingEnvironment;
        public PurchDocumentController(IHostingEnvironment hostingEnvironment
            , ILogger<PurchDocumentController> logger, IOptions<MyConfig> config)
        {
            this.config = config;
            this._logger = logger;
            _hostingEnvironment = hostingEnvironment;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PurchDocument()
        {
            return PartialView();
        }

        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> pvwPurchDocumentUpload([FromBody]PurchDocument _PurchDocumentp)
        {
            PurchDocument _PurchDocument = new PurchDocument();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchDocument/GetPurchDocumentById/" + _PurchDocumentp.PurchDocumentId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchDocument = JsonConvert.DeserializeObject<PurchDocument>(valorrespuesta);

                }

                if (_PurchDocument == null)
                {
                    _PurchDocument = new PurchDocument();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }



            return PartialView(_PurchDocument);

        }


        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
        {
            List<PurchDocument> _PurchDocument = new List<PurchDocument>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchDocument/GetPurchDocument");
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchDocument = JsonConvert.DeserializeObject<List<PurchDocument>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _PurchDocument.ToDataSourceResult(request);

        }


        [HttpGet("[action]")]
        public async Task<DataSourceResult> GeDocumentByPurchId([DataSourceRequest]DataSourceRequest request, Int64 PurchId)
        {
            List<PurchDocument> _PurchDocument = new List<PurchDocument>();
            try
            {

                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchDocument/GeDocumentByPurchId/" + PurchId);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchDocument = JsonConvert.DeserializeObject<List<PurchDocument>>(valorrespuesta);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }


            return _PurchDocument.ToDataSourceResult(request);

        }




        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult<PurchDocument>> SavePurchDocument(IEnumerable<IFormFile> files, PurchDocumentDTO _PurchDocumentp)
        {

            try
            {

                PurchDocument _listPurchDocument = new PurchDocument();
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                var result = await _client.GetAsync(baseadress + "api/PurchDocument/GetPurchDocumentById/" + _PurchDocumentp.PurchDocumentId);
                string valorrespuesta = "";

                foreach (var file in files)
                {


                    FileInfo info = new FileInfo(file.FileName);
                    if (info.Extension.Equals(".pdf") || info.Extension.Equals(".jpg")
                        || info.Extension.Equals(".png")
                       || info.Extension.Equals(".xls") || info.Extension.Equals(".xlsx"))
                    {

                        _PurchDocumentp.ModifiedDate = DateTime.Now;
                        _PurchDocumentp.ModifiedUser = HttpContext.Session.GetString("user");
                        if (result.IsSuccessStatusCode)
                        {

                            valorrespuesta = await (result.Content.ReadAsStringAsync());
                            _listPurchDocument = JsonConvert.DeserializeObject<PurchDocument>(valorrespuesta);
                        }

                        if (_listPurchDocument == null) { _listPurchDocument = new Models.PurchDocument(); }
                        if (_listPurchDocument.PurchDocumentId == 0)
                        {
                            _PurchDocumentp.CreatedDate = DateTime.Now;
                            _PurchDocumentp.DocumentName = file.FileName;
                            _PurchDocumentp.CreatedUser = HttpContext.Session.GetString("user");
                            var insertresult = await Insert(_PurchDocumentp);
                            var value = (insertresult.Result as ObjectResult).Value;
                            _PurchDocumentp = ((PurchDocumentDTO)(value));
                        }
                        else
                        {
                            var updateresult = await Update(_PurchDocumentp.PurchDocumentId, _PurchDocumentp);
                        }



                        var filePath = _hostingEnvironment.WebRootPath + "/PurchDocuments/" + _PurchDocumentp.PurchDocumentId + "_"
                            + file.FileName.Replace(info.Extension, "") + "_" + _PurchDocumentp.DocumentTypeId + "_" + _PurchDocumentp.DocumentTypeName
                            + info.Extension;

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            // MemoryStream mstream = new MemoryStream();
                            //mstream.WriteTo(stream);
                        }

                        _PurchDocumentp.Path = filePath;
                        var updateresult2 = await Update(_PurchDocumentp.PurchDocumentId, _PurchDocumentp);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                throw ex;
            }

            return Json(_PurchDocumentp);
        }

        // POST: PurchDocument/Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<PurchDocumentDTO>> Insert(PurchDocumentDTO _PurchDocumentp)
        {
            PurchDocumentDTO _custo = new PurchDocumentDTO();
            try
            {
                // TODO: Add insert logic here
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                _PurchDocumentp.CreatedUser = HttpContext.Session.GetString("user");
                _PurchDocumentp.ModifiedUser = HttpContext.Session.GetString("user");
                var result = await _client.PostAsJsonAsync(baseadress + "api/PurchDocument/Insert", _PurchDocumentp);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _custo = JsonConvert.DeserializeObject<PurchDocumentDTO>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }
            return Ok(_custo);
            // return new ObjectResult(new DataSourceResult { Data = new[] { _CustomerDocument }, Total = 1 });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PurchDocumentDTO>> Update(Int64 id, PurchDocumentDTO _PurchDocumentp)
        {
            PurchDocumentDTO _PurchDocumentDTO = new PurchDocumentDTO();
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PutAsJsonAsync(baseadress + "api/PurchDocument/Update", _PurchDocumentp);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchDocumentDTO = JsonConvert.DeserializeObject<PurchDocumentDTO>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error{ex.Message}");
            }

            return new ObjectResult(new DataSourceResult { Data = new[] { _PurchDocumentDTO }, Total = 1 });
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<PurchDocument>> Delete([FromBody]PurchDocument _PurchDocument)
        {
            try
            {
                string baseadress = config.Value.urlbase;
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("token"));

                var result = await _client.PostAsJsonAsync(baseadress + "api/PurchDocument/Delete", _PurchDocument);
                string valorrespuesta = "";
                if (result.IsSuccessStatusCode)
                {
                    valorrespuesta = await (result.Content.ReadAsStringAsync());
                    _PurchDocument = JsonConvert.DeserializeObject<PurchDocument>(valorrespuesta);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }



            return new ObjectResult(new DataSourceResult { Data = new[] { _PurchDocument }, Total = 1 });
        }





    }
}