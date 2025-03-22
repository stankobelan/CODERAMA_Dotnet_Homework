using CoderamaInterview.Factories;
using CoderamaInterview.Repositories;
using CoderamaInterview.Serialization;

namespace CoderamaInterviewTests.Services;

public class DocumentServiceTests
    {
        private readonly Mock<IDocumentRepository> _repositoryMock;
        private readonly Mock<IDocumentSerializerFactory> _serializerFactoryMock;
        private readonly Mock<IDocumentSerializer> _serializerMock;
        private readonly DocumentService _documentService;

        public DocumentServiceTests()
        {
            _repositoryMock = new Mock<IDocumentRepository>();
            _serializerFactoryMock = new Mock<IDocumentSerializerFactory>();
            _serializerMock = new Mock<IDocumentSerializer>();

            // Setup the factory to always return our mocked serializer.
            _serializerFactoryMock.Setup(f => f.GetSerializer(It.IsAny<string>()))
                                  .Returns(_serializerMock.Object);

            _documentService = new DocumentService(_repositoryMock.Object, _serializerFactoryMock.Object);
        }

        /// <summary>
        /// Helper method to create a test document.
        /// </summary>
        private Document CreateTestDocument(string id = "doc1", string json = "{\"key\":\"value\"}")
        {
            // Note: Adjust the Document constructor as needed.
            return new Document(){
                Id = id,
                Tags = ["tag1"],
                Data = JsonDocument.Parse(json).RootElement
            };
        }

        [Fact]
        public async Task SaveDocumentAsync_CallsRepositorySaveAsync()
        {
            // Arrange
            var document = CreateTestDocument("doc1");

            // Act
            await _documentService.SaveDocumentAsync(document);

            // Assert
            _repositoryMock.Verify(r => r.SaveAsync(document), Times.Once);
        }

        [Fact]
        public async Task UpdateDocumentAsync_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var document = CreateTestDocument("doc1");

            // Act
            await _documentService.UpdateDocumentAsync(document);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(document), Times.Once);
        }

        [Fact]
        public async Task GetDocumentAsync_ReturnsSerializedDocument_WhenDocumentExists()
        {
            // Arrange
            var document = CreateTestDocument("doc1");
            string acceptHeader = "application/json";
            string expectedSerialized = "{\"id\":\"doc1\",\"data\":{\"key\":\"value\"}}";

            // Setup the repository to return our test document.
            _repositoryMock.Setup(r => r.GetAsync("doc1")).ReturnsAsync(document);
            // Setup the serializer to return the expected serialized string.
            _serializerMock.Setup(s => s.Serialize(document)).Returns(expectedSerialized);

            // Act
            var result = await _documentService.GetDocumentAsync("doc1", acceptHeader);

            // Assert
            Assert.Equal(expectedSerialized, result);
            _repositoryMock.Verify(r => r.GetAsync("doc1"), Times.Once);
            _serializerFactoryMock.Verify(f => f.GetSerializer(acceptHeader), Times.Once);
            _serializerMock.Verify(s => s.Serialize(document), Times.Once);
        }

        [Fact]
        public async Task GetDocumentAsync_ThrowsKeyNotFoundException_WhenDocumentDoesNotExist()
        {
            // Arrange
            string acceptHeader = "application/json";
            _repositoryMock.Setup(r => r.GetAsync("doc1")).ReturnsAsync((IDocument)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _documentService.GetDocumentAsync("doc1", acceptHeader));
            _repositoryMock.Verify(r => r.GetAsync("doc1"), Times.Once);
        }
        
        [Fact]
        public async Task GetDocumentAsync_CallsSerializerFactoryWithCorrectAcceptHeader()
        {
            // Arrange
            var document = CreateTestDocument("doc1");
            string acceptHeader = "application/xml";
            string expectedSerialized = "<document>...</document>";

            _repositoryMock.Setup(r => r.GetAsync("doc1")).ReturnsAsync(document);
            _serializerFactoryMock.Setup(f => f.GetSerializer(acceptHeader)).Returns(_serializerMock.Object);
            _serializerMock.Setup(s => s.Serialize(document)).Returns(expectedSerialized);

            // Act
            var result = await _documentService.GetDocumentAsync("doc1", acceptHeader);

            // Assert
            Assert.Equal(expectedSerialized, result);
            _serializerFactoryMock.Verify(f => f.GetSerializer(acceptHeader), Times.Once);
        }
        
        [Fact]
        public async Task GetDocumentAsync_ReturnsEmptyString_WhenSerializerReturnsEmpty()
        {
            // Arrange
            var document = CreateTestDocument("doc1");
            string acceptHeader = "application/json";
            string expectedSerialized = ""; // empty string

            _repositoryMock.Setup(r => r.GetAsync("doc1")).ReturnsAsync(document);
            _serializerFactoryMock.Setup(f => f.GetSerializer(acceptHeader)).Returns(_serializerMock.Object);
            _serializerMock.Setup(s => s.Serialize(document)).Returns(expectedSerialized);

            // Act
            var result = await _documentService.GetDocumentAsync("doc1", acceptHeader);

            // Assert
            Assert.Equal(expectedSerialized, result);
        }
    }