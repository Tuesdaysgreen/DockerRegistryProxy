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
                    using (var v1Proxy = new DockerRegistryV1Proxy(null, null))
                    {
                        var response = await v1Proxy.GetRepositoryTagAsync(DockerRegistryProxy.DOCKERHUB_BASEURL, "nginx", "");

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

                    using (var v2Proxy = new DockerRegistryV2Provider("", ""))
                    {
                        var response = await v2Proxy.GetRepositoryTagAsync("", "", "");

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
