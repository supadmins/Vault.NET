﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vault.Models;
using Vault.Models.Secret;

namespace Vault.Endpoints
{
    public class Endpoint : IEndpoint
    {
        private readonly VaultClient _client;
        private readonly string _uriBasePath;

        private const string UriRootPath = "/v1";
        private const string WrappedResponseLocation = "cubbyhole/response";


        public Endpoint(VaultClient client, string basePath = null)
        {
            _client = client;

            var path = basePath != null ? $"/{basePath}" : "";
            _uriBasePath = $"{UriRootPath}{path}";

        }

        public Task<Secret<TData>> Read<TData>(string path)
        {
            return Read<TData>(path, CancellationToken.None);
        }

        public Task<Secret<TData>> Read<TData>(string path, CancellationToken ct)
        {
            return _client.Get<Secret<TData>>($"{_uriBasePath}/{path}", ct);
        }

        private Task<Secret<TData>> Read<TData>(string path, string token, CancellationToken ct)
        {
            return _client.Get<Secret<TData>>($"{_uriBasePath}/{path}", token, ct);
        }

        public Task<WrappedSecret> Read(string path, TimeSpan wrapTtl)
        {
            return Read(path, wrapTtl, CancellationToken.None);
        }

        public Task<WrappedSecret> Read(string path, TimeSpan wrapTtl, CancellationToken ct)
        {
            return _client.Get<WrappedSecret>($"{_uriBasePath}/{path}", wrapTtl, ct);
        }
 
        public Task<Secret<TData>> List<TData>(string path)
        {
            return List<TData>(path, CancellationToken.None);
        }

        public Task<Secret<TData>> List<TData>(string path, CancellationToken ct)
        {
            return _client.List<Secret<TData>>($"{_uriBasePath}/{path}", ct);
        }

        public Task Write<TParameters>(string path, TParameters data)
        {
            return Write(path, data, CancellationToken.None);
        }

        public Task Write<TParameters>(string path, TParameters data, CancellationToken ct)
        {
            return _client.PutVoid($"{_uriBasePath}/{path}", data, ct);
        }

        public Task<Secret<TData>>  Write<TParameters, TData>(string path, TParameters data)
        {
            return Write<TParameters, TData>(path, data, CancellationToken.None);
        }

        public Task<Secret<TData>> Write<TParameters, TData>(string path, TParameters data, CancellationToken ct)
        {
            return _client.Put<TParameters, Secret<TData>>($"{_uriBasePath}/{path}", data, ct);
        }

        public Task Delete(string path)
        {
            return Delete(path, CancellationToken.None);
        }

        public Task Delete(string path, CancellationToken ct)
        {
            return _client.DeleteVoid($"{_uriBasePath}/{path}", ct);
        }

        public Task<Secret<TData>> Unwrap<TData>(string unwrappingToken)
        {
            return Unwrap<TData>(unwrappingToken, CancellationToken.None);
        }

        public async Task<Secret<TData>> Unwrap<TData>(string unwrappingToken, CancellationToken ct)
        {
            var wrappedSecret = await Read<WrappedSecretData>(WrappedResponseLocation, unwrappingToken, ct).ConfigureAwait(false);
            return await Task.Run(() => JsonConvert.DeserializeObject<Secret<TData>>(wrappedSecret.Data.Response), ct).ConfigureAwait(false); ;
        }

        internal class WrappedSecretData
        {
            public string Response { get; set; }
        }
    }
}