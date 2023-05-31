// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;

namespace Demo_ClientCredentials
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient()
            {
                // BaseAddress = new Uri("http://localhost:5231")
            };
            var testAddress = "http://localhost:5231";

            var discoResponse = await client.GetDiscoveryDocumentAsync(testAddress);

            if (discoResponse.IsError)
            {
                Console.WriteLine($"Get discovery document error: {discoResponse.Error}");

                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoResponse.TokenEndpoint,
                ClientId = "testclient",
                ClientSecret = "secret"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine($"Get token error: {tokenResponse.Error}, {tokenResponse.ErrorDescription}");
                return;
            }

            Console.WriteLine($"AccessToken: {tokenResponse.AccessToken}");
            Console.WriteLine($"AccessToken ExpiresIn: {tokenResponse.ExpiresIn}");
            Console.WriteLine($"AccessToken Scope: {tokenResponse.Scope}");
            Console.WriteLine($"AccessToken Refresh Token: {tokenResponse.RefreshToken}");



            // introspect token

            var instrospectResponse = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = discoResponse.IntrospectionEndpoint,
                Token = tokenResponse.AccessToken,
                ClientId = "test.api1",
                ClientSecret = "secret"
            });

            if (instrospectResponse.IsError)
            {
                Console.WriteLine($"Introspect token error: {instrospectResponse.Error}");
                return;
            }

            Console.WriteLine($"Introspect token Active: {instrospectResponse.IsActive}");
            if (instrospectResponse.IsActive && instrospectResponse.Claims.Any())
            {
                foreach (var claim in instrospectResponse.Claims)
                {
                    Console.WriteLine($"Introspect token Claims: {claim.Type} -> {claim.Value}");
                }
            }

            Console.ReadKey();
        }
    }
}