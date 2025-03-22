using CoderamaInterview.Serialization;
using MessagePack;

namespace CoderamaInterviewTests.Serialization
{
    public class MessagePackDocumentSerializerTests
    {
        private readonly MessagePackDocumentSerializer _serializer;

        public MessagePackDocumentSerializerTests()
        {
            _serializer = new MessagePackDocumentSerializer();
        }

        /// <summary>
        /// Helper method to create a valid Document instance.
        /// Ensure that the Document type is decorated with MessagePack attributes.
        /// </summary>
        private Document CreateTestDocument()
        {
            return new Document
            {
                Id = "doc1",
                Tags = ["tag1", "tag2"],
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
        public void Serialize_ReturnsValidBase64String_ForValidDocument()
        {
            // Arrange
            var document = CreateTestDocument();

            // Act
            string base64String = _serializer.Serialize(document);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(base64String));
            Assert.True(IsValidBase64(base64String));
        }

        [Fact]
        public void Deserialize_ReturnsDocument_WithSameValues()
        {
            // Arrange
            var originalDoc = CreateTestDocument();
            string base64String = _serializer.Serialize(originalDoc);

            // Act
            var deserialized = _serializer.Deserialize(base64String);

            // Assert
            Assert.IsType<Document>(deserialized);
            var doc = (Document)deserialized;
            Assert.Equal(originalDoc.Id, doc.Id);
            Assert.Equal(originalDoc.Tags, doc.Tags);

            // Compare the Data property by normalizing its JSON via System.Text.Json.JsonSerializer.
            string originalJson = System.Text.Json.JsonSerializer.Serialize(originalDoc.Data);
            string deserializedJson = System.Text.Json.JsonSerializer.Serialize(doc.Data);
            Assert.Equal(originalJson, deserializedJson);
        }

        [Fact]
        public void Serialize_ThrowsInvalidOperationException_ForNonDocumentType()
        {
            // Arrange: Create a dummy implementation of IDocument that is NOT a Document.
            var invalidDoc = new DummyDocument
            {
                Id = "dummy",
                Tags = new List<string> { "dummy" },
                Data = JsonDocument.Parse("{\"dummy\":\"data\"}").RootElement
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _serializer.Serialize(invalidDoc));
            Assert.Equal("Invalid document type for MessagePack serialization.", ex.Message);
        }

        [Fact]
        public void Deserialize_ThrowsFormatException_ForInvalidBase64()
        {
            // Arrange
            string invalidBase64 = "NotAValidBase64String!";

            // Act & Assert
            Assert.Throws<FormatException>(() => _serializer.Deserialize(invalidBase64));
        }

        [Fact]
        public void Deserialize_ThrowsMessagePackSerializationException_ForInvalidMessagePack()
        {
            // Arrange: Create a valid Base64 string from random bytes that do not represent valid MessagePack data.
            byte[] randomBytes = new byte[] { 0x00, 0x01, 0x02 };
            string invalidPayload = Convert.ToBase64String(randomBytes);

            // Act & Assert
            Assert.Throws<MessagePackSerializationException>(() => _serializer.Deserialize(invalidPayload));
        }

        /// <summary>
        /// Helper method to check if a string is valid Base64.
        /// </summary>
        private bool IsValidBase64(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        /// <summary>
        /// Dummy document class implementing IDocument but not a Document.
        /// </summary>
        private class DummyDocument : IDocument
        {
            public string Id { get; set; }
            public List<string> Tags { get; set; }
            public JsonElement Data { get; set; }
        }
    }
}
