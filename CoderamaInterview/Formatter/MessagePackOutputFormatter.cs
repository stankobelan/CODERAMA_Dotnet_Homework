using MessagePack;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace CoderamaInterview.Formatter;

public class MessagePackOutputFormatter : OutputFormatter
{
    public MessagePackOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-msgpack"));
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var response = context.HttpContext.Response;
        byte[] result = MessagePackSerializer.Serialize(context.Object, MessagePackSerializerOptions.Standard);
        await response.Body.WriteAsync(result);
    }
}