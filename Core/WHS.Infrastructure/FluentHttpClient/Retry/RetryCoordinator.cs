using Polly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WHS.Infrastructure.FluentHttpClient.Internal;

namespace WHS.Infrastructure.FluentHttpClient.Retry
{
    /// <summary>A request coordinator which retries failed requests with a delay between each attempt.</summary>
    public class RetryCoordinator : IRequestCoordinator
    {
        /*********
        ** Fields
        *********/
        /// <summary>The retry configuration.</summary>
        private readonly IRetryConfig Config;

        /// <summary>The status code representing a request timeout.</summary>
        /// <remarks>HTTP 598 Network Read Timeout is the closest match, though it's non-standard so there's no <see cref="HttpStatusCode"/> constant. This is needed to avoid passing <c>null</c> into <see cref="IRetryConfig.ShouldRetry"/>, which isn't intuitive and would cause errors.</remarks>
        private readonly HttpStatusCode TimeoutStatusCode = (HttpStatusCode)589;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="shouldRetry">A method which returns whether a request should be retried.</param>
        /// <param name="intervals">The intervals between each retry attempt.</param>
        public RetryCoordinator(Func<HttpResponseMessage, bool> shouldRetry, params TimeSpan[] intervals)
            : this(new RetryConfig(intervals.Length, shouldRetry, (attempts, response) => intervals[attempts - 1])) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="maxRetries">The maximum number of times to retry a request before failing.</param>
        /// <param name="shouldRetry">A method which returns whether a request should be retried.</param>
        /// <param name="getDelay">A method which returns the time to wait until the next retry. This is passed the retry index (starting at 1) and the last HTTP response received.</param>
        public RetryCoordinator(int maxRetries, Func<HttpResponseMessage, bool> shouldRetry, Func<int, HttpResponseMessage, TimeSpan> getDelay)
           : this(new RetryConfig(maxRetries, shouldRetry, getDelay)) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="config">The retry configuration.</param>
        public RetryCoordinator(IRetryConfig? config)
        {
            this.Config = config ?? RetryConfig.None();
        }

        public Task<HttpResponseMessage> ExecuteAsync(IRequest request, Func<IRequest, Task<HttpResponseMessage>> dispatcher)
        {
            HttpStatusCode[] retryCodes = { HttpStatusCode.GatewayTimeout, HttpStatusCode.RequestTimeout };
            return Policy
               .HandleResult<HttpResponseMessage>(response =>
               {
                   return (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout;
               }) // should we retry?
               .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt)) // up to 3 retries with increasing delay
               .ExecuteAsync(() => dispatcher(request)); // begin handling request
        }

        //    public async Task<HttpResponseMessage> ExecuteAsync(IRequest request, Func<IRequest, Task<HttpResponseMessage>> dispatcher)
        //    {
        //        int attempt = 0;
        //        int maxAttempt = 1 + this.Config.MaxRetries;
        //        while (true)
        //        {
        //            // dispatch request
        //            attempt++;
        //            HttpResponseMessage response;
        //            try
        //            {
        //                response = await dispatcher(request).ConfigureAwait(false);
        //            }
        //            catch (TaskCanceledException) when (!request.CancellationToken.IsCancellationRequested)
        //            {
        //                response = request.Message.CreateResponse(this.TimeoutStatusCode);
        //            }

        //            // exit if done
        //            if (!this.Config.ShouldRetry(response))
        //                return response;
        //            if (attempt >= maxAttempt)
        //                throw new ApiException(new Response(response, request.Formatters), $"The HTTP request {(response != null ? "failed" : "timed out")}, and the retry coordinator gave up after the maximum {this.Config.MaxRetries} retries");

        //            // set up retry
        //            TimeSpan delay = this.Config.GetDelay(attempt, response);
        //            if (delay.TotalMilliseconds > 0)
        //                await Task.Delay(delay).ConfigureAwait(false);
        //        }
        //    }
    }
}
