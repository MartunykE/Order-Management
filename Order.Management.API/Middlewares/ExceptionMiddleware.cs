
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace OrderManagement.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (OrderManagement.Application.Exceptions.ApplicationException e)
            {
                logger.LogError(e, e.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;

                string response = JsonSerializer.Serialize(
                    new
                    {
                        Message = e.Message,
                    });

                await httpContext.Response.WriteAsync(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

        }
    }
}
