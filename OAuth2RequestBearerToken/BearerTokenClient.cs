using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OAuth2RequestBearerToken
{
    public class BearerTokenClient
    {
        private HttpClient _httpClient;

        public async Task<OAuth2TokenResponse> GetBearerToken(OAuth2Config config)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            headers.Add(new KeyValuePair<string, string>("client_id", config.client_id));
            headers.Add(new KeyValuePair<string, string>("client_secret", config.client_secret));
            headers.Add(new KeyValuePair<string, string>("resource", config.resource));
            
            _httpClient = new HttpClient();

            HttpContent content = new FormUrlEncodedContent(headers);
            
            var response = _httpClient.PostAsync(config.url, content);

            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                var responseContentAsString = await response.Result.Content.ReadAsStringAsync();
                var ob = JsonConvert.DeserializeObject<OAuth2TokenResponse>(responseContentAsString);
                return ob;
            }

            return new OAuth2TokenResponse();

        }
    }
}