using System.Diagnostics.CodeAnalysis;
using System.Net;
using DocuSign.eSign.Client;
using DocuSign.MyBusiness.Infrustructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DocuSign.MyBusiness.Infrustructure.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DocuSignWebAppExtensions
    {
        public static void ConfigureDocuSignExceptionHandling(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is ApiException apiError)
                        {
                            logger.LogError($"Error occured during Docusign api call: {contextFeature.Error}");

                            if (apiError.ErrorCode == (int)HttpStatusCode.Unauthorized)
                            {
                                context.Session.Clear();
                                await context.SignOutAsync();
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            }
                        }
                        else
                        {
                            logger.LogError($"Error occured: {contextFeature.Error}");
                        }
                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }
    }
}
