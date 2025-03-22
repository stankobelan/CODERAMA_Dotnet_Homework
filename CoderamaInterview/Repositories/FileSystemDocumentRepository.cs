using System.Text.Json;
using CoderamaInterview.Models;
using CoderamaInterview.Serialization;

namespace CoderamaInterview.Repositories;

public class FileSystemDocumentRepository : IDocumentRepository
{
    private readonly string _storagePath;
    private readonly IDocumentSerializer _serializer;

    public FileSystemDocumentRepository(string storagePath, IDocumentSerializer serializer)
    {
        _storagePath = storagePath;
        _serializer = serializer;
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task SaveAsync(IDocument document)
    {
        string filePath = Path.Combine(_storagePath, $"{document.Id}{_serializer.FileExtension}");
        if (File.Exists(filePath))
        {
            throw new InvalidOperationException($"Document with id {document.Id} already exists.");
        }
        string serialized = _serializer.Serialize(document);
        await File.WriteAllTextAsync(filePath, serialized);
    }

    public async Task UpdateAsync(IDocument document)
    {
        string filePath = Path.Combine(_storagePath, $"{document.Id}{_serializer.FileExtension}");
        string serialized = _serializer.Serialize(document);
        await File.WriteAllTextAsync(filePath, serialized);
    }

    public async Task<IDocument?> GetAsync(string id)
    {
        string filePath = Path.Combine(_storagePath, $"{id}{_serializer.FileExtension}");
        if (!File.Exists(filePath))
        {
            return null;
        }
        string serialized = await File.ReadAllTextAsync(filePath);
        return _serializer.Deserialize(serialized);
    }
}