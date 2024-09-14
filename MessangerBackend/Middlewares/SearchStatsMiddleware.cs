using System.Net;
using MessangerBackend.Core.Models;
using MessangerBackend.Storage;

namespace MessangerBackend.Middlewares;

public class SearchStatsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MessangerContext _context;

    public SearchStatsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext ctx)
    {
        if (ctx.Request.Path.StartsWithSegments("/api/users") && ctx.Request.Method == "GET")
        {
            var query = ctx.GetRouteData().DataTokens["name"];

            if (!string.IsNullOrEmpty((string)query))
            {
                var searchStatistic = _context.SearchStatistics.First(s => s.Query == query);

                if (searchStatistic != null)
                {

                    searchStatistic.Count++;
                }
                else
                {
                    searchStatistic = new SearchStatistic
                    {
                        Query = (string)query,
                        Count = 1
                    };
                    _context.SearchStatistics.Add(searchStatistic);
                }
                await _context.SaveChangesAsync();
                await _next(ctx);
            }
        }
    }
}
    
    
