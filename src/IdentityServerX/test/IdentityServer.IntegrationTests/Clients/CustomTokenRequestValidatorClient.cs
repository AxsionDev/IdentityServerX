// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Clients.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients
{
    public class CustomTokenRequestValidatorClient
    {
        private const string TokenEndpoint = "https://server/connect/token";

        private readonly HttpClient _client;

        public CustomTokenRequestValidatorClient()
        {
            var val = new TestCustomTokenRequestValidator();
            Startup.CustomTokenRequestValidator = val;

            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);

            _client = server.CreateClient();
        }

        //TO DO: Fix it up
        [Fact]
        public async Task Client_credentials_request_should_contain_custom_response()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            var fields = GetFields(response);
            //fields.Should().Contain("custom", "custom");
        }

        //TO DO: Fix it up
        [Fact]
        public async Task Resource_owner_credentials_request_should_contain_custom_response()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,

                ClientId = "roclient",
                ClientSecret = "secret",
                Scope = "api1",

                UserName = "bob",
                Password = "bob"
            });

            var fields = GetFields(response);
            //fields.Should().Contain("custom", "custom");
        }

        //TO DO: Fix it up
        [Fact]
        public async Task Refreshing_a_token_should_contain_custom_response()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = TokenEndpoint,

                ClientId = "roclient",
                ClientSecret = "secret",
                Scope = "api1 offline_access",

                UserName = "bob",
                Password = "bob"
            });

            response = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = TokenEndpoint,
                ClientId = "roclient",
                ClientSecret = "secret",

                RefreshToken = response.RefreshToken
            });

            var fields = GetFields(response);
            //fields.Should().Contain("custom", "custom");
        }

        //TO DO: Fix it up
        [Fact]
        public async Task Extension_grant_request_should_contain_custom_response()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                Address = TokenEndpoint,
                GrantType = "custom",

                ClientId = "client.custom",
                ClientSecret = "secret",

                Parameters =
                {
                    { "scope", "api1" },
                    { "custom_credential", "custom credential"}
                }
            });

            var fields = GetFields(response);
            //fields.Should().Contain("custom", "custom");
        }

        private Dictionary<string, object> GetFields(TokenResponse response)
        {
            if (response.Json.HasValue)
            {
                var jsonObject = JsonDocument.Parse(response.Json.Value.GetRawText()).RootElement;
                return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject.GetRawText());
            }
            return new Dictionary<string, object>();
        }
    }
}