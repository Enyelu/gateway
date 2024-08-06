using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace gateway.api.Middlewares
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    if (contextFeature != null)
                    {
                        Log.Error($"ExceptionFailure: {JsonConvert.SerializeObject(contextFeature.Error)}.");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            IsSuccessful = false,
                            Message = "Oops! An error occurred while processing your request. If this persists after another trial, Kindly contact your administrator.",
                            StatusCode = 500
                        }));
                    }
                });
            });
        }
    }
}
