using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace EasyTrade.App
{
    internal class ADSettings
    {
        public const string AllowSpecificOrigin = "AllowSpecificOrigin";

        public ADSettings(IConfigurationRoot config)
        {
            ClientId = config["AzureAD:ClientId"];
            AadInstance = config["AzureAD:AadInstance"];
            Tenant = config["AzureAD:Tenant"];
            RedirectUri = config["AzureAD:RedirectUri"];

            // B2C policy identifiers
            SignUpPolicyId = config["AzureAD:SignUpPolicyId"];
            SignInPolicyId = config["AzureAD:SignInPolicyId"];
            SignUpOrSignInPolicyId = config["AzureAD:SignUpOrSignInPolicyId"];
            UserProfilePolicyId = config["AzureAD:UserProfilePolicyId"];
            ResetPwdPolicyId = config["AzureAD:ResetPwdPolicyId"];
        }

        public string ResetPwdPolicyId;
        public string UserProfilePolicyId;
        public string SignUpOrSignInPolicyId;
        public string SignUpPolicyId;
        public string SignInPolicyId;
        public string ClientId;
        public string RedirectUri;
        public string AadInstance;
        public string Tenant;
        //public string ClientSecret;

        internal OpenIdConnectOptions CreateOptionsFromPolicy(string policy)
        {
            policy = policy.ToLower();
            return new OpenIdConnectOptions
            {
                // For each policy, give OWIN the policy-specific metadata address, and
                // set the authentication type to the id of the policy
                // https://login.microsoftonline.com/adeasytrade.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_devsusi
                MetadataAddress = string.Format(AadInstance, Tenant, policy),
                AuthenticationScheme = policy,
                CallbackPath = new PathString(string.Format("/{0}", policy)),

                // These are standard OpenID Connect parameters, with values pulled from config.json
                ClientId = this.ClientId,
                PostLogoutRedirectUri = this.RedirectUri,
                Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = RemoteFailure,
                    OnRemoteSignOut = RemoteSignOut
                },
                ResponseType = OpenIdConnectResponseType.IdToken,

                // This piece is optional - it is used for displaying the user's name in the navigation bar.
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                },
            };
        }

        private Task RemoteSignOut(RemoteSignOutContext arg)
        {
            return Task.FromResult(0);
        }

        // Used for avoiding yellow-screen-of-death
        private Task RemoteFailure(FailureContext context)
        {
            context.HandleResponse();
            if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
            {
                context.Response.Redirect("/");
            }
            else
            {
                context.Response.Redirect("/Home/Error?message=" + context.Failure.Message);
            }

            return Task.FromResult(0);
        }
    }
}