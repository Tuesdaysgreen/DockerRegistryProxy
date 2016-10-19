using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DockerRegistryProxy
{
    public class DockerRegistryResponse
    {
        public bool Success { get; set; }
        public string Content { get; set; }
    }

    public class DockerRegistryProxy : IDisposable
    {
        public const string DOCKERHUB_BASEURL = "https://index.docker.io";
        protected string BaseUrl { get; set; }
        protected HttpClient _client = null;

        public DockerRegistryProxy(string baseUrl, string username, string password)
        {
            this.BaseUrl = baseUrl;
            this._client = CreateHttpClient(username, password);
        }

        #pragma warning disable 1998
        public virtual async Task<bool> IsApiVersionSupportedAsync()
        {
            throw new NotImplementedException("IsApiVersionSupportedAsync");
        }

        #pragma warning disable 1998
        public virtual async Task<DockerRegistryResponse> GetRepositoryTagAsync(string repository, string tag)
        {
            throw new NotImplementedException("GetRepositoryTagAsync");
        }

        public void Dispose()
        {
            if(this._client != null)
            {
                this._client.Dispose();
                this._client = null;
            }
        }

        private HttpClient CreateHttpClient(string username, string password)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Microsoft Azure");
            }

            if (!string.IsNullOrEmpty(username))
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password));
                var authValue = Convert.ToBase64String(plainTextBytes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            }
            return client;
        }
    }
}
