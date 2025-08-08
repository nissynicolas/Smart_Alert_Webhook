using DatadogWebhookFunction.Models;
using Microsoft.Extensions.Logging;

namespace DatadogWebhookFunction.Services;

/// <summary>
/// Interface for webhook processing service
/// </summary>
public interface IWebhookProcessingService
{
    Task<WebhookProcessingResult> ProcessDatadogWebhookAsync(DatadogWebhookPayload payload);
    Task<bool> ValidateWebhookSignatureAsync(string payload, string signature);
}

/// <summary>
/// Service for processing Datadog webhooks in Azure Functions
/// </summary>
public class WebhookProcessingService : IWebhookProcessingService
{
    private readonly ILogger<WebhookProcessingService> _logger;

    public WebhookProcessingService(ILogger<WebhookProcessingService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Process incoming Datadog webhook payload
    /// </summary>
    public async Task<WebhookProcessingResult> ProcessDatadogWebhookAsync(DatadogWebhookPayload payload)
    {
        var processingId = Guid.NewGuid().ToString("N")[..8];
        
        _logger.LogInformation("Processing Datadog webhook {ProcessingId} - Alert: {AlertId}, Type: {AlertType}, Transition: {AlertTransition}", 
            processingId, payload.AlertId, payload.AlertType, payload.AlertTransition);

        try
        {
            // Log key information about the alert
            LogAlertDetails(payload, processingId);

            // Process based on alert type and transition
            await ProcessAlertLogicAsync(payload, processingId);

            var result = new WebhookProcessingResult
            {
                ProcessingId = processingId,
                AlertType = payload.AlertType,
                AlertTransition = payload.AlertTransition,
                Summary = GenerateProcessingSummary(payload)
            };

            _logger.LogInformation("Successfully processed webhook {ProcessingId}", processingId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process webhook {ProcessingId}", processingId);
            throw;
        }
    }

    /// <summary>
    /// Validate webhook signature (optional)
    /// </summary>
    public async Task<bool> ValidateWebhookSignatureAsync(string payload, string signature)
    {
        // For now, return true since no authentication is required
        // You can implement HMAC-SHA256 signature validation here if needed
        await Task.CompletedTask;
        return true;
    }

    private void LogAlertDetails(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogInformation("Webhook {ProcessingId} Details: Title='{Title}', Hostname='{Hostname}', Priority='{Priority}', Tags={Tags}",
            processingId, 
            payload.Title, 
            payload.Hostname, 
            payload.Priority, 
            payload.Tags != null ? string.Join(", ", payload.Tags) : "None");
    }

    private async Task ProcessAlertLogicAsync(DatadogWebhookPayload payload, string processingId)
    {
        // Implement your business logic here based on alert type and transition
        switch (payload.AlertTransition?.ToLowerInvariant())
        {
            case "triggered":
                await HandleTriggeredAlert(payload, processingId);
                break;
            case "recovered":
                await HandleRecoveredAlert(payload, processingId);
                break;
            case "no data":
                await HandleNoDataAlert(payload, processingId);
                break;
            case "warn":
                await HandleWarningAlert(payload, processingId);
                break;
            default:
                _logger.LogWarning("Unknown alert transition '{Transition}' for webhook {ProcessingId}", 
                    payload.AlertTransition, processingId);
                break;
        }

        // Process based on alert type
        switch (payload.AlertType?.ToLowerInvariant())
        {
            case "error":
                await HandleErrorAlert(payload, processingId);
                break;
            case "warning":
                await HandleWarningTypeAlert(payload, processingId);
                break;
            case "info":
                await HandleInfoAlert(payload, processingId);
                break;
            case "success":
                await HandleSuccessAlert(payload, processingId);
                break;
        }
    }

    private async Task HandleTriggeredAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogWarning("🚨 ALERT TRIGGERED {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        // Add your custom logic here:
        // - Send notifications to Teams/Slack
        // - Create incidents in ServiceNow/Jira
        // - Trigger auto-remediation workflows
        // - Send SMS/Email alerts
        
        await Task.CompletedTask;
    }

    private async Task HandleRecoveredAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogInformation("✅ ALERT RECOVERED {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        // Add your custom logic here:
        // - Send recovery notifications
        // - Update incident status
        // - Clear related alerts
        
        await Task.CompletedTask;
    }

    private async Task HandleNoDataAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogWarning("📊 NO DATA ALERT {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        // Add your custom logic here:
        // - Check data pipeline health
        // - Restart data collection agents
        // - Investigate connectivity issues
        
        await Task.CompletedTask;
    }

    private async Task HandleWarningAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogWarning("⚠️ WARNING ALERT {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        await Task.CompletedTask;
    }

    private async Task HandleErrorAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogError("❌ ERROR ALERT {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        await Task.CompletedTask;
    }

    private async Task HandleWarningTypeAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogWarning("⚠️ WARNING TYPE ALERT {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        await Task.CompletedTask;
    }

    private async Task HandleInfoAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogInformation("ℹ️ INFO ALERT {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        await Task.CompletedTask;
    }

    private async Task HandleSuccessAlert(DatadogWebhookPayload payload, string processingId)
    {
        _logger.LogInformation("✅ SUCCESS ALERT {ProcessingId}: {Title} on {Hostname}", 
            processingId, payload.Title, payload.Hostname);
        
        await Task.CompletedTask;
    }

    private string GenerateProcessingSummary(DatadogWebhookPayload payload)
    {
        return $"Processed {payload.AlertType} alert '{payload.Title}' - {payload.AlertTransition}";
    }
}
