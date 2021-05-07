using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjektWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ProjektWebAPI
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public BasicAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserManager<IdentityUser> userManager)
            : base(options, logger, encoder, clock)
        {
            _userManager = userManager;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            
            IdentityUser user;
            string password;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                password = credentials[1];

                user = await _userManager.FindByNameAsync(username);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            
            if (user == null ||
                password == null ||
                await _userManager.CheckPasswordAsync(user, password) == false)
                return AuthenticateResult.Fail("Invalid Username or Password");

            
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            if (user.Email != null) claims.Add(new Claim(ClaimTypes.Email, user.Email));
            if (user.PhoneNumber != null) claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;

            Response.Headers["WWW-Authenticate"] = "Basic";

            return Task.CompletedTask;
        }
    }
}
