using CoderamaInterview.Models;
using MessagePack;

namespace CoderamaInterview.Serialization;


    public class MessagePackDocumentSerializer : IDocumentSerializer
    {
        public string ContentType => "application/x-msgpack";
        public string FileExtension => ".msgpack";
        public string Serialize(IDocument document)
        {
            // Ensure that the document is of the expected concrete type.
            if (!(document is Document concreteDocument))
                throw new InvalidOperationException("Invalid document type for MessagePack serialization.");

            // Serialize the concrete document using MessagePack.
            byte[] bytes = MessagePackSerializer.Serialize(concreteDocument);
            // Convert the byte array to a Base64 string for transmission.
            return Convert.ToBase64String(bytes);
        }

        public IDocument Deserialize(string payload)
        {
            // Konverzia Base64 reťazca späť na byte[]
            byte[] bytes = Convert.FromBase64String(payload);
            // Deserializácia byte[] do objektu Document
            return MessagePackSerializer.Deserialize<Document>(bytes);
        }
    }

