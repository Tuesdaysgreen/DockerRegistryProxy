using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DockerRegistryProxy
{
    public class DockerRegistryV2Proxy : DockerRegistryProxy
    {
        public DockerRegistryV2Proxy(string baseUrl, string username, string password)
            : base(baseUrl, username, password)
        {
        }

        public override async Task<bool> IsApiVersionSupportedAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(this.BaseUrl);
            uriBuilder.Path = "/v2/";
            using (var response = await this._client.GetAsync(uriBuilder.ToString()))
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public override async Task<DockerRegistryResponse> GetRepositoryTagAsync(
            string repository,
            string tag)
        {
            UriBuilder uriBuilder = new UriBuilder(this.BaseUrl);
            uriBuilder.Path = string.Format("/v2/{0}/tags/list", repository);

            bool success = false;
            string content = null;

            using (var response = await this._client.GetAsync(uriBuilder.ToString()))
            {
                content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(tag))
                {
                    JObject result = JObject.Parse(content);
                    var tags = result["tags"].Values<string>();
                    success = tags.Any((t) => t == tag);

                    if (!success)
                    {
                        var errorResponse = new DockerRegistryErrorResponse()
                        {
                            errors = new DockerRegistryError[]
                            {
                                new DockerRegistryError()
                                {
                                    code = "TAG_NOT_FOUND",
                                    message = "Could not find tag in repository",
                                    detail = new DockerRegistryErrorDetail()
                                    {
                                        name = tag
                                    }
                                }
                            }
                        };

                        content = JsonConvert.SerializeObject(errorResponse);
                    }
                }
                else
                {
                    success = response.IsSuccessStatusCode;
                }

                return new DockerRegistryResponse()
                {
                    Success = success,
                    Content = content
                };
            }
        }
    }

    public class DockerRegistryErrorResponse
    {
        public DockerRegistryError[] errors { get; set; }
    }

    public class DockerRegistryError
    {
        public string code { get; set; }
        public string message { get; set; }
        public DockerRegistryErrorDetail detail { get; set; }
    }

    public class DockerRegistryErrorDetail
    {
        public string name { get; set; }
    }
}
