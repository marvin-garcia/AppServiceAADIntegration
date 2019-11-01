﻿using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using BackendFunctionApp.Utils;

namespace BackendFunctionApp.Services
{
    public static class ScopesRequiredHttpContextExtensions
    {
        //private string _audience;
        //private string _authority;
        //private string _clientId;
        //private ConfigurationManager<OpenIdConnectConfiguration> _configManager;
        //private string _tenant;
        //private  _tokenValidator;

        //protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    string _audience = "";
        //    _clientId = "";
        //    var aadInstance = "";
        //    _tenant = "";
        //    _authority = string.Format(CultureInfo.InvariantCulture, aadInstance, _tenant);
        //    _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        //        $"{_authority}/.well-known/openid-configuration",
        //        new OpenIdConnectConfigurationRetriever());
        //    _tokenValidator = new JwtSecurityTokenHandler();

        //    // For debugging/development purposes, one can enable additional detail in exceptions by setting IdentityModelEventSource.ShowPII to true.
        //    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        //    // check if there is a jwt in the authorization header, return 'Unauthorized' error if the token is null.
        //    if (request.Headers.Authorization == null || request.Headers.Authorization.Parameter == null)
        //        return BuildResponseErrorMessage(HttpStatusCode.Unauthorized);

        //    // Pull OIDC discovery document from Azure AD. For example, the tenant-independent version of the document is located
        //    // at https://login.microsoftonline.com/common/.well-known/openid-configuration.
        //    OpenIdConnectConfiguration config = null;
        //    try
        //    {
        //        config = await _configManager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        #if DEBUG
        //        return BuildResponseErrorMessage(HttpStatusCode.InternalServerError, ex.Message);
        //        #else
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        #endif
        //    }

        //    // You can get a list of issuers for the various Azure AD deployments (global & sovereign) from the following endpoint
        //    //https://login.microsoftonline.com/common/discovery/instance?authorization_endpoint=https://login.microsoftonline.com/common/oauth2/v2.0/authorize&api-version=1.1;

        //    IList<string> validissuers = new List<string>()
        //    {
        //        $"https://login.microsoftonline.com/{_tenant}/",
        //        $"https://login.microsoftonline.com/{_tenant}/v2.0",
        //        $"https://login.windows.net/{_tenant}/",
        //        $"https://login.microsoft.com/{_tenant}/",
        //        $"https://sts.windows.net/{_tenant}/"
        //    };

        //    // Initialize the token validation parameters
        //    TokenValidationParameters validationParameters = new TokenValidationParameters
        //    {
        //        // App Id URI and AppId of this service application are both valid audiences.
        //        ValidAudiences = new[] { _audience, _clientId },

        //        // Support Azure AD V1 and V2 endpoints.
        //        ValidIssuers = validissuers,
        //        IssuerSigningKeys = config.SigningKeys
        //    };

        //    try
        //    {
        //        // Validate token.
        //        SecurityToken securityToken;
        //        var claimsPrincipal = _tokenValidator.ValidateToken(request.Headers.Authorization.Parameter, validationParameters, out securityToken);

        //        #pragma warning disable 1998
        //        // This check is required to ensure that the Web API only accepts tokens from tenants where it has been consented to and provisioned.
        //        if (!claimsPrincipal.Claims.Any(x => x.Type == ClaimConstants.ScopeClaimType)
        //           && !claimsPrincipal.Claims.Any(y => y.Type == ClaimConstants.RolesClaimType))
        //        {
        //            #if DEBUG
        //            return BuildResponseErrorMessage(HttpStatusCode.Forbidden, "Neither 'scope' or 'roles' claim was found in the bearer token.");
        //            #else
        //            return BuildResponseErrorMessage(HttpStatusCode.Forbidden);
        //            #endif
        //        }
        //        #pragma warning restore 1998

        //        // Set the ClaimsPrincipal on the current thread.
        //        Thread.CurrentPrincipal = claimsPrincipal;

        //        // Set the ClaimsPrincipal on HttpContext.Current if the app is running in web hosted environment.
        //        if (HttpContext.Current != null)
        //            HttpContext.Current.User = claimsPrincipal;

        //        // If the token is scoped, verify that required permission is set in the scope claim. This could be done later at the controller level as well
        //        if (ClaimsPrincipal.Current.FindFirst(ClaimConstants.ScopeClaimType).Value != ClaimConstants.ScopeClaimValue)
        //            return BuildResponseErrorMessage(HttpStatusCode.Forbidden);

        //        return await base.SendAsync(request, cancellationToken);
        //    }
        //    catch (SecurityTokenValidationException stex)
        //    {
        //        #if DEBUG
        //        return BuildResponseErrorMessage(HttpStatusCode.Unauthorized, stex.Message);
        //        #else
        //        return BuildResponseErrorMessage(HttpStatusCode.Unauthorized);
        //        #endif
        //    }
        //    catch (Exception ex)
        //    {
        //        #if DEBUG
        //        return BuildResponseErrorMessage(HttpStatusCode.InternalServerError, ex.Message);
        //        #else
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        #endif
        //    }
        //}

        public static async Task ValidateTokenAndScope(this HttpRequest request, string aadInstance, string tenantId, string clientId, string audience, string[] acceptedScopes, CancellationToken cancellationToken)
        {
            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenantId);
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{authority}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());
            ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();

            // For debugging/development purposes, one can enable additional detail in exceptions by setting IdentityModelEventSource.ShowPII to true.
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            // check if there is a jwt in the authorization header, return 'Unauthorized' error if the token is null.
            string token = string.Empty;
            if (request.Headers.ContainsKey("Authorization") && request.Headers["Authorization"][0].StartsWith("Bearer "))
                token = request.Headers["Authorization"][0].Substring("Bearer ".Length);
            else
            {
                request.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                throw new HttpRequestException("Unauthorized");
            }

            // Pull OIDC discovery document from Azure AD. For example, the tenant-independent version of the document is located
            // at https://login.microsoftonline.com/common/.well-known/openid-configuration.
            OpenIdConnectConfiguration config = null;
            try
            {
                config = await configManager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw e;
            }

            // You can get a list of issuers for the various Azure AD deployments (global & sovereign) from the following endpoint
            // https://login.microsoftonline.com/common/discovery/instance?authorization_endpoint=https://login.microsoftonline.com/common/oauth2/v2.0/authorize&api-version=1.1;

            IList<string> validIssuers = new List<string>()
            {
                $"https://login.microsoftonline.com/{tenantId}/",
                $"https://login.microsoftonline.com/{tenantId}/v2.0",
                $"https://login.windows.net/{tenantId}/",
                $"https://login.microsoft.com/{tenantId}/",
                $"https://sts.windows.net/{tenantId}/"
            };

            // Initialize the token validation parameters
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                // App Id URI and AppId of this service application are both valid audiences.
                ValidAudiences = new[] { audience, clientId },

                // Support Azure AD V1 and V2 endpoints.
                ValidIssuers = validIssuers,
                IssuerSigningKeys = config.SigningKeys
            };

            try
            {
                // Validate token.
                SecurityToken securityToken;
                var claimsPrincipal = tokenValidator.ValidateToken(token, validationParameters, out securityToken);

                // This check is required to ensure that the Web API only accepts tokens from tenants where it has been consented to and provisioned.
                if (!claimsPrincipal.Claims.Any(x => x.Type == ClaimConstants.ScopeClaimType)
                   && !claimsPrincipal.Claims.Any(y => y.Type == ClaimConstants.RolesClaimType))
                {
                    request.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    throw new Exception("Forbidden: Neither 'scope' or 'roles' claim was found in the bearer token.");
                }

                // Set the ClaimsPrincipal on the current thread.
                Thread.CurrentPrincipal = claimsPrincipal;
                
                // Set the ClaimsPrincipal on HttpContext.Current if the app is running in web hosted environment.
                if (request.HttpContext.User != null)
                    request.HttpContext.User = claimsPrincipal;

                // If the token is scoped, verify that required permission is set in the scope claim. This could be done later at the controller level as well
                Claim scopeClaim = request.HttpContext.User?.FindFirst("http://schemas.microsoft.com/identity/claims/scope");

                if (scopeClaim == null || !scopeClaim.Value.Split(' ').Intersect(acceptedScopes).Any())
                {
                    request.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    string scopeClaimValue = scopeClaim == null ? null : scopeClaim.Value;
                    string message = $"The 'scope' claim does not contain scopes '{string.Join(",", acceptedScopes)}' or was not found. Supported scopes are: {scopeClaimValue}";
                    throw new HttpRequestException(message);
                }
            }
            catch (SecurityTokenValidationException stex)
            {
                throw stex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private HttpResponseMessage BuildResponseErrorMessage(HttpStatusCode statusCode, string error_description = "")
        //{
        //    var response = new HttpResponseMessage(statusCode);

        //    // The Scheme should be "Bearer", authorization_uri should point to the tenant url and resource_id should point to the audience.
        //    var authenticateHeader = new AuthenticationHeaderValue("Bearer", "authorization_uri=\"" + _authority + "\"" + "," + "resource_id=" + _audience + $",error_description={error_description}");
        //    response.Headers.WwwAuthenticate.Add(authenticateHeader);
        //    return response;
        //}
    }
}