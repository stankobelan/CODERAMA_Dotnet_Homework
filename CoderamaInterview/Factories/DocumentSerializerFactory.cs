using CoderamaInterview.Serialization;

namespace CoderamaInterview.Factories;

public interface IDocumentSerializerFactory
{
    IDocumentSerializer GetSerializer(string acceptHeader);
}

public class DocumentSerializerFactory(IEnumerable<IDocumentSerializer> serializers) : IDocumentSerializerFactory
{
    public IDocumentSerializer GetSerializer(string acceptHeader)
    {
        // Jednoduché porovnanie – rozšíriteľné o regex či zložitejšiu logiku
        var serializer = serializers.FirstOrDefault(s => s.ContentType.Equals(acceptHeader, StringComparison.OrdinalIgnoreCase));
        return serializer ?? serializers.First(s => s.ContentType == "application/json");
    }
}