using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SMS.Web;

public static class Extensions
{
   
    // -------------------------- VIEW Authorisation Helper -------------------------//
    // ClaimsPrincipal - HasOneOfRoles extension method to check if a user has any of the roles in a comma separated string
    public static bool HasOneOfRoles(this ClaimsPrincipal claims, string rolesString)
    {
        // split string into an array of roles
        var roles = rolesString.Split(",");

        // linq query to check that ClaimsPrincipal has one of these roles
        return roles.FirstOrDefault(role => claims.IsInRole(role)) != null;
    }

    // --------------------------- AUTHENTICATION Helpers ----------------------------//
    // IServiceCollection extension method adding cookie authentication 
    public static void AddCookieAuthentication(this IServiceCollection services, 
                                                string notAuthorised = "/User/ErrorNotAuthorised", 
                                                string notAuthenticated= "/User/ErrorNotAuthenticated")
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.AccessDeniedPath = notAuthorised;
                    options.LoginPath = notAuthenticated;
                });
    }

    // ClaimsPrincipal extension method to extract user id (sid) from claims
    public static int GetSignedInUserId(this ClaimsPrincipal user)
    {
        if (user != null && user.Identity != null && user.Identity.IsAuthenticated) {
            // id stored as a string in the Sid claim - convert to an int and return
            Claim sid = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid);
            if (sid == null) {
                throw new KeyNotFoundException("Sid Claim is not found in the identity");
            } 
            try {
                return Int32.Parse(sid.Value);  
            } catch (FormatException) {
                throw new KeyNotFoundException("Sid Claim value is invalid - not an integer");  
            }
        }
        return 0;
    }

    // ClaimsPrincipal extension method to extract user email (Email) from claims
    public static string GetSignedInUserEmail(this ClaimsPrincipal user)
    {
        if (user != null && user.Identity != null && user.Identity.IsAuthenticated) {           
            Claim email = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email);
            if (email == null) {
                throw new KeyNotFoundException("Email Claim is not found in the identity");
            } 
            // return claim value (the email)
            return email.Value;
        }
        return null;
    }   
    
}

