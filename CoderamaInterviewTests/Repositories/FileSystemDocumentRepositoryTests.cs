using CoderamaInterview.Repositories;
using CoderamaInterview.Serialization;

namespace CoderamaInterviewTests.Repositories;

// Dummy serializer for testing FileSystemDocumentRepository.
// It returns a fixed string (document.Id + ":serialized") when serializing,
// and reconstructs a document using the payload (which should be in the form "docId:serialized").
public class DummySerializer : IDocumentSerializer
{
    public string ContentType => "dummy/type";
    public string FileExtension => ".dummy";

    public string Serialize(IDocument document)
    {
        return document.Id + ":serialized";
    }

    public IDocument Deserialize(string payload)
    {
        var parts = payload.Split(':');
        if (parts.Length != 2)
            throw new InvalidOperationException("Invalid payload");
        // For testing, simply return a new Document with a dummy tag and a fixed data JSON.
        return new Document()
        {
            Id = parts[0],
            Tags = ["dummy"],
            Data = JsonDocument.Parse("{\"key\":\"value\"}").RootElement
        };
    }
}

public class FileSystemDocumentRepositoryTests : IDisposable
{
    private readonly string _tempDir;
    private readonly IDocumentSerializer _serializer;
    private readonly FileSystemDocumentRepository _repository;

    public FileSystemDocumentRepositoryTests()
    {
        // Create a unique temporary directory for this test run.
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _serializer = new DummySerializer();
        _repository = new FileSystemDocumentRepository(_tempDir, _serializer);
    }

    // Helper method to create a test document.
    private Document CreateTestDocument(string id, string json = "{\"key\":\"value\"}", List<string>? tags = null)
    {
        return new Document()
        {
            Id = id, 
            Tags = tags ?? new List<string> { "tag1" }, 
            Data = JsonDocument.Parse(json).RootElement
        };
    }

    [Fact]
    public async Task SaveAsync_CreatesFileAndSavesDocument()
    {
        // Arrange
        var document = CreateTestDocument("doc1");

        // Act
        await _repository.SaveAsync(document);

        // Assert: Verify file exists and contains expected content.
        string filePath = Path.Combine(_tempDir, "doc1" + _serializer.FileExtension);
        Assert.True(File.Exists(filePath));
        string content = await File.ReadAllTextAsync(filePath);
        Assert.Equal(document.Id + ":serialized", content);
    }

    [Fact]
    public async Task SaveAsync_ThrowsException_WhenDocumentAlreadyExists()
    {
        // Arrange
        var document = CreateTestDocument("doc1");
        await _repository.SaveAsync(document);

        // Act & Assert: Trying to save again should throw.
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.SaveAsync(document));
        Assert.Equal($"Document with id {document.Id} already exists.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingDocument()
    {
        // Arrange
        var document = CreateTestDocument("doc1", "{\"key\":\"value\"}");
        await _repository.SaveAsync(document);

        // Create an updated version of the document (simulate an update).
        var updatedDocument = CreateTestDocument("doc1", "{\"key\":\"newValue\"}", new List<string> { "tag1", "tag2" });

        // Act
        await _repository.UpdateAsync(updatedDocument);

        // Assert: The file should be overwritten with new content.
        string filePath = Path.Combine(_tempDir, "doc1" + _serializer.FileExtension);
        Assert.True(File.Exists(filePath));
        string content = await File.ReadAllTextAsync(filePath);
        // Our dummy serializer always returns "doc1:serialized" for a document with id "doc1"
        Assert.Equal("doc1:serialized", content);
    }

    [Fact]
    public async Task UpdateAsync_AddsDocument_WhenDocumentDoesNotExist()
    {
        // Arrange
        var document = CreateTestDocument("doc2");

        // Act: Update acts as an upsert in our implementation.
        await _repository.UpdateAsync(document);

        // Assert: Verify file exists and content is as expected.
        string filePath = Path.Combine(_tempDir, "doc2" + _serializer.FileExtension);
        Assert.True(File.Exists(filePath));
        string content = await File.ReadAllTextAsync(filePath);
        Assert.Equal("doc2:serialized", content);
    }

    [Fact]
    public async Task GetAsync_ReturnsDocument_WhenFileExists()
    {
        // Arrange
        var document = CreateTestDocument("doc1");
        await _repository.SaveAsync(document);

        // Act
        var retrieved = await _repository.GetAsync("doc1");

        // Assert: Verify that the retrieved document is not null and has the expected ID.
        Assert.NotNull(retrieved);
        Assert.Equal("doc1", retrieved.Id);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_WhenFileDoesNotExist()
    {
        // Act
        var retrieved = await _repository.GetAsync("nonexistent");

        // Assert
        Assert.Null(retrieved);
    }

    public void Dispose()
    {
        // Cleanup: Delete the temporary directory and its contents.
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }
}