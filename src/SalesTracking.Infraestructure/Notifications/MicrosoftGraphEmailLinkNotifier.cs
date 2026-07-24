using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Infrastructure.Notifications
{
    public sealed class MicrosoftGraphEmailLinkNotifier :
        IInvitationLinkNotifier,
        IPasswordResetLinkNotifier
    {
        private const string GraphScope = "https://graph.microsoft.com/.default";

        private readonly HttpClient _httpClient;
        private readonly MicrosoftGraphSettings _graphSettings;
        private readonly FrontendLinkSettings _linkSettings;

        public MicrosoftGraphEmailLinkNotifier(
            HttpClient httpClient,
            IOptions<MicrosoftGraphSettings> graphSettings,
            IOptions<FrontendLinkSettings> linkSettings)
        {
            _httpClient = httpClient;
            _graphSettings = graphSettings.Value;
            _linkSettings = linkSettings.Value;
        }

        public Task NotifyAsync(CreatedInvitation invitation)
        {
            string link = BuildLink(_linkSettings.InvitationUrl, invitation.Token);
            string body = $"""
                <p>Has sido invitado a SalesTracking.</p>
                <p><a href="{WebUtility.HtmlEncode(link)}">Aceptar invitación</a></p>
                <p>La invitación vence el {invitation.ExpiresAtUtc:yyyy-MM-dd HH:mm} UTC.</p>
                """;

            return SendAsync(invitation.Email, "Invitación a SalesTracking", body);
        }

        public Task NotifyAsync(PasswordForgot passwordForgot)
        {
            string link = BuildLink(_linkSettings.PasswordResetUrl, passwordForgot.Token);
            string body = $"""
                <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                <p><a href="{WebUtility.HtmlEncode(link)}">Restablecer contraseña</a></p>
                <p>El enlace vence el {passwordForgot.ExpiresAtUtc:yyyy-MM-dd HH:mm} UTC.</p>
                <p>Si no realizaste esta solicitud, puedes ignorar este correo.</p>
                """;

            return SendAsync(passwordForgot.Email, "Restablecer contraseña", body);
        }

        private async Task SendAsync(string recipient, string subject, string htmlBody)
        {
            string accessToken = await GetAccessTokenAsync();
            string sender = Uri.EscapeDataString(_graphSettings.SenderEmail);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://graph.microsoft.com/v1.0/users/{sender}/sendMail");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(new
            {
                message = new
                {
                    subject,
                    body = new
                    {
                        contentType = "HTML",
                        content = htmlBody
                    },
                    toRecipients = new[]
                    {
                        new
                        {
                            emailAddress = new
                            {
                                address = recipient
                            }
                        }
                    }
                },
                saveToSentItems = false
            });

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return;

            string responseBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Microsoft Graph rechazó el envío del correo ({(int)response.StatusCode}): {responseBody}");
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string tokenEndpoint =
                $"https://login.microsoftonline.com/{Uri.EscapeDataString(_graphSettings.TenantId)}/oauth2/v2.0/token";

            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _graphSettings.ClientId,
                ["client_secret"] = _graphSettings.ClientSecret,
                ["scope"] = GraphScope,
                ["grant_type"] = "client_credentials"
            });

            using HttpResponseMessage response = await _httpClient.PostAsync(tokenEndpoint, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"No se pudo autenticar con Microsoft Graph ({(int)response.StatusCode}): {responseBody}");
            }

            using JsonDocument document = JsonDocument.Parse(responseBody);
            if (!document.RootElement.TryGetProperty("access_token", out JsonElement tokenElement))
                throw new InvalidOperationException("Microsoft Graph no devolvió un access_token.");

            return tokenElement.GetString()
                ?? throw new InvalidOperationException("Microsoft Graph devolvió un access_token vacío.");
        }

        private static string BuildLink(string baseUrl, string token)
        {
            string separator = baseUrl.Contains('?') ? "&" : "?";
            return $"{baseUrl}{separator}token={Uri.EscapeDataString(token)}";
        }
    }
}
