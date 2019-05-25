using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OAuth2RequestBearerToken
{
    public class BearerTokenClient
    {
        private HttpClient _httpClient;

        public async Task<OAuth2TokenResponse> GetBearerToken(OAuth2Config config)
        {
            _httpClient = new HttpClient();
            IEnumerable<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", config.client_id),
                new KeyValuePair<string, string>("client_secret", config.client_secret),
                new KeyValuePair<string, string>("resource", config.resource)
            };

            HttpContent content = new FormUrlEncodedContent(headers);
            CancellationToken cancellationToken = new CancellationToken();

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(config.url)
            };

            var response = _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var stream = await response.Result.Content.ReadAsStreamAsync();
            if (response.Result.IsSuccessStatusCode)
                return DeserializeJsonFromStream<OAuth2TokenResponse>(stream);
            
            // Throw exception if deserialization fails.
            var streamContent = await StreamToStringAsync(stream);
            throw new SerializationException(streamContent);
        }
        
        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default;

            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var jsonSerializer = new JsonSerializer();
                var searchResult = jsonSerializer.Deserialize<T>(jsonTextReader);
                return searchResult;
            }
        }
        
        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var streamReader = new StreamReader(stream))
                    content = await streamReader.ReadToEndAsync();

            return content;
        }
    }
}