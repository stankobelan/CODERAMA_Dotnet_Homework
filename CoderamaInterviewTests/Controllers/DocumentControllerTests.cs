namespace CoderamaInterviewTests.Controllers;

public class DocumentControllerTests : IDisposable
{
    private readonly Mock<IDocumentService> _documentServiceMock;
    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        // Arrange common dependencies once per test instance.
        _documentServiceMock = new Mock<IDocumentService>();
        Mock<ILogger<DocumentController>> loggerMock = new();
        _controller = new DocumentController(_documentServiceMock.Object, loggerMock.Object);
    }

    /// <summary>
    /// Helper method to create a test document.
    /// </summary>
    private Document CreateTestDocument(string id = "doc1", string json = "{\"key\":\"value\"}")
    {
        return new Document()
        {
            Id = id,
            Tags = ["tag1"],
            Data = JsonDocument.Parse(json).RootElement
        };
    }

    /// <summary>
    /// Helper method to set up HttpContext with a specified  header.
    /// </summary>
    private void SetupHttpContext(string acceptHeader)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Accept"] = acceptHeader;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [Fact]
    public async Task CreateDocument_ReturnsCreatedAtActionResult_WhenDocumentIsCreatedSuccessfully()
    {
        // Arrange
        var document = CreateTestDocument();
        _documentServiceMock.Setup(s => s.SaveDocumentAsync(document))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateDocument(document);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetDocument", createdResult.ActionName);
    }

    [Fact]
    public async Task CreateDocument_ReturnsBadRequest_WhenModelIsInvalid()
    {
        // Arrange: Create a document with invalid data
        var document = CreateTestDocument("", "{}");
        _controller.ModelState.AddModelError("Id", "Document ID is required.");

        // Act
        var result = await _controller.CreateDocument(document);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateDocument_ReturnsStatus500_WhenExceptionIsThrown()
    {
        // Arrange
        var document = CreateTestDocument();
        _documentServiceMock.Setup(s => s.SaveDocumentAsync(document))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _controller.CreateDocument(document);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
    }

    [Fact]
    public async Task UpdateDocument_ReturnsNoContent_WhenDocumentIsUpdatedSuccessfully()
    {
        // Arrange
        var document = CreateTestDocument();
        _documentServiceMock.Setup(s => s.UpdateDocumentAsync(document))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateDocument(document);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateDocument_ReturnsStatus500_WhenExceptionIsThrown()
    {
        // Arrange
        var document = CreateTestDocument();
        _documentServiceMock.Setup(s => s.UpdateDocumentAsync(document))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _controller.UpdateDocument(document);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetDocument_ReturnsContentResult_WhenDocumentIsRetrievedSuccessfully()
    {
        // Arrange
        string documentId = "doc1";
        string acceptHeader = "application/json";
        string serializedDocument = "{\"id\":\"doc1\"}";
        _documentServiceMock.Setup(s => s.GetDocumentAsync(documentId, acceptHeader))
            .ReturnsAsync(serializedDocument);

        SetupHttpContext(acceptHeader);

        // Act
        var result = await _controller.GetDocument(documentId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal(acceptHeader, contentResult.ContentType);
        Assert.Equal(serializedDocument, contentResult.Content);
    }

    [Fact]
    public async Task GetDocument_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
    {
        // Arrange
        string documentId = "doc1";
        string acceptHeader = "application/json";
        _documentServiceMock.Setup(s => s.GetDocumentAsync(documentId, acceptHeader))
            .ThrowsAsync(new KeyNotFoundException());

        SetupHttpContext(acceptHeader);

        // Act
        var result = await _controller.GetDocument(documentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetDocument_ReturnsStatus500_WhenGenericExceptionIsThrown()
    {
        // Arrange
        string documentId = "doc1";
        string acceptHeader = "application/json";
        _documentServiceMock.Setup(s => s.GetDocumentAsync(documentId, acceptHeader))
            .ThrowsAsync(new Exception("Test Exception"));

        SetupHttpContext(acceptHeader);

        // Act
        var result = await _controller.GetDocument(documentId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
    }

    public void Dispose()
    {
        // If you have any resources to dispose, do it here.
        // In this case, mocks and controller are managed by the test framework.
    }
}