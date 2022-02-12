using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.FT
{
    public enum HttpVerb
    {
        GET,
        POST,
    }

    /// <summary>
    /// Class that helps to manage an http client to the backend API.
    /// </summary>
    public class BeimaClient : IDisposable
    {
        /// <summary>
        /// The http client.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Gets and sets the base address of the client.
        /// </summary>
        public Uri? BaseAddress
        {
            get
            {
                return _httpClient.BaseAddress;
            }
            set
            {
                _httpClient.BaseAddress = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUrl">The base url for the backend API.</param>
        public BeimaClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = new TimeSpan(0, 1, 0);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Sends an http request to the backend server and returns the http response.
        /// </summary>
        /// <param name="route">The url route excluding the base address.</param>
        /// <param name="verb">The http verb of the request (i.e. GET/POST).</param>
        /// <param name="body">The object to send with the request.</param>
        /// <returns>The http response message of the request.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<HttpResponseMessage> SendRequest(string route, HttpVerb verb, object? body = null)
        {
            StringContent? jsonString = null;
            HttpResponseMessage response;

            if (body != null)
            {
                var jsonObject = JsonConvert.SerializeObject(body);
                jsonString = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            }

            switch (verb)
            {
                case HttpVerb.GET:
                    response = await _httpClient.GetAsync(route);
                    break;
                case HttpVerb.POST:
                    response = await _httpClient.PostAsync(route, jsonString);
                    break;
                default:
                    throw new HttpRequestException($"Invalid request verb: {verb}.");
            }

            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Convert response content into C# object.
        /// </summary>
        /// <param name="response">The given http response message.</param>
        /// <returns>The response content in the form of a C# object.</returns>
        public static async Task<object> ExtractObject(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(content);
        }

        /// <summary>
        /// Disposes the http client.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
