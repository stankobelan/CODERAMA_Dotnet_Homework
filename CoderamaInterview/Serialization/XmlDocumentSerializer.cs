using System.Xml.Serialization;
using CoderamaInterview.Models;
namespace CoderamaInterview.Serialization;
public class XmlDocumentSerializer : IDocumentSerializer
{
    public string ContentType => "application/xml";
    public string FileExtension => ".xml";

    // Cache a single XmlSerializer instance for the concrete Document type.
    private readonly XmlSerializer _serializer = new(typeof(Document));

    public string Serialize(IDocument document)
    {
        // Ensure the document is of the expected concrete type.
        if (!(document is Document concreteDocument))
            throw new InvalidOperationException("Invalid document type for XML serialization.");

        using var stringWriter = new StringWriter();
        _serializer.Serialize(stringWriter, concreteDocument);
        return stringWriter.ToString();
    }

    public IDocument Deserialize(string payload)
    {
        using var stringReader = new StringReader(payload);
        var result = _serializer.Deserialize(stringReader);
        if (result is Document concreteDocument)
            return concreteDocument;

        throw new InvalidOperationException("Deserialization failed.");
    }
}