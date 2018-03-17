using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Doorman.Master.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace Doorman.Master.API.Utility
{
	public static class TokenBuilder
	{
		public static readonly TokenValidationParameters TokenValidationParams = new TokenValidationParameters()
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["TokenAuthentication:SecretKey"])),
			ValidIssuer = Startup.Configuration["TokenAuthentication:Issuer"],
			ValidateIssuer = true,
			ValidateLifetime = true,
			ValidAudience = Startup.Configuration["TokenAuthentication:Audience"],
			ValidateAudience = true,
			ClockSkew = TimeSpan.Zero,
			RequireSignedTokens = true,
		};

		public static string CreateToken(int userId, ClientConfigVM model)
		{
			var identity = GetClaimIdentity(userId, model.ClientId, model.ClientSecret);
			return GetSecurityToken(identity);
		}

		private static ClaimsIdentity GetClaimIdentity(int userId, string clientId, string secret)
		{
			return new ClaimsIdentity(
				new[]
				{
					new Claim(ClaimTypes.NameIdentifier, $"{userId}+{clientId}+{secret}")
				});
		}

		private static string GetSecurityToken(ClaimsIdentity identity)
		{
			var handler = new JwtSecurityTokenHandler();

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(Startup.Configuration["TokenAuthentication:SecretKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var securityToken = handler.CreateToken(new SecurityTokenDescriptor
			{
				Issuer = Startup.Configuration["TokenAuthentication:Issuer"],
				Audience = Startup.Configuration["TokenAuthentication:Audience"],
				SigningCredentials = creds,
				Subject = identity,
				Expires = DateTime.Now.AddDays(int.Parse(Startup.Configuration["TokenAuthentication:Expires"])),
				NotBefore = DateTime.Now
			});

			return handler.WriteToken(securityToken);
		}

		public static JwtSecurityToken GetValidSecurityToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();

			handler.ValidateToken(token, TokenValidationParams, out var validatedToken);

			return validatedToken as JwtSecurityToken;
		}
	}
}
