using CoderamaInterview.Repositories;

namespace CoderamaInterviewTests.Repositories;

public class InMemoryDocumentRepositoryTests
{
    private readonly InMemoryDocumentRepository _repository;

    public InMemoryDocumentRepositoryTests()
    {
        // Each test gets a new repository instance
        _repository = new InMemoryDocumentRepository();
    }

    /// <summary>
    /// Helper method to create a test document.
    /// </summary>
    private Document CreateTestDocument(string id, string json = "{\"key\":\"value\"}", List<string>? tags = null)
    {
        return new Document(){
            Id = id,
            Tags  = tags ?? ["tag1"],
            Data = JsonDocument.Parse(json).RootElement
        };
    }

    [Fact]
    public async Task SaveAsync_ShouldSaveDocumentSuccessfully()
    {
        // Arrange
        var document = CreateTestDocument("doc1");

        // Act
        await _repository.SaveAsync(document);
        var savedDocument = await _repository.GetAsync("doc1");

        // Assert
        Assert.NotNull(savedDocument);
        Assert.Equal(document.Id, savedDocument.Id);
    }

    [Fact]
    public async Task SaveAsync_ShouldThrowException_WhenDocumentAlreadyExists()
    {
        // Arrange
        var document = CreateTestDocument("doc1");
        await _repository.SaveAsync(document);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.SaveAsync(document));
        Assert.Equal($"Document with id {document.Id} already exists.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDocumentSuccessfully()
    {
        // Arrange
        var document = CreateTestDocument("doc1", "{\"key\":\"value\"}");
        await _repository.SaveAsync(document);

        // Create an updated document with the same ID but different data
        var updatedDocument = CreateTestDocument("doc1", "{\"key\":\"newValue\"}", new List<string> { "tag1", "tag2" });

        // Act
        await _repository.UpdateAsync(updatedDocument);
        var savedDocument = await _repository.GetAsync("doc1");

        // Assert
        Assert.NotNull(savedDocument);
        Assert.Equal(updatedDocument.Id, savedDocument.Id);
        Assert.Equal(updatedDocument.Tags, savedDocument.Tags);
        // Optionally, verify that the serialized data matches if your Document implements equality
    }

    [Fact]
    public async Task UpdateAsync_ShouldAddDocument_WhenDocumentDoesNotExist()
    {
        // Arrange
        var document = CreateTestDocument("doc2");

        // Act
        await _repository.UpdateAsync(document);
        var savedDocument = await _repository.GetAsync("doc2");

        // Assert
        Assert.NotNull(savedDocument);
        Assert.Equal(document.Id, savedDocument.Id);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenDocumentDoesNotExist()
    {
        // Act
        var result = await _repository.GetAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }
}