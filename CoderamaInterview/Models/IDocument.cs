using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MessagePack;
using Microsoft.AspNetCore.Mvc;

namespace CoderamaInterview.Models;

public interface IDocument
{
    string Id { get; set; }
    List<string> Tags { get; set; }
    JsonElement Data { get; set; }
}

[MessagePackObject]
public class Document : IDocument
{
    [MessagePack.Key(0)]
    [Required(ErrorMessage = "Document ID is required.")]
    public string Id { get; set; } = string.Empty;
    
    [MessagePack.Key(1)]
    [Required(ErrorMessage = "At least one tag is required.")]
    [MinLength(1, ErrorMessage = "At least one tag must be provided.")]
    public List<string> Tags { get; set; } = new();

    [IgnoreMember]
    [XmlIgnore]
    [Required(ErrorMessage = "Data is required.")]
    public JsonElement Data { get; set; }

    [MessagePack.Key(2)]
    [XmlElement("Data")]
    [JsonIgnore]
    public string? DataAsString
    {
        get
        {
            // If Data is undefined, return null so it doesn't break serialization.
            return Data.ValueKind != JsonValueKind.Undefined
                ? Data.GetRawText()
                : null;
        }
        set
        {
            // If the XML has <Data>...</Data>, parse it back into a JsonElement.
            if (!string.IsNullOrWhiteSpace(value))
            {
                Data = JsonDocument.Parse(value).RootElement;
            }
            else
            {
                // If <Data /> is empty or missing, set an undefined JsonElement.
                Data = default;
            }
        }
    }
}

