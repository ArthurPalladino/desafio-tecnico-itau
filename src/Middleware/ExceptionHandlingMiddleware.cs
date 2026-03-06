using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var response = new 
            { 
                erro = ex.Message, 
                codigo = ex.Code 
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                codigo = "ERRO_INTERNO",
                mensagem = ex.Message,
                detalhes = ex.InnerException?.Message,
                stackTrace = ex.StackTrace,
                origem = ex.Source
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            await context.Response.WriteAsJsonAsync(response, options);
        }
    }
}