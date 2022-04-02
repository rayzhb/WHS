using System;
using System.Net.Http;

namespace WHS.Infrastructure.FluentHttpClient.Retry
{
    /// <summary>Configures a request retry strategy.</summary>
    public class RetryConfig : IRetryConfig
    {
        /*********
        ** Fields
        *********/
        /// <summary>A method which indicates whether a request should be retried.</summary>
        private Func<HttpResponseMessage, bool> ShouldRetryCallback { get; }

        /// <summary>A method which returns the time to wait until the next retry. This is passed the retry index (starting at 1) and the last HTTP response received.</summary>
        private Func<int, HttpResponseMessage, TimeSpan> GetDelayCallback { get; }


        /*********
        ** Accessors
        *********/
        /// <summary>The maximum number of times to retry a request before failing.</summary>
        public int MaxRetries { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Initializes a new instance of the <see cref="RetryConfig"/> class.</summary>
        /// <param name="maxRetries">The maximum number of retries.</param>
        /// <param name="shouldRetry">A method which indicates whether a request should be retried.</param>
        /// <param name="getDelay">A method which returns the time to wait until the next retry. This is passed the retry index (starting at 1) and the last HTTP response received.</param>
        public RetryConfig(int maxRetries, Func<HttpResponseMessage, bool> shouldRetry, Func<int, HttpResponseMessage, TimeSpan> getDelay)
        {
            this.MaxRetries = maxRetries;
            this.ShouldRetryCallback = shouldRetry;
            this.GetDelayCallback = getDelay;
        }

        /// <summary>Get whether a request should be retried.</summary>
        /// <param name="response">The last HTTP response received.</param>
        public bool ShouldRetry(HttpResponseMessage response)
        {
            return this.ShouldRetryCallback(response);
        }

        /// <summary>Get the time to wait until the next retry.</summary>
        /// <param name="retry">The retry index (starting at 1).</param>
        /// <param name="response">The last HTTP response received.</param>
        public TimeSpan GetDelay(int retry, HttpResponseMessage response)
        {
            return this.GetDelayCallback(retry, response);
        }

        /// <summary>Get a retry config indicating no request should be retried.</summary>
        public static IRetryConfig None()
        {
            return new RetryConfig(
                maxRetries: 0,
                shouldRetry: response => false,
                getDelay: (attempt, response) => TimeSpan.Zero
            );
        }
    }
}
