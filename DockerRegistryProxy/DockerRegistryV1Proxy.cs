using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerRegistryProxy
{

    public class DockerRegistryV1Proxy : DockerRegistryProxy 
    {
        public DockerRegistryV1Proxy(string username, string password) : base(username, password)
        {
        }

        public override async Task<DockerRegistryResponse> GetRepositoryTagAsync(
            string baseUrl,
            string repository,
            string tag)
        {
            UriBuilder uriBuilder = new UriBuilder(baseUrl);

            if (!string.IsNullOrEmpty(tag))
            {
                uriBuilder.Path = string.Format("/v1/repositories/{0}/tags/{1}", repository, tag);
            }
            else
            {
                uriBuilder.Path = string.Format("/v1/repositories/{0}/tags", repository);
            }

            using (var response = await this._client.GetAsync(uriBuilder.ToString()))
            {
                var content = await response.Content.ReadAsStringAsync();
                return new DockerRegistryResponse()
                {
                    Success = response.IsSuccessStatusCode,
                    Content = content
                };
            }
        }
    }
}
