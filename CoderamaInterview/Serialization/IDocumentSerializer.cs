using CoderamaInterview.Models;

namespace CoderamaInterview.Serialization;

public interface IDocumentSerializer
{
    string ContentType { get; } // Napr. "application/json", "application/xml", "application/x-msgpack"
    string FileExtension { get; }  // Napr. ".json", ".xml", ".msgpack"
    string Serialize(IDocument document);
    IDocument Deserialize(string payload);
}