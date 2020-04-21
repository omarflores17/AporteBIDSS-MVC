﻿using ERPMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Policies
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        //protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        //{
        //    // If user does not have the scope claim, get out of here
        //    if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
        //        return Task.CompletedTask;

        //    // Split the scopes string into an array
        //    var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer).Value.Split(' ');

        //    // Succeed if the scope array contains the required scope
        //    if (scopes.Any(s => s == requirement.Scope))
        //        context.Succeed(requirement);

        //    return Task.CompletedTask;
        //}


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == requirement.Type && c.Value == requirement.Value))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == requirement.Type && c.Value == requirement.Value).Value.Split(' ');

            // Succeed if the scope array contains the required scope
          //  if (scopes.Any(s => s == requirement.Scope))
          if(scopes.Any(s=>s==1.ToString()))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }







    }


}
