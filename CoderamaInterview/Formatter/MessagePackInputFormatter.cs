using MessagePack;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
namespace CoderamaInterview.Formatter;

public class MessagePackInputFormatter : InputFormatter
{
    public MessagePackInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-msgpack"));
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        using var memoryStream = new MemoryStream();
        await request.Body.CopyToAsync(memoryStream);
        byte[] data = memoryStream.ToArray();
        try
        {
            var result = MessagePackSerializer.Deserialize(context.ModelType, data, MessagePackSerializerOptions.Standard);
            return await InputFormatterResult.SuccessAsync(result);
        }
        catch (Exception)
        {
            return await InputFormatterResult.FailureAsync();
        }
    }
}