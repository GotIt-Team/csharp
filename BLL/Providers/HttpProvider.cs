using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GotIt.BLL.Providers
{
    public class HttpProvider
    {
        private readonly IWebHostEnvironment _env;
        public HttpProvider(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string PythonUrl
        { 
            get
            {
                if (_env.IsDevelopment())
                {
                    return "http://localhost:5000/api/";
                }
                return "https://ai-got-it.azurewebsites.net/";
            }
        }

        public async Task<TResponse> SendRequest<TRequest, TResponse>(string baseUrl, string path, TRequest data)
        {
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(baseUrl)
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = TimeSpan.FromHours(2);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync(path, data);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }

                return await response.Content.ReadAsAsync<TResponse>();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
