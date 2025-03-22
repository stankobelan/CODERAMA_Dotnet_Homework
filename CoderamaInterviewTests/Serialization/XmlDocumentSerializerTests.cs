using CoderamaInterview.Serialization;

namespace CoderamaInterviewTests.Serialization;

public class XmlDocumentSerializerTests
{
    private readonly XmlDocumentSerializer _serializer;

    public XmlDocumentSerializerTests()
    {
        _serializer = new XmlDocumentSerializer();
    }

    /// <summary>
    /// Creates a valid Document instance for testing.
    /// </summary>
    private Document CreateValidDocument()
    {
        return new Document
        {
            Id = "doc1",
            Tags = ["tag1", "tag2"],
            // Note: XmlSerializer does not naturally handle JsonElement.
            // For testing, we assume that the document's Data property is set to a valid JsonElement.
            Data = JsonDocument.Parse("""
                                      {
                                        "name": "John Doe",
                                        "age": 30,
                                        "skills": ["C#", "ASP.NET Core", "Azure"],
                                        "address": {
                                          "street": "123 Main St",
                                          "city": "Metropolis",
                                          "zip": "12345"
                                        }
                                      }
                                      """).RootElement
        };
    }

    [Fact]
    public void Serialize_ReturnsXmlString_ForValidDocument()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act
        string xml = _serializer.Serialize(document);

        // Assert: Check that the resulting XML contains expected content.
        Assert.Contains("doc1", xml);
        Assert.Contains("tag1", xml);
        // Optionally, you can check for XML declaration or root element names if known.
    }

    [Fact]
    public void Deserialize_ReturnsDocument_WithSameValues()
    {
        // Arrange
        var document = CreateValidDocument();
        string xml = _serializer.Serialize(document);

        // Act
        var deserialized = _serializer.Deserialize(xml);

        // Assert
        Assert.IsType<Document>(deserialized);
        var deserializedDoc = (Document)deserialized;
        Assert.Equal(document.Id, deserializedDoc.Id);
        Assert.Equal(document.Tags.Count, deserializedDoc.Tags.Count);
        Assert.Equal(document.Tags[0], deserializedDoc.Tags[0]);

        // use JsonSerializer.Serialize() to obtain a normalized JSON string for comparison.
        string originalJson = JsonSerializer.Serialize(document.Data);
        string deserializedJson = JsonSerializer.Serialize(deserializedDoc.Data);
        Assert.Equal(originalJson, deserializedJson);
    }

    [Fact]
    public void Serialize_ThrowsException_ForInvalidDocumentType()
    {
        // Arrange: Create a dummy implementation of IDocument that is not a Document.
        var invalidDoc = new DummyDocument
        {
            Id = "dummy",
            Tags = new List<string> { "tag" },
            Data = JsonDocument.Parse("{\"dummy\":\"data\"}").RootElement
        };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _serializer.Serialize(invalidDoc));
        Assert.Equal("Invalid document type for XML serialization.", ex.Message);
    }

    [Fact]
    public void Deserialize_ThrowsException_ForInvalidXml()
    {
        // Arrange: Provide an invalid XML string.
        string invalidXml = "not valid xml";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _serializer.Deserialize(invalidXml));
    }

    // Dummy document class implementing IDocument but not derived from Document.
    private class DummyDocument : IDocument
    {
        public string Id { get; set; }
        public List<string> Tags { get; set; }
        public JsonElement Data { get; set; }
    }
}