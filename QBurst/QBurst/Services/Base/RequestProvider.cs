using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using QBurst.Exceptions;
using QBurst.Navigation;
using QBurst.ViewModels.Base;
using QBurst.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
 using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QBurst.Services.Base
{
    public class RequestProvider : IRequestProvider
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public object Settings { get; private set; }

        public RequestProvider()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public async Task<TResult> GetAsync<TResult>(string uri)
        {
            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();
            TResult result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
        }

        public Task<TResult> PostAsync<TResult>(string uri, TResult data)
        {
            return PostAsync<TResult, TResult>(uri, data);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data)
        {

            INavigationService NavigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            IDialogService DialogService = ViewModelLocator.Instance.Resolve<IDialogService>();
            //if (CrossConnectivity.Current.IsConnected)
            //{

                try
                {
                    HttpClient httpClient = CreateHttpClient();

                    string serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings));

                    HttpResponseMessage response = await httpClient.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
                    await HandleResponse(response);


                    string responseData = await response.Content.ReadAsStringAsync();

                    return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));

                }

                catch (Exception e) when (e is AuthenticationException)
                {
                    var error = e as AuthenticationException;

                    var content = error.Content;
                    if (content.Contains("AUTH_EXCEPTION"))
                    {
                        
                         Device.BeginInvokeOnMainThread(() => DialogService.ShowAlertAsync("Session Expired", "Please Login to continue", "OK"));
                        Device.BeginInvokeOnMainThread(() => NavigationService.NavigateToAsync<ILoginVM>());
                    }
                    var errorString = await errorApiDataString("Auth_Exception");
                    return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(errorString, _serializerSettings));
                }
                catch (Exception ex)
                {
                    var errorString = await errorApiDataString();
                    return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(errorString, _serializerSettings));
                }
            //}
            //else
            //{
            //    var errorString = await errorApiDataString();

            //    return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(errorString, _serializerSettings));
            //}

        }
        private async Task<string> errorApiDataString(string type = "")
        {
            //ApiResponse<object> errorData = new ApiResponse<object>();

            //errorData = new ApiResponse<object>();
            //errorData.Data = null;
            //errorData.HasError = true;
            //if (string.IsNullOrEmpty(type))

            //    errorData.ErrorMessage = "Please try after sometime";
            //else
            //    errorData.ErrorMessage = "Auth_Exception";
            return await Task.Run(() => JsonConvert.SerializeObject("", _serializerSettings));

        }


        public Task<TResult> PutAsync<TResult>(string uri, TResult data)
        {
            return PutAsync<TResult, TResult>(uri, data);
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data)
        {
            HttpClient httpClient = CreateHttpClient();
            string serialized = await Task.Run(() => JsonConvert.SerializeObject(data, _serializerSettings));
            HttpResponseMessage response = await httpClient.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));


            await HandleResponse(response);

            string responseData = await response.Content.ReadAsStringAsync();

            return await Task.Run(() => JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings));
        }

        private HttpClient CreateHttpClient()
        {
            //if (!CrossConnectivity.Current.IsConnected)
            //{
            //    App.Current.MainPage.DisplayAlert(Constants.noInternet, Constants.noInternetMsg, Constants.okMsg);
            //}
            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 3, 0);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            //if (!string.IsNullOrEmpty(Settings.AccessToken))
            //    httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Settings.AccessToken);

            return httpClient;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException(content);
                }

                throw new HttpRequestException(content);
            }
        }
    }
}
