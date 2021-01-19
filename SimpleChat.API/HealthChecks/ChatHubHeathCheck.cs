using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sentry;
using SimpleChat.Core;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data;
using SimpleChat.Domain;
using SimpleChat.ViewModel.Auth;
using SimpleChat.ViewModel.User;

namespace SimpleChat.API.HealthChecks
{
    internal class ChatHubHeathCheck : IHealthCheck
    {
        private readonly RedisDbContext _redis;
        private readonly UserManager<User> _userManager;

        public ChatHubHeathCheck(RedisDbContext redis, UserManager<User> userManager)
        {
            _redis = redis;
            _userManager = userManager;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HubConnection _connection = null;
            try
            {
                var url = GetHostAddress();
                url.Path = "/chathub";
                var accessToken = await GetAccessToken();

                _connection = new HubConnectionBuilder()
                    .WithUrl(url.ToString(), options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(accessToken);
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

        private async Task<string> GetAccessToken()
        {
            HttpClient httpClient = new HttpClient();

            UserLoginVM loginVM = new UserLoginVM()
            {
                UserName = "Testuser",
                Password = "Testuser.123456"
            };

            var url = GetHostAddress();
            url.Path = "/api/tokens/create";
            var response = await httpClient.PostAsJsonAsync(url.ToString(), loginVM);

            var responseString = await response.Content.ReadAsStringAsync();

            UserAuthenticationVM authModel = null;
            APIResultVM apiResult = null;
            try
            {
                if (responseString.Contains("Errors", StringComparison.CurrentCultureIgnoreCase))
                {
                    apiResult = JsonSerializer.Deserialize<APIResultVM>(responseString);

                    if (apiResult.Errors != null && apiResult.Errors.Any(a => a.ErrorCode == APIStatusCode.ERR02025))
                    {
                        var user = await _userManager.FindByNameAsync("testuser");
                        if (user == null)
                            throw new Exception("GetAccessToken for HealthChecks or UnitTests, is failed, user not found");

                        var database = _redis.GetDatabase();
                        var tokenDataResult = database.StringGet(user.Id.ToString());
                        if (!tokenDataResult.HasValue)
                            throw new Exception("GetAccessToken for HealthChecks or UnitTests, is failed, user token data not found");

                        TokenCacheVM tokenData = JsonSerializer.Deserialize<TokenCacheVM>(tokenDataResult);

                        return tokenData.AccessToken;
                    }
                    else
                    {
                        return "";
                    }
                }
                else if (responseString.Contains("UserName", StringComparison.CurrentCultureIgnoreCase))
                {
                    authModel = JsonSerializer.Deserialize<UserAuthenticationVM>(responseString);

                    return authModel.TokenData.AccessToken;
                }
                else
                {
                    SentrySdk.CaptureEvent(new SentryEvent()
                    {
                        Message = "GetAccessToken for HealthChecks or UnitTests, is failed",
                        Level = Sentry.Protocol.SentryLevel.Error
                    });

                    return "";
                }
            }
            catch (System.Exception e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
        }

        private UriBuilder GetHostAddress()
        {
            string ipAddress = "127.0.0.1";

            try
            {
                ipAddress = Dns.GetHostAddresses(new Uri("http://docker.for.win.localhost").Host)[0].ToString();
            }
            catch (Exception)
            {
            }

            return new UriBuilder(Uri.UriSchemeHttp, ipAddress, 5050);
        }
    }
}
