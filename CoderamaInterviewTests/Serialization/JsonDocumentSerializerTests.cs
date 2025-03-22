using CoderamaInterview.Serialization;

namespace CoderamaInterviewTests.Serialization;

public class JsonDocumentSerializerTests
{
    private readonly JsonDocumentSerializer _serializer;

    public JsonDocumentSerializerTests()
    {
        _serializer = new JsonDocumentSerializer();
    }

    /// <summary>
    /// Helper method to create a test Document.
    /// Assumes Document implements IDocument.
    /// </summary>
    private Document CreateTestDocument()
    {
        // For demonstration purposes, we create a Document with some data.
        return new Document
        {
            Id = "doc1",
            Tags = new List<string> { "tag1", "tag2" },
            // Using a meaningful JSON for the Data property.
            Data = JsonDocument.Parse("""
                                      {
                                          "name": "John Doe",
                                          "age": 30,
                                          "skills": ["C#", "ASP.NET Core", "Azure"]
                                      }
                                      """).RootElement
        };
    }

    [Fact]
    public void Serialize_ReturnsValidJson_ForValidDocument()
    {
        // Arrange
        var document = CreateTestDocument();

        // Act
        string json = _serializer.Serialize(document);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(json));
        Assert.Contains("doc1", json);
        Assert.Contains("tag1", json);
        // Optionally, check that the JSON starts with '{' or contains expected property names.
        Assert.StartsWith("{", json.Trim());
    }

    [Fact]
    public void Deserialize_ReturnsDocument_WithSameValues()
    {
        // Arrange
        var originalDoc = CreateTestDocument();
        string json = _serializer.Serialize(originalDoc);

        // Act
        IDocument deserialized = _serializer.Deserialize(json);

        // Assert
        Assert.NotNull(deserialized);
        // Since we expect the deserialized object to be of concrete type Document,
        // we cast it and compare its properties.
        var doc = deserialized as Document;
        Assert.NotNull(doc);
        Assert.Equal(originalDoc.Id, doc!.Id);
        Assert.Equal(originalDoc.Tags, doc.Tags);

        // Compare the Data property by normalizing the JSON.
        string originalData = JsonSerializer.Serialize(originalDoc.Data);
        string deserializedData = JsonSerializer.Serialize(doc.Data);
        Assert.Equal(originalData, deserializedData);
    }

    [Fact]
    public void Deserialize_ThrowsJsonException_ForInvalidJson()
    {
        // Arrange
        string invalidJson = "not valid json";

        // Act & Assert
        Assert.Throws<JsonException>(() => _serializer.Deserialize(invalidJson));
    }
}