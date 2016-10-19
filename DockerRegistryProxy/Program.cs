using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DockerRegistryProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
            
            Task.Run(async () =>
            {
                try
                {
                    using (var v1Proxy = new DockerRegistryV1Proxy(DockerRegistryProxy.DOCKERHUB_BASEURL, null, null))
                    {
                        if(await v1Proxy.IsApiVersionSupportedAsync())
                        {
                            Console.WriteLine("V1 Supported");
                        }
                        else
                        {
                            Console.WriteLine("V1 Not supported");
                        }

                        var response = await v1Proxy.GetRepositoryTagAsync("nginx", "");

                        if (response.Success)
                        {
                            Console.WriteLine("V1 Success");
                        }
                        else
                        {
                            Console.WriteLine("V1 Failed");
                        }

                        Console.WriteLine(response.Content);
                    }

                    using (var v2Proxy = new DockerRegistryV2Proxy("", null, null))
                    {
                        if(await v2Proxy.IsApiVersionSupportedAsync())
                        {
                            Console.WriteLine("V2 Supported");
                        }
                        else
                        {
                            Console.WriteLine("V2 Not Supported");
                        }

                        var response = await v2Proxy.GetRepositoryTagAsync("", "");

                        if (response.Success)
                        {
                            Console.WriteLine("Success");
                        }
                        else
                        {
                            Console.WriteLine("Failed");
                        }

                        Console.WriteLine(response.Content);

                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            });

            Console.ReadLine();
        }
    }
}
