using System.Text.Json.Serialization;

namespace DatadogWebhookFunction.Models;

/// <summary>
/// Root model for Datadog webhook payload
/// </summary>
public class DatadogWebhookPayload
{
    [JsonPropertyName("alert_id")]
    public string? AlertId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("alert_type")]
    public string? AlertType { get; set; }

    [JsonPropertyName("alert_transition")]
    public string? AlertTransition { get; set; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [JsonPropertyName("org")]
    public DatadogOrganization? Organization { get; set; }

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("snapshot")]
    public string? Snapshot { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("aggreg_key")]
    public string? AggregationKey { get; set; }

    [JsonPropertyName("date")]
    public long? Date { get; set; }

    [JsonPropertyName("event_msg")]
    public string? EventMessage { get; set; }

    [JsonPropertyName("event_title")]
    public string? EventTitle { get; set; }

    [JsonPropertyName("event_type")]
    public string? EventType { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("last_updated")]
    public long? LastUpdated { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("source_type_name")]
    public string? SourceTypeName { get; set; }

    [JsonPropertyName("alert_status")]
    public string? AlertStatus { get; set; }

    [JsonPropertyName("alert_scope")]
    public List<string>? AlertScope { get; set; }

    [JsonPropertyName("alert_cycle_key")]
    public string? AlertCycleKey { get; set; }
}

/// <summary>
/// Datadog organization information
/// </summary>
public class DatadogOrganization
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// Function response model
/// </summary>
public class FunctionResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static FunctionResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new FunctionResponse<T>
        {
            Success = true,
            Message = message ?? "Webhook processed successfully",
            Data = data
        };
    }

    public static FunctionResponse<T> ErrorResponse(string message)
    {
        return new FunctionResponse<T>
        {
            Success = false,
            Message = message,
            Data = default(T)
        };
    }
}

/// <summary>
/// Webhook processing result
/// </summary>
public class WebhookProcessingResult
{
    public string? ProcessingId { get; set; }
    public string? AlertType { get; set; }
    public string? AlertTransition { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string? Summary { get; set; }
}

/// <summary>
/// Enum for common Datadog alert types
/// </summary>
public static class DatadogAlertTypes
{
    public const string Info = "info";
    public const string Error = "error";
    public const string Warning = "warning";
    public const string Success = "success";
}

/// <summary>
/// Enum for common Datadog alert transitions
/// </summary>
public static class DatadogAlertTransitions
{
    public const string Triggered = "Triggered";
    public const string Recovered = "Recovered";
    public const string NoData = "No Data";
    public const string Warn = "Warn";
}
