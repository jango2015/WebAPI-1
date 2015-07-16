

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPICommon
{
    public class BearerTokenClientMessageHandler : DelegatingHandler
    {
        private string _bearerToken;
        public BearerTokenClientMessageHandler(string bearerToken, HttpMessageHandler innerHandler)
        {
            _bearerToken = bearerToken;
            InnerHandler = innerHandler;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// 
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param><param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>. The task object representing the asynchronous operation.
        /// 
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
