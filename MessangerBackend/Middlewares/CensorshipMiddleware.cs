using System.Text.Json;
using MessangerBackend.DTOs;
using MessangerBackend.Storage;

namespace MessangerBackend.Middlewares;

public class CensorshipMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _bannedWords = { "russia", "war" };

    public CensorshipMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext ctx)
    {
        if (ctx.Request.Path.StartsWithSegments("/api/messages/sendmessage") && ctx.Request.Method == "POST")
        {
            ctx.Request.EnableBuffering();

            var requestBody = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
            ctx.Request.Body.Position = 0;

            var messageDto = JsonSerializer.Deserialize<MessageDTO>(requestBody);

            foreach (var word in _bannedWords)
            {
                messageDto.Text = messageDto.Text.Replace(word, "***", StringComparison.OrdinalIgnoreCase);
            }

            var updatedRequestBody = JsonSerializer.Serialize(messageDto);
            ctx.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(updatedRequestBody));
        }

        await _next(ctx);
    }
}