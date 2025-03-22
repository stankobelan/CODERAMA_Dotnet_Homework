using System.Text.Json;
using CoderamaInterview.Models;

namespace CoderamaInterview.Serialization;

public class JsonDocumentSerializer : IDocumentSerializer
{
    public string ContentType => "application/json";
    public string FileExtension => ".json";
    public string Serialize(IDocument document) =>
        JsonSerializer.Serialize(document);

    public IDocument Deserialize(string payload) =>
        JsonSerializer.Deserialize<Document>(payload)!;
}