using System;
using System.Net;
using System.Net.Http;

namespace WHS.Infrastructure.FluentHttpClient
{
    /// <summary>Represents an error returned by the upstream server.</summary>
    public class ApiException : Exception
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The HTTP status of the response.</summary>
        public HttpStatusCode Status { get; }

        /// <summary>The HTTP response which caused the exception.</summary>
        public IResponse Response { get; }

        /// <summary>The HTTP response message which caused the exception.</summary>
        public HttpResponseMessage ResponseMessage { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="response">The HTTP response which caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception (or <c>null</c> for no inner exception).</param>
        public ApiException(IResponse response, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            this.Response = response;
            this.ResponseMessage = response.Message;
            this.Status = response.Message.StatusCode;
        }
    }
}
