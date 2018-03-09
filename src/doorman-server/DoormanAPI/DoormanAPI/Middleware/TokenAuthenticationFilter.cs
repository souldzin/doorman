using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoormanAPI.Utility;
using Microsoft.AspNetCore.Http;

namespace DoormanAPI.Middleware
{
	public class TokenAuthenticationFilter
	{
		private readonly RequestDelegate _requestDelegate;

		public TokenAuthenticationFilter(RequestDelegate requestDelegate)
		{
			_requestDelegate = requestDelegate;
		}



		public async Task Invoke(HttpContext httpContext)
		{
			string authoriztionHeader = httpContext.Request.Headers["Authorization"];

			if (authoriztionHeader != null && authoriztionHeader.StartsWith("Bearer"))
			{
				var currentToken = authoriztionHeader.Substring("Bearer ".Length).Trim();

				var securityToken = TokenBuilder.GetValidSecurityToken(currentToken);

				if (securityToken != null)
				{
					// your additional logic here...
					await _requestDelegate.Invoke(httpContext);
				}
				else
				{
					httpContext.Response.StatusCode = 401;
				}
			}
			else
			{
				await _requestDelegate.Invoke(httpContext);
			}
		}
	}
}
