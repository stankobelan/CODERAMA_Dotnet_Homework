using System.Reflection.Metadata;
using CoderamaInterview.Factories;
using CoderamaInterview.Models;
using CoderamaInterview.Repositories;

namespace CoderamaInterview.Services;

public interface IDocumentService
{
    Task SaveDocumentAsync(IDocument document);
    Task UpdateDocumentAsync(IDocument document);
    Task<string> GetDocumentAsync(string id, string acceptHeader);
}

public class DocumentService(IDocumentRepository repository, IDocumentSerializerFactory serializerFactory)
    : IDocumentService
{
    public async Task SaveDocumentAsync(IDocument document) =>
        await repository.SaveAsync(document);

    public async Task UpdateDocumentAsync(IDocument document) =>
        await repository.UpdateAsync(document);

    public async Task<string> GetDocumentAsync(string id, string acceptHeader)
    {
        var document = await repository.GetAsync(id);
        if (document is null)
            throw new KeyNotFoundException($"Document with id {id} not found.");

        var serializer = serializerFactory.GetSerializer(acceptHeader);
        return serializer.Serialize(document);
    }
}