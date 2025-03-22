using CoderamaInterview.Models;

namespace CoderamaInterview.Repositories;

public interface IDocumentRepository
{
    Task SaveAsync(IDocument document);
    Task UpdateAsync(IDocument document);
    Task<IDocument?> GetAsync(string id);
}