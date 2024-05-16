namespace IdentityServerAPI
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;

    using APISupport;

    using APISupportTypes;

    using AuthenticationDto;

    using AuthenticationRepository;
    using AuthenticationRepository.Services.CreateUser;
    using AuthenticationRepositoryTypes;

    using CommonServices;
    using LoggingLibrary;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;

    using SecurityUtilityTypes;

    [AuthorizePolicy]
    [ApiController]
    [Route("auth")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class RoleBasedAuthenticationController : Controller
    {
        private readonly IAuthenticationRepository authenticationRepository;

        private readonly IDateTimeService dateTimeService;

        private readonly ILogger logger;

        private readonly ISecretKeyRetrieval secretKeyRetrieval;

        public RoleBasedAuthenticationController(
            ILogger logger,
            ISecretKeyRetrieval secretKeyRetrieval,
            IDateTimeService dateTimeService,
            IAuthenticationRepository authenticationRepository)
        {
            this.logger = logger;
            this.secretKeyRetrieval = secretKeyRetrieval;
            this.dateTimeService = dateTimeService;
            this.authenticationRepository = authenticationRepository;
        }

        [AllowAnonymousPolicy]
        [HttpGet("health")]
        public IActionResult Health()
        {
            return this.Ok();
        }

        /// <summary>
        ///     Authenticates a User returning a JWT token and a refresh token in a "refreshToken" cookie
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST auth/authenticate
        ///     {
        ///     "User": "carl",
        ///     "Password": "1234",
        ///     "Audience": ""
        ///     }
        /// </remarks>
        [AllowAnonymousPolicy]
        [HttpPost("authenticate")]
        public async Task<ActionResult<ISAuthenticateResponseDto>> Authenticate(ISAuthenticateRequestDto isAuthenticateRequestDto)
        {
            this.logger.Debug(LogClass.General, "Authenticate received");

            var result = await this.authenticationRepository.Authenticate(
                             new AuthenticationRequest(
                                 isAuthenticateRequestDto.OrganizationId,
                                 isAuthenticateRequestDto.User,
                                 isAuthenticateRequestDto.Password,
                                 isAuthenticateRequestDto.Audience,
                                 isAuthenticateRequestDto.Issuer,
                                 isAuthenticateRequestDto.IPAddress));

            if (result.IsSuccessful && result.IsAuthenticated)
            {
                ISRefreshTokenResponseDto? refreshTokenResponseDto = null;
                if (result.RefreshToken != null)
                {
                    refreshTokenResponseDto = new ISRefreshTokenResponseDto
                    {
                        IsSuccessful = true,
                        RefreshTokenDtoType = (RefreshTokenDtoType)result.RefreshToken.ResponseType,
                        JwtToken = result.JwtToken,
                        RefreshToken = result.RefreshToken.RefreshToken,
                        ResponseTypeEnum = result.ResponseType,
                        ErrorMessage = result.ErrorMessage
                    };
                }

                var response = new ISAuthenticateResponseDto
                {
                    IsSuccessful = true,
                    IsAuthenticated = true,
                    AccessToken = result.JwtToken,
                    Claims = result.Claims,
                    RefreshTokenResponseDto = refreshTokenResponseDto,
                    ResponseTypeEnum = result.ResponseType,
                    ErrorMessage = result.ErrorMessage

                };

                // The cookie is only set if a browser is connecting (i.e. no issuer is set)
                if (result.RefreshToken != null && result.RefreshToken.RefreshToken != null && string.IsNullOrEmpty(isAuthenticateRequestDto.Issuer))
                {
                    this.setTokenCookie(result.RefreshToken.RefreshToken);
                }
                else
                {
                    this.setTokenCookie(string.Empty);
                }

                return this.Ok(response);
            }

            this.setTokenCookie(string.Empty);
            return this.StatusCode(
                (int)HttpStatusCode.Forbidden,
                new ISAuthenticateResponseDto
                {
                    IsAuthenticated = false,
                    Claims = string.Empty,
                    AccessToken = string.Empty,
                    IsSuccessful = true,
                    RefreshTokenResponseDto = null,
                    ResponseTypeEnum = result.ResponseType,
                    ErrorMessage = result.ErrorMessage
                });
        }

        [HttpPost("get-user")]
        [AuthorizePolicy(Policy = AuthenticationPolicyConstants.Administrator)]
        public async Task<ActionResult<ISAccountResponseDto>> GetUserByUserName(ISAccountRequestDto accountRequestDto)
        {
            this.logger.Debug(LogClass.General, "GetUserByName received");

            var result = await this.authenticationRepository.GetUserByUserName(accountRequestDto.OrganizationId, accountRequestDto.UserName);
            var acctResponse = new ISAccountResponseDto()
            {
                IsSuccessful = true,
                OrganizationId = result.OrganizationId,
                UserName = result.UserName,
                Claims = result.Claims
            };
            return this.Ok(acctResponse);
        }

        [AllowAnonymousPolicy]
        [HttpPost("logout")]
        public async Task<ActionResult<ISLogoutResponseDto>> Logout(ISLogoutRequestDto logoutRequestDto)
        {
            this.logger.Debug(LogClass.General, "Logout received");

            if (string.IsNullOrEmpty(logoutRequestDto.UserName))
            {
                return this.Ok();
            }

            var result = await this.authenticationRepository.Logout(new LogoutRequest(logoutRequestDto.OrganizationId, logoutRequestDto.UserName, logoutRequestDto.Issuer, logoutRequestDto.Audience));
            if (result.IsSuccessful)
            {
                if (string.IsNullOrEmpty(logoutRequestDto.Issuer))
                {
                    this.setTokenCookie(string.Empty);
                }
                return this.Ok(new ISLogoutResponseDto() { IsSuccessful = true });
            }

            return this.StatusCode(500, new ISLogoutResponseDto() { IsSuccessful = false });
        }

        [AllowAnonymousPolicy]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ISRefreshTokenResponseDto>> RefreshToken(ISRefreshTokenRequestDto refreshTokenRequestDto)
        {
            this.logger.Debug(LogClass.General, "RefreshToken received");

            var refreshToken = refreshTokenRequestDto.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
            {
                return this.Unauthorized("Refresh Token missing.");
            }

            var result = await this.authenticationRepository.RefreshToken(
                             refreshToken,
                             this.ipAddress(),
                             refreshTokenRequestDto.Issuer);

            switch (result.RefreshTokenResponseType)
            {
                case RefreshTokenResponseType.successful:
                    return new ISRefreshTokenResponseDto
                    {
                        IsSuccessful = true,
                        RefreshTokenDtoType = RefreshTokenDtoType.successful,
                        JwtToken = result.JwtToken,
                        RefreshToken = result.RefreshToken?.RefreshToken
                    };
                case RefreshTokenResponseType.notFound:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.notFound };
                case RefreshTokenResponseType.expired:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.expired };
                case RefreshTokenResponseType.currentTokenNotFound:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.currentTokenNotFound };
                case RefreshTokenResponseType.attemptedReuse:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.attemptedReuse };
                case RefreshTokenResponseType.notActive:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.notActive };
                case RefreshTokenResponseType.duplicated:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.duplicated };
                default:
                    return new ISRefreshTokenResponseDto() { IsSuccessful = false, RefreshTokenDtoType = RefreshTokenDtoType.expired };
            }
        }

        /// <summary>
        ///
        ///     Adds a new user or updates an existing
        ///
        ///     Key value is by user name (not case sensitive)
        /// 
        /// </summary>
        /// <remarks>
        ///     Sample request:
        /// 
        ///     POST /authenticate
        /// 
        ///     {
        /// 
        ///         "OrganizationId": 1,
        /// 
        ///         "User": "carl",
        /// 
        ///         "Password": "1234",
        /// 
        ///         "Claims": "role,User,pinnumber,12345,anykey,valueforkey"
        /// 
        ///     }
        /// 
        /// </remarks>
        [HttpPost("update-user")]
        [AuthorizePolicy(Policy = AuthenticationPolicyConstants.Administrator)]
        public async Task<ActionResult<ISAccountUpdateResponseDto>> UpdateUser(ISAccountUpdateRequestDto isAccountUpdateRequestDto)
        {
            this.logger.Debug(LogClass.General, "UpdateUser received");
            var jwtSecurityToken = (JwtSecurityToken?)HttpContext.Items["JwtSecurityToken"];
            if (jwtSecurityToken == null || jwtSecurityToken.Claims == null)
            {
                return this.BadRequest("token now found in IS");
            }

            // User can only update a different org if they are a SuperUser
            var roles = jwtSecurityToken.Claims.Where(x => x.Type.Trim().ToLower() == "roles");
            if (!roles.Any(x => x.Value.ToLower().Contains("superuser")))
            {
                var organizationId = jwtSecurityToken.Claims.SingleOrDefault(x => x.Type.ToLower() == "organizationid");
                if (organizationId == null)
                {
                    return this.BadRequest("Token does not contain organizationId");
                }

                if (int.Parse(organizationId.Value) != isAccountUpdateRequestDto.OrganizationId)
                {
                    return this.BadRequest("Invalid OrganizationId");
                }
            }

            var result = await this.authenticationRepository.UpdateUser(isAccountUpdateRequestDto.OrganizationId,
                             new AccountUpdateDto()
                             {
                                 OrganizationId = isAccountUpdateRequestDto.OrganizationId,
                                 UserName = isAccountUpdateRequestDto.UserName,
                                 Password = isAccountUpdateRequestDto.Password,
                                 Claims = isAccountUpdateRequestDto.Claims
                             });

            var acctResponse = new ISAccountUpdateResponseDto() { IsSuccessful = result.IsSuccessful };

            return this.Ok(acctResponse);
        }

        [AllowAnonymousPolicy]
        [HttpPost("CreateAccount")]
        public async Task<ActionResult<BaseResponseDto>> CreateAccount(CreateAccountRequest request)
        {
            this.logger.Debug(LogClass.General, "Create User");
            var claims = "";
            var jwtSecurityToken = (JwtSecurityToken?)HttpContext.Items["JwtSecurityToken"];
            if (jwtSecurityToken == null || jwtSecurityToken.Claims == null)
            {
                return this.BadRequest("token now found in IS");
            }

            var roles = jwtSecurityToken.Claims.Where(x => x.Type.Trim().ToLower() == "roles");
            if(roles.Count() == 1)
            {
                var temp = roles.First();
                if (temp.Value.Trim() == "5150")
                {
                    claims = "Roles,3001";
                }
                else if(temp.Value.Trim() == "3001")
                {
                    claims = "Roles,1984";
                }
                else if(temp.Value.Trim() == "1984")
                {
                    claims = "Roles,2000";
                }
                else
                {
                    claims = "Roles,2001";
                }
            }
            
            
            if (request == null)
            {
                return BadRequest();
            }
            else
            {
                var response = await this.authenticationRepository.CreateAccount(request,claims);
                if (response == null)
                {
                    return this.BadRequest();
                }
                else
                {
                    return Ok(response.IsSuccessful);
                }
            }
        }
        private string ipAddress()
        {
            string? result;

            // get source ip address for the current request
            if (this.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                result = this.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                result = this.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            }

            return result ?? string.Empty;
        }

        private void setTokenCookie(string token)
        {
            var expires = this.dateTimeService.UtcNow.AddDays(this.secretKeyRetrieval.GetRefreshTokenTTLInDays());
            if (string.IsNullOrEmpty(token))
            {
                expires = this.dateTimeService.UtcNow.AddDays(-1D);
            }

            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                // Domain = "localhost",
                Expires = expires.AddMinutes(15)
            };

            this.Response.Cookies.Append(
                "refreshToken",
                token,
                cookieOptions);
        }
    }
}