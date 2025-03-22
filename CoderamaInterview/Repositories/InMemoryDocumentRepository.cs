using System.Collections.Concurrent;
using CoderamaInterview.Models;

namespace CoderamaInterview.Repositories;

public class InMemoryDocumentRepository : IDocumentRepository
{
    private readonly ConcurrentDictionary<string, IDocument> _documents = new();

    public Task SaveAsync(IDocument document)
    {
        if (!_documents.TryAdd(document.Id, document))
            throw new InvalidOperationException($"Document with id {document.Id} already exists.");
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IDocument document)
    {
        _documents[document.Id] = document;
        return Task.CompletedTask;
    }

    public Task<IDocument?> GetAsync(string id)
    {
        _documents.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }
}