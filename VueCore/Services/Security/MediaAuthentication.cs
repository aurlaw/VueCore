using Microsoft.Azure.Management.Media;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;
using VueCore.Models.Options;

namespace VueCore.Services.Security
{
    public class MediaAuthentication
    {
         public static readonly string TokenType = "Bearer";

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="settings">The param is of type AzureMediaSettings, which reads values from local configuration file.</param>
        /// <returns>A task.</returns>
        // <CreateMediaServicesClientAsync>
        public async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(AzureMediaSettings settings, bool interactive = false)
        {
            ServiceClientCredentials credentials;
            if (interactive)
                credentials = await GetCredentialsInteractiveAuthAsync(settings);
            else
                credentials = await GetCredentialsAsync(settings);

            return new AzureMediaServicesClient(settings.ArmEndpoint, credentials)
            {
                SubscriptionId = settings.SubscriptionId,
            };
        }
        // </CreateMediaServicesClientAsync>

        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="settings">The param is of type AzureMediaSettings. This class reads values from local configuration file.</param>
        /// <returns></returns>
        // <GetCredentialsAsync>
        private async Task<ServiceClientCredentials> GetCredentialsAsync(AzureMediaSettings settings)
        {
            // Use ConfidentialClientApplicationBuilder.AcquireTokenForClient to get a token using a service principal with symmetric key

            var scopes = new[] { settings.ArmAadAudience + "/.default" };

            var app = ConfidentialClientApplicationBuilder.Create(settings.AadClientId)
                .WithClientSecret(settings.AadSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, settings.AadTenantId)
                .Build();

            var authResult = await app.AcquireTokenForClient(scopes)
                                                     .ExecuteAsync()
                                                     .ConfigureAwait(false);

            return new TokenCredentials(authResult.AccessToken, TokenType);
        }
        // </GetCredentialsAsync>

        /// <summary>
        /// Create the ServiceClientCredentials object based on interactive authentication done in the browser
        /// </summary>
        /// <param name="settings">The param is of type AzureMediaSettings. This class reads values from local configuration file.</param>
        /// <returns></returns>
        // <GetCredentialsInteractiveAuthAsync>
        private async Task<ServiceClientCredentials> GetCredentialsInteractiveAuthAsync(AzureMediaSettings settings)
        {
            var scopes = new[] { settings.ArmAadAudience + "/user_impersonation" };

            // client application of Az Cli
            string ClientApplicationId = "04b07795-8ddb-461a-bbee-02f9e1bf7b46";

            AuthenticationResult result = null;

            IPublicClientApplication app = PublicClientApplicationBuilder.Create(ClientApplicationId)
                .WithAuthority(AzureCloudInstance.AzurePublic, settings.AadTenantId)
                .WithRedirectUri("http://localhost")
                .Build();

            var accounts = await app.GetAccountsAsync();

            try
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                try
                {
                    result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
                }
                catch (MsalException maslException)
                {
                    Console.Error.WriteLine($"ERROR: MSAL interactive authentication exception with code '{maslException.ErrorCode}' and message '{maslException.Message}'.");
                }
            }
            catch (MsalException maslException)
            {
                Console.Error.WriteLine($"ERROR: MSAL silent authentication exception with code '{maslException.ErrorCode}' and message '{maslException.Message}'.");
            }

            return new TokenCredentials(result.AccessToken, TokenType);
        }
        // </GetCredentialsInteractiveAuthAsync>        
    }
}