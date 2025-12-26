using System.Text.Json.Serialization;
using Shared.Errors;

namespace DirectoryService.Presentation.EndpointResults;

public record Envelope
{
    public object? Result { get; }

    public Errors? ErrorsList { get; }

    public bool IsError => ErrorsList != null || (ErrorsList != null && ErrorsList.Any());

    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    public Envelope(object? result, Errors? errorsList)
    {
        Result = result;
        ErrorsList = errorsList;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result = null) => new(result, null);

    public static Envelope Error(Errors errorsList) => new(null, errorsList);
}

public record Envelope<T>
{
    public T? Result { get; }

    public Errors? ErrorsList { get; }

    public bool IsError => ErrorsList != null || (ErrorsList != null && ErrorsList.Any());

    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    public Envelope(T? result, Errors? errorsList)
    {
        Result = result;
        ErrorsList = errorsList;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(T? result = default) => new(result, null);

    public static Envelope Error(Errors errorsList) => new(default, errorsList);
}