﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPMVC
{
    [HtmlTargetElement(Attributes = "policy")]
    public  class PolicyTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authService;
        private readonly ClaimsPrincipal _principal;
        private readonly ILogger _logger;

        public PolicyTagHelper(IAuthorizationService authService
            , ILogger<PolicyTagHelper> logger
            , IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _principal = httpContextAccessor.HttpContext.User;
            this._logger = logger;

        }

        public  string Policy { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                if (!(await _authService.AuthorizeAsync(_principal, Policy)).Succeeded)
                    output.SuppressOutput();
            }
            catch (Exception ex)
            {
                output.SuppressOutput();
                _logger.LogError($"Ocurrio un error: { ex.ToString() }");

                // throw ex;
            }
            // if (!await _authService.AuthorizeAsync(_principal, Policy)) ASP.NET Core 1.x



        }
    }



    [HtmlTargetElement(Attributes = "deshabilitar")]
    public class DeshabilitarTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authService;
        private readonly ClaimsPrincipal _principal;

        public DeshabilitarTagHelper(IAuthorizationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _principal = httpContextAccessor.HttpContext.User;
        }

        public string deshabilitar { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // if (!await _authService.AuthorizeAsync(_principal, Policy)) ASP.NET Core 1.x
           // if(Policy!=null)
            if (!(await _authService.AuthorizeAsync(_principal, deshabilitar)).Succeeded)
               output.Attributes.SetAttribute("disabled","disabled");
               output.Attributes.SetAttribute("class","noclick");
           

        }
    }




    //[HtmlTargetElement("textarea", Attributes = ForAttributeName)]
    //public class MyCustomTextArea : TextAreaTagHelper
    //{
    //    private const string ForAttributeName = "asp-for";

    //    [HtmlAttributeName("asp-is-disabled")]
    //    public bool IsDisabled { set; get; }

    //    public MyCustomTextArea(IHtmlGenerator generator) : base(generator)
    //    {
    //    }

    //    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    //    {
    //        if (IsDisabled)
    //        {
    //            output.Attributes.SetAttribute("disabled","disabled");
    //        }

    //        base.Process(context, output);
    //    }


    //}







}
