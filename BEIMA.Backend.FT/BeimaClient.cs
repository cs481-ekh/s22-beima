using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    public enum HttpVerb
    {
        GET,
        POST,
    }

    public class BeimaException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public BeimaException(string message, HttpStatusCode status) : base(message)
        {
            StatusCode = status;
        }
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
        public async Task<HttpResponseMessage> SendRequest(string route, HttpVerb verb, object? body = null, string queryString = "")
        {
            StringContent? jsonString = null;
            HttpResponseMessage response;

            if (body != null)
            {
                if (body is string)
                {
                    jsonString = new StringContent((string)body);
                }
                else
                {
                    var jsonObject = JsonConvert.SerializeObject(body);
                    jsonString = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                }
            }

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
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

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new BeimaException(content, response.StatusCode);
            }

            return response;
        }

        /// <summary>
        /// Sends an multipart form data http request to the backend server and returns the http response.
        /// </summary>
        /// <param name="route">The url route excluding the base address.</param>
        /// <param name="data">The object to send with the request.</param>
        /// <param name="files">List of files to send with the request</param>
        /// <returns>The http response message of the request.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<HttpResponseMessage> SendMultiPartRequest(string route, Object data, FormFileCollection? files = null, string? queryString = "")
        {
            StringContent json;
            HttpResponseMessage response;

            if (data is string)
            {
                json = new StringContent((string)data);
            }
            else
            {
                var jsonObject = JsonConvert.SerializeObject(data);
                json = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(queryString))
            {
                route = $"{route}?{queryString}";
            }

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(json, "data");
            if (files != null)
            {
                foreach (var file in files)
                {
                    form.Add(new StreamContent(file.OpenReadStream()), file.Name, file.FileName);
                }
            }

            response = await _httpClient.PostAsync(route, form);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new BeimaException(content, response.StatusCode);
            }

            return response;
        }

        #region Device Requests

        /// <summary>
        /// Sends a device get request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the device.</param>
        /// <returns>The device with the given id.</returns>
        public async Task<Device> GetDevice(string id)
        {
            var response = await SendRequest($"api/device/{id}", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Device>(content);
        }

        /// <summary>
        /// Sends a device get list request to the BEIMA api.
        /// </summary>
        /// <returns>The device list.</returns>
        public async Task<List<Device>> GetDeviceList()
        {
            var response = await SendRequest("api/device-list", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Device>>(content);
        }

        /// <summary>
        /// Sends a device post request to the BEIMA api.
        /// </summary>
        /// <param name="device">The device to add.</param>
        /// <returns>The id of the new device.</returns>
        public async Task<string> AddDevice(Device device, FormFileCollection? files = null, string? queryString = "")
        {
            var response = await SendMultiPartRequest("api/device", device, files, queryString);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(content);
        }

        /// <summary>
        /// Sends a device delete request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the device to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteDevice(string id)
        {
            var response = await SendRequest($"api/device/{id}/delete", HttpVerb.POST);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a device update request to the BEIMA api.
        /// </summary>
        /// <param name="device">The device to update.</param>
        /// <returns>The id of the new device.</returns>
        public async Task<Device> UpdateDevice(DeviceUpdate device, FormFileCollection? files = null, string? queryString = "")
        {
            var response = await SendMultiPartRequest($"api/device/{device.Id}/update", device, files, queryString);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Device>(content);
        }

        #endregion Device Requests

        #region Device Type Requests

        /// <summary>
        /// Sends a device type get request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the device type.</param>
        /// <returns>The device type with the given id.</returns>
        public async Task<DeviceType> GetDeviceType(string id)
        {
            var response = await SendRequest($"api/device-type/{id}", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeviceType>(content);
        }

        /// <summary>
        /// Sends a device type get list request to the BEIMA api.
        /// </summary>
        /// <returns>The device type list.</returns>
        public async Task<List<DeviceType>> GetDeviceTypeList()
        {
            var response = await SendRequest("api/device-type-list", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DeviceType>>(content);
        }

        /// <summary>
        /// Sends a device type post request to the BEIMA api.
        /// </summary>
        /// <param name="deviceType">The device type to add.</param>
        /// <returns>The id of the new device type.</returns>
        public async Task<string> AddDeviceType(DeviceTypeAdd deviceType)
        {
            var response = await SendRequest("api/device-type", HttpVerb.POST, deviceType);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(content);
        }

        /// <summary>
        /// Sends a device type delete request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the device type to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteDeviceType(string id)
        {
            var response = await SendRequest($"api/device-type/{id}/delete", HttpVerb.POST);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a device type update request to the BEIMA api.
        /// </summary>
        /// <param name="deviceType">The device type to update.</param>
        /// <returns>The id of the new device type.</returns>
        public async Task<DeviceType> UpdateDeviceType(DeviceTypeUpdate deviceType)
        {
            var response = await SendRequest($"api/device-type/{deviceType.Id}/update", HttpVerb.POST, deviceType);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeviceType>(content);
        }

        #endregion Device Type Requests

        #region Building Requests

        /// <summary>
        /// Sends a building get request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the building.</param>
        /// <returns>The building with the given id.</returns>
        public async Task<Building> GetBuilding(string id)
        {
            var response = await SendRequest($"api/building/{id}", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Building>(content);
        }

        /// <summary>
        /// Sends a building get list request to the BEIMA api.
        /// </summary>
        /// <returns>The building list.</returns>
        public async Task<List<Building>> GetBuildingList()
        {
            var response = await SendRequest("api/building-list", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Building>>(content);
        }

        /// <summary>
        /// Sends a building post request to the BEIMA api.
        /// </summary>
        /// <param name="building">The building to add.</param>
        /// <returns>The id of the new building.</returns>
        public async Task<string> AddBuilding(Building building)
        {
            var response = await SendRequest("api/building", HttpVerb.POST, building);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(content);
        }

        /// <summary>
        /// Sends a building delete request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the building to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteBuilding(string id)
        {
            var response = await SendRequest($"api/building/{id}/delete", HttpVerb.POST);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a building update request to the BEIMA api.
        /// </summary>
        /// <param name="building">The building to update.</param>
        /// <returns>The id of the new building.</returns>
        public async Task<Building> UpdateBuilding(Building building)
        {
            var response = await SendRequest($"api/building/{building.Id}/update", HttpVerb.POST, building);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Building>(content);
        }

        #endregion Building Requests

        #region User Requests

        /// <summary>
        /// Sends a user post request to the BEIMA api.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>The id of the new user.</returns>
        public async Task<string> AddUser(User user)
        {
            var response = await SendRequest("api/user", HttpVerb.POST, user);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(content);
        }

        /// <summary>
        /// Sends a user get request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user with the given id.</returns>
        public async Task<User> GetUser(string id)
        {
            var response = await SendRequest($"api/user/{id}", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(content);
        }

        /// <summary>
        /// Sends a user get list request to the BEIMA api.
        /// </summary>
        /// <returns>The user list.</returns>
        public async Task<List<User>> GetUserList()
        {
            var response = await SendRequest("api/user-list", HttpVerb.GET);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<User>>(content);
        }

        /// <summary>
        /// Sends a user delete request to the BEIMA api.
        /// </summary>
        /// <param name="id">The id of the user to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteUser(string id)
        {
            var response = await SendRequest($"api/user/{id}/delete", HttpVerb.POST);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a user update request to the BEIMA api.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The id of the new user.</returns>
        public async Task<User> UpdateUser(User user)
        {
            var response = await SendRequest($"api/user/{user.Id}/update", HttpVerb.POST, user);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(content);
        }

        #endregion

        #region Auth Requests
        
        /// <summary>
        /// Sends a login post request to the BEIMA api.
        /// </summary>
        /// <param name="data">Object containing a user's username and password.</param>
        /// <returns>Jwt token for the user.</returns>
        public async Task<string> Login(LoginRequest data)
        {
            var response = await SendRequest("api/login", HttpVerb.POST, data);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(content);
        }
        #endregion

        /// <summary>
        /// Disposes the http client.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
