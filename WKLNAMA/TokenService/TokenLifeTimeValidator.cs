using Microsoft.IdentityModel.Tokens;
using WKLNAMA.Extensions;

namespace WKLNAMA.TokenService
{
    public static class TokenLifetimeValidator
    {
        public static bool Validate(
            DateTime? notBefore,
            DateTime? expires,
            SecurityToken tokenToValidate,
            TokenValidationParameters @param
        )
        {
            using var serviceScope = ServiceActivator.GetScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var _tokenService = serviceScope.ServiceProvider.GetRequiredService<ITokenService>();
            var token = context.Request.Headers["Authorization"].ToString().Substring(7);
            var result = _tokenService.ValidateToken(token);
            if (result)
                return true;
            else
            {
                //var _portalUOW = serviceScope.ServiceProvider.GetRequiredService<IPortalUnitOfWork>();
                //var refreshToken = context.Request.Headers["RefreshToken"].ToString();
                //var res = _portalUOW.LoginsRepository.Get().IgnoreQueryFilters().Where(p => p.RefreshToken == refreshToken && p.IsDeleted == false).FirstOrDefault();
                //if (res != null)
                //{
                //    if (res.RefreshTokenExpiryTime > DateTime.UtcNow)
                //    {
                //        var principals = _tokenService.GetPrincipalFromExpiredToken(token);
                //        Utils.NewAccessToken = _tokenService.GenerateAccessToken(principals.Claims);
                //        return true;
                //    }
                //    else
                //        return false;
                //}
                //else
                return false;
            }
        }
    }
}
