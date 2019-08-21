using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sela_Reprice
{
    /// <summary>
    /// Specify the amount of concurrent request the action can handle
    /// </summary>
    public class LimitConcurrentRequestAttribute : ActionFilterAttribute
    {
        public int Limit { get; set; }
    }

    /// <summary>
    /// A filter who handle action with limit of concurrent request
    /// </summary>
    public class LimitConcurrentRequestActionFilter : ActionFilterAttribute
    {
        private static ConcurrentDictionary<HttpActionDescriptor, int> _actionLimit =
            new ConcurrentDictionary<HttpActionDescriptor, int>();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var attributes = actionContext.ActionDescriptor.GetCustomAttributes<LimitConcurrentRequestAttribute>(true);
            if (!attributes.Any())
                return;
            var limitConcurrentRequest = attributes[0];
            if (limitConcurrentRequest.Limit == -1)
                return;

            if (!_actionLimit.ContainsKey(actionContext.ActionDescriptor))
            {
                _actionLimit.TryAdd(actionContext.ActionDescriptor, 1);
            }
            else
            {
                var currentRequest = _actionLimit[actionContext.ActionDescriptor];
                if (currentRequest >= limitConcurrentRequest.Limit)
                {

                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.ServiceUnavailable, new { message = "server is busy" },
                        actionContext.ControllerContext.Configuration.Formatters.JsonFormatter
                    );
                    return;

                }

                var previousVale = currentRequest;
                Interlocked.Increment(ref currentRequest);
                _actionLimit.TryUpdate(actionContext.ActionDescriptor, currentRequest, previousVale);
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var attributes = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<LimitConcurrentRequestAttribute>(true);
            if (!attributes.Any())
                return;
            var limitConcurrentRequest = attributes[0];
            if (limitConcurrentRequest.Limit == -1)
                return;

            var currentRequest = _actionLimit[actionExecutedContext.ActionContext.ActionDescriptor];

            var previousVale = currentRequest;
            Interlocked.Decrement(ref currentRequest);
            _actionLimit.TryUpdate(actionExecutedContext.ActionContext.ActionDescriptor, currentRequest, previousVale);

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}