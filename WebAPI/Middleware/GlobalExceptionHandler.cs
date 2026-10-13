using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;

namespace WebAPI.Middleware
{
    /// <summary>
    /// Globalni hvatac gresaka - ASP.NET Core ga automatski pozove kad GOD bilo koji
    /// kontroler baci neuhvacen izuzetak. Ne treba try/catch po svakoj akciji pojedinacno -
    /// dovoljno je registrovati OVO jednom (Program.cs), vazi za SVE trenutne i buduce
    /// kontrolere.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ScoreHubDbContext _context;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ScoreHubDbContext context, ILogger<GlobalExceptionHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

            try
            {
                _context.Logs.Add(new Log
                {
                    Exception = exception.Message,
                    InnerException = exception.InnerException?.Message,
                    StackTrace = exception.StackTrace,
                    HttpMethod = httpContext.Request.Method,
                    HttpUrl = httpContext.Request.Path + httpContext.Request.QueryString,
                    StatusCode = StatusCodes.Status500InternalServerError
                });

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception logEx)
            {
                // Ako ni upis loga ne uspe, bar u konzoli ostavi trag.
                _logger.LogCritical(logEx, "Failed to write error log to database.");
            }

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new
            {
                message = "Doslo je do neocekivane greske. Pokusaj ponovo kasnije."
            }, cancellationToken);

            return true; // "obradjeno" - sprecava dalje prosledjivanje ka default ASP.NET handleru
        }
    }
}
