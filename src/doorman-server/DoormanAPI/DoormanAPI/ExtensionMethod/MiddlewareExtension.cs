using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoormanAPI.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DoormanAPI.ExtensionMethod
{
    public static class MiddlewareExtension
    {
	    /// <summary> 
	    /// Adds middleware to check the authentication token 
	    /// /// </summary> /// <param name="builder"></param> 
	    /// /// <returns></returns>
	     public static IApplicationBuilder UseTokenAuthenticationFilterAuthorize(this IApplicationBuilder builder) { return builder.UseMiddleware<TokenAuthenticationFilter>(); }
	}
}
