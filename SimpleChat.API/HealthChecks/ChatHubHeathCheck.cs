using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sentry;
using SimpleChat.ViewModel.Message;
using SimpleChat.ViewModel.User;

namespace SimpleChat.API.HealthChecks
{
    internal class ChatHubHeathCheck : IHealthCheck
    {
        public ChatHubHeathCheck()
        {
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HubConnection _connection = null;
            try
            {
                string ipAddress = "127.0.0.1";

                try
                {
                    ipAddress = Dns.GetHostAddresses(new Uri("http://docker.for.win.localhost").Host)[0].ToString();
                }
                catch (Exception)
                {
                }

                var urlPath = new UriBuilder(Uri.UriSchemeHttp, ipAddress, 5050);

                HttpClient httpClient = new HttpClient();

                UserLoginVM loginVM = new UserLoginVM()
                {
                    UserName = "Testuser",
                    Password = "Testuser.123456"
                };

                urlPath.Path = "/api/tokens/create";
                var response = await httpClient.PostAsJsonAsync(urlPath.ToString(), loginVM);

                var responseString = await response.Content.ReadAsStringAsync();
                var authModel = JsonSerializer.Deserialize<UserAuthenticationVM>(responseString);

                //----------------

                urlPath.Path = "/chathub";
                _connection = new HubConnectionBuilder()
                    .WithUrl(urlPath.ToString(), options => 
                    {
                        options.AccessTokenProvider = () => Task.FromResult(authModel.TokenData.AccessToken); 
                    })
                    .WithAutomaticReconnect()
                    .Build();

                _connection.On("TestConnection", () =>
                {
                    Console.WriteLine("ChatHub is working.");
                });

                await _connection.StartAsync();

                await _connection.InvokeAsync("CheckHub");

                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return await Task.FromResult(HealthCheckResult.Unhealthy());
            }
            finally
            {
                if (_connection != null && _connection.State == HubConnectionState.Connected)
                {
                    await _connection.StopAsync();
                }
            }
        }
    }
}
