using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WHS.Infrastructure.FluentHttpClient.Internal
{
    /// <summary>Asynchronously parses an HTTP response.</summary>
    public sealed class Response : IResponse
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether the HTTP response was successful.</summary>
        public bool IsSuccessStatusCode => this.Message.IsSuccessStatusCode;

        /// <summary>The underlying HTTP response message.</summary>
        public HttpResponseMessage Message { get; }

        /// <summary>The formatters used for serializing and deserializing message bodies.</summary>
        public MediaTypeFormatterCollection Formatters { get; }

        /// <summary>The HTTP status code.</summary>
        public HttpStatusCode Status => this.Message.StatusCode;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="message">The underlying HTTP request message.</param>
        /// <param name="formatters">The formatters used for serializing and deserializing message bodies.</param>
        public Response(HttpResponseMessage message, MediaTypeFormatterCollection formatters)
        {
            this.Message = message;
            this.Formatters = formatters;
        }

        /// <summary>Asynchronously retrieve the response body as a deserialized model.</summary>
        /// <typeparam name="T">The response model to deserialize into.</typeparam>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public Task<T> As<T>()
        {
            return this.Message.Content.ReadAsAsync<T>(this.Formatters);
        }

        /// <summary>Asynchronously retrieve the response body as a list of deserialized models.</summary>
        /// <typeparam name="T">The response model to deserialize into.</typeparam>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public Task<T[]> AsArray<T>()
        {
            return this.As<T[]>();
        }

        /// <summary>Asynchronously retrieve the response body as an array of <see cref="byte"/>.</summary>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public Task<byte[]> AsByteArray()
        {
            return this.AssertContent().ReadAsByteArrayAsync();
        }

        /// <summary>Asynchronously retrieve the response body as a <see cref="string"/>.</summary>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public Task<string> AsString()
        {
            return this.AssertContent().ReadAsStringAsync();
        }

        /// <summary>Asynchronously retrieve the response body as a <see cref="Stream"/>.</summary>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public async Task<Stream> AsStream()
        {
            Stream stream = await this.AssertContent().ReadAsStreamAsync().ConfigureAwait(false);
            stream.Position = 0;
            return stream;
        }

        /// <summary>Get a raw JSON representation of the response, which can also be accessed as a <c>dynamic</c> value.</summary>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public async Task<JToken> AsRawJson()
        {
            string content = await this.AsString();
            return JToken.Parse(content);
        }

        /// <summary>Get a raw JSON object representation of the response, which can also be accessed as a <c>dynamic</c> value.</summary>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public async Task<JObject> AsRawJsonObject()
        {
            string content = await this.AsString();
            return JObject.Parse(content);
        }

        /// <summary>Get a raw JSON array representation of the response, which can also be accessed as a <c>dynamic</c> value.</summary>
        /// <exception cref="ApiException">An error occurred processing the response.</exception>
        public async Task<JArray> AsRawJsonArray()
        {
            string content = await this.AsString();
            return JArray.Parse(content);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Assert that the response has a body.</summary>
        /// <exception cref="NullReferenceException">The response has no response body to read.</exception>
        private HttpContent AssertContent()
        {
            return this.Message?.Content ?? throw new NullReferenceException("The response has no body to read.");
        }
    }
}
