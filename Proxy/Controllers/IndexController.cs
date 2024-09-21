using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Web;
using System.Net;
using System.Text.Json;
using System.Text;
namespace Proxy.Controllers
{
    public class IndexController : Controller
    {
        private readonly IConfiguration configuration;
        public IndexController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        /// <summary>
        /// Metodo por donde ingresan las peticiones
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>


        [HttpGet, HttpPost, HttpPut, HttpDelete, Route("api/{*url}")]
        public IActionResult Index(string url)
        {
            var client = new HttpClient();
            var response = new HttpResponseMessage();


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(this.configuration["BaseUrl"]);

            var urlBuilder = new UriBuilder(Request.GetDisplayUrl());
            var query = HttpUtility.ParseQueryString(urlBuilder.Query);
            var requestUrl = string.Concat("api/", url, "?", query.ToString());


            requestUrl = requestUrl.Replace(HttpContext.Request.Path, "/");
            string requestMethod = Request.Method.ToString();

            switch (requestMethod)
            {
                case "GET":
                    response = client.GetAsync(requestUrl).Result;
                    break;

                case "POST":
                    HttpContent body = PreparacionJsonRequest(Request);
                    response = client.PostAsync(requestUrl, body).Result;
                    break;

                case "PUT":
                    HttpContent bodyPut = PreparacionJsonRequest(Request);
                    response = client.PutAsync(requestUrl, bodyPut).Result;
                    break;
                case "DELETE":
                    response = client.DeleteAsync(requestUrl).Result;
                    break;


            }


            if (response.StatusCode == HttpStatusCode.OK)
            {

                var deserializedObject = JsonSerializer.Deserialize<object>(response.Content.ReadAsStringAsync().Result);
                return StatusCode((int)response.StatusCode, deserializedObject);
            }

            return StatusCode((int)response.StatusCode);
        }


        private HttpContent PreparacionJsonRequest(HttpRequest Request)
        {
            StreamReader reader = new(Request.Body);
            string json = reader.ReadToEndAsync().Result;
            HttpContent body = new StringContent(json, Encoding.UTF8, "application/json");
            return body;
        }


    }
}
