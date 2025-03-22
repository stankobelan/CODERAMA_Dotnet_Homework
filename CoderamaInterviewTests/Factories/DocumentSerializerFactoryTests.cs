using System;
using System.Collections.Generic;
using CoderamaInterview.Factories;
using CoderamaInterview.Serialization;
using Xunit;

namespace CoderamaInterviewTests.Factories
{
    // Dummy implementations for testing
    public class DummyJsonSerializer : IDocumentSerializer
    {
        public string ContentType => "application/json";
        public string FileExtension => ".json";
        public string Serialize(IDocument document) => "json";
        public IDocument Deserialize(string payload) => null;
    }

    public class DummyXmlSerializer : IDocumentSerializer
    {
        public string ContentType => "application/xml";
        public string FileExtension => ".xml";
        public string Serialize(IDocument document) => "xml";
        public IDocument Deserialize(string payload) => null;
    }

    public class DocumentSerializerFactoryTests
    {
        private List<IDocumentSerializer> CreateSerializers(bool includeJson = true)
        {
            var serializers = new List<IDocumentSerializer>();
            if (includeJson)
            {
                serializers.Add(new DummyJsonSerializer());
            }
            serializers.Add(new DummyXmlSerializer());
            return serializers;
        }

        [Theory]
        [InlineData("application/json")]
        [InlineData("APPLICATION/JSON")]
        [InlineData("Application/Json")]
        public void GetSerializer_ReturnsJsonSerializer_ForJsonAcceptHeader(string acceptHeader)
        {
            // Arrange
            var serializers = CreateSerializers();
            var factory = new DocumentSerializerFactory(serializers);

            // Act
            var result = factory.GetSerializer(acceptHeader);

            // Assert
            Assert.IsType<DummyJsonSerializer>(result);
        }

        [Theory]
        [InlineData("application/xml")]
        [InlineData("APPLICATION/XML")]
        public void GetSerializer_ReturnsXmlSerializer_ForXmlAcceptHeader(string acceptHeader)
        {
            // Arrange
            var serializers = CreateSerializers();
            var factory = new DocumentSerializerFactory(serializers);

            // Act
            var result = factory.GetSerializer(acceptHeader);

            // Assert
            Assert.IsType<DummyXmlSerializer>(result);
        }

        [Theory]
        [InlineData("application/unknown")]
        [InlineData("")]
        [InlineData(null)]
        public void GetSerializer_ReturnsDefaultJsonSerializer_WhenAcceptHeaderDoesNotMatch(string acceptHeader)
        {
            // Arrange
            // Even if acceptHeader is null or empty, the factory should return the default JSON serializer.
            var serializers = CreateSerializers();
            var factory = new DocumentSerializerFactory(serializers);

            // Act
            var result = factory.GetSerializer(acceptHeader);

            // Assert: since no serializer matches the given header, the default JSON serializer is returned.
            Assert.IsType<DummyJsonSerializer>(result);
        }

        [Fact]
        public void GetSerializer_ThrowsException_WhenDefaultJsonSerializerIsMissing()
        {
            // Arrange: Only an XML serializer is provided, so there is no default JSON serializer.
            var serializers = CreateSerializers(includeJson: false);
            var factory = new DocumentSerializerFactory(serializers);

            // Act & Assert:
            Assert.Throws<InvalidOperationException>(() => factory.GetSerializer("application/unknown"));
        }
    }
}
